using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.UI;

namespace MilkItem
{
    
    
    
    
    
    
    
    
    public class MilkFloodEvent : RandomEvent
    {
        private const float EventDuration = 60f;   
        private const float DangerTime = 12f;      
        private const float CaughtBonusTime = 30f; 
        private const float DisplayMaxTime = 12f;  
        private const int MilkCount = 120;         
        private const int MaxMilkOnField = 200;    
        private const float SpawnRadius = 30f;     

        private readonly List<Pickup> spawnedMilks = new List<Pickup>();
        private readonly HashSet<IntVector2> usedCells = new HashSet<IntVector2>();   
        private readonly List<Vector3> placedMilks = new List<Vector3>();            
        private float countdown = DangerTime;
        private bool caughtByPrincipal = false;     
        private TMPro.TextMeshProUGUI countdownText; 
        private bool risingToTwelve = false;        
        private TMPro.TextMeshProUGUI countdownLabel; 
        private int displayedNumber = 12;             
        private bool popping = false;                 
        private Coroutine downedCoroutine;            
        private Entity downedEntity;                  

        
        public bool IsActive => active;

        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            base.Initialize(controller, rng);
            eventType = Plugin.MilkFloodEventType;
            minEventTime = EventDuration;
            maxEventTime = EventDuration;
        }

