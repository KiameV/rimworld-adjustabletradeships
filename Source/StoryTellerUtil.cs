using RimWorld;
using Verse;

namespace AdjustableTradeShips
{
    static class StoryTellerUtil
    {
        public static bool HasOrbitalTraders()
        {
            StorytellerCompProperties_SingleMTB comp;
            return TryGetOrbitalTraders(out comp);
        }

        public static void ApplyMTBOT(float mtbot)
        {
            StorytellerCompProperties_SingleMTB comp;
            if (TryGetOrbitalTraders(out comp))
            {
                comp.mtbDays = mtbot;
            }
        }

        private static bool TryGetOrbitalTraders(out StorytellerCompProperties_SingleMTB comp)
        {
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
                            comp = mtb;
                            return true;
                        }
                    }
                }
            }
            comp = null;
            return false;
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

        public static void ApplyAllyInteraction(float minDaysAllyInteractions, float mtbAllyInteractions)
        {
            StorytellerCompProperties_FactionInteraction comp;
            if (TryGetAllyInteraction(out comp))
            {
                comp.minDaysPassed = minDaysAllyInteractions;
                comp.baseMtbDays = mtbAllyInteractions;
            }
        }

        private static bool TryGetAllyInteraction(out StorytellerCompProperties_FactionInteraction comp)
        {
            if (Current.Game != null && Current.Game.storyteller != null)
            {
                StorytellerDef d = Current.Game.storyteller.def;
                foreach (StorytellerCompProperties c in d.comps)
                {
                    comp = c as StorytellerCompProperties_FactionInteraction;
                    if (comp != null)
                    {
                        return true;
                    }
                }
            }
            comp = null;
            return false;
        }
    }
}
