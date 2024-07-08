using System;
using System.Runtime.InteropServices;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.JobGauge
{
    interface IJobGaugeMemory655 : IJobGaugeMemory { }

    partial class JobGaugeMemory655 : JobGaugeMemory, IJobGaugeMemory655
    {
        private static string jobDataSignature = "488B3D????????33ED";
        private static int jobDataSignatureOffset = -6;

        public JobGaugeMemory655(TinyIoCContainer container)
                : base(container, jobDataSignature, jobDataSignatureOffset)
        { }

        public override Version GetVersion()
        {
            return new Version(6, 5, 5);
        }

        public override IJobGauge GetJobGauge()
        {
            if (!IsValid()) return null;

            var jobGaugeManager = GetJobGaugeManager();

            var ret = new JobGaugeImpl();

            ret.baseObject = jobGaugeManager;
            ret.job = (JobGaugeJob)jobGaugeManager.ClassJobID;
            ret.rawData = jobGaugeManager.GetRawGaugeData;

            switch (ret.job)
            {
                case JobGaugeJob.WHM: ret.data = jobGaugeManager.WhiteMage; break;
                case JobGaugeJob.SCH: ret.data = jobGaugeManager.Scholar; break;
                case JobGaugeJob.AST: ret.data = jobGaugeManager.Astrologian; break;
                case JobGaugeJob.SGE: ret.data = jobGaugeManager.Sage; break;

                case JobGaugeJob.BRD: ret.data = jobGaugeManager.Bard; break;
                case JobGaugeJob.MCH: ret.data = jobGaugeManager.Machinist; break;
                case JobGaugeJob.DNC: ret.data = jobGaugeManager.Dancer; break;

                case JobGaugeJob.BLM: ret.data = jobGaugeManager.BlackMage; break;
                case JobGaugeJob.SMN: ret.data = jobGaugeManager.Summoner; break;
                case JobGaugeJob.RDM: ret.data = jobGaugeManager.RedMage; break;

                case JobGaugeJob.MNK: ret.data = jobGaugeManager.Monk; break;
                case JobGaugeJob.DRG: ret.data = jobGaugeManager.Dragoon; break;
                case JobGaugeJob.NIN: ret.data = jobGaugeManager.Ninja; break;
                case JobGaugeJob.SAM: ret.data = jobGaugeManager.Samurai; break;
                case JobGaugeJob.RPR: ret.data = jobGaugeManager.Reaper; break;

                case JobGaugeJob.DRK: ret.data = jobGaugeManager.DarkKnight; break;
                case JobGaugeJob.PLD: ret.data = jobGaugeManager.Paladin; break;
                case JobGaugeJob.WAR: ret.data = jobGaugeManager.Warrior; break;
                case JobGaugeJob.GNB: ret.data = jobGaugeManager.Gunbreaker; break;
            }

            return ret;
        }

        private unsafe JobGaugeManager GetJobGaugeManager()
        {
            var rawData = memory.GetByteArray(jobGaugeAddress, sizeof(JobGaugeManager));
            fixed (byte* buffer = rawData)
            {
                return (JobGaugeManager)Marshal.PtrToStructure(new IntPtr(buffer), typeof(JobGaugeManager));
            }
        }

        #region OverlayPlugin Interfaces

        public interface IBaseJobGauge
        {
        }

        #region Healer

        public interface IBaseWhiteMageGauge : IBaseJobGauge
        {
            short LilyTimer { get; }
            byte Lily { get; }
            byte BloodLily { get; }
        }

        public interface IBaseScholarGauge : IBaseJobGauge
        {
            byte Aetherflow { get; }
            byte FairyGauge { get; }
            short SeraphTimer { get; }
            byte DismissedFairy { get; }
        }

        public interface IBaseAstrologianGauge : IBaseJobGauge
        {
            short Timer { get; }
            byte Card { get; }
            byte Seals { get; }

            AstrologianCard CurrentCard { get; }

            AstrologianSeal[] CurrentSeals { get; }
        }

        public interface IBaseSageGauge : IBaseJobGauge
        {
            short AddersgallTimer { get; }
            byte Addersgall { get; }
            byte Addersting { get; }
            byte Eukrasia { get; }

            bool EukrasiaActive { get; }
        }

        #endregion

        #region MagicDPS

        public interface IBaseBlackMageGauge : IBaseJobGauge
        {
            short EnochianTimer { get; }
            short ElementTimeRemaining { get; }
            sbyte ElementStance { get; }
            byte UmbralHearts { get; }
            byte PolyglotStacks { get; }
            EnochianFlags EnochianFlags { get; }

            int UmbralStacks { get; }
            int AstralStacks { get; }
            bool EnochianActive { get; }
            bool ParadoxActive { get; }
        }

        public interface IBaseSummonerGauge : IBaseJobGauge
        {
            ushort SummonTimer { get; }
            ushort AttunementTimer { get; }
            byte ReturnSummon { get; }
            byte ReturnSummonGlam { get; }
            byte Attunement { get; }
            AetherFlags AetherFlags { get; }
        }

        public interface IBaseRedMageGauge : IBaseJobGauge
        {
            byte WhiteMana { get; }
            byte BlackMana { get; }
            byte ManaStacks { get; }
        }

        #endregion

        #region RangeDPS

        public interface IBaseBardGauge : IBaseJobGauge
        {
            ushort SongTimer { get; }
            byte Repertoire { get; }
            byte SoulVoice { get; }
            SongFlags SongFlags { get; }
        }

        public interface IBaseMachinistGauge : IBaseJobGauge
        {
            short OverheatTimeRemaining { get; }
            short SummonTimeRemaining { get; }
            byte Heat { get; }
            byte Battery { get; }
            byte LastSummonBatteryPower { get; }
            byte TimerActive { get; }
        }

        public interface IBaseDancerGauge : IBaseJobGauge
        {
            byte Feathers { get; }
            byte Esprit { get; }
            byte[] DanceSteps { get; }
            byte StepIndex { get; }

            DanceStep CurrentStep { get; }
        }

        #endregion

        #region MeleeDPS

        public interface IBaseMonkGauge : IBaseJobGauge
        {
            byte Chakra { get; }

            BeastChakraType BeastChakra1 { get; }

            BeastChakraType BeastChakra2 { get; }

            BeastChakraType BeastChakra3 { get; }

            NadiFlags Nadi { get; }
            ushort BlitzTimeRemaining { get; }

            BeastChakraType[] BeastChakra { get; }
        }

        public interface IBaseDragoonGauge : IBaseJobGauge
        {
            short LotdTimer { get; }
            byte LotdState { get; }
            byte EyeCount { get; }
            byte FirstmindsFocusCount { get; }
        }

        public interface IBaseNinjaGauge : IBaseJobGauge
        {
            ushort HutonTimer { get; }
            byte Ninki { get; }
            byte HutonManualCasts { get; }
        }

        public interface IBaseSamuraiGauge : IBaseJobGauge
        {
            KaeshiAction Kaeshi { get; }
            byte Kenki { get; }
            byte MeditationStacks { get; }
            SenFlags SenFlags { get; }
        }

        public interface IBaseReaperGauge : IBaseJobGauge
        {
            byte Soul { get; }
            byte Shroud { get; }
            ushort EnshroudedTimeRemaining { get; }
            byte LemureShroud { get; }
            byte VoidShroud { get; }
        }

        #endregion

        #region Tanks

        public interface IBaseDarkKnightGauge : IBaseJobGauge
        {
            byte Blood { get; }
            ushort DarksideTimer { get; }
            byte DarkArtsState { get; }
            ushort ShadowTimer { get; }
        }

        public interface IBasePaladinGauge : IBaseJobGauge
        {
            byte OathGauge { get; }
        }

        public interface IBaseWarriorGauge : IBaseJobGauge
        {
            byte BeastGauge { get; }
        }

        public interface IBaseGunbreakerGauge : IBaseJobGauge
        {
            byte Ammo { get; }
            short MaxTimerDuration { get; }
            byte AmmoComboStep { get; }
        }

        public enum AstrologianCard
        {
            None = 0,
            Balance = 1,
            Bole = 2,
            Arrow = 3,
            Spear = 4,
            Ewer = 5,
            Spire = 6,
            Lord = 0x70,
            Lady = 0x80
        }

        public enum AstrologianSeal
        {
            Solar = 1,
            Lunar = 2,
            Celestial = 3
        }

        public enum DanceStep : byte
        {
            Finish = 0,
            Emboite = 1,
            Entrechat = 2,
            Jete = 3,
            Pirouette = 4
        }

        [Flags]
        public enum EnochianFlags : byte
        {
            None = 0,
            Enochian = 1,
            Paradox = 2
        }

        public enum KaeshiAction : byte
        {
            Higanbana = 1,
            Goken = 2,
            Setsugekka = 3,
            Namikiri = 4
        }

        [Flags]
        public enum SenFlags : byte
        {
            None = 0,
            Setsu = 1 << 0,
            Getsu = 1 << 1,
            Ka = 1 << 2
        }

        [Flags]
        public enum SongFlags : byte
        {
            None = 0,
            MagesBallad = 1 << 0,
            ArmysPaeon = 1 << 1,
            WanderersMinuet = MagesBallad | ArmysPaeon,
            MagesBalladLastPlayed = 1 << 2,
            ArmysPaeonLastPlayed = 1 << 3,
            WanderersMinuetLastPlayed = MagesBalladLastPlayed | ArmysPaeonLastPlayed,
            MagesBalladCoda = 1 << 4,
            ArmysPaeonCoda = 1 << 5,
            WanderersMinuetCoda = 1 << 6
        }

        [Flags]
        public enum AetherFlags : byte
        {
            None = 0,
            Aetherflow1 = 1 << 0,
            Aetherflow2 = 1 << 1,
            Aetherflow = Aetherflow1 | Aetherflow2,
            IfritAttuned = 1 << 2,
            TitanAttuned = 1 << 3,
            GarudaAttuned = TitanAttuned | IfritAttuned,
            PhoenixReady = 1 << 4,
            IfritReady = 1 << 5,
            TitanReady = 1 << 6,
            GarudaReady = 1 << 7
        }

        public enum BeastChakraType : byte
        {
            None = 0,
            Coeurl = 1,
            OpoOpo = 2,
            Raptor = 3
        }

        [Flags]
        public enum NadiFlags : byte
        {
            Lunar = 2,
            Solar = 4
        }

        #endregion
        #endregion

    }
}
