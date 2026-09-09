using HarmonyLib;
using UnityEngine;

namespace BugRevival
{
    // Player-related bug revivals.
    //   - PlayerDoubleStamina (0.1): running drains double stamina
    // Use a prefix to temporarily double staminaDrop and a postfix to restore it, avoiding permanent field overwrites that stack endlessly.
    [HarmonyPatch(typeof(PlayerMovement), "StaminaUpdate")]
    internal class PlayerStaminaPatch
    {
        private static float _origDrop;   // save the original drain value for this frame

        [HarmonyPrefix]
        internal static void Prefix(PlayerMovement __instance)
        {
            _origDrop = __instance.staminaDrop;
            if (BugConfig.PlayerDoubleStamina.Value)
                __instance.staminaDrop = _origDrop * 2f;
        }

        [HarmonyPostfix]
        internal static void Postfix(PlayerMovement __instance)
        {
            __instance.staminaDrop = _origDrop;   // restore, avoid repeated multiplication
        }
    }
}