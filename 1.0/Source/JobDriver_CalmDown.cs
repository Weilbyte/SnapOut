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
                    float rand = UnityEngine.Random.Range(0f, 0.70f);
                    pawn.interactions.TryInteractWith(pieceofs, SnapDefOf.CalmDownInteraction);
                    float num = SnapUtils.DoFormula(pawn, pieceofs);
                    SnapUtils.DebugLog("Calm chance was " + num.ToString() + " versus random of " + rand.ToString());
                    if (rand > num)
                    {
                        if (SOMod.Settings.SOmessagesEnabled)
                        {
                            MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, SnapUtils.GetCalmingMessage, Color.red, 3.85f);
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
                            int srand = Random.Range(0, 100);
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

                    if (SOMod.Settings.SOmessagesEnabled)
                    {
                        MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, SnapUtils.GetCalmingMessage, Color.green, 3.85f);
                    }

                    pawn.needs.mood.thoughts.memories.TryGainMemory(SnapDefOf.HelpedThought, null);
                    pawn.skills.Learn(SkillDefOf.Social, Rand.Range(50, 125));
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
                defaultDuration = SOMod.Settings.SOCalmDuration
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
            var cdown = CalmDown(PieceOfShit, SOMod.Settings.SOCalmDuration);

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
                    float randa = UnityEngine.Random.Range(0f, 0.85f);
                    float numba = pawn.GetStatValue(StatDefOf.SocialImpact, true);
                    numba = numba * SOMod.Settings.SOStunWeight;
                    SnapUtils.DebugLog("Aggressive stun chance was " + numba.ToString() + " versus random of " + randa.ToString());
                    if (randa > numba)
                    {
                        SnapUtils.DoStatusMessage(3, pawn, pieceofs);
                        pieceofs.TryStartAttack(pawn);
                        pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                        EndJobWith(JobCondition.Incompletable);
                    }

                    if (numba > randa)
                    {
                        pieceofs.stances.stunner.StunFor(SOMod.Settings.SOCalmDuration, pawn);
                    }
                }
            });
            yield return Toils_Interpersonal.GotoInteractablePosition(PieceOfShit);
            yield return Toils_General.WaitWith(PieceOfShit, SOMod.Settings.SOCalmDuration, true, true);
            yield return cdown;
            yield return Toils_Interpersonal.SetLastInteractTime(PieceOfShit);
        }
    }
}