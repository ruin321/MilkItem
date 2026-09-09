using HarmonyLib;
using UnityEngine;

namespace BugRevival
{
    // GumThroughWindow: gum passes through windows (0.3.6)
    [HarmonyPatch(typeof(Gum), "OnEntityMoveCollision")]
    internal class GumThroughWindowPatch
    {
        [HarmonyPrefix]
        internal static void Prefix(ref bool __runOriginal, Gum __instance, RaycastHit hit)
        {
            if (!BugConfig.GumThroughWindow.Value) return;
            try
            {
                // When hitting a window/indoor passable object, don't stick the gum to the wall; let it fly through.
                if (hit.transform != null && hit.transform.CompareTag("Window"))
                {
                    __runOriginal = false;
                }
            }
            catch (System.Exception) { }
        }
    }

    // HookThroughWall: hook passes through rotary/swinging-door walls (0.3.5)
    [HarmonyPatch(typeof(ITM_GrapplingHook), "OnEntityMoveCollision")]
    internal class HookThroughWallPatch
    {
        static void Prefix(ITM_GrapplingHook __instance, ref bool __runOriginal, RaycastHit hit)
        {
            if (!BugConfig.HookThroughWall.Value) return;
            try
            {
                if (hit.transform != null &&
                    (hit.transform.CompareTag("Window") ||
                     hit.transform.GetComponentInParent<SwingDoor>() != null))
                {
                    __runOriginal = false;
                }
            }
            catch (System.Exception) { }
        }
    }

    // NanaPeelBounce: banana peel bounces off bullies endlessly + noise (0.5.1)
    [HarmonyPatch(typeof(ITM_NanaPeel), "EntityTriggerStay")]
    internal class NanaPeelBouncePatch
    {
        static void Postfix(ITM_NanaPeel __instance, Entity otherEntity)
        {
            if (!BugConfig.NanaPeelBounce.Value) return;
            try
            {
                if (otherEntity == null || otherEntity.name == null) return;
                bool isBully = otherEntity.name.ToLowerInvariant().Contains("bully");
                if (!isBully) return;

                var entity = NanaPeelReflect.PeelEntity(__instance);
                if (entity == null) return;

                // Give the peel a sustained outward push → after hitting a bully it keeps bouncing away and re-colliding, forming endless bounces
                Vector3 direction = (__instance.transform.position - otherEntity.transform.position);
                direction.y = 0f;
                if (direction.sqrMagnitude < 0.01f) direction = Vector3.forward;
                direction.Normalize();
                float speed = 12f;
                entity.UpdateInternalMovement(direction * speed);
            }
            catch (System.Exception) { }
        }
    }

    // Use reflection to get the Entity held by ITM_NanaPeel (field name: entity)
    internal static class NanaPeelReflect
    {
        internal static Entity PeelEntity(ITM_NanaPeel peel)
        {
            try
            {
                var f = typeof(ITM_NanaPeel).GetField("entity",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (Entity)f?.GetValue(peel);
            }
            catch (System.Exception) { return null; }
        }
    }

    // DoorNoBlock: walk through even when closed (0.2) — re-open the block the instant the door shuts, letting the player and all entities (incl. Baldi) pass straight through
    [HarmonyPatch(typeof(Door), "Shut")]
    internal class DoorNoBlockPatch
    {
        static void Postfix(Door __instance)
        {
            if (!BugConfig.DoorNoBlock.Value) return;
            try
            {
                __instance.open = true;   // door appears closed but actually remains passable
                __instance.Block(block: false); // remove the tile block → player and NPCs can pass
            }
            catch (System.Exception) { }
        }
    }

    // DoorStaysOpen (0.3): doors take several times longer to close after opening — player/entities pass freely throughout
    [HarmonyPatch(typeof(Door), "OpenTimed")]
    internal class DoorStaysOpenPatch
    {
        static void Prefix(ref float time)
        {
            if (!BugConfig.DoorStaysOpen.Value) return;
            try { time = time * 4f; } catch (System.Exception) { }
        }
    }

    // MathMachineAlwaysWrong (0.1.3): math machine always marks you wrong and still enrages Baldi (revives the "no points for correct + pointless Baldi anger" flaw)
    [HarmonyPatch(typeof(MathMachine), "Clicked")]
    internal class MathMachineAlwaysWrongPatch
    {
        private static readonly System.Reflection.FieldInfo answerField =
            typeof(MathMachine).GetField("answer",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        static void Prefix(MathMachine __instance)
        {
            if (!BugConfig.MathMachineAlwaysWrong.Value) return;
            try
            {
                // Set the correct answer to an impossible value → the == answer check in vanilla Clicked is always false, always taking the wrong-answer branch
                if (answerField != null) answerField.SetValue(__instance, -999999);
            }
            catch (System.Exception) { }
        }
    }

    // TutorialCanPause (0.10): tutorial sets CoreGameManager.disablePause=true → pressing ESC cannot pause. Revive the "can pause in tutorial" behavior.
    [HarmonyPatch(typeof(CoreGameManager), "Pause")]
    internal class TutorialCanPausePatch
    {
        static void Prefix(CoreGameManager __instance)
        {
            if (!BugConfig.TutorialCanPause.Value) return;
            try { __instance.disablePause = false; } catch (System.Exception) { }
        }
    }

    // AppleTreeNoFruit (0.3): revive "apple trees never fruit" - swallow the hook trigger so trees never drop fruit.
    [HarmonyPatch(typeof(AppleTree), "OnTriggerEnter")]
    internal class AppleTreeNoFruitPatch
    {
        static bool Prefix()
        {
            return !BugConfig.AppleTreeNoFruit.Value; // skip original if enabled -> no fruit ever drops
        }
    }
}