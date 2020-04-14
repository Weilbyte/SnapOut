namespace SnapOut
{
    using RimWorld;
    using Verse;
    using Verse.AI;

    public class WorkGiver_CalmPawnDown : WorkGiver_Warden_Chat
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return base.ShouldSkip(pawn) && (pawn.Faction != Faction.OfPlayer) && (!pawn.RaceProps.Humanlike);
        }

        public override Job JobOnThing(Pawn pawn, Thing targetThing, bool forced = true)
        {
            if ((targetThing == null) || (pawn == targetThing))
            {
                return null;
            } else {
                Pawn targetPawn = (Pawn)targetThing;
                if (targetPawn.RaceProps.Humanlike && targetPawn.InMentalState)
                {
                    if (SnapCheck.CompatCheck(targetPawn.MentalState.def.ToString()))
                    {
                        bool recent = Find.TickManager.TicksGame < targetPawn.mindState.lastAssignedInteractTime + SOMod.Settings.Cooldown;
                        if (pawn.CurJobDef != SnapDefOf.CalmDownJob)
                        {
                            if (SnapCheck.CanDo(targetPawn) && SnapCheck.IsCapable(pawn) && !recent && pawn.CanReserve(targetPawn))
                            {
                                SnapUtils.DebugLog(string.Format("{0} given calm job with {1} as target", pawn.Name.ToStringShort, targetPawn.Name.ToStringShort));
                                return new Job(SnapDefOf.CalmDownJob, targetPawn);
                            }
                        }
                        SnapUtils.DebugLog(string.Format("{0} has been considered but conditions were not fulfilled", targetPawn.Name.ToStringShort));
                        return null;
                    }
                    else
                    {
                        SnapUtils.DebugLog(string.Format("{0} mental state def has failed compatability check", targetPawn.MentalState.def.ToString()));
                        return null;
                    }
                }
            }
            return null;
        }
    }
}