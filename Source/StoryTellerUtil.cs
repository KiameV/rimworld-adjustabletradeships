using RimWorld;
using Verse;

namespace AdjustableTradeShips
{
    static class StoryTellerUtil
    {
        public static bool HasOrbitalTraders()
        {
            return TryGetOrbitalTraders(out StorytellerCompProperties_OnOffCycle comp);
        }

        public static void ApplyOrbitalTrade(float days, float instances)
        {
            StorytellerCompProperties_OnOffCycle comp;
            if (TryGetOrbitalTraders(out comp))
            {
                Log.Message($"Orbitabl Trader On/Off Cycle applied.");
                comp.onDays = days;
                comp.offDays = 0.001f;
                comp.numIncidentsRange.min = instances;
                comp.numIncidentsRange.max = instances;
            };
        }

        public static bool TryGetOrbitalTraders(out StorytellerCompProperties_OnOffCycle comp)
        {
            if (Current.Game != null && Current.Game.storyteller != null)
            {
                StorytellerDef d = Current.Game.storyteller.def;
                foreach (StorytellerCompProperties c in d.comps)
                {
                    StorytellerCompProperties_OnOffCycle ooc = c as StorytellerCompProperties_OnOffCycle;
                    if (ooc != null && ooc.incident == IncidentDefOf.OrbitalTraderArrival)
                    {
                        comp = ooc;
                        return true;
                    }
                }
            }
            comp = null;
            return false;
        }

        public static bool HasRandom()
        {
            return TryGetRandom(out StorytellerCompProperties_RandomMain comp);
        }

        public static bool ApplyRandom(IncidentCategoryDef def, float weight)
        {
            bool applied = false;
            if (TryGetRandom(out StorytellerCompProperties_RandomMain comp))
            {
                foreach (IncidentCategoryEntry e in comp.categoryWeights)
                {
                    if (e.category == def)
                    {
                        Log.Message($"{def.defName} weight of {weight} has been applied.");
                        e.weight = weight;
                        applied = true;
                    }
                }
            }
            return applied;
        }

        public static bool TryGetRandomWeight(IncidentCategoryDef def, out float weight)
        {
            if (TryGetRandom(out StorytellerCompProperties_RandomMain comp))
            {
                foreach (IncidentCategoryEntry e in comp.categoryWeights)
                {
                    if (e.category == def)
                    {
                        weight = e.weight;
                        return true;
                    }
                }
            }
            weight = 0;
            return false;
        }

        private static bool TryGetRandom(out StorytellerCompProperties_RandomMain comp)
        {
            comp = null;
            if (Current.Game != null && Current.Game.storyteller != null)
            {
                StorytellerDef d = Current.Game.storyteller.def;
                foreach (StorytellerCompProperties c in d.comps)
                {
                    if (c is StorytellerCompProperties_RandomMain rm)
                    {
                        comp = rm;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HasInteraction(IncidentDef def)
        {
            StorytellerCompProperties_FactionInteraction comp;
            return TryGetAllyInteraction(def, out comp);
        }

        public static void ApplyAllyInteraction(IncidentDef def, StorytellerCompProperties_FactionInteraction fi)
        {
            StorytellerCompProperties_FactionInteraction comp;
            if (TryGetAllyInteraction(def, out comp))
            {
                comp.minDaysPassed = fi.minDaysPassed;
                comp.baseIncidentsPerYear = fi.baseIncidentsPerYear;
                comp.minSpacingDays = fi.minSpacingDays;
                comp.minDaysPassed = fi.minDaysPassed;
            }
        }

        private static bool TryGetAllyInteraction(IncidentDef def, out StorytellerCompProperties_FactionInteraction comp)
        {
            if (Current.Game != null && Current.Game.storyteller != null)
            {
                StorytellerDef d = Current.Game.storyteller.def;
                foreach (StorytellerCompProperties c in d.comps)
                {
                    comp = c as StorytellerCompProperties_FactionInteraction;
                    if (comp != null && comp.incident == def)
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
