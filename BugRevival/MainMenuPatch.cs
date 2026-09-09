using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BugRevival
{
    // Models the milk mod's MainMenuPatch: after the main menu starts,
    // rewrites the version label to "BUG" and the copyright line to "©2026 BUG Games".
    // Both are hardcoded TMP_Text children of the menu scene, so a set_text patch alone
    // never touches them; we scan the hierarchy directly instead.
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public static class MainMenuPatch
    {
        [HarmonyPostfix]
        public static void Postfix(MainMenu __instance)
        {
            // Version label: a child object named "Version" holding a TMP_Text.
            Transform root = __instance.transform;
            Transform version = root.Find("Version");
            if (version == null && root.parent != null) version = root.parent.Find("Version");
            if (version != null)
            {
                TMP_Text vt = version.GetComponent<TMP_Text>();
                if (vt != null) vt.text = "BUG";
            }

            // Copyright line: any TMP_Text under the menu containing the copyright marker.
            TMP_Text[] all = __instance.GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text tmp in all)
            {
                if (tmp == null || string.IsNullOrEmpty(tmp.text)) continue;
                if (tmp.text.Contains("\u00a9") || tmp.text.Contains("Basically Games"))
                {
                    tmp.text = "\u00a92026 BUG Games";
                }
            }
        }
    }
}