using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FastLoader
{
    [BepInPlugin("Ruin321.bbp.fastloader", "FastLoader", "1.3.1")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class FastLoaderPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Log = null;
        public static ConfigEntry<bool> EnableMultithreading = null;
        public static ConfigEntry<bool> ShowProgress = null;
        public static ConfigEntry<bool> EnableAssetCache = null;
        public static ConfigEntry<bool> SkipWait = null;

        public static bool CurrentMultithreading = false;
        public static bool CurrentShowProgress = true;
        public static bool CurrentAssetCache = true;
        public static bool CurrentSkipWait = false;

        public static FastLoaderPlugin Instance = null;

        public bool optionsMenuBuilt;
        public MenuToggle multithreadingToggle;
        public MenuToggle progressToggle;
        public MenuToggle assetCacheToggle;
        public MenuToggle skipWaitToggle;

        private void Awake()
        {
            Instance = this;
            Log = Logger;
            EnableMultithreading = Config.Bind("Experimental", "EnableMultithreading", false, "Force multithreading (UNSAFE!)");
            ShowProgress = Config.Bind("General", "ShowProgress", true, "Show generation progress in logs");
            EnableAssetCache = Config.Bind("General", "EnableAssetCache", true, "Cache decoded mod textures (auto-invalidates when the file changes)");
            SkipWait = Config.Bind("General", "SkipWait", false, "Reduce the 30 second forced wait when a level stalls, so the game force-starts much sooner");
            CurrentMultithreading = EnableMultithreading.Value;
            CurrentShowProgress = ShowProgress.Value;
            CurrentAssetCache = EnableAssetCache.Value;
            CurrentSkipWait = SkipWait.Value;
            CustomOptionsCore.OnMenuInitialize += OnMenu;
            new Harmony("thefunnyartist.bbp.fastloader").PatchAll();
            if (CurrentMultithreading)
            {
                Log.LogWarning("[FastLoader] Multithreading ENABLED - EXPERIMENTAL!");
            }
            Log.LogInfo("[FastLoader] Loaded - Level generation acceleration + fast asset loading enabled");
        }

        private void OnMenu(OptionsMenu menu, CustomOptionsHandler handler)
        {
            handler.AddCategory<FastLoaderOptionsCategory>("FastLoader");
        }

        private void Update()
        {
            if (!optionsMenuBuilt)
            {
                return;
            }
            if (multithreadingToggle != null && multithreadingToggle.Value != CurrentMultithreading)
            {
                CurrentMultithreading = multithreadingToggle.Value;
                EnableMultithreading.Value = CurrentMultithreading;
                Log.LogWarning("[FastLoader] Multithreading " + (CurrentMultithreading ? "ENABLED" : "DISABLED") + " - EXPERIMENTAL!");
            }
            if (progressToggle != null && progressToggle.Value != CurrentShowProgress)
            {
                CurrentShowProgress = progressToggle.Value;
                ShowProgress.Value = CurrentShowProgress;
            }
            if (assetCacheToggle != null && assetCacheToggle.Value != CurrentAssetCache)
            {
                CurrentAssetCache = assetCacheToggle.Value;
                EnableAssetCache.Value = CurrentAssetCache;
                if (!CurrentAssetCache)
                {
                    AssetBytesCache.Flush();
                }
                Log.LogInfo("[FastLoader] Fast asset loading " + (CurrentAssetCache ? "ENABLED" : "DISABLED"));
            }
            if (skipWaitToggle != null && skipWaitToggle.Value != CurrentSkipWait)
            {
                CurrentSkipWait = skipWaitToggle.Value;
                SkipWait.Value = CurrentSkipWait;
                Log.LogInfo("[FastLoader] Skip 30s wait " + (CurrentSkipWait ? "ENABLED" : "DISABLED"));
            }
        }
    }

    public class FastLoaderOptionsCategory : CustomOptionsCategory
    {
        private readonly System.Collections.Generic.List<GameObject> _pages = new System.Collections.Generic.List<GameObject>();
        private int _pageIndex = 0;
        private TextMeshProUGUI _pageIndicator;

        public override void Build()
        {
            // ---- 第 1 页：生成进度 + 资源加速 ----
            GameObject p1 = MakePage("Page1");
            MenuToggle progressVal = AddToggleTo(p1, "FLProgressToggle", "Show Progress", FastLoaderPlugin.ShowProgress.Value, 1,
                "Show level generation progress in the console");
            FastLoaderPlugin.Instance.progressToggle = progressVal;

            MenuToggle assetVal = AddToggleTo(p1, "FLAssetCacheToggle", "Fast Resource Loading", FastLoaderPlugin.EnableAssetCache.Value, 2,
                "Cache decoded mod textures by file, so repeated loads skip disk IO.\nAuto-invalidates whenever the file on disk changes,\nso hot-reload still picks up edits.");
            FastLoaderPlugin.Instance.assetCacheToggle = assetVal;

            // ---- 第 2 页：实验性 + 去掉 30 秒等待 ----
            GameObject p2 = MakePage("Page2");
            MenuToggle mtVal = AddToggleTo(p2, "FLMultithreadingToggle", "Multithreading", FastLoaderPlugin.EnableMultithreading.Value, 1,
                "If you enable this, the game might get weird!\nLike a bunch of strange bugs and stuff...\nAlso, the BB+ generator itself is inherently <color=red>UNSAFE THREADS</color>.\nThis option is off by default");
            FastLoaderPlugin.Instance.multithreadingToggle = mtVal;

            MenuToggle skipVal = AddToggleTo(p2, "FLSkipWaitToggle", "Skip 30s Wait", FastLoaderPlugin.SkipWait.Value, 2,
                "When a level gets stuck, the game normally forces you to wait 30 seconds\nbefore force-starting it. Enable this to cut that wait to ~5 seconds.");
            FastLoaderPlugin.Instance.skipWaitToggle = skipVal;

            // ---- 翻页箭头 + 页码 ----
            CreateButton(new UnityAction(() => SwitchPage(-1)), base.menuArrowLeft, base.menuArrowLeftHighlight,
                "FLPrevPage", new Vector3(-88f, -122f, 0f));
            CreateButton(new UnityAction(() => SwitchPage(1)), base.menuArrowRight, base.menuArrowRightHighlight,
                "FLNextPage", new Vector3(88f, -122f, 0f));
            _pageIndicator = CreateText("FLPageIndicator", "Page 1/2", new Vector3(0f, -122f, 0f),
                BaldiFonts.ComicSans24, TextAlignmentOptions.Center, new Vector2(140f, 32f), Color.black, false);

            // ---- Apply 按钮 ----
            CreateApplyButton(new UnityAction(ApplySettings));

            // 默认显示第 1 页
            _pages[0].SetActive(true);
            if (_pages.Count > 1)
                for (int i = 1; i < _pages.Count; i++) _pages[i].SetActive(false);

            FastLoaderPlugin.Instance.optionsMenuBuilt = true;
        }

        // 创建一页全屏容器（默认隐藏，由翻页翻转）。中心锚点铺满页面，子级 anchoredPosition 才有可靠参照。
        private GameObject MakePage(string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(transform, false);
            go.transform.localScale = Vector3.one;
            go.layer = LayerMask.NameToLayer("UI");
            RectTransform prt = (RectTransform)go.transform;
            prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 0.5f);
            prt.pivot = new Vector2(0.5f, 0.5f);
            prt.sizeDelta = new Vector2(680f, 540f);
            prt.anchoredPosition = Vector2.zero;
            go.SetActive(false);
            _pages.Add(go);
            return go;
        }

        // 在指定页创建开关。用中心锚点 + anchoredPosition 强制定位（x=0 水平居中）。
        private MenuToggle AddToggleTo(GameObject page, string id, string text, bool value, int order, string tooltip)
        {
            MenuToggle t = CreateToggle(id, text, value, Vector3.zero, 300f);
            t.transform.SetParent(page.transform, true);
            t.transform.localScale = Vector3.one;
            // 两行：第 1 行 y=18，第 2 行 y=-50
            CenterRect((RectTransform)t.transform, new Vector2(0f, order == 1 ? 18f : -50f), new Vector2(308f, 32f));
            try { AddTooltip(t, tooltip); } catch (System.Exception) { }
            return t;
        }

        internal static void CenterRect(RectTransform rt, Vector2 anchoredPos, Vector2 size)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;
        }

        private void SwitchPage(int delta)
        {
            if (_pages.Count == 0) return;
            _pages[_pageIndex].SetActive(false);
            _pageIndex = (_pageIndex + delta + _pages.Count) % _pages.Count;
            _pages[_pageIndex].SetActive(true);
            if (_pageIndicator != null)
                _pageIndicator.text = "Page " + (_pageIndex + 1) + "/" + _pages.Count;
        }

        private void ApplySettings()
        {
            if (FastLoaderPlugin.Instance.multithreadingToggle != null)
            {
                FastLoaderPlugin.CurrentMultithreading = FastLoaderPlugin.Instance.multithreadingToggle.Value;
                FastLoaderPlugin.EnableMultithreading.Value = FastLoaderPlugin.CurrentMultithreading;
                FastLoaderPlugin.Log.LogWarning("[FastLoader] Multithreading " + (FastLoaderPlugin.CurrentMultithreading ? "ENABLED" : "DISABLED") + " - EXPERIMENTAL!");
            }
            if (FastLoaderPlugin.Instance.progressToggle != null)
            {
                FastLoaderPlugin.CurrentShowProgress = FastLoaderPlugin.Instance.progressToggle.Value;
                FastLoaderPlugin.ShowProgress.Value = FastLoaderPlugin.CurrentShowProgress;
            }
            if (FastLoaderPlugin.Instance.assetCacheToggle != null)
            {
                FastLoaderPlugin.CurrentAssetCache = FastLoaderPlugin.Instance.assetCacheToggle.Value;
                FastLoaderPlugin.EnableAssetCache.Value = FastLoaderPlugin.CurrentAssetCache;
                if (!FastLoaderPlugin.CurrentAssetCache)
                {
                    AssetBytesCache.Flush();
                }
            }
            if (FastLoaderPlugin.Instance.skipWaitToggle != null)
            {
                FastLoaderPlugin.CurrentSkipWait = FastLoaderPlugin.Instance.skipWaitToggle.Value;
                FastLoaderPlugin.SkipWait.Value = FastLoaderPlugin.CurrentSkipWait;
            }
        }
    }
}
