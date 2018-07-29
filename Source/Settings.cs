using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdjustableTradeShips
{
    public class SettingsController : Mod
    {
        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "AdjustableTradeShips.ModName".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }

    public class Settings : ModSettings
    {
        private const float MIN_ONOFF_VALUE = 0.01f;

        private const float MIN_VALUE = 0.01f;
        private const float MAX_VALUE = 1000f;


        private enum Tabs
        {
            ATS_Game,
            ATS_Global
        };

        public static OnOffIncident GlobalOrbitalTrade = null;
        public static OnOffIncident GameOrbitalTrade = null;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref GlobalOrbitalTrade, "AdjustableTradeShips.GlobalOrbitalTrade");
        }

        public static void DoSettingsWindowContents(Rect inRect)
        {
            // Init
            StoryTellerDefaultsUtil.Init();

            StorytellerDef currentStoryTeller;
            StoryTellerDefaults defaults;
            //List<TabRecord> tabs = new List<TabRecord>(2);
            //Tabs selectedTab;
            if (Current.Game != null)
            {
                currentStoryTeller = Current.Game.storyteller.def;
                defaults = StoryTellerDefaultsUtil.GetStoryTellerDefaults(currentStoryTeller);

                /*selectedTab = Tabs.ATS_Game;
                tabs.Add(new TabRecord(
                    Tabs.ATS_Game.ToString().Translate(),
                    delegate { selectedTab = Tabs.ATS_Game; },
                    selectedTab == Tabs.ATS_Game));*/
            }
            else
            {
                currentStoryTeller = null;
                defaults = StoryTellerDefaultsUtil.defaultDefaults;

                /*selectedTab = Tabs.ATS_Global;
                tabs.Add(new TabRecord(
                    Tabs.ATS_Global.ToString().Translate(),
                    delegate { selectedTab = Tabs.ATS_Global; },
                    selectedTab == Tabs.ATS_Global));*/
            }
            if (GlobalOrbitalTrade == null)
            {
                StoryTellerDefaultsUtil.defaultDefaults.TryGetIncident(IncidentDefOf.OrbitalTraderArrival, out GlobalOrbitalTrade);
            }


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
            
            NumberInput(40, y, "AdjustableTradeShips.OnDays".Translate(), ref GlobalOrbitalTrade.OnDays, MIN_ONOFF_VALUE, MAX_VALUE, otDefaults.OnDays);
            y += 40;
            
            NumberInput(40, y, "AdjustableTradeShips.OffDays".Translate(), ref GlobalOrbitalTrade.OffDays, MIN_ONOFF_VALUE, MAX_VALUE, otDefaults.OffDays);
            y += 40;

            NumberInput(40, y, "AdjustableTradeShips.MinInstances".Translate(), ref GlobalOrbitalTrade.MinInstances, MIN_VALUE, MAX_VALUE, otDefaults.MinInstances);
            y += 40;

            if (GlobalOrbitalTrade.MinInstances > GlobalOrbitalTrade.MaxInstances)
            {
                GlobalOrbitalTrade.MaxInstances = GlobalOrbitalTrade.MinInstances;
            }

            NumberInput(40, y, "AdjustableTradeShips.MaxInstances".Translate(), ref GlobalOrbitalTrade.MaxInstances, MIN_VALUE, MAX_VALUE, otDefaults.MaxInstances);
            y += 40;

            // Current Game
            if (Current.Game != null && GameOrbitalTrade != null)
            {
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
                    NumberInput(40, y, "AdjustableTradeShips.OnDays".Translate(), ref GameOrbitalTrade.OnDays, MIN_ONOFF_VALUE, MAX_VALUE, otDefaults.OnDays, ref changed);
                    y += 40;

                    NumberInput(40, y, "AdjustableTradeShips.OffDays".Translate(), ref GameOrbitalTrade.OffDays, MIN_ONOFF_VALUE, MAX_VALUE, otDefaults.OffDays, ref changed);
                    y += 40;

                    NumberInput(40, y, "AdjustableTradeShips.MinInstances".Translate(), ref GameOrbitalTrade.MinInstances, MIN_VALUE, MAX_VALUE, otDefaults.MinInstances, ref changed);
                    y += 40;

                    if (GameOrbitalTrade.MinInstances > GameOrbitalTrade.MaxInstances)
                    {
                        GameOrbitalTrade.MaxInstances = GameOrbitalTrade.MinInstances;
                    }

                    NumberInput(40, y, "AdjustableTradeShips.MaxInstances".Translate(), ref GameOrbitalTrade.MaxInstances, MIN_VALUE, MAX_VALUE, otDefaults.MaxInstances, ref changed);
                    y += 40;

                    if (changed)
                    {
                        StoryTellerUtil.ApplyOrbitalTrade(GameOrbitalTrade.OnDays, GameOrbitalTrade.OffDays, GameOrbitalTrade.MinInstances, GameOrbitalTrade.MaxInstances);
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

        private static void NumberInput(float x, float y, string label, ref float val, float min, float max, float defaultValue)
        {
            float orig = val;
            try
            {
                Widgets.Label(new Rect(x, y, 175, 20), label);
                string buffer = val.ToString();
                Widgets.TextFieldNumeric<float>(new Rect(x + 180, y, 115 - x, 20), ref val, ref buffer, min, max);
                if (Widgets.ButtonText(new Rect(300, y, 100, 20), "AdjustableTradeShips.Default".Translate()))
                {
                    val = defaultValue;
                    buffer = defaultValue.ToString();
                }
            }
            catch
            {
                val = min;
            }
        }

        private static void NumberInput(float x, float y, string label, ref float val, float min, float max, float defaultValue, ref bool changed)
        {
            float orig = val;
            NumberInput(x, y, label, ref val, min, max, defaultValue);
            if (orig != val)
            {
                changed = true;
            }
        }
    }
}