using System.Collections.Generic;
using RimWorld;
using Verse.AI;
using Verse;

namespace SnapOut
{
    class JobDriver_SnappingOut : JobDriver
    {
        #region toilreservations
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        #endregion

        #region toilstuffs
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn rpawn = this.pawn;
            this.TargetThingB = this.pawn;
            Building ownedbed = this.pawn.ownership.OwnedBed;
            Job actuallyrecoverjob = new Job(SnapDefOf.ActuallyRecoverJob);
            if (ownedbed != null)
            {
                if (pawn.guest.IsPrisoner)
                {
                    IntVec3 c = RCellFinder.RandomWanderDestFor(rpawn, rpawn.Position, 0.3f, null, Danger.None); 
                    yield return Toils_Goto.GotoCell(c, PathEndMode.OnCell);
                }
                yield return Toils_Goto.GotoCell(ownedbed.Position, PathEndMode.OnCell);
            }
            else
            {
                IntVec3 c = RCellFinder.RandomWanderDestFor(rpawn, rpawn.Position, 2f, null, Danger.None);
                yield return Toils_Goto.GotoCell(c, PathEndMode.OnCell);
            }
            Toil waitonspot = Toils_General.Wait(500);

            waitonspot.socialMode = RandomSocialMode.Off;
            yield return waitonspot;
            Toil snappingout = Toils_General.Do(delegate
            {
                rpawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                actuallyrecoverjob.playerForced = true;
                rpawn.jobs.StartJob(actuallyrecoverjob, JobCondition.Succeeded);
            });
            snappingout.socialMode = RandomSocialMode.Off;
            yield return snappingout;            
        }
        #endregion
    }
}