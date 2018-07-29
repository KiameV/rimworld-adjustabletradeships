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
            StoryTellerDefaultsUtil.Init();

            if (StoryTellerUtil.HasOrbitalTraders())
            {
                Settings.GameOrbitalTrade = new OnOffIncident();
                Settings.GameOrbitalTrade.Incident = Settings.GlobalOrbitalTrade.Incident;
                Settings.GameOrbitalTrade.OnDays = Settings.GlobalOrbitalTrade.OnDays;
                Settings.GameOrbitalTrade.OffDays = Settings.GlobalOrbitalTrade.OffDays;
                Settings.GameOrbitalTrade.MinInstances = Settings.GlobalOrbitalTrade.MinInstances;
                Settings.GameOrbitalTrade.MaxInstances = Settings.GlobalOrbitalTrade.MaxInstances;

                StoryTellerUtil.ApplyOrbitalTrade(
                    Settings.GameOrbitalTrade.OnDays, Settings.GameOrbitalTrade.OffDays,
                    Settings.GameOrbitalTrade.MinInstances, Settings.GameOrbitalTrade.MaxInstances);
            }
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            WorldComp.Initialize();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            OnOffIncident gameOT = Settings.GameOrbitalTrade;

            Scribe_Deep.Look(ref gameOT, "AdjustableTradeShips.OrbitalTrader");

            Settings.GameOrbitalTrade = gameOT;

            if (Scribe.mode == LoadSaveMode.PostLoadInit && Settings.GameOrbitalTrade != null)
            {
                StoryTellerUtil.ApplyOrbitalTrade(
                    Settings.GameOrbitalTrade.OnDays, Settings.GameOrbitalTrade.OffDays,
                    Settings.GameOrbitalTrade.MinInstances, Settings.GameOrbitalTrade.MaxInstances);
            }
        }
    }
}
