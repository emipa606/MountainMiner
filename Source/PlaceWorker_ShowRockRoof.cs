using System.Linq;
using UnityEngine;
using Verse;

namespace MountainMiner
{
    public class PlaceWorker_ShowRockRoof : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (loc != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 intVec = loc + GenRadial.RadialPattern[i];
                    if (intVec.InBounds(map) && map.roofGrid.RoofAt(intVec) != null && map.roofGrid.RoofAt(intVec).isThickRoof)
                            return true;
                }
                return new AcceptanceReport("Must be placed under overhead Mountain");
            }
            return false;
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Map visibleMap = Find.CurrentMap; //:: was VisibleMap; uncertain if replacement correct

            foreach (IntVec3 current in from cur in visibleMap.AllCells where visibleMap.roofGrid.RoofAt(cur) != null && visibleMap.roofGrid.RoofAt(cur).isThickRoof select cur)
                CellRenderer.RenderCell(current);
        }
    }
}