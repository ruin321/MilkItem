using BepInEx.Configuration;

namespace BugRevival
{
    public static class BugConfig
    {
        // Master visibility for the settings menu (config-only, not shown in the menu itself).
        public static ConfigEntry<bool> ShowSettingsMenu;

        // UI & Interaction
        public static ConfigEntry<bool> GumThroughWindow;    // Gum passes through windows (0.3.6)
        public static ConfigEntry<bool> HookThroughWall;     // Hook passes through rotary-wall (0.3.5)
        public static ConfigEntry<bool> DoorNoBlock;         // Walk through closed doors, ghost-door (0.2)
        public static ConfigEntry<bool> DoorStaysOpen;       // Doors take very long to close (0.3)
        public static ConfigEntry<bool> MathMachineAlwaysWrong; // Math machine always marks you wrong + enrages Baldi (0.1.3)

        // Stamina & Camera
        public static ConfigEntry<bool> StaminaInLocker;     // No stamina regen in locker (0.1.5)
        public static ConfigEntry<bool> LockerCamRotate;     // Camera can rotate inside locker (0.14.2 fixed)
        public static ConfigEntry<bool> PlayerDoubleStamina; // Running drains double stamina (0.1)

        // System & Environment
        public static ConfigEntry<bool> TutorialCanPause;    // Can pause during tutorial (0.10)

        // Entity & Physics
        public static ConfigEntry<bool> NanaPeelBounce;      // Banana peel bounces off bullies endlessly + noise (0.5.1)
        public static ConfigEntry<bool> WhirlpoolToPillar;   // Whirlpool squeezes entities into pillars (0.13)
        public static ConfigEntry<bool> NpcPhaseMachines;    // NPCs pass through math machines (0.1.3)
        public static ConfigEntry<bool> NpcStuckWalls;       // NPCs occasionally get stuck in walls (0.13)
        public static ConfigEntry<bool> BalloonHyperSpeed;   // Balloons fly faster and faster, never slowing/turning (0.3)
        public static ConfigEntry<bool> AppleTreeNoFruit;    // Apple trees never drop fruit to the hook (0.3)
        public static ConfigEntry<bool> SweepIgnoresSlowmo;  // Gotta Sweep's push ignores NPC time scale (0.12)

        // System / destructive
        public static ConfigEntry<bool> EraseAllSaves;     // (0.9) one-shot wipe of every save file

        // UI
        public static ConfigEntry<bool> HideHudStuck;      // (0.6) HUD stays hidden after un-hiding

        // Original (bless)
        public static ConfigEntry<bool> AppleTreeFullDrop;  // Original: trees always drop fruit; auto-disables AppleTreeNoFruit
        public static ConfigEntry<bool> BSODAFreezeNPC;    // Original: BSODA pins an NPC to the wall forever
        public static ConfigEntry<bool> TiltedCamera;       // Original: the camera slowly tilts side-to-side while active

        // Entity / spawn
        public static ConfigEntry<bool> FirstPrizeNoSpawn; // (0.2) Hide-&-Seek never spawned 1st Prize → destroy it on spawn

        // Baldi & AI
        public static ConfigEntry<bool> BaldiSlowAtBooks;    // Baldi gets slower as you collect books (0.13)
        public static ConfigEntry<bool> BaldiWallHit;        // Baldi hits player through walls (0.4)
        public static ConfigEntry<bool> BaldiAngerBoost;     // Baldi's anger doubles, enrages instantly (0.2)
        public static ConfigEntry<bool> BaldiHyperSpeed;     // Baldi speed skyrockets (0.4)
        public static ConfigEntry<bool> PrincipalInfiniteDetention; // Principal infinite detention duration (0.2)
        public static ConfigEntry<bool> PlaytimeNoWin;       // Jump-rope never ends, play until punished (0.5)

