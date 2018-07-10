using RimWorld;
using Verse;

namespace AdjustableTradeShips
{
    static class StoryTellerUtil
    {
        public static StorytellerDef ModifiedAllyInteractions = null;
        public static StorytellerDef ModifiedOrbitalTrader = null;

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
                        if (mtb != null && 
                            mtb.incident == IncidentDefOf.OrbitalTraderArrival)
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

        public static void AddOrbitalTraders()
        {
            if (HasOrbitalTraders())
            {
                Log.Warning("Not adding Orbital Traders");
                return;
            }

            RemoveOrbitalTraders();
            Log.Warning("Adding Orbital Traders");
            ModifiedOrbitalTrader = Current.Game.storyteller.def;
            StorytellerCompProperties_SingleMTB c = new StorytellerCompProperties_SingleMTB();
            c.incident = IncidentDefOf.OrbitalTraderArrival;
            c.mtbDays = Settings.GameMTBOT;
            ModifiedOrbitalTrader.comps.Add(c);
        }

        public static void AddAllyInteraction()
        {
            if (HasAllyInteraction())
            {
                Log.Warning("Not adding Ally Interactions");
                return;
            }

            RemoveAllyInteraction();
            Log.Warning("Adding Ally Interactions");
            ModifiedAllyInteractions = Current.Game.storyteller.def;
            StorytellerCompProperties_FactionInteraction c = new StorytellerCompProperties_FactionInteraction();
            c.minDaysPassed = Settings.MinDaysBetweenAllyInteraction;
            c.baseMtbDays = Settings.MtbAllyInteractions;
            ModifiedAllyInteractions.comps.Add(c);
        }

        public static void RemoveOrbitalTraders()
        {
            if (ModifiedOrbitalTrader != null)
            {
                StorytellerDef d = ModifiedOrbitalTrader;
                ModifiedOrbitalTrader = null;
                for (int i = 0; i < d.comps.Count; ++i)
                {
                    StorytellerCompProperties_SingleMTB comp = d.comps[i] as StorytellerCompProperties_SingleMTB;
                    if (comp != null &&
                        comp.incident == IncidentDefOf.OrbitalTraderArrival)
                    {
                        d.comps.RemoveAt(i);
                        Log.Warning("Remove Orbital Traders");
                        return;
                    }
                }
            }
            Log.Warning("Not removing Orbital Traders");
        }

        public static void RemoveAllyInteraction()
        {
            if (ModifiedAllyInteractions != null)
            {
                StorytellerDef d = ModifiedAllyInteractions;
                ModifiedAllyInteractions = null;
                for (int i = 0; i < d.comps.Count; ++i)
                {
                    StorytellerCompProperties_FactionInteraction comp = d.comps[i] as StorytellerCompProperties_FactionInteraction;
                    if (comp != null)
                    {
                        d.comps.RemoveAt(i);
                        Log.Warning("Remove Ally Interactions");
                        return;
                    }
                }
            }
            Log.Warning("Not removing Ally Interactions");
        }
    }
}
