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
            if (Settings.GlobalOrbitalTrade == null)
            {
                Settings.GlobalOrbitalTrade = StoryTellerDefaultsUtil.GetGlobalDefault(IncidentDefOf.OrbitalTraderArrival);
            }

            if (Settings.GameOrbitalTrade == null)
            {
                Settings.GameOrbitalTrade = new OnOffIncident()
                {
                    Incident = Settings.GlobalOrbitalTrade.Incident,
                    Days = Settings.GlobalOrbitalTrade.Days,
                    Instances = Settings.GlobalOrbitalTrade.Instances,
                };
            }

            if (Settings.GameWeightBuffer == null || Settings.GameWeightBuffer.Trim() == "")
            {
                Settings.GameWeight = Settings.GlobalWeight;
                Settings.GameWeightBuffer = Settings.GlobalWeightBuffer;
            }

            if (StoryTellerUtil.HasOrbitalTraders())
            {
                StoryTellerUtil.ApplyOrbitalTrade(
                    Settings.GameOrbitalTrade.Days, Settings.GameOrbitalTrade.Instances);
            }
            else if (StoryTellerUtil.HasRandom())
            {
                StoryTellerUtil.ApplyRandom(IncidentCategoryDefOf.OrbitalVisitor, Settings.GameWeight);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            OnOffIncident gameOT = Settings.GameOrbitalTrade;

            Scribe_Deep.Look(ref gameOT, "AdjustableTradeShips.OrbitalTrader");
            Scribe_Values.Look(ref Settings.GameWeight, "AdjustableTradeShips.Weight", 1.0f);
            Settings.GameWeightBuffer = Settings.GameWeight.ToString();
            Settings.GameOrbitalTrade = gameOT;

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Initialize();
            }
        }
    }
}