        public static void Init(ConfigFile c)
        {
            ShowSettingsMenu = c.Bind("General", "ShowSettingsMenu", true,
                "Whether the BugRevival settings menu is shown. Set to false to hide it (bugs stay as configured).");

            GumThroughWindow = c.Bind("Interaction", "GumThroughWindow", true,
                "Gum flies straight through windows instead of sticking to them.");
            HookThroughWall = c.Bind("Interaction", "HookThroughWall", true,
                "The grappling hook passes through swinging-door/rotary walls.");
            DoorNoBlock = c.Bind("Interaction", "DoorNoBlock", false,
                "Closed doors no longer block anyone; everyone (and Baldi) walks right through. WARNING: enabling may cause in-game lag or a soft-lock.");
            DoorStaysOpen = c.Bind("Interaction", "DoorStaysOpen", false,
                "Doors take several times longer to close after opening. WARNING: enabling may cause in-game lag or a soft-lock.");
            MathMachineAlwaysWrong = c.Bind("Interaction", "MathMachineAlwaysWrong", true,
                "Math machines always mark your answer wrong and enrage Baldi anyway.");

            StaminaInLocker = c.Bind("Locker", "StaminaInLocker", true,
                "Stamina no longer recharges while hiding inside a locker.");
            LockerCamRotate = c.Bind("Locker", "LockerCamRotate", true,
                "The camera can rotate freely while inside a locker.");
            PlayerDoubleStamina = c.Bind("Locker", "PlayerDoubleStamina", true,
                "Running drains twice the stamina, so you tire out almost instantly.");

            TutorialCanPause = c.Bind("System", "TutorialCanPause", true,
                "Re-enables pausing during the tutorial.");

            NanaPeelBounce = c.Bind("Entity", "NanaPeelBounce", false,
                "Banana peels bounce off bullies endlessly with a looped noise. WARNING: enabling may cause in-game lag or a soft-lock.");
            WhirlpoolToPillar = c.Bind("Entity", "WhirlpoolToPillar", false,
                "Whirlpools pull entities into pillars/walls. WARNING: enabling may cause in-game lag or a soft-lock.");
            NpcPhaseMachines = c.Bind("Entity", "NpcPhaseMachines", false,
                "NPCs walk straight through Math Machines. WARNING: enabling may cause in-game lag or a soft-lock.");
            NpcStuckWalls = c.Bind("Entity", "NpcStuckWalls", false,
                "NPCs occasionally get stuck inside walls. WARNING: enabling may cause in-game lag or a soft-lock.");
            BalloonHyperSpeed = c.Bind("Entity", "BalloonHyperSpeed", false,
                "Balloons speed up endlessly, ignoring their min-speed cap. WARNING: enabling may cause in-game lag or a soft-lock.");
            AppleTreeNoFruit = c.Bind("Entity", "AppleTreeNoFruit", true,
                "Apple trees never drop their fruit to the grappling hook.");
            SweepIgnoresSlowmo = c.Bind("Entity", "SweepIgnoresSlowmo", false,
                "Gotta Sweep's push ignores the NPC time scale and hits at full force even in slow-mo. WARNING: enabling may cause in-game lag or a soft-lock.");
            FirstPrizeNoSpawn = c.Bind("Entity", "FirstPrizeNoSpawn", true,
                "1st Prize is destroyed the moment it spawns, so it's never a threat (matches the V0.2 Hide-&-Seek bug where it never spawned).");
            EraseAllSaves = c.Bind("System", "EraseAllSaves", false,
                "IRREVERSIBLE: turning this ON and pressing Apply deletes ALL saved progress, scores and achievements.");
            HideHudStuck = c.Bind("UI", "HideHudStuck", true,
                "The HUD never comes back after you hide; it's stuck hidden for the rest of the floor.");
            AppleTreeFullDrop = c.Bind("Original", "AppleTreeFullDrop", true,
                "Apple trees always drop a fruit when hooked (100% chance). Turning this ON auto-disables the 'trees never fruit' bug.");
            BSODAFreezeNPC = c.Bind("Original", "BSODAFreezeNPC", false,
                "Using a BSODA on an NPC pins it to the wall forever (speedMultiplier 0, never removed). WARNING: enabling may cause in-game lag or a soft-lock.");
            TiltedCamera = c.Bind("Original", "TiltedCamera", true,
                "While active, the camera slowly sways/tilts side-to-side for a woozy effect.");

            BaldiSlowAtBooks = c.Bind("AI", "BaldiSlowAtBooks", true,
                "Baldi gets slower the more notebooks you collect on a floor.");
            BaldiWallHit = c.Bind("AI", "BaldiWallHit", true,
                "Baldi can hit the player through walls.");
            BaldiAngerBoost = c.Bind("AI", "BaldiAngerBoost", true,
                "Every noise/rule-break doubles Baldi's anger, making him enrage instantly.");
            BaldiHyperSpeed = c.Bind("AI", "BaldiHyperSpeed", true,
                "Baldi moves at nearly triple speed no matter what.");
            PrincipalInfiniteDetention = c.Bind("AI", "PrincipalInfiniteDetention", false,
                "Detention always lasts the maximum 99 seconds, and stacks forever. WARNING: enabling may cause in-game lag or a soft-lock.");
            PlaytimeNoWin = c.Bind("AI", "PlaytimeNoWin", false,
                "Playtime's jump-rope never lets you win; you keep jumping until punished. WARNING: enabling may cause in-game lag or a soft-lock.");
        }
    }
}