        public override void Begin()
        {
            active = true;
            countdown = DangerTime;
            caughtByPrincipal = false;
            if (Singleton<CoreGameManager>.Instance != null && Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
            {
                try { Singleton<PlayerFileManager>.Instance.Find(Singleton<PlayerFileManager>.Instance.foundEvnts, (int)eventType); } catch { }
            }
            
            Plugin.PlayMilkDrinkSound();

            CreateCountdownUI();
            SpawnMilks(MilkCount);
            StartCoroutine(MilkRefreshLoop());
            StartCoroutine(CountdownLoop());
            StartCoroutine(DurationTimer(EventDuration));
        }

        public override void End()
        {
            active = false;
            caughtByPrincipal = false;
            
            
            
            Plugin.RemoveSelfFromEcActiveEvents(this);
            CleanupMilks();
            CleanupCountdownUI();
            StopAllCoroutines();
            base.End();
        }

        
        
        private void OnDestroy()
        {
            active = false;
            CleanupCountdownUI();
        }

        
        
        private void PlayBaldiTVBroadcast(string audioRelativePath, string soundKey)
        {
            try
            {
                if (Singleton<CoreGameManager>.Instance == null) return;
                AudioClip clip = AssetLoader.AudioClipFromMod(Plugin.Instance, audioRelativePath);
                if (clip == null) return;
                SoundObject so = ObjectCreators.CreateSoundObject(clip, soundKey, SoundType.Voice, Color.green, clip.length);
                so.subtitle = true; 
                HudManager hud = Singleton<CoreGameManager>.Instance.GetHud(0);
                if (hud != null && hud.BaldiTv != null)
                {
                    hud.BaldiTv.Speak(so);
                }
                else
                {
                    
                    var g = new GameObject("MilkFlood_EventAudio");
                    var s = g.AddComponent<AudioSource>();
                    s.clip = clip; s.spatialBlend = 0f; s.volume = 1f;
                    Plugin.RouteToMixer(s, Plugin.MilkMixerRoute.Voice);
                    s.Play();
                    UnityEngine.Object.Destroy(g, clip.length + 0.5f);
                }
            }
            catch (System.Exception) { }
        }

        
        private IEnumerator DurationTimer(float time)
        {
            yield return new WaitForSeconds(time);
            End();
        }

        
        
        
        private IEnumerator CountdownLoop()
        {
            while (active)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0f)
                {
                    countdown = 0f;
                    
                    
                    
                    CaughtByPrincipalIfSeen();
                }
                UpdateCountdownUI();
                yield return null;
            }
        }

        
        
        private bool CaughtByPrincipalIfSeen()
        {
            try
            {
                if (caughtByPrincipal) return false;   
                PlayerManager player = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
                if (player == null || player.plm == null || player.plm.Entity == null) return false;
                Principal principal = UnityEngine.Object.FindObjectOfType<Principal>();
                if (principal == null || principal.looker == null) return false;
                if (!principal.looker.PlayerInSight(player)) return false;
                
                player.RuleBreak("Drinking", 10f, 0.25f);
                caughtByPrincipal = true;
                
                countdown = CaughtBonusTime;
                return true;
            }
            catch (System.Exception) { return false; }
        }

        
        
        public void OnMilkCollected(Pickup pickup, int player)
        {
            if (!active) return;
            if (pickup != null)
            {
                spawnedMilks.Remove(pickup);
            }
            countdown = DangerTime;
            ClearPrincipalGuilt();   
            
            if (!risingToTwelve)
            {
                try { StartCoroutine(AnimateRiseToTwelve()); } catch (System.Exception) { }
            }
            
            try
            {
                PlayerManager pm = Singleton<CoreGameManager>.Instance?.GetPlayer(player);
                if (pm != null && pm.plm != null)
                {
                    pm.plm.AddStamina(Random.Range(10f, 20f), limited: true);
                }
            }
            catch (System.Exception) { }
        }

        
        
        
        public void ConsumeFloodMilk(Pickup pickup, MilkFloodPickup.FloodMilkType type, int player)
        {
            if (!active) return;
            switch (type)
            {
                case MilkFloodPickup.FloodMilkType.Normal:
                    OnMilkCollected(pickup, player); 
                    break;
                case MilkFloodPickup.FloodMilkType.DeepGreen:
                    OnMilkCollected(pickup, player); 
                    PlayerDowned(player);
                    break;
                case MilkFloodPickup.FloodMilkType.Green:
                    OnMilkCollected(pickup, player); 
                    AddGreenYtps(player);
                    break;
                case MilkFloodPickup.FloodMilkType.Black:
                    OnMilkCollected(pickup, player); 
                    BlackShockwave(pickup);
                    break;
            }
        }

        
        private MilkFloodPickup.FloodMilkType RollMilkType()
        {
            double r = crng.NextDouble();
            if (r < 0.40) return MilkFloodPickup.FloodMilkType.Normal;
            if (r < 0.62) return MilkFloodPickup.FloodMilkType.DeepGreen;
            if (r < 0.84) return MilkFloodPickup.FloodMilkType.Green;
            return MilkFloodPickup.FloodMilkType.Black;
        }

        
        private void ApplyMilkTint(Pickup milk, MilkFloodPickup.FloodMilkType type)
        {
            try
            {
                if (milk == null) return;
                SpriteRenderer[] sprites = milk.GetComponentsInChildren<SpriteRenderer>(true);
                SpriteRenderer sr = (sprites != null && sprites.Length > 0) ? sprites[0] : null;
                if (sr == null) return;
                Color c = Color.white;
                switch (type)
                {
                    case MilkFloodPickup.FloodMilkType.DeepGreen: c = new Color(0.13f, 0.47f, 0.20f); break;
                    case MilkFloodPickup.FloodMilkType.Green:     c = new Color(0.25f, 0.95f, 0.35f); break;
                    case MilkFloodPickup.FloodMilkType.Black:     c = new Color(0.06f, 0.06f, 0.06f); break;
                }
                for (int i = 0; i < sprites.Length; i++) sprites[i].color = c; 
            }
            catch (System.Exception) { }
        }

        
        
        
        
        private void PlayerDowned(int player)
        {
            try
            {
                PlayerManager pm = Singleton<CoreGameManager>.Instance?.GetPlayer(player);
                if (pm == null || pm.plm == null || pm.plm.Entity == null) return;
                Entity e = pm.plm.Entity;
                e.SetFrozen(false); 
                if (downedCoroutine != null)
                {
                    try { StopCoroutine(downedCoroutine); } catch (System.Exception) { }
                    downedCoroutine = null;
                }
                downedEntity = e;
                downedCoroutine = StartCoroutine(DownedRoutine(e));
            }
            catch (System.Exception) { }
        }

        private System.Collections.IEnumerator DownedRoutine(Entity e)
        {
            e.SetFrozen(true); 
            yield return new WaitForSeconds(3f);
            if (e != null) e.SetFrozen(false); 
            if (e != null && ReferenceEquals(downedEntity, e)) { downedCoroutine = null; downedEntity = null; }
        }

        
        private void AddGreenYtps(int player)
        {
            try
            {
                int amt = Random.Range(20, 41);
                Singleton<CoreGameManager>.Instance?.AddPoints(amt, player, playAnimation: true);
            }
            catch (System.Exception) { }
        }

        
        
        private void BlackShockwave(Pickup pickup)
        {
            try
            {
                if (pickup == null) return;
                Vector3 center = pickup.transform.position;
                const float radius = 9f;
                
                try
                {
                    PlayerManager pm = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
                    if (pm != null && pm.plm != null && pm.plm.Entity != null)
                    {
                        Vector3 to = pm.transform.position - center; to.y = 0f;
                        float dist = to.magnitude;
                        Vector3 dir = (dist > 0.2f) ? to.normalized : RandomHorizontalDir(); 
                        if (dist <= radius) StartCoroutine(KnockFly(pm.plm.Entity, dir, 42f, 0.55f));
                    }
                }
                catch (System.Exception) { }
                
                try
                {
                    if (ec == null || ec.Npcs == null) return;
                    foreach (var npc in ec.Npcs)
                    {
                        if (npc == null || npc.Entity == null) continue;
                        Vector3 to = npc.transform.position - center; to.y = 0f;
                        float dist = to.magnitude;
                        Vector3 dir = (dist > 0.2f) ? to.normalized : RandomHorizontalDir();
                        if (dist <= radius) StartCoroutine(KnockFly(npc.Entity, dir, 42f, 0.55f));
                    }
                }
                catch (System.Exception) { }
            }
            catch (System.Exception) { }
        }

        
        private static Vector3 RandomHorizontalDir()
        {
            Vector2 d = Random.insideUnitCircle;
            if (d == Vector2.zero) d = Vector2.right;
            return new Vector3(d.x, 0f, d.y).normalized;
        }

        
        private System.Collections.IEnumerator KnockFly(Entity e, Vector3 dir, float power = 42f, float dur = 0.55f)
        {
            float t = 0f;
            while (t < dur && e != null)
            {
                Vector3 v = dir * power;
                v.y += Mathf.Lerp(9f, 0f, t / dur); 
                e.UpdateInternalMovement(v);
                t += Time.deltaTime;
                yield return null;
            }
            if (e != null) e.UpdateInternalMovement(Vector3.zero);
        }

        
        public void OnFloodMilkFalling(MilkFloodPickup tag)
        {
            try
            {
                if (tag != null && tag.owner == this)
                {
                    spawnedMilks.Remove(tag.GetComponent<Pickup>());
                }
            }
            catch (System.Exception) { }
        }

        
        private void ClearPrincipalGuilt()
        {
            try
            {
                PlayerManager pm = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
                if (pm != null) pm.ClearGuilt();
            }
            catch (System.Exception) { }
            caughtByPrincipal = false;
        }

        
        
        private List<Cell> BuildSafeCellPool()
        {
            List<Cell> pool = new List<Cell>();
            try
            {
                if (ec == null || ec.rooms == null) return pool;
                foreach (var r in ec.rooms)
                {
                    if (r == null || r.cells == null || r.cells.Count == 0) continue;
                    List<Cell> safe = null;
                    try { safe = r.AllEntitySafeCellsNoGarbage(); } catch (System.Exception) { }
                    if (safe == null || safe.Count == 0) { try { safe = r.AllTilesNoGarbage(false, true); } catch (System.Exception) { } }
                    if (safe == null) continue;
                    
                    foreach (var c in safe)
                    {
                        if (c == null || !c.open || c.offLimits) continue;
                        pool.Add(c);
                    }
                }
                
                if (ec.mainHall != null && ec.mainHall.cells != null && ec.mainHall.cells.Count > 0)
                {
                    List<Cell> safe = null;
                    try { safe = ec.mainHall.AllEntitySafeCellsNoGarbage(); } catch (System.Exception) { }
                    if (safe == null || safe.Count == 0) { try { safe = ec.mainHall.AllTilesNoGarbage(false, true); } catch (System.Exception) { } }
                    if (safe != null)
                    {
                        foreach (var c in safe)
                        {
                            if (c == null || !c.open || c.offLimits) continue;
                            pool.Add(c);
                        }
                    }
                }
            }
            catch (System.Exception) { }
            return pool;
        }

        
        
        
        
        
        
        private void SpawnMilks(int count)
        {
            try
            {
                if (ec == null || Plugin.MilkItemObject == null) return;

                List<Cell> safe = BuildSafeCellPool();
                if (safe.Count == 0) return;

                
                const float DensityRatio = 0.3f;
                int byMap = Mathf.RoundToInt(safe.Count * DensityRatio);
                int toSpawn = Mathf.Clamp(byMap, 6, count);

                
                Vector3 playerPos = Vector3.zero;
                bool hasPlayer = false;
                try
                {
                    PlayerManager pm = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
                    if (pm != null) { playerPos = pm.transform.position; hasPlayer = true; }
                }
                catch (System.Exception) { }

                
                
                float[] keys = new float[safe.Count];
                for (int i = 0; i < safe.Count; i++)
                {
                    Cell c = safe[i];
                    if (c == null) { keys[i] = float.MaxValue; continue; }
                    float dist = 0f;
                    if (hasPlayer)
                    {
                        Vector3 wp = c.FloorWorldPosition;
                        dist = Vector3.Distance(new Vector3(wp.x, 0f, wp.z), new Vector3(playerPos.x, 0f, playerPos.z));
                    }
                    keys[i] = dist * (0.5f + (float)crng.NextDouble() * 0.9f);
                }
                int[] order = Enumerable.Range(0, safe.Count).OrderBy(i => keys[i]).ToArray();

                
                List<Vector3> waypoints = new List<Vector3>(safe.Count);
                foreach (var c in safe) if (c != null) waypoints.Add(c.FloorWorldPosition);

                toSpawn = Mathf.Min(toSpawn, safe.Count);
                int attempts = 0;
                int spawned = 0;
                int oi = 0;
                while (spawned < toSpawn && oi < safe.Count && attempts < 3000)
                {
                    attempts++;
                    Cell cell = safe[order[oi++]];
                    if (cell == null) continue;
                    
                    if (!usedCells.Add(cell.position)) continue;

                    
                    Vector3 wp = cell.FloorWorldPosition;
                    Vector2 worldSpawn = new Vector2(
                        wp.x + Random.Range(-0.30f, 0.30f),
                        wp.z + Random.Range(-0.30f, 0.30f));

                    
                    bool tooClose = false;
                    for (int k = placedMilks.Count - 1; k >= 0; k--)
                    {
                        if (Vector2.Distance(placedMilks[k], worldSpawn) < 0.4f) { tooClose = true; break; }
                    }
                    if (tooClose) continue;

                    Pickup milk = ec.CreateItem(cell.room, Plugin.MilkItemObject, worldSpawn);
                    if (milk == null) continue;
                    milk.OnItemCollected += OnMilkCollected; 
                    if (milk.gameObject != null) MilkFloater.Attach(milk.gameObject, waypoints, milk.transform.position.y);
                    MilkFloodPickup.FloodMilkType type = RollMilkType();
                    MilkFloodPickup.Attach(milk.gameObject, this, type);
                    ApplyMilkTint(milk, type);
                    spawnedMilks.Add(milk);
                    placedMilks.Add(worldSpawn);
                    spawned++;
                }
            }
            catch (System.Exception e) { Plugin.Log?.LogWarning("[MilkFlood] SpawnMilks error: " + e.Message); }
        }

        
        private IEnumerator MilkRefreshLoop()
        {
            while (active)
            {
                yield return new WaitForSeconds(0.25f); 
                if (spawnedMilks.Count >= MaxMilkOnField) continue;
                SpawnMilks(40); 
            }
        }

        
        private void CleanupMilks()
        {
            foreach (var milk in spawnedMilks)
            {
                if (milk == null) continue;
                try { milk.OnItemCollected -= OnMilkCollected; } catch { }
                if (ec != null && ec.items != null) ec.items.Remove(milk);
                if (milk.gameObject != null) UnityEngine.Object.Destroy(milk.gameObject);
            }
            spawnedMilks.Clear();
            usedCells.Clear();
            placedMilks.Clear();
        }

        
        private void CreateCountdownUI()
        {
            try
            {
                HudManager hud = null;
                try { hud = (Singleton<CoreGameManager>.Instance != null) ? Singleton<CoreGameManager>.Instance.GetHud(0) : null; } catch (System.Exception) { }
                if (hud == null) { Plugin.Log?.LogWarning("[MilkFlood] countdown UI: no HUD available, skip display."); return; }
                Transform parent = hud.transform;
                try
                {
                    Canvas hudCanvas = hud.GetComponentInChildren<Canvas>(true);
                    if (hudCanvas != null) parent = hudCanvas.transform;
                }
                catch (System.Exception) { }
                countdownText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans36, "4", parent, Vector3.zero, false);
                countdownText.color = Color.red;
                countdownText.fontStyle = FontStyles.Bold;
                countdownText.alignment = TextAlignmentOptions.Right;
                
                countdownText.rectTransform.pivot = new Vector2(1f, 0.5f);
                countdownText.rectTransform.anchorMin = new Vector2(1f, 0.5f);
                countdownText.rectTransform.anchorMax = new Vector2(1f, 0.5f);
                countdownText.rectTransform.anchoredPosition = new Vector2(-30f, 0f);

                
                countdownLabel = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "Time left", parent, Vector3.zero, false);
                countdownLabel.color = new Color(1f, 1f, 1f, 0.9f);
                countdownLabel.alignment = TextAlignmentOptions.Right;
                countdownLabel.rectTransform.pivot = new Vector2(1f, 0.5f);
                countdownLabel.rectTransform.anchorMin = new Vector2(1f, 0.5f);
                countdownLabel.rectTransform.anchorMax = new Vector2(1f, 0.5f);
                countdownLabel.rectTransform.anchoredPosition = new Vector2(-30f, 40f);
            }
            catch (System.Exception) { }
        }

        private void UpdateCountdownUI()
        {
            if (countdownText == null) return;
            if (risingToTwelve) return; 
            int target = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(countdown, DisplayMaxTime)));
            if (target == displayedNumber) return;
            if (popping)
            {
                
                displayedNumber = target;
                return;
            }
            displayedNumber = target;
            try { StartCoroutine(PopTick(target)); } catch (System.Exception) { }
        }

        
        
        private System.Collections.IEnumerator PopTick(int value)
        {
            popping = true;
            if (countdownText == null) { popping = false; yield break; }
            countdownText.color = Color.green;
            countdownText.text = value.ToString();
            countdownText.rectTransform.localScale = new Vector3(1.6f, 1.6f, 1f); 
            float t = 0f;
            const float dur = 0.12f; 
            while (t < dur)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / dur);
                float s = Mathf.LerpUnclamped(1.6f, 1f, Mathf.SmoothStep(0f, 1f, k)); 
                countdownText.rectTransform.localScale = new Vector3(s, s, 1f);
                yield return null;
                if (countdownText == null) break;
            }
            if (countdownText != null)
            {
                countdownText.rectTransform.localScale = Vector3.one;
                countdownText.color = Color.red;
            }
            popping = false;
        }

        
        
        private System.Collections.IEnumerator AnimateRiseToTwelve()
        {
            risingToTwelve = true;
            if (countdownText == null) { risingToTwelve = false; yield break; }
            countdownText.color = Color.green;
            for (int v = 1; v <= 12; v++)
            {
                if (countdownText == null) break;
                countdownText.text = v.ToString();
                countdownText.rectTransform.localScale = new Vector3(1.7f, 1.7f, 1f); 
                float t = 0f;
                const float dur = 0.08f; 
                while (t < dur)
                {
                    t += Time.deltaTime;
                    float k = Mathf.Clamp01(t / dur);
                    float s = Mathf.LerpUnclamped(1.7f, 1f, Mathf.SmoothStep(0f, 1f, k)); 
                    countdownText.rectTransform.localScale = new Vector3(s, s, 1f);
                    yield return null;
                    if (countdownText == null) break;
                }
            }
            if (countdownText != null)
            {
                countdownText.rectTransform.localScale = Vector3.one;
                countdownText.color = Color.red;
            }
            displayedNumber = 12; 
            risingToTwelve = false;
            UpdateCountdownUI(); 
        }

        private void CleanupCountdownUI()
        {
            if (countdownText != null)
            {
                try { UnityEngine.Object.Destroy(countdownText.gameObject); } catch { }
                countdownText = null;
            }
            if (countdownLabel != null)
            {
                try { UnityEngine.Object.Destroy(countdownLabel.gameObject); } catch { }
                countdownLabel = null;
            }
        }
    }
}