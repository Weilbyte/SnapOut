namespace SnapOut
{
    using System.Collections.Generic;
    using System.Linq;
    using RimWorld;
    using Verse.AI;
    using UnityEngine;
    using Verse;

    internal class SnapUtils
    {
        /// <summary>
        /// Gets a random calming message from the translation files. Picks random message with ID between 1-21.
        /// </summary>
        public static string GetCalmingMessage()
        {
            int rand = Rand.RangeSeeded(1, 21, Find.TickManager.TicksAbs);
            string cmrand = "CM" + rand;
            DebugLog(string.Format("Calming message ID is {0}", cmrand));
            return cmrand.Translate();
        }

        /// <summary>
        /// Logs debug message to the game's console
        /// </summary>
        /// <param name="message">Message</param>
        public static void DebugLog(string message)
        {
            if (SOMod.Settings.Debug)
            {
                Log.Message("SnapOut :: " + message);
            }
        }

        /// <summary>    
        /// Creates comforting text above the pawn
        /// </summary>
        /// <param name="pawn">Pawn to create text above</param>
        /// <param name="color">Color of the text</param>
        public static void CalmText(Pawn pawn, Color color)
        {
            MoteMaker.ThrowText(pawn.DrawPos + pawn.Drawer.renderer.BaseHeadOffsetAt(pawn.Rotation), pawn.Map, GetCalmingMessage(), color, 3.85f);
        }

        /// <summary>
        /// Attempt to send pawn to safety
        /// </summary>
        /// <param name="subjectee">Pawn to send to safety</param>
        public static void AttemptSendSafety(Pawn subjectee)
        {
            Job goToSafetyJob = new Job(SnapDefOf.GoToSafetyJob);
            if (subjectee.ownership.OwnedRoom != null)
            {
                int srand = Rand.RangeSeeded(0, 100, Find.TickManager.TicksAbs);
                SnapUtils.DebugLog(subjectee.Name.ToStringShort + " has a bedroom. Chance to get sent to safety.. " + srand);
                if (srand <= 65) // 65% chance
                {
                    SnapUtils.DebugLog(subjectee.Name.ToStringShort + " has been sent to safety!");
                    goToSafetyJob.playerForced = true;
                    goToSafetyJob.locomotionUrgency = LocomotionUrgency.Jog;
                    subjectee.jobs.EndCurrentJob(JobCondition.Succeeded);
                    subjectee.jobs.StartJob(goToSafetyJob, JobCondition.Succeeded);
                }
                else
                {
                    SnapUtils.DebugLog(subjectee.Name.ToStringShort + " has not been sent to safety!");
                }
            }
        }

        /// <summary>
        /// Runs the chance formula
        /// </summary>
        /// <param name="doer">Pawn</param>
        /// <param name="subjectee">Target pawn</param>
        /// <returns>Chance of Success</returns>
        public static float DoFormula(Pawn doer, Pawn subjectee)
        {
            float num = doer.GetStatValue(StatDefOf.SocialImpact, true);
            int opinion = subjectee.relations.OpinionOf(doer);
            num = num * SOMod.Settings.DipWeight + (float)opinion * SOMod.Settings.OpnWeight; // Formula
            if (SOMod.Settings.OpinionOnly)
            {
                num = (float)opinion * SOMod.Settings.OOpnWeight;
            }
            num = Mathf.Clamp01(num);
            return num;
        }

        /// <summary>
        /// Summons a status message
        /// </summary>
        /// <param name="type">The type of message: 
        /// 1 - Success;
        /// 2 - Failure;
        /// 3 - Critical Failure</param>
        /// <param name="doer">Pawn</param>
        /// <param name="subjectee">Target Pawn</param>
        public static void DoStatusMessage(int type, Pawn doer, Pawn subjectee)
        {
            switch (type)
            {
                case 1: // Success
                    Messages.Message(
                        "SuccessCalm".Translate(doer.Name.ToStringShort, subjectee.Name.ToStringShort),
                        MessageTypeDefOf.TaskCompletion);
                    break;

                case 2: // Failure
                    Messages.Message(
                        "FailCalm".Translate(doer.Name.ToStringShort, subjectee.Name.ToStringShort),
                        MessageTypeDefOf.TaskCompletion);
                    break;

                case 3: // Critical Failure
                    Messages.Message(
                        "AggroFailCalm".Translate(doer.Name.ToStringShort, subjectee.Name.ToStringShort),
                        MessageTypeDefOf.TaskCompletion);
                    break;
            }
        }
    }
}