
using RimWorld;
using Verse;
using Verse.AI;

namespace SnapOut
{
    class WorkGiver_CalmPawnDown : WorkGiver_Warden_Chat
    {
        #region shouldskip
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return base.ShouldSkip(pawn) && (pawn.Faction != Faction.OfPlayer) && (!pawn.RaceProps.Humanlike);           
        }
        #endregion

        #region jobonthing
        public override Job JobOnThing(Pawn pawn, Thing thang, bool forced = true)
        {
            Pawn pawn2 = (Pawn)thang;
            if (pawn2.RaceProps.Humanlike)
            {
                if (pawn2.InMentalState) 
                {
                    if (SnapUtils.breakCompatCheck(pawn2.MentalState.def.ToString()))
                    {
                        SnapUtils.logThis(pawn2.MentalState.def.ToString());
                        SnapUtils.logThis(pawn2.Name.ToStringShort + " has met HumanLike and InMentalState conditions.");
                        bool recent = Find.TickManager.TicksGame < pawn2.mindState.lastAssignedInteractTime + 15000;
                        if (SnapUtils.canDo(pawn2) && SnapUtils.IsCapableOf(pawn) && !recent && pawn.CanReserve(pawn2)) //Only on non-aggressive mental state pawns
                        {
                            SnapUtils.logThis("Calming job initiated on " + pawn2.Name.ToStringShort + " by " + pawn.Name.ToStringShort);
                            return new Job(SnapDefOf.CalmDownJob, pawn2);
                        }
                    }               
                    return null;
                }
                return null;
            }
            return null;
        }
        #endregion 
    }


}
