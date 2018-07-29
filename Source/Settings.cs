using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdjustableTradeShips
{
    public class Settings : ModSettings
    {
        public static OnOffIncident GlobalOrbitalTrade = null;
        public static OnOffIncident GameOrbitalTrade = null;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref GlobalOrbitalTrade, "AdjustableTradeShips.GlobalOrbitalTrade");
        }
    }

    struct OrbitalTradeBuffers
    {
        public string OnDays;
        public string OffDays;
        public string MinInstances;
        public string MaxInstances;
        public OrbitalTradeBuffers(OnOffIncident ooi)
        {
            this.OnDays = ooi.OnDays.ToString();
            this.OffDays = ooi.OffDays.ToString();
            this.MinInstances = ooi.MinInstances.ToString();
            this.MaxInstances = ooi.MaxInstances.ToString();
        }
    }

    public class SettingsController : Mod
    {
        private const float MIN_ONOFF_VALUE = 0.01f;

        private const float MIN_VALUE = 0.01f;
        private const float MAX_VALUE = 1000f;

        private bool isInitialized = false;
        private StorytellerDef currentStoryTeller = null;
        private StoryTellerDefaults defaults = null;
        private OrbitalTradeBuffers globalOtBuffers;
        private OrbitalTradeBuffers gameOtBuffers;

        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "AdjustableTradeShips.ModName".Translate();
        }

        public override void WriteSettings()
        {
            // Happen after Settings.ExposeData
            base.WriteSettings();
            this.MakeBuffers();
        }

        public void Init()
        {
            if (!isInitialized)
            {
                this.isInitialized = true;
                StoryTellerDefaultsUtil.Init();

                //List<TabRecord> tabs = new List<TabRecord>(2);
                //Tabs selectedTab;
                if (Current.Game != null)
                {
                    this.currentStoryTeller = Current.Game.storyteller.def;
                    this.defaults = StoryTellerDefaultsUtil.GetStoryTellerDefaults(currentStoryTeller);

                    /*selectedTab = Tabs.ATS_Game;
                    tabs.Add(new TabRecord(
                        Tabs.ATS_Game.ToString().Translate(),
                        delegate { selectedTab = Tabs.ATS_Game; },
                        selectedTab == Tabs.ATS_Game));*/
                }
                else
                {
                    this.currentStoryTeller = null;
                    this.defaults = StoryTellerDefaultsUtil.defaultDefaults;

                    /*selectedTab = Tabs.ATS_Global;
                    tabs.Add(new TabRecord(
                        Tabs.ATS_Global.ToString().Translate(),
                        delegate { selectedTab = Tabs.ATS_Global; },
                        selectedTab == Tabs.ATS_Global));*/
                }

                if (Settings.GlobalOrbitalTrade == null)
                {
                    StoryTellerDefaultsUtil.defaultDefaults.TryGetIncident(IncidentDefOf.OrbitalTraderArrival, out Settings.GlobalOrbitalTrade);
                }

                if (Settings.GameOrbitalTrade == null && StoryTellerUtil.HasOrbitalTraders())
                {
                    Settings.GameOrbitalTrade = new OnOffIncident
                    {
                        Incident = Settings.GlobalOrbitalTrade.Incident,
                        OnDays = Settings.GlobalOrbitalTrade.OnDays,
                        OffDays = Settings.GlobalOrbitalTrade.OffDays,
                        MinInstances = Settings.GlobalOrbitalTrade.MinInstances,
                        MaxInstances = Settings.GlobalOrbitalTrade.MaxInstances,
                    };
                }

                this.MakeBuffers();
            }
        }

        private void MakeBuffers()
        {
            this.globalOtBuffers = new OrbitalTradeBuffers(Settings.GlobalOrbitalTrade);
            if (Current.Game != null && Settings.GameOrbitalTrade != null)
            {
                this.gameOtBuffers = new OrbitalTradeBuffers(Settings.GameOrbitalTrade);
            }
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            this.Init();

            // Draw Contents
            // Global
            Text.Font = GameFont.Small;
            float y = 60;
            Widgets.Label(new Rect(0, y, 600, 40), "AdjustableTradeShips.Global".Translate());
            y += 40;

            Widgets.Label(new Rect(20, y, 600, 40), "AdjustableTradeShips.TradeShips".Translate());
            y += 40;

            OnOffIncident otDefaults;
            StoryTellerDefaultsUtil.defaultDefaults.TryGetIncident(IncidentDefOf.OrbitalTraderArrival, out otDefaults);

            NumberInput(40, y, "AdjustableTradeShips.OnDays".Translate(), ref Settings.GlobalOrbitalTrade.OnDays, ref globalOtBuffers.OnDays, MIN_ONOFF_VALUE, MAX_VALUE, otDefaults.OnDays);
            y += 40;

            NumberInput(40, y, "AdjustableTradeShips.OffDays".Translate(), ref Settings.GlobalOrbitalTrade.OffDays, ref globalOtBuffers.OffDays, MIN_ONOFF_VALUE, MAX_VALUE, otDefaults.OffDays);
            y += 40;

            NumberInput(40, y, "AdjustableTradeShips.MinInstances".Translate(), ref Settings.GlobalOrbitalTrade.MinInstances, ref globalOtBuffers.MinInstances, MIN_VALUE, MAX_VALUE, otDefaults.MinInstances);
            y += 40;

            if (Settings.GlobalOrbitalTrade.MinInstances > Settings.GlobalOrbitalTrade.MaxInstances)
            {
                Settings.GlobalOrbitalTrade.MaxInstances = Settings.GlobalOrbitalTrade.MinInstances;
            }

            NumberInput(40, y, "AdjustableTradeShips.MaxInstances".Translate(), ref Settings.GlobalOrbitalTrade.MaxInstances, ref globalOtBuffers.MaxInstances, MIN_VALUE, MAX_VALUE, otDefaults.MaxInstances);
            y += 40;

            // Current Game
            if (Current.Game != null && Settings.GameOrbitalTrade != null)
            {
                OnOffIncident gameOt = Settings.GameOrbitalTrade;
                Widgets.DrawLineHorizontal(20, y, inRect.width - 40);
                y += 40;

                Widgets.Label(new Rect(0, y, 600, 40), "AdjustableTradeShips.CurrentGame".Translate());
                y += 40;

                if (StoryTellerUtil.HasOrbitalTraders())
                {
                    Widgets.Label(new Rect(20, y, 600, 40), "AdjustableTradeShips.TradeShips".Translate());
                    y += 40;

                    defaults.TryGetIncident(IncidentDefOf.OrbitalTraderArrival, out otDefaults);

                    // Game Orbital Trade
                    bool changed = false;
                    NumberInput(40, y, "AdjustableTradeShips.OnDays".Translate(), ref gameOt.OnDays, ref gameOtBuffers.OnDays, MIN_ONOFF_VALUE, MAX_VALUE, otDefaults.OnDays, ref changed);
                    y += 40;

                    NumberInput(40, y, "AdjustableTradeShips.OffDays".Translate(), ref gameOt.OffDays, ref gameOtBuffers.OffDays, MIN_ONOFF_VALUE, MAX_VALUE, otDefaults.OffDays, ref changed);
                    y += 40;

                    NumberInput(40, y, "AdjustableTradeShips.MinInstances".Translate(), ref gameOt.MinInstances, ref gameOtBuffers.MinInstances, MIN_VALUE, MAX_VALUE, otDefaults.MinInstances, ref changed);
                    y += 40;

                    if (gameOt.MinInstances > gameOt.MaxInstances)
                    {
                        gameOt.MaxInstances = gameOt.MinInstances;
                    }

                    NumberInput(40, y, "AdjustableTradeShips.MaxInstances".Translate(), ref gameOt.MaxInstances, ref gameOtBuffers.MaxInstances, MIN_VALUE, MAX_VALUE, otDefaults.MaxInstances, ref changed);
                    y += 40;

                    if (changed)
                    {
                        StoryTellerUtil.ApplyOrbitalTrade(gameOt.OnDays, gameOt.OffDays, gameOt.MinInstances, gameOt.MaxInstances);
                        Settings.GameOrbitalTrade = gameOt;
                    }
                }
                else
                {
                    Widgets.Label(new Rect(20, y, 300, 20), Current.Game.storyteller.def.label + ": " + "AdjustableTradeShips.CannotModifyOrbitalTraderTimes".Translate());
                }
                y += 25;
                /*
                                bool hasAllyInteraction = StoryTellerUtil.HasAllyInteraction();
                                if (hasAllyInteraction)
                                {
                                    y += 20;
                                    Widgets.Label(new Rect(0, y, 200, 30), "AdjustableTradeShips.AllyInteractions".Translate());
                                    y += 25;
                                    float origMinDays = minDaysBetweenAllyInteraction;
                                    NumberInput(20, y, "AdjustableTradeShips.MinDaysBetween".Translate(), ref minDaysBetweenAllyInteraction, ref minDaysBetweenAllyInteractionString, MIN_VALUE, MAX_VALUE, DEFAULT_MIN_DAYS_BETWEEN_ALLY_INTERACTIONS);
                                    y += 25;

                                    float origMTB = mtbAllyInteractions;
                                    NumberInput(20, y, "AdjustableTradeShips.AverageDaysBetween".Translate(), ref mtbAllyInteractions, ref mtbAllyInteractionsString, MIN_VALUE, MAX_VALUE, DEFAULT_MTB_ALLY_INTERACTIONS);

                                    if (origMinDays != minDaysBetweenAllyInteraction || 
                                        origMTB != mtbAllyInteractions)
                                    {
                                        StoryTellerUtil.ApplyAllyInteraction(minDaysBetweenAllyInteraction, mtbAllyInteractions);
                                    }
                                }
                                else
                                {
                                    Widgets.Label(new Rect(0, y, 300, 20), Current.Game.storyteller.def.label + ": " + "AdjustableTradeShips.CannotModifyAllyInteractionTimes".Translate());
                                }
                                y += 25;*/
            }
        }

        private void NumberInput(float x, float y, string label, ref float val, ref string buffer, float min, float max, float defaultValue)
        {
            Widgets.Label(new Rect(x, y, 175, 20), label);
                buffer = Widgets.TextField(new Rect(x + 180, y, 115 - x, 20), buffer);
            if (buffer.Length > 0)
            {
                if (float.TryParse(buffer, out float v))
                {
                    val = v;
                }
            }
            if (Widgets.ButtonText(new Rect(300, y, 100, 20), "AdjustableTradeShips.Default".Translate()))
            {
                val = defaultValue;
                buffer = defaultValue.ToString();
            }
        }

        private void NumberInput(float x, float y, string label, ref float val, ref string buffer, float min, float max, float defaultValue, ref bool changed)
        {
            float orig = val;
            NumberInput(x, y, label, ref val, ref buffer, min, max, defaultValue);
            if (orig != val)
            {
                changed = true;
            }
        }
    }
}