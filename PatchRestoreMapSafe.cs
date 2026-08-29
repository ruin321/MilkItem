using System;
using System.Reflection;
using HarmonyLib;

namespace MilkItem
{
    
    
    
    
    [HarmonyPatch(typeof(CoreGameManager), "RestoreMap")]
    public class PatchRestoreMapSafe
    {
        private static readonly FieldInfo FoundTiles =
            typeof(CoreGameManager).GetField("foundTilesToRestore",
                BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPriority(Priority.First)]
        static void Prefix(CoreGameManager __instance, Map map)
        {
            try
            {
                if (map == null || FoundTiles == null) return;
                bool[,] cur = FoundTiles.GetValue(__instance) as bool[,];
                int szX = map.size.x;
                int szZ = map.size.z;

                
                if (cur != null && cur.GetLength(0) == szX && cur.GetLength(1) == szZ) return;

                bool[,] neu = new bool[szX, szZ];
                if (cur != null)
                {
                    int minX = Math.Min(cur.GetLength(0), szX);
                    int minZ = Math.Min(cur.GetLength(1), szZ);
                    for (int i = 0; i < minX; i++)
                        for (int j = 0; j < minZ; j++)
                            neu[i, j] = cur[i, j];
                }
                FoundTiles.SetValue(__instance, neu);
            }
            catch (Exception) { }
        }
    }
}