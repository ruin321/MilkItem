using UnityEngine;

namespace MilkItem
{
    
    
    
    
    
    public class KeyComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            if (player == null || player.ec == null) return false;
            try
            {
                Camera cam = Camera.main;
                if (cam == null) return false;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Door door = null;
                if (Physics.Raycast(ray, out hit, 60f) && hit.collider != null)
                {
                    door = hit.collider.GetComponentInParent<Door>();
                }
                if (door == null)
                {
                    
                    return false;
                }
                if (!door.locked)
                {
                    
                    return false;
                }
                door.Unlock();
                if (Is99RoomDoor(door))
                {
                    Plugin.nineNineDoorUnlockedByPlayer = true;
                    
                }
                door.Open(true, true);
                
                return true; 
            }
            catch (System.Exception )
            {
                
                return false;
            }
        }

        
        private static bool Is99RoomDoor(Door door)
        {
            if (door == null || !Plugin.Room99CategoryReady) return false;
            try
            {
                foreach (var cell in new Cell[] { door.aTile, door.bTile })
                {
                    if (cell != null && !cell.Null && cell.room != null
                        && (int)(object)cell.room.category == (int)(object)Plugin.Room99Category)
                    {
                        return true;
                    }
                }
            }
            catch (System.Exception) { }
            return false;
        }
    }
}
