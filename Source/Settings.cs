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
        public const float DEFAULT_MIN_DAYS_BETWEEN_ALLY_INTERACTIONS = 5f;
        public const float DEFAULT_MTB_ALLY_INTERACTIONS = 6f;
        private const float MIN_VALUE = 0.0001f;
        private const float MAX_VALUE = 1000f;

        private static float globalMTBOT = DEFAULT_MTBOT;
        private static string globalMTBOTString = DEFAULT_MTBOT.ToString();
        public static float GlobalMTBOT
        {
            get { return globalMTBOT; }
            set { globalMTBOT = value; globalMTBOTString = value.ToString(); }
        }

        private static float gameMTBOT = DEFAULT_MTBOT;
        private static string gameMTBOTString = DEFAULT_MTBOT.ToString();
        public static float GameMTBOT
        {
            get { return gameMTBOT; }
            set { gameMTBOT = value; gameMTBOTString = value.ToString(); }
        }

        private static float minDaysBetweenAllyInteraction = DEFAULT_MIN_DAYS_BETWEEN_ALLY_INTERACTIONS;
        private static string minDaysBetweenAllyInteractionString = DEFAULT_MIN_DAYS_BETWEEN_ALLY_INTERACTIONS.ToString();
        public static float MinDaysBetweenAllyInteraction
        {
            get { return minDaysBetweenAllyInteraction; }
            set { minDaysBetweenAllyInteraction = value; minDaysBetweenAllyInteractionString = value.ToString(); }
        }

        private static float mtbAllyInteractions = DEFAULT_MTB_ALLY_INTERACTIONS;
        private static string mtbAllyInteractionsString = DEFAULT_MTB_ALLY_INTERACTIONS.ToString();
        public static float MtbAllyInteractions
        {
            get { return mtbAllyInteractions; }
            set { mtbAllyInteractions = value; mtbAllyInteractionsString = value.ToString(); }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref globalMTBOT, "AdjustableTradeShips.GlobalMTBOT", DEFAULT_MTBOT, false);
            GlobalMTBOT = globalMTBOT;
        }

        public static void DoSettingsWindowContents(Rect rect)
        {
            Text.Font = GameFont.Small;
            float y = 60;
            Widgets.Label(new Rect(0, y, 600, 40), "AdjustableTradeShips.SetMTBOT".Translate());
            y += 40;

            // Global MTBOT
            NumberInput(0, y, "AdjustableTradeShips.Global".Translate(), ref globalMTBOT, ref globalMTBOTString, MIN_VALUE, MAX_VALUE, DEFAULT_MTBOT);
            y += 40;

            // Current Game
            if (Current.Game != null)
            {
                Widgets.DrawLineHorizontal(20, y, rect.width - 40);
                y += 40;

                bool hasOrbitalTraders = StoryTellerUtil.HasOrbitalTraders();
                if (hasOrbitalTraders)
                {
                    // Game MTBOT
                    float origMTBOT = gameMTBOT;
                    NumberInput(0, y, "AdjustableTradeShips.CurrentGame".Translate(), ref gameMTBOT, ref gameMTBOTString, MIN_VALUE, MAX_VALUE, DEFAULT_MTBOT);

                    if (origMTBOT != gameMTBOT)
                    {
                        StoryTellerUtil.ApplyMTBOT(gameMTBOT);
                    }
                }
                else
                {
                    Widgets.Label(new Rect(0, y, 300, 20), Current.Game.storyteller.def.label + ": " + "AdjustableTradeShips.CannotModifyOrbitalTraderTimes".Translate());
                }
                y += 25;

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
                y += 25;
            }
        }

        private static void NumberInput(float x, float y, string label, ref float val, ref string buffer, float min, float max, float defaultValue)
        {
            try
            {
                Widgets.Label(new Rect(x, y, 175, 20), label);
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
                buffer = min.ToString();
            }
        }
    }
}