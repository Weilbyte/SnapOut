using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SnapOut
{
    class JobDriver_GoToSafety : JobDriver
    {
        //Toil Reservations
        #region toilreservations
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        #endregion

        //Toil stuffs
        #region toilstuffs
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn tpawn = this.pawn;
            this.TargetThingB = this.pawn;
            Room bedroom = tpawn.ownership.OwnedRoom;
            bedroom.Cells.TryRandomElement<IntVec3>(out IntVec3 c);
            yield return Toils_Goto.GotoCell(c, PathEndMode.ClosestTouch);
            int wticks = UnityEngine.Random.Range(1750, 3500);
            Toil relax = Toils_General.Wait(wticks);
            relax.socialMode = RandomSocialMode.Off;
            yield return relax;
     
        }
        #endregion
    }
}
