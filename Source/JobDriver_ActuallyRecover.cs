using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace SnapOut
{
    class JobDriver_ActuallyRecover : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn rpawn = this.pawn;
            yield return Toils_General.Wait(1);
            rpawn.MentalState.RecoverFromState();
        }

    }
}
