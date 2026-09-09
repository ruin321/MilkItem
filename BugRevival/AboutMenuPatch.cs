using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BugRevival
{
    // Mirrors the milk mod's AboutMenuPatch: the About popup GameObject is named "About"
    // and only activates when the player opens it, so we hook GameObject.SetActive and
    // rewrite its body (child "DevUpdateText") the moment it appears.
    [HarmonyPatch(typeof(GameObject), "SetActive")]
    public static class AboutMenuPatch
    {
        private const string AboutBody =
            "Hello Baldi's Basics Plus players!\n\n" +
            "Thank you for playing Baldi's Basics Plus. After so many updates, we felt it was time to introduce a branch update.\n\n" +
            "This update adds a few more bugs. Yes, you read that correctly\u2014more bugs! Along with some fixed bugs, self-made bugs... and a few more bugs.\n\n" +
            "That's all for now. Goodbye, and we hope you enjoy this version!\n\n" +
            "\u2014 Micah McGonigal, August 2026";

        [HarmonyPostfix]
        public static void Postfix(GameObject __instance, bool value)
        {
            if (!value || __instance == null) return;
            if (__instance.name != "About") return;
            Transform body = FindChildByName(__instance.transform, "DevUpdateText");
            if (body == null) return;
            TMP_Text t = body.GetComponent<TMP_Text>();
            if (t != null) t.text = AboutBody;
        }

        private static Transform FindChildByName(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                Transform nested = FindChildByName(child, name);
                if (nested != null) return nested;
            }
            return null;
        }
    }
}