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

            Scribe_Values.Look<string>(ref Settings.InputGameMTBOT, "AdjustableTradeShips.MTBOT");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
#if DEBUG
                Log.Warning(Scribe.mode + " Apply MTBOT");
#endif
                Settings.ApplyMTBOT();
            }
        }
    }
}
