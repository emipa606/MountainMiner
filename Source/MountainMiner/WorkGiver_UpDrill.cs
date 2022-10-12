using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MountainMiner;

public class WorkGiver_UpDrill : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest =>
        ThingRequest.ForDef(ThingDef.Named("ManualMountainMiner"));

    public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ManualMountainMiner"));
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        var allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
        foreach (var buildings in allBuildingsColonist)
        {
            if (buildings.def != ThingDef.Named("ManualMountainMiner"))
            {
                continue;
            }

            var comp = buildings.GetComp<CompPowerTrader>();
            if (comp == null || comp.PowerOn)
            {
                return false;
            }
        }

        return true;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t.Faction != pawn.Faction)
        {
            return false;
        }

        if (!(t is Building building))
        {
            return false;
        }

        if (building.IsForbidden(pawn))
        {
            return false;
        }

        if (!pawn.CanReserve(building))
        {
            return false;
        }

        var mountainDrill = (Building_MountainDrill)building;
        return mountainDrill.CanDrillNow() && !building.IsBurning();
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(JobDefOf_MM.OperateHighDrill, t, 1500, true);
    }
}