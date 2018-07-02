using RimWorld.Planet;
using Verse;

namespace AdjustableTradeShips
{
    class WorldComp : WorldComponent
    {
        public static float CurrentFactor = 1f;

        public WorldComp(World world) : base(world) { }

        public static void InitializeNewGame()
        {
#if DEBUG
            Log.Warning("WorldComp.InitializeNewGame");
#endif
            if (StoryTellerUtil.HasOrbitalTraders())
            {
                Settings.GameMTBOT = Settings.GlobalMTBOT;
                StoryTellerUtil.ApplyMTBOT(Settings.GameMTBOT);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            float gameMtbot = Settings.GameMTBOT;
            float mtbAllyInteractions = Settings.MtbAllyInteractions;
            float minDaysBetweenAllyInteractions = Settings.MinDaysBetweenAllyInteraction;

            Scribe_Values.Look<float>(ref gameMtbot, "AdjustableTradeShips.MTBOT", Settings.DEFAULT_MTBOT);
            Scribe_Values.Look<float>(ref mtbAllyInteractions, "AdjustableTradeShips.MTBAlly", Settings.DEFAULT_MTB_ALLY_INTERACTIONS);
            Scribe_Values.Look<float>(ref minDaysBetweenAllyInteractions, "AdjustableTradeShips.MinDaysAlly", Settings.DEFAULT_MIN_DAYS_BETWEEN_ALLY_INTERACTIONS);

            Settings.GameMTBOT = gameMtbot;
            Settings.MtbAllyInteractions = mtbAllyInteractions;
            Settings.MinDaysBetweenAllyInteraction = minDaysBetweenAllyInteractions;

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
#if DEBUG
                Log.Warning(Scribe.mode + " Apply MTBOT");
#endif
                StoryTellerUtil.ApplyMTBOT(gameMtbot);
                StoryTellerUtil.ApplyAllyInteraction(minDaysBetweenAllyInteractions, mtbAllyInteractions);
            }
        }
    }
}
