using HarmonyLib;
using UnityEngine;

namespace BugRevival.Patches
{
    // Original blessing: using a BSODA on an NPC pins it in place (speedMultiplier 0) forever,
    // so the target gets stuck on whatever wall/object was behind it when hit.
    [HarmonyPatch(typeof(ITM_BSODA), "EntityTriggerEnter")]
    internal class BSODAFreezePatch
    {
        [HarmonyPostfix]
        internal static void Postfix(ITM_BSODA __instance, Entity otherEntity)
        {
            if (!BugConfig.BSODAFreezeNPC.Value) return;
            if (otherEntity == null || otherEntity.name == null) return;
            // Only stick NPCs — never the player (whose movement must never be zeroed permanently).
            if (otherEntity.CompareTag("Player") || otherEntity.name.Contains("Player")) return;
            try
            {
                var mod = new MovementModifier(Vector3.zero, 0f);
                otherEntity.ExternalActivity.moveMods.Add(mod);
                // Intentionally NOT registered in activityMods, so it never gets removed on trigger-exit:
                // the NPC stays frozen to the wall for the rest of the floor.
            }
            catch (System.Exception) { }
        }
    }
}