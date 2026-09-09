using UnityEngine;

namespace BugRevival
{
    // Check config every frame to keep injecting "persistent" bug effects.
    // Note: never call FindObjectsOfType + reflection in Update (the old version stuttered because of it).
    // Instead, cache the locker array and just iterate it each frame, reading the public bool playerInside (zero reflection, near-zero cost).
    public class EffectsUpdater : MonoBehaviour
    {
        public static EffectsUpdater Instance;

        private HideableLocker[] _lockers;   // cached reference, expires automatically on floor change
        private float _rescanTimer = 0f;
        private const float RescanInterval = 0.5f; // throttled rescan to detect floor changes
        private float _cfgRefresh = 0.3f;           // throttles the per-frame BepInEx config reads
        private bool _lockerOnCache;
        private bool _appleBothOn;
        private bool _tiltCache;

        public static void Ensure()
        {
            if (Instance != null) return;
            GameObject go = new GameObject("BugRevivalEffects");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<EffectsUpdater>();
        }

        private void Awake() { Instance = this; }

        private void Update()
        {
            // Throttled config reads: only touch BepInEx ConfigEntry.Value (locked, per-frame expensive) every 0.3s.
            // When nothing persistent is on, the rest of Update does zero config reads and zero scanning each frame.
            _cfgRefresh -= Time.deltaTime;
            if (_cfgRefresh <= 0f)
            {
                _cfgRefresh = 0.3f;
                _lockerOnCache = BugConfig.StaminaInLocker.Value || BugConfig.LockerCamRotate.Value;
                _appleBothOn = BugConfig.AppleTreeFullDrop.Value && BugConfig.AppleTreeNoFruit.Value;
                _tiltCache = BugConfig.TiltedCamera.Value;
            }

            // Mutual exclusion: the Original bless "always drop fruit" overrides the "trees never fruit" bug so they can't both be on.
            if (_appleBothOn) BugConfig.AppleTreeNoFruit.Value = false;

            // Tilt runs regardless of the locker gate.
            if (_tiltCache) UpdateTiltCamera();

            if (!_lockerOnCache)
            {
                _lockers = null;
                return;
            }

            _rescanTimer -= Time.deltaTime;
            // Cache empty, or all references invalidated (floor change), or timer expired → infrequent rescan
            if (_lockers == null || _rescanTimer <= 0f)
            {
                _rescanTimer = RescanInterval;
                bool anyAlive = false;
                if (_lockers != null)
                    foreach (var l in _lockers)
                        if (l != null) { anyAlive = true; break; }
                if (_lockers == null || !anyAlive)
                    _lockers = Object.FindObjectsOfType<HideableLocker>();
            }

            bool inside = false;
            if (_lockers != null)
                foreach (var locker in _lockers)
                    if (locker != null && locker.playerInside) { inside = true; break; }

            // LockerCamRotate must run unconditionally: it overrides the locker's SetControllable(false) every frame.
            // Stamina-in-locker only applies while actually inside.
            if (BugConfig.LockerCamRotate.Value) UpdateLockerCamRotate();
            if (!inside) return;

            if (BugConfig.StaminaInLocker.Value) UpdateStaminaInLocker();
        }

        private void UpdateStaminaInLocker()
        {
            var cgm = Singleton<CoreGameManager>.Instance;
            if (cgm == null) return;
            for (int i = 0; i < cgm.setPlayers; i++)
            {
                var plm = cgm.GetPlayer(i)?.plm;
                if (plm == null) continue;
                // Original bug (0.1.5): no stamina regen in locker. Block the idle auto-refill.
                plm.stamina -= plm.staminaRise * Time.deltaTime;
                if (plm.stamina < 0f) plm.stamina = 0f;
            }
        }

        // Original: the camera slowly sways its Z-roll side to side for a woozy effect.
        private void UpdateTiltCamera()
        {
            var cgm = Singleton<CoreGameManager>.Instance;
            if (cgm == null) return;
            float roll = Mathf.Sin(Time.time * 1.5f) * 9f;
            for (int i = 0; i < cgm.setPlayers; i++)
            {
                var cam = cgm.GetCamera(i);
                if (cam == null) continue;
                var e = cam.transform.localEulerAngles;
                cam.transform.localEulerAngles = new Vector3(e.x, e.y, roll);
            }
        }

        private void UpdateLockerCamRotate()
        {
            var cgm = Singleton<CoreGameManager>.Instance;
            if (cgm == null) return;
            for (int i = 0; i < cgm.setPlayers; i++)
            {
                var cam = cgm.GetCamera(i);
                if (cam == null) continue;
                // Vanilla sets SetControllable(false) on entering the locker, locking the camera; revive the old version allowing rotation
                cam.SetControllable(true);
            }
        }
    }
}