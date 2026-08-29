using BepInEx.Configuration;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MilkItem
{
    
    
    
    
    
    
    
    
    
    
    
    public static class MilkSettings
    {
        
        public static ConfigEntry<bool> Enable99EasterEgg;
        
        public static ConfigEntry<bool> Remove20sLoading;
        
        public static ConfigEntry<bool> LogOutput;
        
        public static ConfigEntry<bool> Skip99SnowScreen;
        
        public static ConfigEntry<bool> NoFakeMilkSalesman;
        
        public static ConfigEntry<bool> WindowEffects;

        public static void Init(ConfigFile config)
        {
            Enable99EasterEgg = config.Bind("MilkSettings", "Enable99EasterEgg", true,
                "Enable the 99 Easter Egg sequence.");
            Remove20sLoading = config.Bind("MilkSettings", "Remove20sLoading", false,
                "Remove the 20 second joke loading delay on startup.");
            LogOutput = config.Bind("MilkSettings", "LogOutput", false,
                "Enable info log output (for troubleshooting).");
            Skip99SnowScreen = config.Bind("MilkSettings", "Skip99SnowScreen", false,
                "Skip the 50 second snow/static movie after drinking 99 Milk.");
            NoFakeMilkSalesman = config.Bind("MilkSettings", "NoFakeMilkSalesman", false,
                "Do not spawn the Fake Black Milk Salesman in red-heat floors.");
            WindowEffects = config.Bind("WindowEffects", "WindowEffects", true,
                "Master toggle for all window tricks (red-heat rhythmic movement, snow screen jitter, window-milk falling).");

            try { CustomOptionsCore.OnMenuInitialize += MilkSettings.Register; }
            catch (System.Exception) { }
        }

        private static void Register(OptionsMenu menu, CustomOptionsHandler handler)
        {
            handler.AddCategory<MilkOptionsCategory>("Milk");
        }
    }

    
    
    
    
    public class MilkOptionsCategory : CustomOptionsCategory
    {
        private readonly System.Collections.Generic.List<GameObject> _pages = new System.Collections.Generic.List<GameObject>();
        private int _pageIndex = 0;
        private TextMeshProUGUI _pageIndicator;
        private MenuToggle _t99, _t20s, _tSnow, _tNoFake, _tWinFx;
        private StandardMenuButton _reloadBtn;

        public override void Build()
        {
            
            Sprite milkSpr = null;
            try { milkSpr = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 50f, "Milk_Large.png"); }
            catch (System.Exception) { }
            if (milkSpr != null)
            {
                
                
                Image icon = CreateImage(milkSpr, "MilkIcon", Vector3.zero, new Vector2(96f, 96f));
                CenterRect((RectTransform)icon.transform, new Vector2(0f, 60f), new Vector2(96f, 96f));
            }

            
            GameObject p1 = MakePage("Page1");
            _t99 = AddToggleTo(p1, "Enable99", "Enable 99 Easter Egg", MilkSettings.Enable99EasterEgg.Value, 1,
                "When off, drinking 99 Milk becomes a normal milk.");
            _t20s = AddToggleTo(p1, "Remove20s", "Remove 20s Loading", MilkSettings.Remove20sLoading.Value, 2,
                "Skip the 20 second joke loading delay on startup.");

            
            GameObject p2 = MakePage("Page2");
            _reloadBtn = MakeReloadButton(p2);

            
            GameObject p3 = MakePage("Page3");
            _tSnow = AddToggleTo(p3, "SkipSnow", "Skip 99 Snow Screen", MilkSettings.Skip99SnowScreen.Value, 1,
                "Skip the long snow/static movie after drinking 99 Milk.");
            _tNoFake = AddToggleTo(p3, "NoFakeSales", "No Fake Milk Salesman", MilkSettings.NoFakeMilkSalesman.Value, 2,
                "Do not spawn the Fake Black Milk Salesman in red-heat floors.");

            
            GameObject p4 = MakePage("Page4");
            _tWinFx = AddToggleTo(p4, "WindowFx", "Window Effects", MilkSettings.WindowEffects.Value, 1,
                "Master toggle: enable/disable ALL window tricks.\n" +
                "Turns off red-heat rhythmic motion, snow screen\njitter and window-milk falling.");

            
            CreateButton(new UnityAction(() => SwitchPage(-1)), base.menuArrowLeft, base.menuArrowLeftHighlight,
                "PrevPage", new Vector3(-88f, -122f, 0f));
            CreateButton(new UnityAction(() => SwitchPage(1)), base.menuArrowRight, base.menuArrowRightHighlight,
                "NextPage", new Vector3(88f, -122f, 0f));
            _pageIndicator = CreateText("PageIndicator", "Page 1/4", new Vector3(0f, -122f, 0f),
                BaldiFonts.ComicSans24, TextAlignmentOptions.Center, new Vector2(140f, 32f), Color.black, false);

            
            CreateApplyButton(new UnityAction(Apply));

            
            _pages[0].SetActive(true);
            if (_pages.Count > 1)
                for (int i = 1; i < _pages.Count; i++) _pages[i].SetActive(false);
        }

        
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

        
        
        private MenuToggle AddToggleTo(GameObject page, string id, string text, bool value, int order, string tooltip)
        {
            MenuToggle t = CreateToggle(id, text, value, Vector3.zero, 300f);
            t.transform.SetParent(page.transform, true);
            t.transform.localScale = Vector3.one;
            
            CenterRect((RectTransform)t.transform, new Vector2(0f, order == 1 ? 18f : -50f), new Vector2(308f, 32f));
            try { AddTooltip(t, tooltip); } catch (System.Exception) { }
            return t;
        }

        
        private StandardMenuButton MakeReloadButton(GameObject page)
        {
            StandardMenuButton btn = null;
            try
            {
                btn = CreateTextButton(new UnityAction(OnReloadClicked), "ReloadButton",
                    "Reload Mod Content", Vector3.zero, BaldiFonts.ComicSans24, TextAlignmentOptions.Center,
                    new Vector2(360f, 32f), Color.black);
                btn.transform.SetParent(page.transform, true);
                btn.transform.localScale = Vector3.one;
                CenterRect((RectTransform)btn.transform, new Vector2(0f, 18f), new Vector2(360f, 32f));
                try { AddTooltip(btn, "Reload all poster textures and localization from the mod folder. No restart needed."); } catch (System.Exception) { }
            }
            catch (System.Exception) { }
            return btn;
        }

        private void OnReloadClicked()
        {
            try { Plugin.ReloadModdedContent(); } catch (System.Exception) { }
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
                _pageIndicator.text = $"Page {_pageIndex + 1}/{_pages.Count}";
        }

        private void Apply()
        {
            if (_t99 != null) MilkSettings.Enable99EasterEgg.Value = _t99.Value;
            if (_t20s != null) MilkSettings.Remove20sLoading.Value = _t20s.Value;
            if (_tSnow != null) MilkSettings.Skip99SnowScreen.Value = _tSnow.Value;
            if (_tNoFake != null) MilkSettings.NoFakeMilkSalesman.Value = _tNoFake.Value;
            if (_tWinFx != null) MilkSettings.WindowEffects.Value = _tWinFx.Value;
        }
    }
}