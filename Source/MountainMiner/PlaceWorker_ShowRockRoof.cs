using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MountainMiner
{
    public class PlaceWorker_ShowRockRoof : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
            Thing thingToIgnore = null, Thing thing = null)
        {
            for (var i = 0; i < 9; i++)
            {
                var intVec = loc + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map) && map.roofGrid.RoofAt(intVec) != null &&
                    map.roofGrid.RoofAt(intVec) == RoofDefOf.RoofRockThick)
                {
                    return true;
                }
            }

            return new AcceptanceReport("Must be placed under overhead Mountain");
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            var visibleMap = Find.CurrentMap; //:: was VisibleMap; uncertain if replacement correct

            foreach (var current in from cur in visibleMap.AllCells
                where visibleMap.roofGrid.RoofAt(cur) != null &&
                      visibleMap.roofGrid.RoofAt(cur) == RoofDefOf.RoofRockThick && !visibleMap.fogGrid.IsFogged(cur)
                select cur)
            {
                CellRenderer.RenderCell(current);
            }
        }
    }
}