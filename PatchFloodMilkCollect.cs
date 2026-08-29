using HarmonyLib;
using UnityEngine;

namespace MilkItem
{
    
    
    
    
    
    
    [HarmonyPatch(typeof(Pickup))]
    [HarmonyPatch("Collect")]
    internal static class PatchFloodMilkCollect
    {
        [HarmonyPrefix]
        private static bool Prefix(Pickup __instance, int player)
        {
            try
            {
                if (__instance == null) return true;
                MilkFloodPickup tag = __instance.GetComponent<MilkFloodPickup>();
                if (tag == null || tag.owner == null) return true; 
                if (!tag.owner.IsActive) return true;              

                tag.owner.ConsumeFloodMilk(__instance, tag.type, player); 
                Plugin.PlayMilkDrinkSound();                        
                tag.StartFallAndDestroy();                          
                return false;                                       
            }
            catch (System.Exception)
            {
                return true; 
            }
        }
    }
}