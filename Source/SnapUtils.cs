namespace SnapOut
{
    using System.Collections.Generic;
    using System.Linq;
    using RimWorld;
    using UnityEngine;
    using Verse;

    internal class SnapUtils
    {
        private static List<string> incompatDef = new List<string>(new string[] { "RunWild", "PanicFlee", "HuntingTrip", "SocialFighting" }); // Def names of incompatible mental states

        /// <summary>
        /// Gets a random calming message from the translation files. Picks random message with ID between 1-21.
        /// </summary>
        public static string GetCalmingMessage
        {
            get
            {
                int rand = UnityEngine.Random.Range(1, 21);
                string cmrand = "CM" + rand;
                DebugLog(string.Format("Calming message ID is {0}", cmrand));
                return cmrand.Translate();
            }
        }

        /// <summary>
        /// Checks to see if target pawn meets conditions to be calmed
        /// </summary>
        /// <param name="subjectee">Target pawn</param>
        /// <returns>True or False</returns>
        public static bool CanDo(Pawn subjectee)
        {
            if (subjectee == null)
            {
                SnapUtils.DebugLog("A null subjectee was provided to CanDo");
                return false;
            } else if (subjectee.Faction == null)
            {
                return false;
            }

            bool prisonerDo = false, traderDo = false, stateTypeDo = false, awakeDo = false, friendly = true;

            // Awake check
            if (RestUtility.Awake(subjectee))
            {
                awakeDo = true;
            }

            // Aggro check
            if ((subjectee.InAggroMentalState == SOMod.Settings.AggroCalmEnabled) || (!subjectee.InAggroMentalState))
            {
                stateTypeDo = true;
            }

            // Friendly check
            if (!subjectee.Faction.IsPlayer)
            {
                if (subjectee.Faction.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Hostile)
                {
                    friendly = false;
                }
            }

            // Prisoner check
            if (subjectee.guest.IsPrisoner == SOMod.Settings.NonFaction)
            {
                prisonerDo = true;
                traderDo = true;
            }

            // Trader check
            if (!subjectee.Faction.IsPlayer)
            {
                if (friendly)
                {
                    if (SOMod.Settings.TraderCalm)
                    {
                        traderDo = true;
                        prisonerDo = true;
                    }
                }
            }

            // Colonist check
            if (subjectee.Faction == Faction.OfPlayer)
            {
                prisonerDo = true;
                traderDo = true;
            }

            if (prisonerDo && traderDo && stateTypeDo && awakeDo)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if pawn is capable of calming others down
        /// </summary>
        /// <param name="pawn">The Pawn to Check</param>
        /// <returns>True or False</returns>
        public static bool IsCapable(Pawn pawn)
        {
            if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking) && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Hearing) && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if provided mental state def name is listed as incompatible
        /// </summary>
        /// <param name="defname">Def Name</param>
        /// <returns>True or False</returns>
        public static bool CompatCheck(string defname)
        {
            if (incompatDef.Any(s => defname.Contains(s)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Logs debug message to the game's console
        /// </summary>
        /// <param name="message">Message</param>
        public static void DebugLog(string message)
        {
            if (SOMod.Settings.Debug)
            {
                Log.Message("[SnapOut] " + message);
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
                        string.Format(
                            "SuccessCalm".Translate(),
                        new object[]
                    {
                                    doer.Name.ToStringShort,
                                    subjectee.Name.ToStringShort,
                    }),
                        MessageTypeDefOf.TaskCompletion);
                    break;

                case 2: // Failure
                    Messages.Message(
                        string.Format(
                        "FailCalm".Translate(),
                        new object[]
                                {
                                    doer.Name.ToStringShort,
                                    subjectee.Name.ToStringShort,
                                }),
                                MessageTypeDefOf.TaskCompletion);
                    break;

                case 3: // Critical Failure
                    Messages.Message(
                        string.Format(
                        "AggroFailCalm".Translate(),
                        new object[]
                                    {
                                    doer.Name.ToStringShort,
                                    subjectee.Name.ToStringShort,
                                    }),
                                    MessageTypeDefOf.TaskCompletion);
                    break;
            }
        }
    }
}