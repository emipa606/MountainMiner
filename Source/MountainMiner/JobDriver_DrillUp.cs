using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace MountainMiner;

internal class JobDriver_DrillUp : JobDriver
{
    private const int ticks = GenDate.TicksPerDay;
    private Building_MountainDrill Comp => (Building_MountainDrill)TargetA.Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOn(() => !Comp.CanDrillNow());
        yield return Toils_Reserve.Reserve(TargetIndex.A).FailOnDespawnedNullOrForbidden(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A);

        var mine = new Toil();
        mine.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
        mine.WithProgressBar(TargetIndex.A, () => Comp.Progress);
        mine.tickAction = delegate
        {
            var mineActor = mine.actor;
            Comp.Drill(mineActor.GetStatValue(StatDefOf.MiningSpeed) / ticks);
            mineActor.skills.Learn(SkillDefOf.Mining, 0.125f);
            if (!(Comp.Progress >= 1))
            {
                return;
            }

            if (!Comp.DrillWorkDone(mineActor))
            {
                return;
            }

            EndJobWith(JobCondition.Succeeded);
            mineActor.records.Increment(RecordDefOf.CellsMined);
        };
        mine.WithEffect(TargetThingA.def.repairEffect, TargetIndex.A);
        mine.defaultCompleteMode = ToilCompleteMode.Never;
        yield return mine;
    }
}