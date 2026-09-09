using System.Collections.Generic;
using MTM101BaldAPI;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BugRevival
{
    internal class BugRevivalSettings : CustomOptionsCategory
    {
        private readonly List<GameObject> _pages = new List<GameObject>();
        private int _pageIndex = 0;
        private TextMeshProUGUI _pageIndicator;

        // Toggles per page (used by Apply to batch-write)
        private readonly List<MenuToggle> _toggles = new List<MenuToggle>();

        // First-page warning shown before any bug toggles.
        private const string WarningBodyText =
            "Some of the bug reverts in this mod might not work correctly, and some toggles could be risky or just make the game harder.\n\n" +
            "Be careful with any toggle that has a softlock warning \u2014 it might cause lag!\n\n" +
            "Also, I used AI to help track down some bugs I thought I could actually revert (sorry about the AI, my bad!). And I added some original bugs too... yeah.\n\n" +
            "If you're cool with all that, turn to the next page.";

        // id → (introduced version, requires restart). Requires restart = the effect only applies when the object is created/started; turning it on at runtime won't take effect immediately.
        private static readonly Dictionary<string, System.Tuple<string, bool>> BugMeta =
            new Dictionary<string, System.Tuple<string, bool>>
            {
                { "GumThroughWindow",    System.Tuple.Create("0.3.6", false) },
                { "HookThroughWall",     System.Tuple.Create("0.3.5", false) },
                { "DoorNoBlock",         System.Tuple.Create("0.2",   false) },
                { "DoorStaysOpen",       System.Tuple.Create("0.3",   false) },
                { "MathMachineWrong",    System.Tuple.Create("0.1.3", false) },
                { "NanaPeelBounce",      System.Tuple.Create("0.5.1", false) },
                { "WhirlpoolToPillar",   System.Tuple.Create("0.13",  false) },
                { "NpcPhaseMachines",    System.Tuple.Create("0.1.3", false) },
                { "NpcStuckWalls",       System.Tuple.Create("0.13",  false) },
                { "BalloonHyperSpeed",   System.Tuple.Create("0.3",   false) },
                { "AppleTreeNoFruit",     System.Tuple.Create("0.3",   false) },
                { "DoubleStaminaDrain",  System.Tuple.Create("0.1",   false) },
                { "StaminaInLocker",     System.Tuple.Create("0.1.5", false) },
                { "LockerCamRotate",     System.Tuple.Create("0.14.2", false) },
                { "BaldiSlowAtBooks",    System.Tuple.Create("0.13",  false) },
                { "BaldiWallHit",        System.Tuple.Create("0.4",   false) },
                { "BaldiAngerBoost",     System.Tuple.Create("0.2",   false) },
                { "BaldiHyperSpeed",     System.Tuple.Create("0.4",   false) },
                { "InfiniteDetention",   System.Tuple.Create("0.2",   false) },
                { "PlaytimeNoWin",       System.Tuple.Create("0.5",   false) },
                { "SweepIgnoresSlowmo",   System.Tuple.Create("0.12",   false) },
                { "EraseAllSaves",        System.Tuple.Create("0.9",    false) },
                { "HideHudStuck",         System.Tuple.Create("0.6",    false) },
                { "TutorialCanPause",    System.Tuple.Create("0.10",  false) },
                { "AppleTreeFullDrop",   System.Tuple.Create("Original", false) },
                { "BSODAFreezeNPC",       System.Tuple.Create("Original", false) },
                { "TiltedCamera",          System.Tuple.Create("Original", false) },
                { "FirstPrizeNoSpawn",    System.Tuple.Create("0.2",    false) },
            };

            // High-risk behavior bugs that mess with physics/state: can destabilize or crash/softlock the session.
            private static readonly HashSet<string> HighRiskBugs = new HashSet<string>
            {
                "DoorNoBlock", "DoorStaysOpen", "NanaPeelBounce", "WhirlpoolToPillar",
                "NpcPhaseMachines", "NpcStuckWalls", "BalloonHyperSpeed",
                "BaldiWallHit", "BaldiAngerBoost", "BaldiHyperSpeed", "InfiniteDetention",
                "SweepIgnoresSlowmo", "FirstPrizeNoSpawn",
            };

        public static void Register(OptionsMenu menu, CustomOptionsHandler handler)
        {
            handler.AddCategory<BugRevivalSettings>("BugRevival");
        }

        public override void Build()
        {
            try { Plugin.ReloadLocalization(); } catch (System.Exception) { }

            AddWarningPage();
            AddPage1();
            AddPage2();
            AddPage3();
            AddPage4();
            AddPage5();
            AddPage6();

            // Page buttons + page indicator (same as MilkSettings)
            CreateButton(new UnityAction(() => SwitchPage(-1)), base.menuArrowLeft, base.menuArrowLeftHighlight,
                "PrevPage", new Vector3(-88f, -166f, 0f));
            CreateButton(new UnityAction(() => SwitchPage(1)), base.menuArrowRight, base.menuArrowRightHighlight,
                "NextPage", new Vector3(88f, -166f, 0f));
            _pageIndicator = CreateText("PageIndicator",
                PageIndicatorText(0), new Vector3(0f, -166f, 0f),
                BaldiFonts.ComicSans24, TextAlignmentOptions.Center, new Vector2(140f, 32f), Color.black);

            CreateApplyButton(new UnityAction(Apply));

            if (_pages.Count > 0) _pages[0].SetActive(true);
            for (int i = 1; i < _pages.Count; i++) _pages[i].SetActive(false);
        }

        // ---------- Page content ----------

        private void AddWarningPage()
        {
            GameObject p = MakePage("Warning");

            TextMeshProUGUI title = CreateText("WarningTitle", "<color=red>WARNING:</color>",
                Vector3.zero, BaldiFonts.ComicSans24, TextAlignmentOptions.Center,
                new Vector2(500f, 30f), Color.black, false);
            title.transform.SetParent(p.transform, true);
            title.transform.localScale = Vector3.one;
            CenterRect((RectTransform)title.transform, new Vector2(0f, 90f), new Vector2(500f, 30f));

            TextMeshProUGUI body = CreateText("WarningBody", "<size=70%>" + WarningBodyText + "</size>",
                Vector3.zero, BaldiFonts.ComicSans18, TextAlignmentOptions.Center,
                new Vector2(400f, 300f), Color.black, false);
            body.transform.SetParent(p.transform, true);
            body.transform.localScale = Vector3.one;
            CenterRect((RectTransform)body.transform, new Vector2(0f, -20f), new Vector2(400f, 300f));
        }

        private void AddPage1() // Interaction · Doors · Machines
        {
            GameObject p = MakePage("Page1");
            AddToggleTo(p, "GumThroughWindow", "Gum passes windows",
                BugConfig.GumThroughWindow.Value, 0,
                "Gum flies straight through windows instead of sticking.");
            AddToggleTo(p, "HookThroughWall", "Hook passes walls",
                BugConfig.HookThroughWall.Value, 1,
                "The grappling hook passes through swinging/rotary walls.");
            AddToggleTo(p, "DoorNoBlock", "Doors never block",
                BugConfig.DoorNoBlock.Value, 2,
                "Closed doors don't stop anyone; everyone (incl. Baldi) walks through.");
            AddToggleTo(p, "DoorStaysOpen", "Doors stay open forever",
                BugConfig.DoorStaysOpen.Value, 3,
                "Doors take several times longer to close after opening.");
            AddToggleTo(p, "MathMachineWrong", "Math is always wrong",
                BugConfig.MathMachineAlwaysWrong.Value, 4,
                "Math machines always mark your answer wrong and enrage Baldi.");
        }

        private void AddPage2() // Entity · Physics
        {
            GameObject p = MakePage("Page2");
            AddToggleTo(p, "NanaPeelBounce", "Peel bounces on bullies",
                BugConfig.NanaPeelBounce.Value, 0,
                "Banana peels bounce off bullies endlessly with noise.");
            AddToggleTo(p, "WhirlpoolToPillar", "Whirlpools crush to walls",
                BugConfig.WhirlpoolToPillar.Value, 1,
                "Whirlpools pull entities into pillars/walls.");
            AddToggleTo(p, "NpcPhaseMachines", "NPCs pass Math Machines",
                BugConfig.NpcPhaseMachines.Value, 2,
                "NPCs walk straight through Math Machines.");
            AddToggleTo(p, "NpcStuckWalls", "NPCs stick in walls",
                BugConfig.NpcStuckWalls.Value, 3,
                "NPCs occasionally get stuck inside walls.");
            AddToggleTo(p, "BalloonHyperSpeed", "Balloons speed up forever",
                BugConfig.BalloonHyperSpeed.Value, 4,
                "Balloons keep accelerating endlessly, ignoring their speed cap.");
            AddToggleTo(p, "AppleTreeNoFruit", "Trees never bear fruit",
                BugConfig.AppleTreeNoFruit.Value, 5,
                "Apple trees never drop fruit to the grappling hook.");
        }

        private void AddPage3() // Player · Stamina
        {
            GameObject p = MakePage("Page3");
            AddToggleTo(p, "DoubleStaminaDrain", "Stamina drains 2x",
                BugConfig.PlayerDoubleStamina.Value, 0,
                "Running drains twice the stamina; you tire almost instantly.");
            AddToggleTo(p, "StaminaInLocker", "No stamina in locker",
                BugConfig.StaminaInLocker.Value, 1,
                "Stamina no longer recharges inside a locker.");
            AddToggleTo(p, "LockerCamRotate", "Locker camera spins",
                BugConfig.LockerCamRotate.Value, 2,
                "The camera can rotate freely inside a locker.");
            AddToggleTo(p, "TutorialCanPause", "Pause in tutorial",
                BugConfig.TutorialCanPause.Value, 3,
                "Re-enables pausing during the tutorial.");
            AddToggleTo(p, "SweepIgnoresSlowmo", "Sweep shoves full-force",
                BugConfig.SweepIgnoresSlowmo.Value, 4,
                "Gotta Sweep's push ignores the NPC time scale and hits at full force even in slow-mo.");
            AddToggleTo(p, "EraseAllSaves", "Wipe all saves",
                BugConfig.EraseAllSaves.Value, 5,
                "One-shot: turning this on and pressing Apply deletes every save file.");
        }

        private void AddPage4() // Character · AI
        {
            GameObject p = MakePage("Page4");
            AddToggleTo(p, "BaldiSlowAtBooks", "Baldi slows as you collect",
                BugConfig.BaldiSlowAtBooks.Value, 0,
                "Baldi gets slower the more notebooks you collect.");
            AddToggleTo(p, "BaldiWallHit", "Baldi hits through walls",
                BugConfig.BaldiWallHit.Value, 1,
                "Baldi can hit the player through walls.");
            AddToggleTo(p, "BaldiAngerBoost", "Baldi enrages instantly",
                BugConfig.BaldiAngerBoost.Value, 2,
                "Every noise/rule-break doubles Baldi's anger.");
            AddToggleTo(p, "BaldiHyperSpeed", "Baldi runs 3x speed",
                BugConfig.BaldiHyperSpeed.Value, 3,
                "Baldi moves at nearly triple speed no matter what.");
            AddToggleTo(p, "InfiniteDetention", "Detention lasts forever",
                BugConfig.PrincipalInfiniteDetention.Value, 4,
                "Principal detention always lasts the max 99s and stacks forever.");
            AddToggleTo(p, "PlaytimeNoWin", "Jump-rope never ends",
                BugConfig.PlaytimeNoWin.Value, 5,
                "Playtime's jump-rope never lets you win; you jump until punished.");
        }

        private void AddPage5() // UI / Spawn
        {
            GameObject p = MakePage("Page5");
            AddToggleTo(p, "HideHudStuck", "HUD stays hidden",
                BugConfig.HideHudStuck.Value, 0,
                "After you hide, the HUD never comes back for the rest of the floor.");
            AddToggleTo(p, "FirstPrizeNoSpawn", "1st Prize never spawns",
                BugConfig.FirstPrizeNoSpawn.Value, 1,
                "Destroy 1st Prize the moment it spawns, so it's never a threat.");
        }

        // Page6 = Original, for custom beneficial tweaks (not vanilla bugs)
        private void AddPage6() // Original
        {
            GameObject p = MakePage("Original");
            AddToggleTo(p, "AppleTreeFullDrop", "Apples always drop",
                BugConfig.AppleTreeFullDrop.Value, 0,
                "Hooking an apple tree always drops a fruit (100%). Auto-disables the 'trees never fruit' bug.");
            AddToggleTo(p, "BSODAFreezeNPC", "BSODA pins NPCs",
                BugConfig.BSODAFreezeNPC.Value, 1,
                "Using a BSODA on an NPC pins it to the wall forever.");
            AddToggleTo(p, "TiltedCamera", "Tilted camera",
                BugConfig.TiltedCamera.Value, 2,
                "The camera slowly sways side to side for a woozy effect.");
        }

        // ---------- Layout helpers ----------

        private GameObject MakePage(string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(transform, false);
            go.transform.localScale = Vector3.one;
            go.layer = LayerMask.NameToLayer("UI");
            RectTransform prt = go.GetComponent<RectTransform>();
            prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 0.5f);
            prt.pivot = new Vector2(0.5f, 0.5f);
            prt.sizeDelta = new Vector2(680f, 500f);
            prt.anchoredPosition = Vector2.zero;
            go.SetActive(false);
            _pages.Add(go);
            return go;
        }

        // index: increments from 0, single column top-to-bottom, 40px row spacing; shifted down overall (lower start) + shifted right overall. The tooltip automatically appends the introduced version and restart hint. All visible text is localized via GetLangText.
        private MenuToggle AddToggleTo(GameObject page, string id, string text, bool value, int index, string tooltip)
        {
            string label = Plugin.GetLangText(id + "_Title", text);
            string tt = Plugin.GetLangText(id + "_Desc", tooltip);

            float x = 8f;                            // shifted right overall
            float y = 66f - index * 40f;             // start lowered to 66, pushed further down
            // version + requires-restart note (line-broken)
            if (BugMeta.TryGetValue(id, out var meta))
            {
                tt += "\n\n" + Plugin.GetLangText("Bug_Introduced", "Introduced in v") + meta.Item1;
                if (meta.Item2)
                    tt += "\n" + Plugin.GetLangText("Bug_RestartWarn",
                        "<color=red>NOTE: Requires a game restart to take effect (can't apply while running)</color>");
            }
            if (HighRiskBugs.Contains(id))
                tt += "\n" + Plugin.GetLangText("Bug_HighRisk",
                    "<color=red>HIGH RISK: may crash or softlock while playing</color>");
            if (id == "EraseAllSaves")
                tt += "\n" + Plugin.GetLangText("Bug_Irreversible",
                    "<color=red>IRREVERSIBLE: pressing Apply deletes ALL saved progress, scores and achievements</color>");
            MenuToggle t = CreateToggle(id, label, value, Vector3.zero, 320f);
            t.transform.SetParent(page.transform, true);
            t.transform.localScale = Vector3.one;
            CenterRect((RectTransform)t.transform, new Vector2(x, y), new Vector2(320f, 32f));
            try { AddTooltip(t, tt); } catch (System.Exception) { }
            _toggles.Add(t);
            return t;
        }

        private string PageIndicatorText(int pageIndex)
        {
            return string.Format(Plugin.GetLangText("Bug_PageFmt", "Page {0}/{1}"), pageIndex + 1, _pages.Count);
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
                _pageIndicator.text = PageIndicatorText(_pageIndex);
        }

        private void Apply()
        {
            // First check: any "requires restart" toggle changing from off→on (currently off, now being turned on) needs a restart to take effect after Apply → quit the game
            bool needRestart = false;
            for (int i = 0; i < _toggles.Count; i++)
            {
                string id = _toggles[i].name;
                if (!BugMeta.TryGetValue(id, out var meta) || !meta.Item2) continue;
                if (!GetConfigValue(id) && _toggles[i].Value) needRestart = true; // off→on
            }

            try { BugConfig.GumThroughWindow.Value = _toggles[0].Value; } catch (System.Exception) { }
            try { BugConfig.HookThroughWall.Value = _toggles[1].Value; } catch (System.Exception) { }
            try { BugConfig.DoorNoBlock.Value = _toggles[2].Value; } catch (System.Exception) { }
            try { BugConfig.DoorStaysOpen.Value = _toggles[3].Value; } catch (System.Exception) { }
            try { BugConfig.MathMachineAlwaysWrong.Value = _toggles[4].Value; } catch (System.Exception) { }
            try { BugConfig.NanaPeelBounce.Value = _toggles[5].Value; } catch (System.Exception) { }
            try { BugConfig.WhirlpoolToPillar.Value = _toggles[6].Value; } catch (System.Exception) { }
            try { BugConfig.NpcPhaseMachines.Value = _toggles[7].Value; } catch (System.Exception) { }
            try { BugConfig.NpcStuckWalls.Value = _toggles[8].Value; } catch (System.Exception) { }
            try { BugConfig.BalloonHyperSpeed.Value = _toggles[9].Value; } catch (System.Exception) { }
            try { BugConfig.AppleTreeNoFruit.Value = _toggles[10].Value; } catch (System.Exception) { }
            try { BugConfig.PlayerDoubleStamina.Value = _toggles[11].Value; } catch (System.Exception) { }
            try { BugConfig.StaminaInLocker.Value = _toggles[12].Value; } catch (System.Exception) { }
            try { BugConfig.LockerCamRotate.Value = _toggles[13].Value; } catch (System.Exception) { }
            try { BugConfig.TutorialCanPause.Value = _toggles[14].Value; } catch (System.Exception) { }
            try { BugConfig.SweepIgnoresSlowmo.Value = _toggles[15].Value; } catch (System.Exception) { }
            // One-shot save wipe (destructive, user opted in): fires on off→on
            if (!BugConfig.EraseAllSaves.Value && _toggles[16].Value)
            {
                try
                {
                    string dir = UnityEngine.Application.persistentDataPath;
                    if (System.IO.Directory.Exists(dir))
                    {
                        foreach (var f in System.IO.Directory.GetFiles(dir, "PlayerFile_*.sav"))
                        {
                            try { System.IO.File.Delete(f); } catch (System.Exception) { }
                        }
                    }
                    var pfm = Singleton<PlayerFileManager>.Instance;
                    pfm?.ResetSaveData();
                }
                catch (System.Exception) { }
            }
            try { BugConfig.BaldiSlowAtBooks.Value = _toggles[17].Value; } catch (System.Exception) { }
            try { BugConfig.BaldiWallHit.Value = _toggles[18].Value; } catch (System.Exception) { }
            try { BugConfig.BaldiAngerBoost.Value = _toggles[19].Value; } catch (System.Exception) { }
            try { BugConfig.BaldiHyperSpeed.Value = _toggles[20].Value; } catch (System.Exception) { }
            try { BugConfig.PrincipalInfiniteDetention.Value = _toggles[21].Value; } catch (System.Exception) { }
            try { BugConfig.PlaytimeNoWin.Value = _toggles[22].Value; } catch (System.Exception) { }
            try { BugConfig.HideHudStuck.Value = _toggles[23].Value; } catch (System.Exception) { }
            try { BugConfig.FirstPrizeNoSpawn.Value = _toggles[24].Value; } catch (System.Exception) { }
            try { BugConfig.AppleTreeFullDrop.Value = _toggles[25].Value; } catch (System.Exception) { }
            try { BugConfig.BSODAFreezeNPC.Value = _toggles[26].Value; } catch (System.Exception) { }
            try { BugConfig.TiltedCamera.Value = _toggles[27].Value; } catch (System.Exception) { }
            // Original bless turns OFF the conflicting fruit-less bug (full handshake also enforced per-frame in EffectsUpdater)
            if (BugConfig.AppleTreeFullDrop.Value) BugConfig.AppleTreeNoFruit.Value = false;

            // Lazy-create the persistent-effects updater only if a locker-based continuous effect is now wanted.
            try
            {
                if (BugConfig.StaminaInLocker.Value || BugConfig.LockerCamRotate.Value || BugConfig.TiltedCamera.Value)
                    EffectsUpdater.Ensure();
            }
            catch (System.Exception) { }

            // Config already persisted (assigning to a BepInEx ConfigEntry persists immediately); if a restart is needed, quit the game so the next launch applies it
            if (needRestart) Application.Quit();
        }

        // Read the current ConfigEntry value corresponding to a toggle (for the off→on check)
        private bool GetConfigValue(string id)
        {
            // Mirrors the _toggles order, index directly to the matching config
            switch (id)
            {
                case "PlaytimeNoWin": return BugConfig.PlaytimeNoWin.Value;
                default: return false;
            }
        }
    }
}