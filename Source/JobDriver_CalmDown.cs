using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SnapOut

{
    public class JobDriver_CalmDown : JobDriver
    {
        #region variables

        private const TargetIndex pieceofshit = TargetIndex.A;
        private Job recoverjob = new Job(SnapDefOf.SnappingOutJob);
        private Job gotosafetyjob = new Job(SnapDefOf.GoToSafetyJob);

        #endregion variables

        #region MainToil

        protected Toil CalmDown(TargetIndex CTrg, int dur)
        {
            Pawn pieceofs = (Pawn)this.pawn.CurJob.targetA.Thing;
            var toil = new Toil
            {
                initAction = () =>
                {
                    pieceofs.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    this.TargetThingB = this.pawn; //Defining our initiator pawn
                    float rand = UnityEngine.Random.Range(0f, 0.70f);
                    pawn.interactions.TryInteractWith(pieceofs, SnapDefOf.CalmDownInteraction);
                    float num = SnapUtils.doFormula(pawn, pieceofs);
                    SnapUtils.logThis("Calm chance was " + num.ToString() + " versus random of " + rand.ToString());
                    if (rand > num)
                    {
                        #region failcondition

                        if (SOMod.settings.SOmessagesEnabled)
                        {
                            MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, SnapUtils.GetCalmingMessage, Color.red, 3.85f);
                        }
                        if (pieceofs.InAggroMentalState)
                        {
                            pieceofs.TryStartAttack(pawn);
                            SnapUtils.doStatusMessage(3, pawn, pieceofs);
                            pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                            return;
                        }

                        SnapUtils.doStatusMessage(2, pawn, pieceofs);
                        pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                        Room bedroom = pieceofs.ownership.OwnedRoom;
                        if (bedroom != null)
                        {
                            int srand = Random.Range(0, 100);
                            SnapUtils.logThis(pieceofs.Name.ToStringShort + " has a bedroom. Chance to get job is.. " + srand);
                            if (srand <= 65) //65% chance
                            {
                                SnapUtils.logThis(pieceofs.Name.ToStringShort + " received gotosafety job!");
                                gotosafetyjob.playerForced = true;
                                gotosafetyjob.locomotionUrgency = LocomotionUrgency.Jog;
                                pieceofs.jobs.EndCurrentJob(JobCondition.Succeeded);
                                pieceofs.jobs.StartJob(gotosafetyjob, JobCondition.Succeeded);
                            }
                            else
                            {
                                SnapUtils.logThis(pieceofs.Name.ToStringShort + " didnt receive gotosafety job!");
                            }
                        }
                        return;

                        #endregion failcondition
                    }

                    #region successcondition

                    if (SOMod.settings.SOmessagesEnabled)
                    {
                        MoteMaker.ThrowText(this.pawn.DrawPos + this.pawn.Drawer.renderer.BaseHeadOffsetAt(this.pawn.Rotation), this.pawn.Map, SnapUtils.GetCalmingMessage, Color.green, 3.85f);
                    }
                    pawn.needs.mood.thoughts.memories.TryGainMemory(SnapDefOf.HelpedThought, null);
                    pawn.skills.Learn(SkillDefOf.Social, Rand.Range(50, 125));
                    SnapUtils.doStatusMessage(1, pawn, pieceofs);
                    if (pieceofs.InAggroMentalState) { pieceofs.MentalState.RecoverFromState(); pieceofs.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Sad); }
                    recoverjob.playerForced = true;
                    pieceofs.jobs.EndCurrentJob(JobCondition.Succeeded);
                    pieceofs.jobs.StartJob(recoverjob, JobCondition.Succeeded);
                    pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;

                    #endregion successcondition
                },
                socialMode = RandomSocialMode.Off,
                defaultCompleteMode = ToilCompleteMode.Instant,
                defaultDuration = SOMod.settings.SOCalmDuration
            };
            return toil.WithProgressBarToilDelay(TargetIndex.B);
        }

        #endregion MainToil

        #region toilreservation

        public override bool TryMakePreToilReservations(bool erroronfailed = false)
        {
            return this.pawn.Reserve(this.job.GetTarget(pieceofshit), this.job, 1, -1, null);
        }

        #endregion toilreservation

        #region toilstuffs

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn pieceofs = (Pawn)this.pawn.CurJob.targetA.Thing;
            var cdown = CalmDown(pieceofshit, SOMod.settings.SOCalmDuration);

            this.FailOnDowned(pieceofshit);
            this.FailOnDespawnedOrNull(pieceofshit);
            this.FailOnNotAwake(pieceofshit);

            yield return Toils_Goto.GotoThing(pieceofshit, PathEndMode.Touch);
            yield return Toils_Interpersonal.WaitToBeAbleToInteract(this.pawn);
            yield return Toils_Interpersonal.GotoInteractablePosition(pieceofshit);
            yield return Toils_General.Do(delegate
            {
                SnapUtils.modCompatDoer(pieceofs.MentalState.def.ToString(), pawn, pieceofs);
                pieceofs.rotationTracker.FaceCell(pawn.PositionHeld);
                if (pieceofs.InAggroMentalState)
                {
                    float randa = UnityEngine.Random.Range(0f, 0.85f);
                    float numba = pawn.GetStatValue(StatDefOf.SocialImpact, true);
                    numba = numba * SOMod.settings.SOStunWeight;
                    SnapUtils.logThis("Aggressive stun chance was " + numba.ToString() + " versus random of " + randa.ToString());
                    if (randa > numba)
                    {
                        SnapUtils.doStatusMessage(3, pawn, pieceofs);
                        pieceofs.TryStartAttack(pawn);
                        pieceofs.mindState.lastAssignedInteractTime = Find.TickManager.TicksGame;
                        EndJobWith(JobCondition.Incompletable);
                    }
                    if (numba > randa) pieceofs.stances.stunner.StunFor(SOMod.settings.SOCalmDuration, pawn);
                }
            });
            yield return Toils_Interpersonal.GotoInteractablePosition(pieceofshit);
            yield return Toils_General.WaitWith(pieceofshit, SOMod.settings.SOCalmDuration, true, true);
            yield return cdown;
            yield return Toils_Interpersonal.SetLastInteractTime(pieceofshit);
        }

        #endregion toilstuffs
    }
}