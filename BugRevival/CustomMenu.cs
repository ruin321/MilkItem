using BepInEx;
using BepInEx.Bootstrap;
using MTM101BaldAPI.AssetTools;
using UnityEngine;

namespace BugRevival
{
    // Registers a custom main menu background via CustomMainMenusAPI.
    // The sprite is loaded from the mod's data folder: .../Ruin321.bbbug.ohno2manybug/misc/mainmenu.png
    // It only takes effect if CustomMainMenusAPI is installed; otherwise this is a no-op.
    public static class CustomMenu
    {
        private const string ApiGuid = "pixelguy.pixelmodding.baldiplus.custommainmenusapi";

        public static void Register()
        {
            try
            {
                if (!Chainloader.PluginInfos.ContainsKey(ApiGuid)) return;

                Sprite sprite = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 1f, "misc", "mainmenu.png");
                if (sprite == null) return;

                CustomMainMenusAPI.MainMenuObject.CreateMenuObject("BaldisbasicsBUG Menu", sprite);
                Plugin.Log.LogInfo("[BugRevival] registered custom main menu.");
            }
            catch (System.Exception)
            {
                // CustomMainMenusAPI changes between versions; never let this take down the whole mod.
            }
        }
    }
}