using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace BugRevival
{
    // WhirlpoolToPillar: whirlpool squeezes entities into corridor pillars/walls (0.13)
    // Vanilla Whirlpool.Update uses a raycast to check whether an obstacle lies between the entity and the whirlpool, stopping the pull if so (_obstacleHit).
    // Revived bug = ignore obstacles and keep pulling entities toward the wall/pillar via centripetal force.
    [HarmonyPatch(typeof(Whirlpool), "Update")]
    internal class WhirlpoolPatches
    {
        private static FieldInfo _entities, _moveMods;

        private static void LoadFields()
        {
            if (_entities != null) return;
            _entities = typeof(Whirlpool).GetField("entities", BindingFlags.NonPublic | BindingFlags.Instance);
            _moveMods = typeof(Whirlpool).GetField("moveMods", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPostfix]
        internal static void Postfix(Whirlpool __instance)
        {
            if (!BugConfig.WhirlpoolToPillar.Value) return;
            try
            {
                LoadFields();
                var list = _entities?.GetValue(__instance) as List<Entity>;
                var mods = _moveMods?.GetValue(__instance) as List<MovementModifier>;
                if (list == null || mods == null) return;
                for (int i = 0; i < list.Count && i < mods.Count; i++)
                {
                    var ent = list[i];
                    if (ent == null) continue;
                    Vector3 toCenter = __instance.transform.position - ent.transform.position;
                    toCenter.y = 0f;
                    if (toCenter.sqrMagnitude < 0.01f) continue;
                    // Apply a strong centripetal force, ignoring any wall occlusion, pushing entities toward the whirlpool center (often a wall/pillar).
                    mods[i].movementAddend = toCenter.normalized * 18f;
                }
            }
            catch (System.Exception) { }
        }
    }
}