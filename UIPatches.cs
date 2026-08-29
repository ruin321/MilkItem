using System;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace MilkItem
{
    
    
    internal static class LocalizedUI
    {
        public static string L(string key)
        {
            try { return Singleton<LocalizationManager>.Instance.GetLocalizedText(key); }
            catch (System.Exception) { return key; }
        }
    }

    
    
    
    
    

    
    
    
    
    [HarmonyPatch(typeof(TMPro.TMP_Text), "text", MethodType.Setter)]
    public class VersionTextPatch
    {
        
        
        
        public static void Prefix(TMPro.TMP_Text __instance, ref string __0)
        {
            if (string.IsNullOrEmpty(__0)) return;
            
            string ver = UnityEngine.Application.version;
            if (!string.IsNullOrEmpty(ver) && __0.Contains(ver))
            {
                __0 = "Milk";
                return;
            }
            
            if (__0.Contains("0.14.5") || __0.Contains("0.14.2"))
            {
                __0 = "Milk";
            }
        }
    }

    
    [HarmonyPatch(typeof(GameObject), "SetActive")]
    public class AboutMenuPatch
    {
        [HarmonyPostfix]
        public static void PostfixSetActive(GameObject __instance, bool value)
        {
            if (__instance.name == "About" && value)
            {
                ApplyCustomText(__instance.transform);
            }
        }

        private static void ApplyCustomText(Transform aboutTransform)
        {
            Transform titleTransform = FindChildByName(aboutTransform, "DevUpdateTitle");
            if (titleTransform != null)
            {
                TextMeshProUGUI titleText = titleTransform.GetComponent<TextMeshProUGUI>();
                if (titleText != null)
                {
                    titleText.text = LocalizedUI.L("Mu_About_Title");
                }
            }

            Transform textTransform = FindChildByName(aboutTransform, "DevUpdateText");
            if (textTransform != null)
            {
                TextMeshProUGUI devText = textTransform.GetComponent<TextMeshProUGUI>();
                if (devText != null)
                {
                    devText.text = LocalizedUI.L("Mu_About_Body");
                }
            }
        }

        private static Transform FindChildByName(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform result = FindChildByName(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }

    
    
    
    
    
    
    
    
    
    
    
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public class MainMenuPatch
    {
        
        public static bool UseCustomMenuAPI = false;

        
        [HarmonyPrefix]
        public static void Prefix()
        {
            if (Plugin.Instance == null) return;
            
            
            
            if (HasPersistedMenuChoice()) return;
            ForceMilkMenuDefault();
        }

        
        
        private static bool HasPersistedMenuChoice()
        {
            try
            {
                var pfm = Singleton<PlayerFileManager>.Instance;
                string fileName = (pfm != null) ? pfm.fileName : null;
                if (string.IsNullOrEmpty(fileName)) return true; 
                string dir = System.IO.Path.Combine(
                    Application.persistentDataPath, "Modded", fileName,
                    "pixelguy.pixelmodding.baldiplus.custommainmenusapi");
                return System.IO.File.Exists(System.IO.Path.Combine(dir, "moddedMenusData.dat"));
            }
            catch (System.Exception)
            {
                return true; 
            }
        }

        [HarmonyPostfix]
        public static void Postfix(MainMenu __instance)
        {
            if (Plugin.Instance == null) return;

            
            TMPro.TMP_Text[] texts = __instance.GetComponentsInChildren<TMPro.TMP_Text>(true);
            foreach (TMPro.TMP_Text txt in texts)
            {
                if (!string.IsNullOrEmpty(txt.text) && txt.text.Contains("Baldi's Basics Plus"))
                {
                    txt.text = txt.text.Replace("Baldi's Basics Plus", LocalizedUI.L("Mu_MainTitle"));
                }
            }

            
            
            
            
            
            ReplaceVersionLabel(__instance.transform);
        }

        private static void ReplaceVersionLabel(Transform menuRoot)
        {
            Transform[] candidates = new Transform[]
            {
                menuRoot.Find("Version"),
                (menuRoot.parent != null) ? menuRoot.parent.Find("Version") : null
            };
            foreach (Transform verT in candidates)
            {
                if (verT == null) continue;
                TMPro.TMP_Text vt = verT.GetComponent<TMPro.TMP_Text>();
                if (vt != null)
                {
                    vt.text = "Milk";
                    
                    break;
                }
            }
        }

        
        
        
        private static void ForceMilkMenuDefault()
        {
            try
            {
                Type pluginType = Type.GetType("CustomMainMenusAPI.CustomMainMenusPlugin, CustomMainMenusAPI");
                Type objType = Type.GetType("CustomMainMenusAPI.MainMenuObject, CustomMainMenusAPI");
                if (pluginType == null || objType == null) return;
                var availField = objType.GetField("availableObjects", BindingFlags.NonPublic | BindingFlags.Static);
                var idxField = pluginType.GetField("mainMenuObjIndex", BindingFlags.Public | BindingFlags.Static);
                if (availField == null || idxField == null) return;
                var list = availField.GetValue(null) as System.Collections.IList;
                if (list == null) return;
                for (int i = 0; i < list.Count; i++)
                {
                    var o = list[i];
                    var nameField = o.GetType().GetField("localizedName", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (nameField?.GetValue(o) as string == "Ed_MilkMenu")
                    {
                        idxField.SetValue(null, i);
                        
                        return;
                    }
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }
    }
}
