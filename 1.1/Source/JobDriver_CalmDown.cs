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
        private const TargetIndex PieceOfShit = TargetIndex.A;
        private Job recoverjob = new Job(SnapDefOf.SnappingOutJob);
        private Job gotosafetyjob = new Job(SnapDefOf.GoToSafetyJob);

        protected Toil CalmDown(TargetIndex ctrg, int dur)
        {
            Pawn pieceofs = (Pawn)this.pawn.CurJob.targetA.Thing;
            var toil = new Toil
            {
                initAction = () =>
                {
                    if (pieceofs == null)
                    {
                        return;
                    }

                    pieceofs.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    this.TargetThingB = this.pawn; // Defining our initiator pawn
                    float rand = Rand.RangeSeeded(0f, 0.70f, Find.TickManager.TicksAbs);
                    pawn.interactions.TryInteractWith(pieceofs, SnapDefOf.CalmDownInteraction);
                    float num = SnapUtils.DoFormula(pawn, pieceofs);
                    
                    if (SOMod.Settings.AlwaysSucceed)
                    {
                        rand = 0f;
                        num = 1f;
                    }
                    SnapUtils.DebugLog(string.Format("Success chance of {0} opposed to failure chance of {1}", num.ToString(), rand.ToString()));
                    if (rand > num)
                    {
                        if (SOMod.Settings.MessagesEnabled)
                        {
                            MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, SnapUtils.GetCalmingMessage(), Color.red, 3.85f);
                        }

                        if (pieceofs.InAggroMentalState)
                        {
                            pieceofs.TryStartAttack(pawn);
                            SnapUtils.DoStatusMessage(3, pawn, pieceofs);
                            pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                            return;
                        }

                        SnapUtils.DoStatusMessage(2, pawn, pieceofs);
                        pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                        Room bedroom = pieceofs.ownership.OwnedRoom;
                        if (bedroom != null)
                        {
                            int srand = Rand.RangeSeeded(0, 100, Find.TickManager.TicksAbs);
                            SnapUtils.DebugLog(pieceofs.Name.ToStringShort + " has a bedroom. Chance to get job is.. " + srand);
                            if (srand <= 65) // 65% chance
                            {
                                SnapUtils.DebugLog(pieceofs.Name.ToStringShort + " received gotosafety job!");
                                gotosafetyjob.playerForced = true;
                                gotosafetyjob.locomotionUrgency = LocomotionUrgency.Jog;
                                pieceofs.jobs.EndCurrentJob(JobCondition.Succeeded);
                                pieceofs.jobs.StartJob(gotosafetyjob, JobCondition.Succeeded);
                            }
                            else
                            {
                                SnapUtils.DebugLog(pieceofs.Name.ToStringShort + " didnt receive gotosafety job!");
                            }
                        }

                        return;
                    }

                    if (SOMod.Settings.MessagesEnabled)
                    {
                        MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, SnapUtils.GetCalmingMessage(), Color.green, 3.85f);
                    }

                    pawn.needs.mood.thoughts.memories.TryGainMemory(SnapDefOf.HelpedThought, null);
                    pawn.skills.Learn(SkillDefOf.Social, Rand.RangeSeeded(50, 125, Find.TickManager.TicksAbs));
                    SnapUtils.DoStatusMessage(1, pawn, pieceofs);
                    if (pieceofs.InAggroMentalState)
                    {
                        pieceofs.MentalState.RecoverFromState();
                        pieceofs.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Sad);
                    }

                    recoverjob.playerForced = true;
                    pieceofs.jobs.EndCurrentJob(JobCondition.Succeeded);
                    pieceofs.jobs.StartJob(recoverjob, JobCondition.Succeeded);
                    pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                },
                socialMode = RandomSocialMode.Off,
                defaultCompleteMode = ToilCompleteMode.Instant,
                defaultDuration = SOMod.Settings.CalmDuration
            };
            return toil.WithProgressBarToilDelay(TargetIndex.B);
        }

        public override bool TryMakePreToilReservations(bool erroronfailed = false)
        {
            return this.pawn.Reserve(this.job.GetTarget(PieceOfShit), this.job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn pieceofs = (Pawn)this.pawn.CurJob.targetA.Thing;
            var cdown = CalmDown(PieceOfShit, SOMod.Settings.CalmDuration);

            this.FailOnDowned(PieceOfShit);
            this.FailOnDespawnedOrNull(PieceOfShit);
            this.FailOnNotAwake(PieceOfShit);

            yield return Toils_Goto.GotoThing(PieceOfShit, PathEndMode.Touch);
            yield return Toils_Interpersonal.WaitToBeAbleToInteract(this.pawn);
            yield return Toils_Interpersonal.GotoInteractablePosition(PieceOfShit);
            yield return Toils_General.Do(delegate
            {
                pieceofs.rotationTracker.FaceCell(pawn.PositionHeld);
                if (pieceofs.InAggroMentalState)
                {
                    float rand = Rand.RangeSeeded(0f, 0.85f, Find.TickManager.TicksAbs);
                    float socialImpact = pawn.GetStatValue(StatDefOf.SocialImpact, true);
                    socialImpact *= SOMod.Settings.StunWeight;
                    SnapUtils.DebugLog(string.Format("Aggressive stun success chance was {0} opposed to a failure chance of {1}", socialImpact.ToString(), rand.ToString()));
                    if (rand > socialImpact)
                    {
                        SnapUtils.DoStatusMessage(3, pawn, pieceofs);
                        pieceofs.TryStartAttack(pawn);
                        pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                        EndJobWith(JobCondition.Incompletable);
                    } else {
                        pieceofs.stances.stunner.StunFor(SOMod.Settings.CalmDuration, pawn);
                    }
                }
            });
            yield return Toils_Interpersonal.GotoInteractablePosition(PieceOfShit);
            yield return Toils_General.WaitWith(PieceOfShit, SOMod.Settings.CalmDuration, true, true);
            yield return cdown;
            yield return Toils_Interpersonal.SetLastInteractTime(PieceOfShit);
        }
    }
}