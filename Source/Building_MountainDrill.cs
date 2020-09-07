using RimWorld;
using System;
using Verse;

namespace MountainMiner
{
#pragma warning disable IDE1006 // Naming Styles
    class Building_MountainDrill : Building
#pragma warning restore IDE1006 // Naming Styles
    {
        private CompPowerTrader powerComp;
        private int currentTile = -1;
        private ThingDef currentChunk = null;
        private ThingDef tileChunk;
        private ThingDef bottomChunk;


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
            Scribe_Values.Look(ref this.currentTile, "Tile");
            Scribe_Defs.Look(ref this.currentChunk, "chunk");
        }

        public void Drill(float miningPoints)
        {
            this.progress += (9 * miningPoints);
            if (UnityEngine.Random.Range(0, 1000) == 0)
            {
                ProduceLump();
            }
        }

        public bool DrillWorkDone(Pawn driller)
        {
            IntVec3 intVec = this.Position + GenRadial.RadialPattern[currentTile];
            if (intVec.InBounds(this.Map) && this.Map.roofGrid.RoofAt(intVec) == RoofDefOf.RoofRockThick)
            {
                this.Map.roofGrid.SetRoof(intVec, RoofDefOf.RoofRockThin);
            }
            this.Progress = 0f;

            return !RoofPresent();

            /*
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(this.Map) && this.Map.roofGrid.RoofAt(intVec) == RoofDefOf.RoofRockThick)
                {
                    this.Map.roofGrid.SetRoof(intVec, RoofDefOf.RoofRockThin);
                }
            }
            */

            //add message here?
            //Messages.Message("DeepDrillExhaustedNoFallback".Translate(), this.parent, MessageTypeDefOf.TaskCompletion, true);
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
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(this.Map) && this.Map.roofGrid.RoofAt(intVec) != null && this.Map.roofGrid.RoofAt(intVec) == RoofDefOf.RoofRockThick)
                {
                    TerrainDef terrain = this.Map.terrainGrid.TerrainAt(intVec);
                    if (terrain != null)
                    {
                        String[] terrainNameArray = terrain.defName.Split('_');
                        if (terrain.scatterType == "Rocky" && terrainNameArray.Length == 2)
                        {
                            ThingDef thingDef = DefDatabase<ThingDef>.GetNamed("Chunk" + terrainNameArray[0], false);
                            if (thingDef != null)
                            {
                                tileChunk = thingDef;
                            }
                        }
                    }
                    terrain = this.Map.terrainGrid.UnderTerrainAt(intVec);
                    if (terrain != null)
                    {
                        String[] terrainNameArray = terrain.defName.Split('_');
                        if (terrain.scatterType == "Rocky" && terrainNameArray.Length == 2)
                        {
                            ThingDef thingDef = DefDatabase<ThingDef>.GetNamed("Chunk" + terrainNameArray[0], false);
                            if (thingDef != null)
                            {
                                bottomChunk = thingDef;
                            }
                        }
                    }

                    if (tileChunk != null)
                    {
                        resDef = tileChunk;
                        cell = intVec;
                        currentChunk = tileChunk;
                        return true;
                    } else if (bottomChunk != null)
                    {
                        resDef = bottomChunk;
                        cell = intVec;
                        currentChunk = bottomChunk;
                        return true;
                    } else
                    {
                        ThingDef baseRes = DeepDrillUtility.GetBaseResource(this.Map, intVec);
                        if (baseRes != null)
                        {
                            resDef = baseRes;
                            cell = intVec;
                            currentChunk = baseRes;
                            return true;
                        }
                    }
                }
            }
            resDef = null;
            cell = IntVec3.Invalid;
            currentChunk = null;

            // or add a message here? the miner should be done at this point or something broke, but there should be no new lumps...
            return false;
        }

        public bool CanDrillNow() => (this.powerComp == null || this.powerComp.PowerOn) && RoofPresent();

        public bool RoofPresent()
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(this.Map) && this.Map.roofGrid.RoofAt(intVec) != null && this.Map.roofGrid.RoofAt(intVec) == RoofDefOf.RoofRockThick)
                {
                    currentTile = i;
                    return true;
                }
            }
            currentTile = -1;
            return false;
        }

        public override string GetInspectString() => string.Concat(new string[]
            {
                base.GetInspectString(),
                "Progress"/*.Translate()*/,
                ": ",
                this.Progress.ToStringPercent()
            });
    }
}