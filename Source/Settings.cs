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
        public const float DEFAULT_MTBOT = 12f;
        public const float DEFAULT_MIN_DAYS_BETWEEN_ALLY_INTERACTIONS = 5;
        public const float DEFAULT_MTB_ALLY_INTERACTIONS = 6;

        private static float globalMTBOT = DEFAULT_MTBOT;
        private static string inputGlobalMTBOT = globalMTBOT.ToString();
        public static string InputGameMTBOT = globalMTBOT.ToString();

        public static string InputMinDaysBetweenAllyInteraction = DEFAULT_MIN_DAYS_BETWEEN_ALLY_INTERACTIONS.ToString();
        public static string InputMTBAllyInteractions = DEFAULT_MTB_ALLY_INTERACTIONS.ToString();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref globalMTBOT, "AdjustableTradeShips.GlobalMTBOT", DEFAULT_MTBOT, false);
        }

        public static void DoSettingsWindowContents(Rect rect)
        {
            GUI.BeginGroup(new Rect(0, 60, 600, 400));
            Text.Font = GameFont.Small;
            int y = 0;
            Widgets.Label(new Rect(0, y, 600, 40), "AdjustableTradeShips.SetMTBOT".Translate());
            y += 40;

            // Global
            Widgets.Label(new Rect(0, y, 200, 20), "AdjustableTradeShips.Global".Translate() + ":");
            inputGlobalMTBOT = Widgets.TextField(new Rect(220, y, 100, 20), inputGlobalMTBOT);
            if (Widgets.ButtonText(new Rect(340, y, 100, 20), "AdjustableTradeShips.Default".Translate()))
            {
                inputGlobalMTBOT = DEFAULT_MTBOT.ToString();
            }
            y += 25;
            if (Widgets.ButtonText(new Rect(220, y, 100, 20), "AdjustableTradeShips.Apply".Translate()))
            {
                if (Validate(ref inputGlobalMTBOT))
                {
                    globalMTBOT = float.Parse(inputGlobalMTBOT);
                }
                Messages.Message("AdjustableTradeShips.SettingsApplied".Translate(), MessageTypeDefOf.PositiveEvent);
            }
            y += 40;

            // Current Game
            if (Current.Game != null)
            {
                Widgets.DrawLineHorizontal(0, y, 400);
                y += 20;

                Widgets.Label(new Rect(0, y, 200, 20), "AdjustableTradeShips.CurrentGame".Translate() + ":");
                InputGameMTBOT = Widgets.TextField(new Rect(220, y, 100, 20), InputGameMTBOT);
                if (Widgets.ButtonText(new Rect(340, y, 100, 20), "AdjustableTradeShips.Default".Translate()))
                {
                    InputGameMTBOT = DEFAULT_MTBOT.ToString();
                }

                bool hasAllyInteraction = HasAllyInteraction();
                if (hasAllyInteraction)
                {
                    y += 40;
                    Text.Font = GameFont.Medium;
                    Widgets.Label(new Rect(0, y, 200, 30), "AdjustableTradeShips.AllyInteractions".Translate());
                    Text.Font = GameFont.Small;
                    y += 25;
                    Widgets.Label(new Rect(0, y, 200, 20), "AdjustableTradeShips.MinDaysBetween".Translate());
                    InputMinDaysBetweenAllyInteraction = Widgets.TextField(new Rect(220, y, 100, 20), InputMinDaysBetweenAllyInteraction);
                    if (Widgets.ButtonText(new Rect(340, y, 100, 20), "AdjustableTradeShips.Default".Translate()))
                    {
                        InputMinDaysBetweenAllyInteraction = DEFAULT_MIN_DAYS_BETWEEN_ALLY_INTERACTIONS.ToString();
                    }
                    y += 25;

                    Widgets.Label(new Rect(0, y, 200, 20), "AdjustableTradeShips.AverageDaysBetween".Translate());
                    InputMTBAllyInteractions = Widgets.TextField(new Rect(220, y, 100, 20), InputMTBAllyInteractions);
                    if (Widgets.ButtonText(new Rect(340, y, 100, 20), "AdjustableTradeShips.Default".Translate()))
                    {
                        InputMTBAllyInteractions = DEFAULT_MTB_ALLY_INTERACTIONS.ToString();
                    }
                }

                y += 25;
                if (Widgets.ButtonText(new Rect(220, y, 100, 20), "AdjustableTradeShips.Apply".Translate()))
                {
                    ApplyMTBOT();
                    if (hasAllyInteraction)
                    {
                        ApplyAllyInteraction();
                    }
                    Messages.Message("AdjustableTradeShips.SettingsApplied".Translate(), MessageTypeDefOf.PositiveEvent);
                }
            }
            GUI.EndGroup();
        }

        public static void NewGame()
        {
            InputGameMTBOT = globalMTBOT.ToString();
            ApplyMTBOT();
        }

        public static void ApplyMTBOT()
        {
            if (Validate(ref InputGameMTBOT))
            {
                float mtbot = float.Parse(InputGameMTBOT);
                if (Current.Game != null && Current.Game.storyteller != null)
                {
                    StorytellerDef d = Current.Game.storyteller.def;
                    foreach (StorytellerCompProperties c in d.comps)
                    {
                        if (c is StorytellerCompProperties_SingleMTB)
                        {
                            StorytellerCompProperties_SingleMTB mtb = c as StorytellerCompProperties_SingleMTB;
                            if (mtb != null && mtb.incident.defName.EqualsIgnoreCase("OrbitalTraderArrival"))
                            {
                                mtb.mtbDays = mtbot;
                            }
                        }
                    }
                }
            }
        }

        public static bool HasAllyInteraction()
        {
            if (Current.Game != null && Current.Game.storyteller != null)
            {
                StorytellerDef d = Current.Game.storyteller.def;
                foreach (StorytellerCompProperties c in d.comps)
                {
                    if (c is StorytellerCompProperties_FactionInteraction)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ApplyAllyInteraction()
        {
            if (Validate(ref InputMinDaysBetweenAllyInteraction) &&
                Validate(ref InputMTBAllyInteractions))
            {
                float mtbot = float.Parse(InputGameMTBOT);

                float minDaysAllyInteractions = float.Parse(InputMinDaysBetweenAllyInteraction);
                float mtbAllyInteractions = float.Parse(InputMTBAllyInteractions);

                if (Current.Game != null && Current.Game.storyteller != null)
                {
                    StorytellerDef d = Current.Game.storyteller.def;
                    foreach (StorytellerCompProperties c in d.comps)
                    {
                        StorytellerCompProperties_FactionInteraction fi = c as StorytellerCompProperties_FactionInteraction;
                        if (fi != null)
                        {
                            fi.minDaysPassed = minDaysAllyInteractions;
                            fi.baseMtbDays = mtbAllyInteractions;
                        }
                    }
                }
            }
        }

        public static bool Validate(ref string input)
        {
            if (input.Trim().NullOrEmpty())
            {
                Messages.Message("No value entered", MessageTypeDefOf.RejectInput);
                return false;
            }

            float f;
            if (!float.TryParse(input, out f))
            {
                Messages.Message("Invalid value", MessageTypeDefOf.RejectInput);
                return false;
            }

            if (f < 0.0001f)
            {
                input = 0.0001f.ToString();
            }
            return true;
        }

        /*
        private struct StoryTellerMTB
        {
            public readonly float DEFAULT_MTBOT;

            public StorytellerDef StoryTeller;
            public float MTBOT;
            public string Input;

            public StoryTellerMTB(StorytellerDef teller, float defaultMtbot)
            {
                this.StoryTeller = teller;
                this.DEFAULT_MTBOT = defaultMtbot;
                this.MTBOT = defaultMtbot;
                this.Input = defaultMtbot.ToString();
            }
        }
        
        public static void InitStoryTellers()
        {
            if (storyTellers.Count == 0)
            {
                Log.Warning("Begin: " + Scribe.mode + ": No loaded story tellers");
                foreach (StorytellerDef d in DefDatabase<StorytellerDef>.AllDefs)
                {
                    storyTellers.Add(new StoryTellerMTB(d, GetMTBOT(d)));
                }
            }

            Log.Warning("End: " + Scribe.mode + ": Loaded story tellers = " + storyTellers.Count);
        }

        private static void ApplySettings()
        {
            foreach (StoryTellerMTB t in storyTellers)
            {
                if (t.Input.Trim().NullOrEmpty())
                {
                    Messages.Message("No " + "AdjustableTradeShips.MTBOT".Translate() + " given for " + t.StoryTeller.label, MessageTypeDefOf.RejectInput);
                    return;
                }

                float f;
                if (!float.TryParse(t.Input, out f) || f < 0)
                {
                    Messages.Message("invalid " + "AdjustableTradeShips.MTBOT".Translate() + " for " + t.StoryTeller.label, MessageTypeDefOf.RejectInput);
                    return;
                }
            }

            for (int i = 0; i < storyTellers.Count; ++i)
            {
                StoryTellerMTB t = storyTellers[i];
                t.MTBOT = float.Parse(t.Input);
                ChangeStoryTellerMeanTimeBetweenTradeShips(t.StoryTeller, t.MTBOT);
            }
        }

        private static float GetMTBOT(StorytellerDef d)
        {
            if (d != null)
            {
                foreach (StorytellerCompProperties c in d.comps)
                {
                    if (c is StorytellerCompProperties_SingleMTB)
                    {
                        StorytellerCompProperties_SingleMTB mtb = c as StorytellerCompProperties_SingleMTB;
                        if (mtb != null && mtb.incident.defName.EqualsIgnoreCase("OrbitalTraderArrival"))
                        {
                            return mtb.mtbDays;
                        }
                    }
                }
            }
            return 12f;
        }

        private static void ChangeStoryTellerMeanTimeBetweenTradeShips(StorytellerDef d, float newMeanTime)
        {
            if (d != null)
            {
                foreach (StorytellerCompProperties c in d.comps)
                {
                    if (c is StorytellerCompProperties_SingleMTB)
                    {
                        StorytellerCompProperties_SingleMTB mtb = c as StorytellerCompProperties_SingleMTB;
                        if (mtb != null && mtb.incident.defName.EqualsIgnoreCase("OrbitalTraderArrival"))
                        {
                            mtb.mtbDays = newMeanTime;
                        }
                    }
                }
            }
        }*/
    }
}