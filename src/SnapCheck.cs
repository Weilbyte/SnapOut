

namespace SnapOut
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Verse;
    using RimWorld;

    /// <summary>
    /// SnapCheck contains the methods for checking certain conditions related to pawns
    /// </summary>
    class SnapCheck
    {

        private static readonly List<string> incompatDef = new List<string>(new string[] { "RunWild", "PanicFlee", "HuntingTrip", "SocialFighting" }); // Def names of incompatible mental states


        /// <summary>
        /// Checks whether a pawn is a prisoner or not
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns>True or False</returns>
        public static bool isPrisoner(Pawn pawn) {
            if (pawn.guest.IsPrisoner) {
                return true;
            } 
            return false;
        }

        /// <summary>
        /// Checks whether a pawn is a trader or not
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns>True or False</returns>
        public static bool isTrader(Pawn pawn)
        {
            if (!pawn.Faction.IsPlayer) {
                if (!(pawn.Faction.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Hostile))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether a pawn should be calmed or not, depending on the severity of their mental state and mod settings
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns>True or False</returns>
        public static bool aggroPass(Pawn pawn) {
            if (pawn.InAggroMentalState) {
                if (SOMod.Settings.AggroCalmEnabled) {
                    return true;
                }
                return false;
            } 
            return true;
        }

        /// <summary>
        /// Returns true if the pawn has a null faction or is null by itself
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns>True or False</returns>
        public static bool isNull(Pawn pawn)
        {
            if (pawn == null)
            {
                return true;
            }
            else if (pawn.Faction == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if target pawn meets conditions to be calmed
        /// </summary>
        /// <param name="pawn">Target pawn</param>
        /// <returns>True or False</returns>
        public static bool CanDo(Pawn pawn)
        {
            if (isNull(pawn)) {
                return false;
            } else {
                if (RestUtility.Awake(pawn)) {
                    if (isPrisoner(pawn)) {
                        if (!SOMod.Settings.NonFaction) {
                            return false;
                        }
                    } else if (isTrader(pawn)) {
                        if (!SOMod.Settings.TraderCalm) {
                            return false;
                        }
                    }

                    return aggroPass(pawn);
                }
                return false;
            }
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
    }
}
