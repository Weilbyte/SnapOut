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
                DebugLog("Picked " + cmrand + " as a calming message");
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
            if ((subjectee.InAggroMentalState == SOMod.Settings.SOAggroCalmEnabled) || (!subjectee.InAggroMentalState))
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
            if (subjectee.guest.IsPrisoner == SOMod.Settings.SONonFaction)
            {
                prisonerDo = true;
                traderDo = true;
            }

            // Trader check
            if (!subjectee.Faction.IsPlayer)
            {
                if (friendly)
                {
                    if (SOMod.Settings.SOTraderCalm)
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

        public static bool IsCapable(Pawn doer)
        {
            if (doer.health.capacities.CapableOf(PawnCapacityDefOf.Talking) && doer.health.capacities.CapableOf(PawnCapacityDefOf.Hearing) && doer.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
            {
                return true;
            }

            return false;
        }

        public static bool CompatCheck(string defname)
        {
            if (incompatDef.Any(s => defname.Contains(s)))
            {
                return false;
            }

            return true;
        }

        public static void DebugLog(string message)
        {
            if (SOMod.Settings.SODebug)
            {
                Log.Message("[SnapOut] " + message);
            }
        }

        public static float DoFormula(Pawn doer, Pawn subjectee)
        {
            float num = doer.GetStatValue(StatDefOf.SocialImpact, true);
            int opinion = subjectee.relations.OpinionOf(doer);
            num = num * SOMod.Settings.SODipWeight + (float)opinion * SOMod.Settings.SOOpnWeight; // Formula
            if (SOMod.Settings.SOOpnOnly)
            {
                num = (float)opinion * SOMod.Settings.SOOOpnWeight;
            }

            num = Mathf.Clamp01(num);
            return num;
        }

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