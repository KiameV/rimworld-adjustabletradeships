﻿using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdjustableTradeShips
{
    public class Settings : ModSettings
    {
        public static OnOffIncident GlobalOrbitalTrade = null;
        public static OnOffIncident GameOrbitalTrade = null;
        public static string GlobalWeightBuffer = "1.0";
        public static float GlobalWeight = 1.0f;
        public static string GameWeightBuffer = "1.0";
        public static float GameWeight = 1.0f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref GlobalOrbitalTrade, "AdjustableTradeShips.GlobalOrbitalTrade");
            Scribe_Values.Look(ref GlobalWeight, "AdjustableTradeShips.GlobalWeight", 1.0f);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (GlobalWeightBuffer == null || GlobalWeightBuffer.Trim() == "")
                {
                    GlobalWeight = 1.0f;
                }
            }

            GlobalWeightBuffer = GlobalWeight.ToString();
        }
    }

    class OrbitalTradeBuffers
    {
        public string Days;
        public string Instances;
        public OrbitalTradeBuffers(OnOffIncident ooi)
        {
            this.Days = ooi.Days.ToString();
            this.Instances = ooi.Instances.ToString();
        }
    }

    public class SettingsController : Mod
    {
        private const float MIN_ONOFF_VALUE = 0.1f;

        private const float MIN_VALUE = 0.0001f;
        private const float MAX_VALUE = 1000f;

        private bool isInitialized = false;
        private StorytellerDef currentStoryTeller = null;
        private OrbitalTradeBuffers globalOtBuffers = null;
        private OrbitalTradeBuffers gameOtBuffers = null;

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

                //List<TabRecord> tabs = new List<TabRecord>(2);
                //Tabs selectedTab;
                if (Current.Game != null)
                {
                    this.currentStoryTeller = Current.Game.storyteller.def;

                    /*selectedTab = Tabs.ATS_Game;
                    tabs.Add(new TabRecord(
                        Tabs.ATS_Game.ToString().Translate(),
                        delegate { selectedTab = Tabs.ATS_Game; },
                        selectedTab == Tabs.ATS_Game));*/
                }
                else
                {
                    this.currentStoryTeller = null;

                    /*selectedTab = Tabs.ATS_Global;
                    tabs.Add(new TabRecord(
                        Tabs.ATS_Global.ToString().Translate(),
                        delegate { selectedTab = Tabs.ATS_Global; },
                        selectedTab == Tabs.ATS_Global));*/
                }

                if (Settings.GlobalOrbitalTrade == null)
                {
                    Settings.GlobalOrbitalTrade = StoryTellerDefaultsUtil.GetGlobalDefault(IncidentDefOf.OrbitalTraderArrival);
                }

                if (Settings.GameOrbitalTrade == null && StoryTellerUtil.HasOrbitalTraders())
                {
                    Settings.GameOrbitalTrade = new OnOffIncident
                    {
                        Incident = Settings.GlobalOrbitalTrade.Incident,
                        Days = Settings.GlobalOrbitalTrade.Days,
                        Instances = Settings.GlobalOrbitalTrade.Instances,
                    };
                }
            }
            this.MakeBuffers();
        }

        private void MakeBuffers()
        {
            if (this.globalOtBuffers == null)
            {
                this.globalOtBuffers = new OrbitalTradeBuffers(Settings.GlobalOrbitalTrade);
            }
            if (Settings.GlobalWeightBuffer.NullOrEmpty())
            {
                Settings.GlobalWeightBuffer = "1.0";
                Settings.GlobalWeight = 1.0f;
            }

            if (this.gameOtBuffers == null && Current.Game != null && Settings.GameOrbitalTrade != null)
            {
                this.gameOtBuffers = new OrbitalTradeBuffers(Settings.GameOrbitalTrade);
            }

            if (Settings.GameWeightBuffer.NullOrEmpty() && Current.Game != null)
            {
                Settings.GameWeightBuffer = Settings.GlobalWeightBuffer;
                if (!float.TryParse(Settings.GameWeightBuffer, out Settings.GameWeight))
                {
                    Settings.GameWeight = 1.0f;
                    Settings.GameWeightBuffer = "1.0";
                }
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

            
            NumberInput(40, y, ref Settings.GlobalOrbitalTrade.Instances, ref globalOtBuffers.Instances, MIN_ONOFF_VALUE, MAX_VALUE);

            Widgets.Label(new Rect(100, y, inRect.width - 200, 32), "AdjustableTradeShips.OTS".Translate());
            y += 40;

            NumberInput(40, y, ref Settings.GlobalOrbitalTrade.Days, ref globalOtBuffers.Days, MIN_ONOFF_VALUE, MAX_VALUE);

            Widgets.Label(new Rect(100, y, inRect.width - 200, 32), "AdjustableTradeShips.Days".Translate());
            y += 40;

            NumberInput(20, y, "AdjustableTradeShips.OrbitalVisitorWeight".Translate(), ref Settings.GlobalWeight, ref Settings.GlobalWeightBuffer, MIN_VALUE, MAX_VALUE);
            y += 40;

            if (Widgets.ButtonText(new Rect(0, y, 100, 32), "AdjustableTradeShips.Default".Translate()))
            {
                OnOffIncident ooi = StoryTellerDefaultsUtil.GetGlobalDefault(IncidentDefOf.OrbitalTraderArrival);
                Settings.GlobalOrbitalTrade.Days = ooi.Days;
                globalOtBuffers.Days = ooi.Days.ToString();
                Settings.GlobalOrbitalTrade.Instances = ooi.Instances;
                globalOtBuffers.Instances = ooi.Instances.ToString();
                Settings.GlobalWeightBuffer = "1.0";
                Settings.GlobalWeight = 1.0f;
            }

            if (Widgets.ButtonText(new Rect(200, y, 100, 32), "AdjustableTradeShips.Apply".Translate()))
            {
                StoryTellerUtil.ApplyOrbitalTrade(Settings.GlobalOrbitalTrade.Days, Settings.GlobalOrbitalTrade.Instances);
                Messages.Message("AdjustableTradeShips.GlobalSettingsApplied".Translate(), MessageTypeDefOf.PositiveEvent);
                this.globalOtBuffers = null;
            }
            y += 40;

            // Current Game
            if (Current.Game != null)
            {
                Widgets.DrawLineHorizontal(20, y, inRect.width - 40);
                y += 40;

                Widgets.Label(new Rect(0, y, 600, 40), "AdjustableTradeShips.CurrentGame".Translate());
                y += 40;

                if (StoryTellerUtil.HasOrbitalTraders() && Settings.GameOrbitalTrade != null)
                {
                    Widgets.Label(new Rect(20, y, 600, 40), "AdjustableTradeShips.TradeShips".Translate());
                    y += 40;

                    // Game Orbital Trade
                    NumberInput(40, y, ref Settings.GameOrbitalTrade.Instances, ref gameOtBuffers.Instances, MIN_ONOFF_VALUE, MAX_VALUE);

                    Widgets.Label(new Rect(100, y, inRect.width - 200, 32), "AdjustableTradeShips.OTS".Translate());
                    y += 40;

                    NumberInput(40, y, ref Settings.GameOrbitalTrade.Days, ref gameOtBuffers.Days, MIN_ONOFF_VALUE, MAX_VALUE);

                    Widgets.Label(new Rect(100, y, inRect.width - 200, 32), "AdjustableTradeShips.Days".Translate());
                    y += 40;

                    if (Widgets.ButtonText(new Rect(0, y, 100, 32), "AdjustableTradeShips.Default".Translate()))
                    {
                        if (StoryTellerDefaultsUtil.TryGetStoryTellerDefault(IncidentDefOf.OrbitalTraderArrival, out OnOffIncident ooi))
                        {
                            Settings.GameOrbitalTrade.Days = ooi.Days;
                            gameOtBuffers.Days = ooi.Days.ToString();
                            Settings.GameOrbitalTrade.Instances = ooi.Instances;
                            gameOtBuffers.Instances = ooi.Instances.ToString();
                        }
                        else
                        {
                            ooi = StoryTellerDefaultsUtil.GetGlobalDefault(IncidentDefOf.OrbitalTraderArrival);
                            Settings.GameOrbitalTrade.Days = ooi.Days;
                            gameOtBuffers.Days = ooi.Days.ToString();
                            Settings.GameOrbitalTrade.Instances = ooi.Instances;
                            gameOtBuffers.Instances = ooi.Instances.ToString();
                        }
                    }

                    if (Widgets.ButtonText(new Rect(200, y, 100, 32), "AdjustableTradeShips.Apply".Translate()))
                    {
                        StoryTellerUtil.ApplyOrbitalTrade(Settings.GameOrbitalTrade.Days, Settings.GameOrbitalTrade.Instances);
                        Messages.Message("AdjustableTradeShips.GameSettingsApplied".Translate(), MessageTypeDefOf.PositiveEvent);
                        this.gameOtBuffers = null;
                    }
                }
                else if (StoryTellerUtil.HasRandom())
                {
                    StoryTellerUtil.TryGetRandomWeight(IncidentCategoryDefOf.OrbitalVisitor, out float w);
                    Widgets.Label(new Rect(20, y, 300, 32), $"Current Weight: {w}");
                    y += 40;

                    if (Settings.GameWeightBuffer.NullOrEmpty())
                    {
                        if (!StoryTellerUtil.TryGetRandomWeight(IncidentCategoryDefOf.OrbitalVisitor, out Settings.GameWeight))
                        {
                            Settings.GameWeight = 1f;
                        }
                        Settings.GameWeightBuffer = Settings.GameWeight.ToString();
                    }

                    NumberInput(20, y, "AdjustableTradeShips.OrbitalVisitorWeight".Translate(), ref Settings.GameWeight, ref Settings.GameWeightBuffer, MIN_VALUE, MAX_VALUE);
                    y += 40;

                    if (Widgets.ButtonText(new Rect(0, y, 100, 32), "AdjustableTradeShips.Default".Translate()))
                    {
                        Settings.GameWeight = 1.0f;
                        Settings.GameWeightBuffer = "1.0";
                    }

                    if (Widgets.ButtonText(new Rect(200, y, 100, 32), "AdjustableTradeShips.Apply".Translate()))
                    {
                        if (StoryTellerUtil.ApplyRandom(IncidentCategoryDefOf.OrbitalVisitor, Settings.GameWeight))
                            Messages.Message("AdjustableTradeShips.GameSettingsApplied".Translate(), MessageTypeDefOf.PositiveEvent);
                        else
                            Log.Error("Failed to apply settings to random story teller");
                    }
                }
                else
                {
                    Widgets.Label(new Rect(20, y, 300, 32), Current.Game.storyteller.def.label + ": " + "AdjustableTradeShips.CannotModifyOrbitalTraderTimes".Translate());
                }
                y += 25;
            }
        }

        private void NumberInput(float x, float y, string label, ref float val, ref string buffer, float min, float max)
        {
            Widgets.Label(new Rect(x, y, 200, 20), label);
            buffer = Widgets.TextField(new Rect(x + 210, y, 100, 20), buffer);
            if (buffer.Length > 0)
            {
                if (float.TryParse(buffer, out float v))
                {
                    val = v;
                    if (val > max)
                        val = max;
                    else if (val < min)
                        val = min;
                }
                else
                {
                    val = min;
                }
            }
        }

        private void NumberInput(float x, float y, ref float val, ref string buffer, float min, float max)
        {
            buffer = Widgets.TextField(new Rect(x, y, 50, 20), buffer);
            if (buffer.Length > 0)
            {
                if (float.TryParse(buffer, out float v))
                {
                    val = v;
                    if (val > max)
                        val = max;
                    else if (val < min)
                        val = min;
                }
                else
                {
                    val = min;
                }
            }
        }
    }
}