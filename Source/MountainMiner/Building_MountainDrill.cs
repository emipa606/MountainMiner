using RimWorld;
using UnityEngine;
using Verse;

namespace MountainMiner
{
    internal class Building_MountainDrill : Building
    {
        private ThingDef bottomChunk;
        private ThingDef currentChunk;
        private int currentTile = -1;
        private CompPowerTrader powerComp;

        private float progress;
        private ThingDef tileChunk;

        public float Progress
        {
            get => progress;
            private set => progress = value;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref progress, "MountainProgress");
            Scribe_Values.Look(ref currentTile, "Tile");
            Scribe_Defs.Look(ref currentChunk, "chunk");
        }

        public void Drill(float miningPoints)
        {
            progress += miningPoints * GetRoofFactor();
            var multiplier = LoadedModManager.GetMod<MountainMinerMod>().GetSettings<MountainMinerSettings>()
                .ChunkMultiplier;
            if (Random.Range(0, 1000 * multiplier) < 1)
            {
                ProduceLump();
            }
        }

        public bool DrillWorkDone(Pawn driller)
        {
            var intVec = Position + GenRadial.RadialPattern[currentTile];
            if (intVec.InBounds(Map) && Map.roofGrid.RoofAt(intVec) == RoofDefOf.RoofRockThick)
            {
                Map.roofGrid.SetRoof(intVec, RoofDefOf.RoofRockThin);
            }

            Progress = 0f;

            return !RoofPresent();
        }

        private void ProduceLump()
        {
            if (!TryGetNextResource(out var thingDef, out _))
            {
                return;
            }

            var thing = ThingMaker.MakeThing(thingDef);
            GenPlace.TryPlaceThing(thing, InteractionCell, Map, ThingPlaceMode.Near);
        }

        private float GetRoofFactor()
        {
            var tiles = 0;
            for (var i = 0; i < 9; i++)
            {
                var intVec = Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(Map) && Map.roofGrid.RoofAt(intVec) != null &&
                    Map.roofGrid.RoofAt(intVec).isThickRoof)
                {
                    tiles++;
                }
            }

            if (tiles == 0)
            {
                tiles++;
            }

            return 9 / tiles;
        }

        public bool TryGetNextResource(out ThingDef resDef, out IntVec3 cell)
        {
            for (var i = 0; i < 9; i++)
            {
                var intVec = Position + GenRadial.RadialPattern[i];
                if (!intVec.InBounds(Map) || Map.roofGrid.RoofAt(intVec) == null ||
                    Map.roofGrid.RoofAt(intVec) != RoofDefOf.RoofRockThick)
                {
                    continue;
                }

                var terrain = Map.terrainGrid.TerrainAt(intVec);
                if (terrain != null)
                {
                    var terrainNameArray = terrain.defName.Split('_');
                    if (terrain.scatterType == "Rocky" && terrainNameArray.Length == 2)
                    {
                        var thingDef = DefDatabase<ThingDef>.GetNamed("Chunk" + terrainNameArray[0], false);
                        if (thingDef != null)
                        {
                            tileChunk = thingDef;
                        }
                    }
                }

                terrain = Map.terrainGrid.UnderTerrainAt(intVec);
                if (terrain != null)
                {
                    var terrainNameArray = terrain.defName.Split('_');
                    if (terrain.scatterType == "Rocky" && terrainNameArray.Length == 2)
                    {
                        var thingDef = DefDatabase<ThingDef>.GetNamed("Chunk" + terrainNameArray[0], false);
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
                }

                if (bottomChunk != null)
                {
                    resDef = bottomChunk;
                    cell = intVec;
                    currentChunk = bottomChunk;
                    return true;
                }

                var baseRes = DeepDrillUtility.GetBaseResource(Map, intVec);
                if (baseRes == null)
                {
                    continue;
                }

                resDef = baseRes;
                cell = intVec;
                currentChunk = baseRes;
                return true;
            }

            resDef = null;
            cell = IntVec3.Invalid;
            currentChunk = null;

            // or add a message here? the miner should be done at this point or something broke, but there should be no new lumps...
            return false;
        }

        public bool CanDrillNow()
        {
            return (powerComp == null || powerComp.PowerOn) && RoofPresent();
        }

        public bool RoofPresent()
        {
            for (var i = 0; i < 9; i++)
            {
                var intVec = Position + GenRadial.RadialPattern[i];
                if (!intVec.InBounds(Map) || Map.roofGrid.RoofAt(intVec) == null ||
                    Map.roofGrid.RoofAt(intVec) != RoofDefOf.RoofRockThick)
                {
                    continue;
                }

                currentTile = i;
                return true;
            }

            currentTile = -1;
            return false;
        }

        public override string GetInspectString()
        {
            return string.Concat(new[]
            {
                base.GetInspectString(),
                "Progress" /*.Translate()*/,
                ": ",
                Progress.ToStringPercent()
            });
        }
    }
}