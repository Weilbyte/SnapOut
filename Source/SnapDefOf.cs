using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;


namespace SnapOut
{
    //References to our defs
    public static class SnapDefOf
    {
        public static JobDef CalmDownJob = DefDatabase<JobDef>.GetNamed("SnapOut_CalmDownJob"); //Formerly CalmDown
        public static JobDef SnappingOutJob = DefDatabase<JobDef>.GetNamed("SnapOut_SnappingOutJob"); //Formerly SnappingOut
        public static JobDef GoToSafetyJob = DefDatabase<JobDef>.GetNamed("SnapOut_GoToSafetyJob"); //Formerly GoToSafety
        public static JobDef ActuallyRecoverJob = DefDatabase<JobDef>.GetNamed("SnapOut_ActuallyRecoverJob");
        public static InteractionDef CalmDownInteraction = DefDatabase<InteractionDef>.GetNamed("SnapOut_CalmDownInteraction"); //Formerly CalmDownInt
        public static ThoughtDef HelpedThought = DefDatabase<ThoughtDef>.GetNamed("SnapOut_HelpedSomeoneThought"); //Formerly CDGaveCareThought
    }
}
