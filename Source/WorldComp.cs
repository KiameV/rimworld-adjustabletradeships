using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AdjustableTradeShips
{
    class WorldComp : WorldComponent
    {
        public static float CurrentFactor = 1f;

        public WorldComp(World world) : base(world) { }

        public static void Initialize()
        {
#if DEBUG
            Log.Warning("WorldComp.InitializeNewGame");
#endif
            if (StoryTellerUtil.HasOrbitalTraders())
            {
                Settings.GameOrbitalTrade = new OnOffIncident
                {
                    Incident = Settings.GlobalOrbitalTrade.Incident,
                    Days = Settings.GlobalOrbitalTrade.Days,
                    Instances = Settings.GlobalOrbitalTrade.Instances,
                };

                StoryTellerUtil.ApplyOrbitalTrade(
                    Settings.GameOrbitalTrade.Days, Settings.GameOrbitalTrade.Instances);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            OnOffIncident gameOT = Settings.GameOrbitalTrade;

            Scribe_Deep.Look(ref gameOT, "AdjustableTradeShips.OrbitalTrader");

            Settings.GameOrbitalTrade = gameOT;

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (Settings.GameOrbitalTrade == null)
                {
                    Initialize();
                }
                else
                {
                    StoryTellerUtil.ApplyOrbitalTrade(
                        Settings.GameOrbitalTrade.Days, Settings.GameOrbitalTrade.Instances);
                }
            }
        }
    }
}
