namespace SnapOut
{
    using System.Collections.Generic;
    using RimWorld;
    using UnityEngine;
    using Verse;
    using Verse.AI;

    /// <summary>
    /// Job driver. This is the first job assigned when calming down a pawn.
    /// </summary>
    public class JobDriver_CalmDown : JobDriver
    {
        private const TargetIndex subjecteeTargetIndex = TargetIndex.A;
        private Job recoverjob = new Job(SnapDefOf.SnappingOutJob);

        protected Toil AttemptCalm(TargetIndex ctrg)
        {
            Pawn subjectee = (Pawn)this.pawn.CurJob.targetA.Thing;
            var toil = new Toil
            {
                initAction = () =>
                {
                    if (subjectee == null) return;
                    
                    subjectee.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    this.TargetThingB = this.pawn; // Defining our initiator pawn
                    float rand = Rand.RangeSeeded(0f, 0.70f, Find.TickManager.TicksAbs);
                    pawn.interactions.TryInteractWith(subjectee, SnapDefOf.CalmDownInteraction);
                    float num = SnapUtils.DoFormula(pawn, subjectee);

                    if (SOMod.Settings.AlwaysSucceed) rand = 0f;
                    
                    SnapUtils.DebugLog(string.Format("Success chance of {0} opposed to failure chance of {1}", num.ToString(), rand.ToString()));
                    if (rand > num)
                    {
                        if (SOMod.Settings.MessagesEnabled) SnapUtils.CalmText(this.pawn, Color.red);
                        
                        if (subjectee.InAggroMentalState)
                        {
                            subjectee.TryStartAttack(pawn);
                            SnapUtils.DoStatusMessage(3, pawn, subjectee);
                            subjectee.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                            return;
                        }

                        SnapUtils.DoStatusMessage(2, pawn, subjectee);
                        subjectee.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                        SnapUtils.AttemptSendSafety(subjectee);
                        return;
                    }
                    
                    if (SOMod.Settings.MessagesEnabled) SnapUtils.CalmText(this.pawn, Color.green);
                    
                    if (subjectee.InAggroMentalState)
                    {
                        subjectee.MentalState.RecoverFromState();
                        subjectee.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Sad);
                    }
                    
                    SnapUtils.DoStatusMessage(1, pawn, subjectee);
                    
                    pawn.needs.mood.thoughts.memories.TryGainMemory(SnapDefOf.HelpedThought, null);
                    pawn.skills.Learn(SkillDefOf.Social, Rand.RangeSeeded(50, 125, Find.TickManager.TicksAbs));
                    subjectee.jobs.EndCurrentJob(JobCondition.Succeeded);
                    recoverjob.playerForced = true;
                    subjectee.jobs.StartJob(recoverjob, JobCondition.Succeeded);
                    subjectee.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                },
                socialMode = RandomSocialMode.Off,
                defaultCompleteMode = ToilCompleteMode.Instant,
                defaultDuration = SOMod.Settings.CalmDuration
            };
            return toil.WithProgressBarToilDelay(TargetIndex.B);
        }

        public override bool TryMakePreToilReservations(bool erroronfailed = false)
        {
            return this.pawn.Reserve(this.job.GetTarget(subjecteeTargetIndex), this.job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn subjectee = (Pawn)this.pawn.CurJob.targetA.Thing;
            var attemptCalmJob = AttemptCalm(subjecteeTargetIndex);

            this.FailOnDowned(subjecteeTargetIndex);
            this.FailOnDespawnedOrNull(subjecteeTargetIndex);
            this.FailOnNotAwake(subjecteeTargetIndex);

            yield return Toils_Goto.GotoThing(subjecteeTargetIndex, PathEndMode.Touch);
            yield return Toils_Interpersonal.WaitToBeAbleToInteract(this.pawn);
            yield return Toils_Interpersonal.GotoInteractablePosition(subjecteeTargetIndex);
            yield return Toils_General.Do(delegate
            {
                subjectee.rotationTracker.FaceCell(pawn.PositionHeld);
                if (subjectee.InAggroMentalState)
                {
                    float rand = Rand.RangeSeeded(0f, 0.85f, Find.TickManager.TicksAbs);
                    float socialImpact = pawn.GetStatValue(StatDefOf.SocialImpact, true);
                    socialImpact *= SOMod.Settings.StunWeight;
                    SnapUtils.DebugLog(string.Format("Aggressive stun success chance was {0} opposed to a failure chance of {1}", socialImpact.ToString(), rand.ToString()));
                    if (rand > socialImpact)
                    {
                        SnapUtils.DoStatusMessage(3, pawn, subjectee);
                        subjectee.TryStartAttack(pawn);
                        subjectee.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                        EndJobWith(JobCondition.Incompletable);
                    } else {
                        subjectee.stances.stunner.StunFor(SOMod.Settings.CalmDuration, pawn);
                    }
                }
            });
            yield return Toils_Interpersonal.GotoInteractablePosition(subjecteeTargetIndex);
            yield return Toils_General.WaitWith(subjecteeTargetIndex, SOMod.Settings.CalmDuration, true, true);
            yield return attemptCalmJob;
            yield return Toils_Interpersonal.SetLastInteractTime(subjecteeTargetIndex);
        }
    }
}