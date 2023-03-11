using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RainbowMage.OverlayPlugin.MemoryProcessors.Combatant;
using System.Reflection;
using System.Linq;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Combatant
{
    public class LineCombatant
    {
        public const uint LogFileLineID = 261;
        private ILogger logger;
        private readonly FFXIVRepository ffxiv;
        private ICombatantMemory combatantMemoryManager;
        private ConcurrentDictionary<uint, CombatantStateInfo> combatantStateMap = new ConcurrentDictionary<uint, CombatantStateInfo>();

        // Only emit a log line when this information changes every X milliseconds
        private struct CombatantChangeCriteria {
            // in milliseconds
            public const int PollingRate = 20;

            // in milliseconds
            public const uint DelayDefault = 1000; // If any property has changed in this timeframe, a line will be written

            public const uint DelayNameID = 0;
            public const uint DelayPosition = 250;
            public const uint DelayTransformationID = 0;
            public const uint DelayWeaponID = 0;
            public const uint DelayTargetID = 0;

            // in in-game distance
            public const float DistancePosition = 5F;

            // in radians
            public const float DistanceHeading = (float)(45 * (Math.PI / 180)); // 45º turns

            // Check these fields for changes to determine if we should dump a full line
            public static readonly FieldInfo[] defaultCheckFields = new FieldInfo[] {
                // This is just every field except Effects, Distances, and specifically checked values for now, can remove as needed
                typeof(Combatant).GetField("ID"),
                typeof(Combatant).GetField("OwnerID"),
                typeof(Combatant).GetField("Type"),
                typeof(Combatant).GetField("MonsterType"),
                typeof(Combatant).GetField("Status"),
                typeof(Combatant).GetField("ModelStatus"),
                typeof(Combatant).GetField("AggressionStatus"),
                typeof(Combatant).GetField("IsTargetable"),
                typeof(Combatant).GetField("Job"),
                typeof(Combatant).GetField("Name"),
                typeof(Combatant).GetField("CurrentHP"),
                typeof(Combatant).GetField("MaxHP"),
                typeof(Combatant).GetField("Radius"),
                typeof(Combatant).GetField("BNpcID"),
                typeof(Combatant).GetField("CurrentMP"),
                typeof(Combatant).GetField("MaxMP"),
                typeof(Combatant).GetField("Level"),
                typeof(Combatant).GetField("WorldID"),
                typeof(Combatant).GetField("CurrentWorldID"),
                typeof(Combatant).GetField("NPCTargetID"),
                typeof(Combatant).GetField("CurrentGP"),
                typeof(Combatant).GetField("MaxGP"),
                typeof(Combatant).GetField("CurrentCP"),
                typeof(Combatant).GetField("MaxCP"),
                typeof(Combatant).GetField("PCTargetID"),
                typeof(Combatant).GetField("IsCasting1"),
                typeof(Combatant).GetField("IsCasting2"),
                typeof(Combatant).GetField("CastBuffID"),
                typeof(Combatant).GetField("CastTargetID"),
                typeof(Combatant).GetField("CastDurationCurrent"),
                typeof(Combatant).GetField("CastDurationMax"),
            };
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
            combatantMemoryManager = container.Resolve<ICombatantMemory>();
            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "CombatantMemory",
                Source = "OverlayPlugin",
                ID = LogFileLineID,
                Version = 1,
            });

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
            try {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var now = DateTime.Now;

                    var combatants = combatantMemoryManager.GetCombatantList();

                    // Check combatants currently in memory first
                    foreach (var combatant in combatants)
                    {
                        // If this is a new combatant, always write a line for it
                        if (!combatantStateMap.ContainsKey(combatant.ID))
                        {
                            combatantStateMap[combatant.ID] = new CombatantStateInfo(){
                                lastUpdated = now,
                                combatant = combatant,
                            };
                            WriteLine(CombatantMemoryChangeType.Add, combatant.ID, JObject.FromObject(combatant).ToString(Newtonsoft.Json.Formatting.None));
                            continue;
                        }

                        var oldCombatant = combatantStateMap[combatant.ID].combatant;
                        var lastUpdatedDiff = (now - combatantStateMap[combatant.ID].lastUpdated).TotalMilliseconds;

                        if (lastUpdatedDiff > CombatantChangeCriteria.DelayDefault)
                        {
                            var changed = false;
                            foreach(var fi in CombatantChangeCriteria.defaultCheckFields)
                            {
                                var oldVal = fi.GetValue(oldCombatant);
                                var newVal = fi.GetValue(combatant);
                                // There's some weird behavior with just using `==` or `.Equals` here, where two UInt32 values that are the same somehow aren't.
                                if (oldVal is IComparable)
                                {
                                    if (((IComparable)oldVal).CompareTo(newVal) != 0)
                                    {
                                        changed = true;
                                        break;
                                    }
                                }
                                if (!oldVal.Equals(newVal))
                                {
                                    changed = true;
                                    break;
                                }
                            }
                            if (changed)
                            {
                                combatantStateMap[combatant.ID] = new CombatantStateInfo()
                                {
                                    lastUpdated = now,
                                    combatant = combatant,
                                };
                                WriteLine(CombatantMemoryChangeType.PartialChange, combatant.ID, JObject.FromObject(combatant).ToString(Newtonsoft.Json.Formatting.None));
                            }
                        }

                        // Check for partial change lines at the end
                        // Batch these partial changes
                        var obj = new JObject();
                        if (lastUpdatedDiff > CombatantChangeCriteria.DelayNameID && combatant.BNpcNameID != oldCombatant.BNpcNameID)
                        {
                            obj["BNpcNameID"] = combatant.BNpcNameID;
                        }
                        if (lastUpdatedDiff > CombatantChangeCriteria.DelayTransformationID && combatant.TransformationId != oldCombatant.TransformationId)
                        {
                            obj["TransformationId"] = combatant.TransformationId;
                        }
                        if (lastUpdatedDiff > CombatantChangeCriteria.DelayWeaponID && combatant.WeaponId != oldCombatant.WeaponId)
                        {
                            obj["WeaponId"] = combatant.WeaponId;
                        }
                        if (lastUpdatedDiff > CombatantChangeCriteria.DelayTargetID && combatant.TargetID != oldCombatant.TargetID)
                        {
                            obj["TargetID"] = combatant.TargetID;
                        }
                        if (lastUpdatedDiff > CombatantChangeCriteria.DelayPosition)
                        {
                            if (
                                (combatant.PosX != oldCombatant.PosX || combatant.PosY != oldCombatant.PosY || combatant.PosZ != oldCombatant.PosZ)
                                && Math.Sqrt(Math.Pow(combatant.PosX, 2) + Math.Pow(combatant.PosY, 2) + Math.Pow(combatant.PosZ, 2)) > CombatantChangeCriteria.DistancePosition)
                            {
                                obj["PosX"] = combatant.PosX;
                                obj["PosY"] = combatant.PosY;
                                obj["PosZ"] = combatant.PosZ;
                            } else if (
                                combatant.Heading != oldCombatant.Heading
                                && Math.Abs((combatant.Heading + Math.PI) - (oldCombatant.Heading + Math.PI)) > CombatantChangeCriteria.DistanceHeading)
                            {
                                obj["Heading"] = combatant.Heading;
                            }
                        }
                        if (obj.Count > 0)
                        {
                            combatantStateMap[combatant.ID] = new CombatantStateInfo()
                            {
                                lastUpdated = now,
                                combatant = combatant,
                            };
                            WriteLine(CombatantMemoryChangeType.FullChange, combatant.ID, obj.ToString(Newtonsoft.Json.Formatting.None));
                        }
                    }

                    // Any combatants no longer in memory, signify that they were removed
                    foreach (var ID in combatantStateMap.Keys)
                    {
                        if (!combatants.Any((c) => c.ID == ID))
                        {
                            combatantStateMap.TryRemove(ID, out var _);
                            WriteLine(CombatantMemoryChangeType.Remove, ID, "");
                        }
                    }

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
            } catch (Exception e) {
                logger.Log(LogLevel.Debug, $"LineCombatant: Exception: {e}");
            }
        }

        private void WriteLine(CombatantMemoryChangeType type, uint combatantID, string info)
        {
            var line = $"{type}|{combatantID:X8}|{info}";
            logWriter(line, ffxiv.GetServerTimestamp());
        }

        private enum CombatantMemoryChangeType
        {
            Add,
            Remove,
            FullChange,
            PartialChange,
        }
    }
}
