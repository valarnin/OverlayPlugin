using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using RainbowMage.OverlayPlugin.MemoryProcessors.InCombat;
using RainbowMage.OverlayPlugin.NetworkProcessors;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Combatant
{
    public class LineCombatant
    {
        public const uint LogFileLineID = 261;
        private ILogger logger;
        private readonly FFXIVRepository ffxiv;
        private ICombatantMemory combatantMemoryManager;
        private IInCombatMemory inCombatMemory;
        private ConcurrentDictionary<uint, CombatantStateInfo> combatantStateMap = new ConcurrentDictionary<uint, CombatantStateInfo>();

        int offsetHeaderActorID;
        int offsetHeaderLoginUserID;

        // Only emit a log line when this information changes every X milliseconds
        private struct CombatantChangeCriteria
        {
            // in milliseconds
            public const int PollingRate = 20;

            // in milliseconds
            public const uint InCombatDelayDefault = 1000; // If any property has changed in this timeframe, a line will be written
            public const uint OutOfCombatDelayDefault = 5000;

            public const uint InCombatDelayNameID = 0;
            public const uint OutOfCombatDelayNameID = 1000;

            public const uint InCombatDelayPosition = 250;
            public const uint OutOfCombatDelayPosition = 1250;

            public const uint InCombatDelayTransformationID = 0;
            public const uint OutOfCombatDelayTransformationID = 1000;

            public const uint InCombatDelayWeaponID = 0;
            public const uint OutOfCombatDelayWeaponID = 1000;

            public const uint InCombatDelayTargetID = 0;
            public const uint OutOfCombatDelayTargetID = 1000;

            // in in-game distance
            public const float InCombatDistancePosition = 5F;
            public const float OutOfCombatDistancePosition = 15F;

            // in radians
            public const float InCombatDistanceHeading = (float)(45 * (Math.PI / 180)); // 45º turns
            public const float OutOfCombatDistanceHeading = 20f; // Effectively disabled

            // Check these fields for changes to determine if we should dump a full list of all changes
            private static readonly string[] DefaultCheckFieldNames = new string[] {
                "OwnerID",
                "Type",
                "MonsterType",
                "Status",
                "ModelStatus",
                "AggressionStatus",
                "IsTargetable",
                "Name",
                "Radius",
                "BNpcID",
                "CurrentMP",
                "IsCasting1",
            };

            // Fields that should be written out for add or full list of changes
            public static readonly FieldInfo[] AllFields = typeof(Combatant).GetFields()
                // Exclude "Effects" due to object complexity, "ID" because that's always printed
                .Where((field) => field.Name != "Effects" && field.Name != "ID")
                .ToArray();

            public static readonly FieldInfo[] DefaultCheckFields = AllFields.Where((f) => DefaultCheckFieldNames.Contains(f.Name)).ToArray();

            private static object GetDefault(Type type)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
                if (type == typeof(string))
                {
                    return string.Empty;
                }
                return null;
            }

            public static readonly ReadOnlyDictionary<Type, object> DefaultValues =
                new ReadOnlyDictionary<Type, object>(
                    AllFields.Select((fi) => fi.FieldType).Distinct().ToDictionary((t) => t, (t) => GetDefault(t)));
        }

        private class CombatantStateInfo
        {
            public DateTime lastUpdated;
            public Combatant combatant;
        }

        private Func<string, DateTime, bool> logWriter;

        private CancellationTokenSource cancellationToken;

        public LineCombatant(TinyIoCContainer container)
        {
            logger = container.Resolve<ILogger>();
            ffxiv = container.Resolve<FFXIVRepository>();
            if (!ffxiv.IsFFXIVPluginPresent())
                return;
            combatantMemoryManager = container.Resolve<ICombatantMemory>();
            inCombatMemory = container.Resolve<IInCombatMemory>();
            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "CombatantMemory",
                Source = "OverlayPlugin",
                ID = LogFileLineID,
                Version = 1,
            });

            var netHelper = container.Resolve<NetworkParser>();
            try
            {
                var mach = Assembly.Load("Machina.FFXIV");
                var msgHeaderType = mach.GetType("Machina.FFXIV.Headers.Server_MessageHeader");
                offsetHeaderActorID = netHelper.GetOffset(msgHeaderType, "ActorID");
                offsetHeaderLoginUserID = netHelper.GetOffset(msgHeaderType, "LoginUserID");
                ffxiv.RegisterNetworkParser(MessageReceived);
            }
            catch (System.IO.FileNotFoundException)
            {
                logger.Log(LogLevel.Error, Resources.NetworkParserNoFfxiv);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, Resources.NetworkParserInitException, e);
            }

            cancellationToken = new CancellationTokenSource();

            Task.Run(PollCombatants, cancellationToken.Token);
        }

        ~LineCombatant()
        {
            if (cancellationToken != null)
            {
                cancellationToken.Cancel();
            }
        }

        private void PollCombatants()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;

                    CheckCombatants(now);

                    // Wait for next poll
                    var delay = CombatantChangeCriteria.PollingRate - (int)Math.Ceiling((DateTime.Now - now).TotalMilliseconds);
                    if (delay > 0)
                    {
                        Thread.Sleep(delay);
                    }
                    else
                    {
                        // If we're lagging enough to not have a sleep duration, delay by PollingRate to reduce lag
                        Thread.Sleep(CombatantChangeCriteria.PollingRate);
                    }
                }
                catch (Exception e)
                {
                    logger.Log(LogLevel.Debug, $"LineCombatant: Exception: {e}");
                }
            }
        }

        private void CheckCombatants(DateTime now, params uint[] filter)
        {
            bool inCombat = inCombatMemory.GetInCombat();
            var combatants = combatantMemoryManager.GetCombatantList();

            // Check combatants currently in memory first
            foreach (var combatant in combatants)
            {
                // If we're only checking specific actor IDs, filter to those
                if (filter.Length > 0 && !filter.Contains(combatant.ID))
                {
                    continue;
                }

                // If this is a new combatant, always write a line for it
                if (!combatantStateMap.ContainsKey(combatant.ID))
                {
                    combatantStateMap[combatant.ID] = new CombatantStateInfo()
                    {
                        lastUpdated = now,
                        combatant = combatant,
                    };
                    WriteLine(
                        CombatantMemoryChangeType.Add,
                        combatant.ID,
                        string.Join("", CombatantChangeCriteria.AllFields.Select((fi) => FormatFieldChange(fi, combatant, true))));
                    continue;
                }

                var oldCombatant = combatantStateMap[combatant.ID].combatant;
                var lastUpdatedDiff = (now - combatantStateMap[combatant.ID].lastUpdated).TotalMilliseconds;
                var changed = new List<FieldInfo>();

                // Check the general case of "if any mapped field has changed since the default delay duration, write all changes"
                var delayDefault = inCombat ? CombatantChangeCriteria.InCombatDelayDefault : CombatantChangeCriteria.OutOfCombatDelayDefault;
                if (lastUpdatedDiff > delayDefault)
                {
                    foreach (var fi in CombatantChangeCriteria.DefaultCheckFields)
                    {
                        var oldVal = fi.GetValue(oldCombatant);
                        var newVal = fi.GetValue(combatant);
                        // There's some weird behavior with just using `==` or `.Equals` here, where two UInt32 values that are the same somehow aren't.
                        if (oldVal is IComparable)
                        {
                            if (((IComparable)oldVal).CompareTo(newVal) != 0)
                            {
                                changed.Add(fi);
                                continue;
                            }
                        }
                        if (!oldVal.Equals(newVal))
                        {
                            changed.Add(fi);
                            continue;
                        }
                    }
                    if (changed.Count > 0)
                    {
                        combatantStateMap[combatant.ID] = new CombatantStateInfo()
                        {
                            lastUpdated = now,
                            combatant = combatant,
                        };
                        WriteLine(
                            CombatantMemoryChangeType.Change,
                            combatant.ID,
                            string.Join("", changed.Select((fi) => FormatFieldChange(fi, combatant))));
                        continue;
                    }
                }

                // Check for partial change lines with a delay less than the default delay
                // Batch these partial changes
                var delayNameID = inCombat ? CombatantChangeCriteria.InCombatDelayNameID : CombatantChangeCriteria.OutOfCombatDelayNameID;
                if (lastUpdatedDiff > delayNameID && combatant.BNpcNameID != oldCombatant.BNpcNameID)
                {
                    changed.Add(combatant.GetType().GetField("BNpcNameID"));
                }
                var delayTransformationID = inCombat ? CombatantChangeCriteria.InCombatDelayTransformationID : CombatantChangeCriteria.OutOfCombatDelayTransformationID;
                if (lastUpdatedDiff > delayTransformationID && combatant.TransformationId != oldCombatant.TransformationId)
                {
                    changed.Add(combatant.GetType().GetField("TransformationId"));
                }
                var delayWeaponID = inCombat ? CombatantChangeCriteria.InCombatDelayWeaponID : CombatantChangeCriteria.OutOfCombatDelayWeaponID;
                if (lastUpdatedDiff > delayWeaponID && combatant.WeaponId != oldCombatant.WeaponId)
                {
                    changed.Add(combatant.GetType().GetField("WeaponId"));
                }
                var delayTargetID = inCombat ? CombatantChangeCriteria.InCombatDelayTargetID : CombatantChangeCriteria.OutOfCombatDelayTargetID;
                if (lastUpdatedDiff > delayTargetID && combatant.TargetID != oldCombatant.TargetID)
                {
                    changed.Add(combatant.GetType().GetField("TargetID"));
                }
                var delayPosition = inCombat ? CombatantChangeCriteria.InCombatDelayPosition : CombatantChangeCriteria.OutOfCombatDelayPosition;
                if (lastUpdatedDiff > delayPosition)
                {
                    // This check seems redundant but it's less expensive than the check below against distance
                    // so it uses less CPU
                    if (combatant.PosX != oldCombatant.PosX || combatant.PosY != oldCombatant.PosY || combatant.PosZ != oldCombatant.PosZ)
                    {
                        var dist = Math.Sqrt(
                            Math.Pow(combatant.PosX - oldCombatant.PosX, 2)
                            + Math.Pow(combatant.PosY - oldCombatant.PosY, 2)
                            + Math.Pow(combatant.PosZ - oldCombatant.PosZ, 2));
                        var checkDist = inCombat ? CombatantChangeCriteria.InCombatDistancePosition : CombatantChangeCriteria.OutOfCombatDistancePosition;
                        if (dist > checkDist)
                        {
                            if (combatant.PosX != oldCombatant.PosX)
                            {
                                changed.Add(combatant.GetType().GetField("PosX"));
                            }
                            if (combatant.PosY != oldCombatant.PosY)
                            {
                                changed.Add(combatant.GetType().GetField("PosY"));
                            }
                            if (combatant.PosZ != oldCombatant.PosZ)
                            {
                                changed.Add(combatant.GetType().GetField("PosZ"));
                            }
                        }
                    }
                    else if (combatant.Heading != oldCombatant.Heading)
                    {
                        var diff = Math.Abs((combatant.Heading + Math.PI) - (oldCombatant.Heading + Math.PI));
                        var checkDiff = inCombat ? CombatantChangeCriteria.InCombatDistanceHeading : CombatantChangeCriteria.OutOfCombatDistanceHeading;
                        if (diff > checkDiff)
                        {
                            changed.Add(combatant.GetType().GetField("Heading"));
                        }
                    }
                }
                if (changed.Count > 0)
                {
                    combatantStateMap[combatant.ID] = new CombatantStateInfo()
                    {
                        lastUpdated = now,
                        combatant = combatant,
                    };
                    WriteLine(
                        CombatantMemoryChangeType.Change,
                        combatant.ID,
                        string.Join("", changed.Select((fi) => FormatFieldChange(fi, combatant))));
                }
            }

            // Any combatants no longer in memory, signify that they were removed
            foreach (var ID in combatantStateMap.Keys)
            {
                // If we're filtering, only consider removing those in the filters
                if (filter.Length > 0 && !filter.Contains(ID))
                {
                    continue;
                }
                if (!combatants.Any((c) => c.ID == ID))
                {
                    combatantStateMap.TryRemove(ID, out var _);
                    WriteLine(CombatantMemoryChangeType.Remove, ID, "");
                }
            }
        }

        private string FormatFieldChange(FieldInfo info, Combatant combatant, bool skipDefaultValues = false)
        {
            var value = info.GetValue(combatant);

            if (value == null)
            {
                if (skipDefaultValues)
                {
                    return string.Empty;
                }
                return $"|{info.Name}|NULL";
            }

            if (skipDefaultValues && value.Equals(CombatantChangeCriteria.DefaultValues[info.FieldType]))
            {
                return string.Empty;
            }

            if (info.Name == "PCTargetID" || info.Name == "NPCTargetID" || info.Name == "BNpcNameID" || info.Name == "BNpcID" || info.Name == "TargetID" || info.Name == "OwnerID" || info.Name == "CastTargetID")
            {
                return $"|{info.Name}|0x{value:X}";
            }

            if (info.FieldType.IsEnum)
            {
                return $"|{info.Name}|{Convert.ChangeType(value, Enum.GetUnderlyingType(info.FieldType))}";
            }
            return $"|{info.Name}|{value}";
        }

        private void WriteLine(CombatantMemoryChangeType type, uint combatantID, string info)
        {
            var line = $"{type}|{combatantID:X8}{info}";
            logWriter(line, ffxiv.GetServerTimestamp());
        }

        private unsafe void MessageReceived(string id, long epoch, byte[] message)
        {
            fixed (byte* buffer = message)
            {
                uint actorID = *(uint*)&buffer[offsetHeaderActorID];
                uint loginID = *(uint*)&buffer[offsetHeaderLoginUserID];
                // Only check if we're not looking at a packet that's for just us
                if (actorID != loginID)
                {
                    DateTime serverTime = ffxiv.EpochToDateTime(epoch);
                    // Also only check if we're beyond the default delay for this ID, or if this ID doesn't exist yet
                    if (!combatantStateMap.ContainsKey(actorID) || (serverTime - combatantStateMap[actorID].lastUpdated).TotalMilliseconds > CombatantChangeCriteria.InCombatDelayDefault)
                    {
                        CheckCombatants(serverTime, actorID);
                    }
                }
            }
        }

        private enum CombatantMemoryChangeType
        {
            Add,
            Remove,
            Change,
        }
    }
}
