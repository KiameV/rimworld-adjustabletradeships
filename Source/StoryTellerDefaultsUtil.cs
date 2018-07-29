using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdjustableTradeShips
{
    class StoryTellerDefaultsUtil
    {
        public static StoryTellerDefaults defaultDefaults;
        public static readonly List<StoryTellerDefaults> Defaults = new List<StoryTellerDefaults>();

        public static void Init()
        {
            if (Defaults.Count == 0)
            {
                foreach (StorytellerDef def in DefDatabase<StorytellerDef>.AllDefs)
                {
                    List<OnOffIncident> onOffIncidents = new List<OnOffIncident>();
                    List<FactionInteraction> interactions = new List<FactionInteraction>();

                    foreach(StorytellerCompProperties comp in def.comps)
                    {
                        if (comp is StorytellerCompProperties_OnOffCycle)
                        {
                            StorytellerCompProperties_OnOffCycle ooc = (StorytellerCompProperties_OnOffCycle)comp;
                            onOffIncidents.Add(new OnOffIncident
                            {
                                Incident = ooc.incident,
                                OnDays = ooc.onDays,
                                OffDays = ooc.offDays,
                                MinInstances = ooc.numIncidentsRange.min,
                                MaxInstances = ooc.numIncidentsRange.max,
                            });
                        }
                        else if (comp is StorytellerCompProperties_FactionInteraction)
                        {
                            StorytellerCompProperties_FactionInteraction fi = (StorytellerCompProperties_FactionInteraction)comp;
                            interactions.Add(new FactionInteraction
                            {
                                Incident = fi.incident,
                                IncidenctsPerYear = fi.baseIncidentsPerYear,
                                MinSpacingDays = fi.minSpacingDays,
                                FullAlliesOnly = fi.fullAlliesOnly,
                                MinDanger = fi.minDanger,
                                MinDaysPassed = fi.minDaysPassed
                            });
                        }
                    }
                    Defaults.Add(new StoryTellerDefaults(def, onOffIncidents, interactions));
                }
                List<OnOffIncident> onOffCycle = new List<OnOffIncident>(1);
                onOffCycle.Add(new OnOffIncident
                {
                    Incident = IncidentDefOf.OrbitalTraderArrival,
                    OnDays = 7,
                    OffDays = 8,
                    MinInstances = FloatRange.One.min,
                    MaxInstances = FloatRange.One.max,
                });
                defaultDefaults = new StoryTellerDefaults(null, onOffCycle, new List<FactionInteraction>(0));
            }
        }

        public static StoryTellerDefaults GetStoryTellerDefaults(StorytellerDef def)
        {
            foreach(StoryTellerDefaults d in Defaults)
            {
                if (d.StorytellerDef == def)
                {
                    return d;
                }
            }
            return defaultDefaults;
        }
    }

    public class StoryTellerDefaults
    {
        public readonly StorytellerDef StorytellerDef;
        public readonly IEnumerable<OnOffIncident> OnOffIncidents;
        public readonly IEnumerable<FactionInteraction> Interactions;

        public StoryTellerDefaults(
            StorytellerDef storytellerDef,
            IEnumerable<OnOffIncident> onOffIncidents, 
            IEnumerable<FactionInteraction> interactions)
        {
            this.StorytellerDef = storytellerDef;
            this.OnOffIncidents = onOffIncidents;
            this.Interactions = interactions;
        }

        public bool TryGetIncident(IncidentDef incident, out OnOffIncident onOffIncident)
        {
            foreach (OnOffIncident ooi in OnOffIncidents)
            {
                if (ooi.Incident == incident)
                {
                    onOffIncident = ooi;
                    return true;
                }
            }
            onOffIncident = null;
            return false;
        }

        public bool TryGetInteraction(IncidentDef incident, out FactionInteraction interaction)
        {
            foreach (FactionInteraction fi in Interactions)
            {
                if (fi.Incident == incident)
                {
                    interaction = fi;
                    return true;
                }
            }
            interaction = null;
            return false;
        }
    }

    public class FactionInteraction : IExposable
    {
        public IncidentDef Incident;
        public float IncidenctsPerYear;
        public float MinSpacingDays;
        public bool FullAlliesOnly;
        public StoryDanger MinDanger;
        public float MinDaysPassed;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref Incident, "incident");
            Scribe_Values.Look(ref IncidenctsPerYear, "incidenctsPerYear");
            Scribe_Values.Look(ref MinSpacingDays, "minSpacingDays");
            Scribe_Values.Look(ref FullAlliesOnly, "fullAlliesOnly");
            Scribe_Values.Look(ref MinDanger, "minDanger");
            Scribe_Values.Look(ref MinDaysPassed, "minDaysPassed");
        }
    }

    public class OnOffIncident : IExposable
    {
        public IncidentDef Incident;
        public float OnDays;
        public float OffDays;
        public float MinInstances;
        public float MaxInstances;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref Incident, "incident");
            Scribe_Values.Look(ref OnDays, "onDays");
            Scribe_Values.Look(ref OffDays, "offDays");
            Scribe_Values.Look(ref MinInstances, "minInstances");
            Scribe_Values.Look(ref MaxInstances, "maxInstances");
        }
    }
}
