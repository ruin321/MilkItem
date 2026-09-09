using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using UnityEngine;

namespace BugRevival
{
    [BepInPlugin("Ruin321.bbbug.ohno2manybug", "BaldisbasicsBUG", "1.0.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    [BepInDependency("pixelguy.pixelmodding.baldiplus.custommainmenusapi", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "Ruin321.bbbug.ohno2manybug";
        public static Plugin Instance;
        public static ManualLogSource Log;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            BugConfig.Init(Config);

            var harmony = new Harmony(GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // The settings menu is optional: only register it when the config option is enabled.
            try
            {
                if (BugConfig.ShowSettingsMenu.Value)
                    CustomOptionsCore.OnMenuInitialize += BugRevivalSettings.Register;
            }
            catch (System.Exception) { }

            try { ReloadLocalization(); } catch (System.Exception) { }

            // Register the custom main menu background (no-op if CustomMainMenusAPI isn't loaded).
            CustomMenu.Register();

            // Start persistent per-frame effects (stamina-in-locker, locker-camera, tilted camera) right now
            // when their config is on — NOT only from the settings Apply — so even a hidden settings menu
            // (ShowSettingsMenu off) still applies them.
            try
            {
                if (BugConfig.StaminaInLocker.Value || BugConfig.LockerCamRotate.Value || BugConfig.TiltedCamera.Value)
                    EffectsUpdater.Ensure();
            }
            catch (System.Exception) { }

            // EffectsUpdater is lazy-loaded otherwise; with everything off it adds zero resident overhead.
            Logger.LogInfo("[BugRevival] loaded.");
        }

        // ---------- Localization (matches the funk/milk mods' pattern) ----------

        [System.Serializable]
        public class LocalizationData
        {
            public LocalizationItem[] items;
        }

        [System.Serializable]
        public class LocalizationItem
        {
            public string key;
            public string value;
        }

        public static void ReloadLocalization()
        {
            try
            {
                var mgr = Singleton<LocalizationManager>.Instance;
                if (mgr == null) return;
                var field = typeof(LocalizationManager).GetField("localizedText",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null) return;
                var dict = field.GetValue(mgr) as Dictionary<string, string>;
                if (dict == null) return;

                LoadLocaFileInto(dict, "Localization.json");

                string lang = DetectCurrentLanguage();
                if (lang == "SChinese") LoadLocaFileInto(dict, "Localization.Chinese.json");
                else if (lang == "TChinese") LoadLocaFileInto(dict, "Localization.ChineseTraditional.json");
            }
            catch (System.Exception) { }
        }

        private static void LoadLocaFileInto(Dictionary<string, string> dict, string file)
        {
            try
            {
                string path = Path.Combine(AssetLoader.GetModPath(Instance), file);
                if (!File.Exists(path)) return;
                var data = JsonUtility.FromJson<LocalizationData>(File.ReadAllText(path));
                if (data == null || data.items == null) return;
                foreach (var item in data.items)
                {
                    if (item == null || string.IsNullOrEmpty(item.key)) continue;
                    dict[item.key] = item.value;
                }
            }
            catch (System.Exception) { }
        }

        private static string DetectCurrentLanguage()
        {
            try
            {
                System.Reflection.Assembly bbpcAsm = null;
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.GetName().Name == "BBPC") { bbpcAsm = asm; break; }
                }
                if (bbpcAsm == null) return "";
                System.Type bbpcPlugin = bbpcAsm.GetType("BBPC.Plugin");
                if (bbpcPlugin == null) return "";
                var instanceProp = bbpcPlugin.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                object bbpcInstance = instanceProp?.GetValue(null);
                if (bbpcInstance == null) return "";
                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                System.Type cfgMgr = bbpcAsm.GetType("BBPC.API.ConfigManager") ?? bbpcAsm.GetType("BBPC.ConfigManager");
                object cfgEntry = null;
                var prop = cfgMgr?.GetProperty("currect_lang", flags);
                if (prop != null) cfgEntry = prop.GetValue(null) ?? prop.GetValue(bbpcInstance);
                if (cfgEntry == null)
                {
                    var backing = cfgMgr?.GetField("<currect_lang>k__BackingField", flags);
                    cfgEntry = backing?.GetValue(null) ?? backing?.GetValue(bbpcInstance);
                }
                if (cfgEntry == null)
                {
                    var field = cfgMgr?.GetField("currect_lang", flags);
                    cfgEntry = field?.GetValue(null) ?? field?.GetValue(bbpcInstance);
                }
                return (string)cfgEntry?.GetType().GetProperty("Value")?.GetValue(cfgEntry) ?? "";
            }
            catch (System.Exception) { return ""; }
        }

        public static string GetLangText(string key, string fallback)
        {
            try
            {
                var mgr = Singleton<LocalizationManager>.Instance;
                if (mgr == null) return fallback;
                string s = mgr.GetLocalizedText(key);
                if (!string.IsNullOrEmpty(s) && s != key) return s;
            }
            catch (System.Exception) { }
            return fallback;
        }
    }
}