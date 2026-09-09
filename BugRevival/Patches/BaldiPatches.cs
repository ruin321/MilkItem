using HarmonyLib;
using UnityEngine;

namespace BugRevival
{
    // Baldi-related bug revivals.
    //   - BaldiSlowAtBooks (0.13): Baldi gets slower the further a floor's progress goes (matching the "slower after collecting books" feel)
    //   - BaldiWallHit (0.4): Baldi can hit/catch up to the player through walls
    [HarmonyPatch(typeof(Baldi), "Speed", MethodType.Getter)]
    internal class BaldiSpeedPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(ref float __result)
        {
            try
            {
                // Both speed toggles share one getter; BaldiHyper takes priority (mutually exclusive with Slow)
                if (BugConfig.BaldiHyperSpeed.Value)
                {
                    __result *= 2.8f;   // enraged overspeed
                }
                else if (BugConfig.BaldiSlowAtBooks.Value)
                {
                    __result *= 0.7f;   // revive "Baldi slows down"
                }
            }
            catch (System.Exception) { }
        }
    }

    [HarmonyPatch(typeof(Baldi), "DistanceCheck")]
    internal class BaldiWallHitPatch
    {
        // DistanceCheck is the entity-movement hook; the returned value is the distance actually moved this call.
        // Doubling it makes Baldi cover each step faster, so he catches up to the player across doorways far sooner (0.4 "catch through walls" flavour).
        [HarmonyPrefix]
        internal static void Prefix(ref float val)
        {
            if (!BugConfig.BaldiWallHit.Value) return;
            try { val = val * 2.2f; } catch (System.Exception) { }
        }
    }

    // BaldiAngerBoost (0.2): each GetAngry doubles anger accumulation → instantly enrages at the slightest provocation
    [HarmonyPatch(typeof(Baldi), "GetAngry")]
    internal class BaldiAngerBoostPatch
    {
        [HarmonyPrefix]
        internal static void Prefix(ref float value)
        {
            if (!BugConfig.BaldiAngerBoost.Value) return;
            try { value = value * 2f; } catch (System.Exception) { }
        }
    }
}