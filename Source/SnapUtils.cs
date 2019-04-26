using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace SnapOut
{
    class SnapUtils
    {
        private static readonly string FCalm = "FailCalm".Translate();
        private static readonly string SCalm = "SuccessCalm".Translate();
        private static readonly string AFCalm = "AggroFailCalm".Translate();
        private static List<string> incompatDef = new List<string>(new string[] { "RunWild", "PanicFlee", "HuntingTrip", "SocialFighting" }); //Def names of incompatible mental states

        /// <summary>
        /// Checks to see if target pawn meets mod conditions to be calmed
        /// </summary>
        /// <param name="subjectee">Target pawn</param>
        /// <returns></returns>
        public static bool CanDo(Pawn subjectee)
        {
            bool prisonerDo, traderDo, stateTypeDo, awakeDo;
            prisonerDo = traderDo = stateTypeDo = awakeDo = false;

            //Awake check
            if (RestUtility.Awake(subjectee)) { awakeDo = true; }


            //Aggro check
            if (subjectee.InAggroMentalState == SOMod.settings.SOAggroCalmEnabled) stateTypeDo = true;
            if (!subjectee.InAggroMentalState) stateTypeDo = true;
            bool friendly = true;
            
            if (!subjectee.Faction.IsPlayer)
            {
                if (subjectee.Faction.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Hostile)
                {
                    friendly = false;
                }
            }
            

            //Prisoner check
            if (subjectee.guest.IsPrisoner == SOMod.settings.SONonFaction) { prisonerDo = true; traderDo = true; }

            //Trader check
            if (!subjectee.Faction.IsPlayer);
            {
                if (friendly)
                {
                    if (SOMod.settings.SOTraderCalm) { traderDo = true; prisonerDo = true; }
                }
            }

            //Colonist check
            if (subjectee.Faction == Faction.OfPlayer) { prisonerDo = true; traderDo = true; }
            if (prisonerDo && traderDo && stateTypeDo && awakeDo) return true;
            return false;
        }

        public static bool IsCapableOf(Pawn doer)
        {
            if (doer.health.capacities.CapableOf(PawnCapacityDefOf.Talking) && doer.health.capacities.CapableOf(PawnCapacityDefOf.Hearing) && doer.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
            {
                return true;
            }
            return false;
        }

        public static bool CompatCheck(string defname)
        {
            if(incompatDef.Any(s => defname.Contains(s)))
            {
                return false;
            }
            return true;
        }

        public static void modCompatDoer(string defname, Pawn doer, Pawn subjectee)
        {
            //Psychology compat.
            //if (defname.Contains()) { subjectee.stances.stunner.StunFor(SOMod.settings.SOCalmDuration, doer); }

        }

        public static void DebugLog(string message)
        {
            if (SOMod.settings.SODebug)
            {
                Log.Message("[SnapOut] " + message);
            }
        }

        public static float DoFormula(Pawn doer, Pawn subjectee)
        {
            float num = doer.GetStatValue(StatDefOf.SocialImpact, true);
            int opinion = subjectee.relations.OpinionOf(doer);
            num = num * SOMod.settings.SODipWeight + (float)opinion * SOMod.settings.SOOpnWeight; //Formula
            if (SOMod.settings.SOOpnOnly)
            {
                num = (float)opinion * SOMod.settings.SOOOpnWeight;
            }
            num = Mathf.Clamp01(num);
            return num;
        }

        public static void DoStatusMessage(int type, Pawn doer, Pawn subjectee)
        {
            switch (type)
            {
                case 1: //Success
                    Messages.Message(string.Format(SCalm, new object[]
                    {
                                    doer.Name.ToStringShort,
                                    subjectee.Name.ToStringShort,
                    }), MessageTypeDefOf.TaskCompletion);
                    break;
                case 2: //Failure
                    Messages.Message(string.Format(FCalm, new object[]
                                {
                                    doer.Name.ToStringShort,
                                    subjectee.Name.ToStringShort,
                                }), MessageTypeDefOf.TaskCompletion);
                    break;
                case 3: //Critical Failure
                    Messages.Message(string.Format(AFCalm, new object[]
                                    {
                                    doer.Name.ToStringShort,
                                    subjectee.Name.ToStringShort,
                                    }), MessageTypeDefOf.TaskCompletion);
                    break;
            }
        }

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
    }
}
