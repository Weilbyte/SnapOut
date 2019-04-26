namespace SnapOut
{
    using RimWorld;
    using Verse;

    public static class SnapDefOf
    {
        public static JobDef CalmDownJob = DefDatabase<JobDef>.GetNamed("SnapOut_CalmDownJob"); 
        public static JobDef SnappingOutJob = DefDatabase<JobDef>.GetNamed("SnapOut_SnappingOutJob"); 
        public static JobDef GoToSafetyJob = DefDatabase<JobDef>.GetNamed("SnapOut_GoToSafetyJob"); 
        public static JobDef ActuallyRecoverJob = DefDatabase<JobDef>.GetNamed("SnapOut_ActuallyRecoverJob");
        public static InteractionDef CalmDownInteraction = DefDatabase<InteractionDef>.GetNamed("SnapOut_CalmDownInteraction");
        public static ThoughtDef HelpedThought = DefDatabase<ThoughtDef>.GetNamed("SnapOut_HelpedSomeoneThought");
    }
}
