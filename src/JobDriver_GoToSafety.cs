namespace SnapOut
{
    using System.Collections.Generic;
    using RimWorld; 
    using Verse;
    using Verse.AI;

    /// <summary>
    /// Job driver. Makes the pawn go their bedroom for safety.
    /// </summary>
    public class JobDriver_GoToSafety : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

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
    }
}