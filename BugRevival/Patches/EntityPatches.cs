using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace BugRevival
{
    // Entity movement/collision related bug revivals.
    //   - NpcPhaseMachines (0.1.3): NPCs pass through math machines
    //   - NpcStuckWalls (0.13): NPCs occasionally get stuck in walls
    [HarmonyPatch(typeof(Entity), "MoveWithCollision")]
    internal class EntityCollisionPatch
    {
        private static readonly System.Random rng = new System.Random();

        [HarmonyPrefix]
        internal static void Prefix(Entity __instance, ref bool __runOriginal, ref Vector3 movement)
        {
            bool stuckWalls = BugConfig.NpcStuckWalls.Value;
            bool phaseMachines = BugConfig.NpcPhaseMachines.Value;
            if (!stuckWalls && !phaseMachines) return;

            try
            {
                // Only affects NPCs (not players), to avoid trapping/clipping the player
                if (__instance.name != null && __instance.name.Contains("Player")) return;

                if (phaseMachines && NearMathMachine(__instance))
                {
                    // NPCs pass straight through math machine cells
                    __runOriginal = false;
                    return;
                }

                if (stuckWalls && rng.NextDouble() < 0.03)
                {
                    // Occasionally invalidate this collision → NPC clips into the wall, forming a stuck-in-wall effect
                    __runOriginal = false;
                }
            }
            catch (System.Exception) { }
        }

        private static bool NearMathMachine(Entity e)
        {
            try
            {
                foreach (var mm in Object.FindObjectsOfType<MathMachine>())
                {
                    if (Vector3.Distance(mm.transform.position, e.transform.position) < 2.2f)
                        return true;
                }
            }
            catch (System.Exception) { }
            return false;
        }
    }

    // BalloonHyperSpeed (0.3): balloon speed accumulates each frame, breaking past the original 1~10 speed range, flying faster and faster without ever slowing
    [HarmonyPatch(typeof(Balloon), "Update")]
    internal class BalloonHyperSpeedPatch
    {
        static void Postfix(Balloon __instance)
        {
            if (!BugConfig.BalloonHyperSpeed.Value) return;
            try
            {
                __instance.speed += Time.deltaTime * 12f;   // unlimited continuous acceleration
                __instance.minSpeed = 0f;                    // disable the "slow down then turn" fallback to keep accelerating
            }
            catch (System.Exception) { }
        }
    }

    // PlaytimeNoWin (0.5): the jump-rope must never let you win → in the private RopeDown(), every completed
    // rope actually keeps the jump counter low, so RopeTimer's while(jumps<maxJumps) loop never exits. You can
    // only stop by failing the rope and getting punished (matches "jump until punished"). Immediate at runtime.
    [HarmonyPatch(typeof(Jumprope), "RopeDown")]
    internal class PlaytimeNoWinPatch
    {
        static void Postfix(Jumprope __instance)
        {
            if (!BugConfig.PlaytimeNoWin.Value) return;
            try
            {
                // Reset the success counter after every rope hit so it can never reach maxJumps (5).
                var f = typeof(Jumprope).GetField("jumps",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (f != null) f.SetValue(__instance, 0);
            }
            catch (System.Exception) { }
        }
    }

    // PrincipalInfiniteDetention (0.2): detention always lasts a very long time → reproducing the "back-to-back detention" nightmare
    [HarmonyPatch(typeof(DetentionRoomFunction), "Activate")]
    internal class PrincipalDetentionPatch
    {
        [HarmonyPrefix]
        static void Prefix(ref float time)
        {
            if (!BugConfig.PrincipalInfiniteDetention.Value) return;
            try { time = 99f; } catch (System.Exception) { }
        }
    }

    // SweepIgnoresSlowmo (0.12): Gotta Sweep's push no longer scales with the NPC time scale, so it still shoves you at full force during slow-mo/time-stopped events
    [HarmonyPatch(typeof(GottaSweep), "VirtualUpdate")]
    internal class GottaSweepPushPatch
    {
        private static readonly System.Reflection.FieldInfo moveModField =
            typeof(GottaSweep).GetField("moveMod",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        private static readonly System.Reflection.FieldInfo ecField =
            typeof(NPC).GetField("ec",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        static void Postfix(GottaSweep __instance)
        {
            if (!BugConfig.SweepIgnoresSlowmo.Value) return;
            try
            {
                var moveMod = moveModField?.GetValue(__instance) as MovementModifier;
                var ec = ecField?.GetValue(__instance) as EnvironmentController;
                if (moveMod == null || ec == null) return;
                float ts = ec.NpcTimeScale;
                if (ts != 0f) moveMod.movementAddend = moveMod.movementAddend / ts; // undo the NpcTimeScale factor → full-force push
            }
            catch (System.Exception) { }
        }
    }

    // FirstPrizeNoSpawn (0.2): Hide-&-Seek never generated 1st Prize. Destroy the NPC the instant it initializes.
    [HarmonyPatch(typeof(FirstPrize), "Initialize")]
    internal class FirstPrizeNoSpawnPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(FirstPrize __instance)
        {
            if (!BugConfig.FirstPrizeNoSpawn.Value) return;
            try
            {
                Object.Destroy(__instance.gameObject);
            }
            catch (System.Exception) { }
        }
    }

    // HideHudStuck (0.6): the HUD never restores after the player un-hides; swallow the restore call so it stays hidden
    [HarmonyPatch(typeof(HudManager), "Hide")]
    internal class HideHudStuckPatch
    {
        static bool Prefix(bool val)
        {
            if (!BugConfig.HideHudStuck.Value) return true;
            return val; // when already hidden (val=true) keep hiding; when asked to restore (val=false) skip → HUD stays stuck hidden
        }
    }
}