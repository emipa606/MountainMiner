using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MountainMiner
{
#pragma warning disable IDE1006 // Naming Styles
    class Building_MountainDrill : Building
#pragma warning restore IDE1006 // Naming Styles
    {
        private CompPowerTrader powerComp;

        private float progress;

        public float Progress
        {
            get => this.progress;
            set => this.progress = value;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = GetComp<CompPowerTrader>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.progress, "MountainProgress", 0f);
        }

        public void Drill(float miningPoints)
        {
            this.progress += (miningPoints * GetRoofFactor());
            if (UnityEngine.Random.Range(0, 1000) == 0)
            {
                ProduceLump();
            }
        }

        public void DrillWorkDone(Pawn driller)
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(this.Map) && this.Map.roofGrid.RoofAt(intVec) == RoofDefOf.RoofRockThick)
                {
                    this.Map.roofGrid.SetRoof(intVec, RoofDefOf.RoofRockThin);
                }
            }

            //add message here?
            this.Progress = 0f;
        }

        private void ProduceLump()
        {
            if (TryGetNextResource(out ThingDef def, out IntVec3 c))
            {
                Thing thing = ThingMaker.MakeThing(def, null);
                GenPlace.TryPlaceThing(thing, this.InteractionCell, this.Map, ThingPlaceMode.Near);
            }
            return;
        }

        public bool TryGetNextResource(out ThingDef resDef, out IntVec3 cell)
        {
            //List<IntVec3> coordList = new List<IntVec3>();
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(this.Map) && this.Map.roofGrid.RoofAt(intVec) != null && this.Map.roofGrid.RoofAt(intVec).isThickRoof)
                {
                    ThingDef thingDef = DeepDrillUtility.GetBaseResource(this.Map, intVec);
                    //GenStep_RocksFromGrid.RockDefAt(intVec);
                    if (thingDef != null)
                    {
                        resDef = thingDef;
                        cell = intVec;
                        return true;
                    }
                }
            }
            resDef = null;
            cell = IntVec3.Invalid;

            // or add a message here? the miner should be done at this point or something broke, but there should be no new lumps...
            return false;
        }

        public bool CanDrillNow() => (this.powerComp == null || this.powerComp.PowerOn) && RoofPresent();

        public bool RoofPresent()
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(this.Map) && this.Map.roofGrid.RoofAt(intVec) != null && this.Map.roofGrid.RoofAt(intVec).isThickRoof)
                    return true;
            }
            return false;
        }

        public override string GetInspectString() => string.Concat(new string[]
            {
                base.GetInspectString(),
                "Progress"/*.Translate()*/,
                ": ",
                this.Progress.ToStringPercent()
            });
        private float GetRoofFactor()
        {
            int tiles = 0;
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(this.Map) && this.Map.roofGrid.RoofAt(intVec) != null && this.Map.roofGrid.RoofAt(intVec).isThickRoof)
                    tiles++;
            }
            if (tiles == 0)
                tiles++;
            return (9 / tiles);
        }
    }
}