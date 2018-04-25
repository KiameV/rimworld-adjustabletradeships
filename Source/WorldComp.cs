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
            Settings.NewGame();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<string>(ref Settings.InputGameMTBOT, "AdjustableTradeShips.MTBOT", Settings.DEFAULT_MTBOT.ToString());
            Scribe_Values.Look<string>(ref Settings.InputMTBAllyInteractions, "AdjustableTradeShips.MTBAlly", Settings.DEFAULT_MTB_ALLY_INTERACTIONS.ToString());
            Scribe_Values.Look<string>(ref Settings.InputMinDaysBetweenAllyInteraction, "AdjustableTradeShips.MinDaysAlly", Settings.DEFAULT_MIN_DAYS_BETWEEN_ALLY_INTERACTIONS.ToString());

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
#if DEBUG
                Log.Warning(Scribe.mode + " Apply MTBOT");
#endif
                Settings.ApplyMTBOT();
                Settings.ApplyAllyInteraction();
            }
        }
    }
}
