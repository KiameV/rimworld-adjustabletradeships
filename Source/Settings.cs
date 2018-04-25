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
        private const float DEFAULT_MTBOT = 12f;
        private static float globalMTBOT = DEFAULT_MTBOT;
        private static string inputGlobalMTBOT = globalMTBOT.ToString();
        public static string InputGameMTBOT = globalMTBOT.ToString();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref globalMTBOT, "AdjustableTradeShips.GlobalMTBOT", DEFAULT_MTBOT, false);
        }

        public static void DoSettingsWindowContents(Rect rect)
        {
            GUI.BeginGroup(new Rect(0, 60, 600, 200));
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(0, 0, 600, 40), "AdjustableTradeShips.SetMTBOT".Translate());

            // Global
            Widgets.Label(new Rect(0, 40, 200, 20), "AdjustableTradeShips.Global".Translate() + ":");
            inputGlobalMTBOT = Widgets.TextField(new Rect(220, 40, 100, 20), inputGlobalMTBOT);
            if (Widgets.ButtonText(new Rect(220, 65, 100, 20), "AdjustableTradeShips.Apply".Translate()))
            {
                if (Validate(ref inputGlobalMTBOT))
                {
                    globalMTBOT = float.Parse(inputGlobalMTBOT);
                }
            }
            if (Widgets.ButtonText(new Rect(340, 65, 100, 20), "AdjustableTradeShips.Default".Translate()))
            {
                inputGlobalMTBOT = DEFAULT_MTBOT.ToString();
            }

            // Current Game
            if (Current.Game != null)
            {
                Widgets.Label(new Rect(0, 90, 200, 20), "AdjustableTradeShips.CurrentGame".Translate() + ":");
                InputGameMTBOT = Widgets.TextField(new Rect(220, 90, 100, 20), InputGameMTBOT);
                if (Widgets.ButtonText(new Rect(220, 115, 100, 20), "AdjustableTradeShips.Apply".Translate()))
                {
                    ApplyMTBOT();
                }
                if (Widgets.ButtonText(new Rect(340, 115, 100, 20), "AdjustableTradeShips.Default".Translate()))
                {
                    InputGameMTBOT = DEFAULT_MTBOT.ToString();
                }
            }
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