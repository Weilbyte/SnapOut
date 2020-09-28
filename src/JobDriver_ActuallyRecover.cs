namespace SnapOut
{
    using RimWorld;
    using System.Collections.Generic;
    using Verse;
    using Verse.AI;

    /// <summary>
    /// Job driver. Calls RecoverFromState on the pawn - recovering it from its mental breakdown.
    /// </summary>
    public class JobDriver_ActuallyRecover : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_General.Wait(1);
            this.pawn.MentalState.RecoverFromState();
            if (SOMod.Settings.DisableCath) this.pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.Catharsis);
            this.pawn.jobs.ClearQueuedJobs();
            this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
        }
    }
}