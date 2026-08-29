using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;

namespace MilkItem
{
    
    
    
    
    
    public class ColdRoomFunction : RoomFunction
    {
        public float slowMultiplier = 0.4f; 
        public float blueMax = 0.5f;        

        
        private readonly Dictionary<PlayerManager, MovementModifier> slowMods =
            new Dictionary<PlayerManager, MovementModifier>();
        private readonly Dictionary<PlayerManager, GameObject> overlays =
            new Dictionary<PlayerManager, GameObject>();

        
        private Sprite snowSprite;
        private readonly List<GameObject> snowFlakes = new List<GameObject>();
        private readonly List<float> snowSpeeds = new List<float>();
        private bool snowActive = false;
        private float snowCenterX, snowCenterZ, snowHalfX, snowHalfZ;
        private const int coldSnowCount = 160;
        private const float coldSnowTop = 24f;     
        private const float coldSnowBottom = 0.5f; 
        private const float coldFallMin = 4f;
        private const float coldFallMax = 10f;
        private const float coldFlakeScale = 1.6f;
        private const float coldInset = 2f;        

        public override void OnPlayerEnter(PlayerManager player)
        {
            base.OnPlayerEnter(player);
            if (player == null || slowMods.ContainsKey(player)) return;

            
            MovementModifier slow = null;
            if (player.plm != null && player.plm.am != null)
            {
                slow = new MovementModifier(Vector3.zero, 1f, 0);
                slow.movementMultiplier = slowMultiplier;
                player.plm.am.moveMods.Add(slow);
            }
            slowMods[player] = slow;

            
            GameCamera gameCam = null;
            try { gameCam = Singleton<CoreGameManager>.Instance.GetCamera(player.playerNumber); }
            catch (System.Exception) { }
            GameObject overlay = null;
            if (gameCam != null)
            {
                Image img;
                overlay = CreateColdOverlay(gameCam, out img);
                if (img != null) img.color = new Color(0.25f, 0.55f, 1f, blueMax);
            }
            overlays[player] = overlay;

            
            StartSnow();
        }

        public override void OnPlayerExit(PlayerManager player)
        {
            base.OnPlayerExit(player);

            if (slowMods.TryGetValue(player, out MovementModifier slow))
            {
                if (slow != null && player.plm != null && player.plm.am != null)
                    player.plm.am.moveMods.Remove(slow);
                slowMods.Remove(player);
            }
            if (overlays.TryGetValue(player, out GameObject overlay))
            {
                if (overlay != null) UnityEngine.Object.Destroy(overlay);
                overlays.Remove(player);
            }
        }

        
        private void StartSnow()
        {
            if (snowActive) return;
            EnvironmentController ec = null;
            try { ec = Singleton<CoreGameManager>.Instance?.GetComponent<EnvironmentController>(); }
            catch (System.Exception) { }
            if (ec == null || this.Room == null)
            {
                
                return;
            }

            Vector3 mn = ec.RealRoomMin(this.Room);
            Vector3 mx = ec.RealRoomMax(this.Room);
            snowCenterX = (mn.x + mx.x) * 0.5f;
            snowCenterZ = (mn.z + mx.z) * 0.5f;
            snowHalfX = Mathf.Max(0.5f, (mx.x - mn.x) * 0.5f - coldInset);
            snowHalfZ = Mathf.Max(0.5f, (mx.z - mn.z) * 0.5f - coldInset);

            try
            {
                snowSprite = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 64f, "snow.png");
            }
            catch (System.Exception )
            {
                
            }
            if (snowSprite == null) return;

            for (int i = 0; i < coldSnowCount; i++)
            {
                GameObject f = new GameObject("ColdFlake" + i);
                f.transform.SetParent(null); 
                var sr = f.AddComponent<SpriteRenderer>();
                sr.sprite = snowSprite;
                sr.sortingOrder = 50;
                f.transform.position = new Vector3(
                    snowCenterX + Random.Range(-snowHalfX, snowHalfX),
                    Random.Range(coldSnowBottom, coldSnowTop),
                    snowCenterZ + Random.Range(-snowHalfZ, snowHalfZ));
                f.transform.localScale = Vector3.one * coldFlakeScale;
                snowFlakes.Add(f);
                snowSpeeds.Add(Random.Range(coldFallMin, coldFallMax));
            }
            snowActive = true;
            StartCoroutine(ColdSnowRoutine());
            
        }

        private IEnumerator ColdSnowRoutine()
        {
            while (snowActive)
            {
                Transform camT = null;
                try
                {
                    var gm = Singleton<CoreGameManager>.Instance;
                    var cam = gm?.GetCamera(0);
                    if (cam != null) camT = cam.transform;
                }
                catch (System.Exception) { }

                for (int i = 0; i < snowFlakes.Count; i++)
                {
                    var f = snowFlakes[i];
                    if (f == null) continue;
                    Vector3 p = f.transform.position;
                    p.y -= snowSpeeds[i] * Time.deltaTime;
                    if (p.y <= coldSnowBottom)
                    {
                        p = new Vector3(
                            snowCenterX + Random.Range(-snowHalfX, snowHalfX),
                            coldSnowTop,
                            snowCenterZ + Random.Range(-snowHalfZ, snowHalfZ));
                        snowSpeeds[i] = Random.Range(coldFallMin, coldFallMax);
                    }
                    f.transform.position = p;
                    if (camT != null) f.transform.LookAt(camT);
                }
                yield return null;
            }
        }

        private void OnDestroy()
        {
            snowActive = false;
            foreach (var f in snowFlakes)
            {
                if (f != null) UnityEngine.Object.Destroy(f);
            }
            snowFlakes.Clear();
            snowSpeeds.Clear();
        }

        
        
        
        
        public static GameObject CreateColdOverlay(GameCamera gameCam, out Image image)
        {
            image = null;
            if (gameCam == null || gameCam.canvasCam == null) return null;

            GameObject root = new GameObject("ColdOverlay");
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = gameCam.canvasCam;
            canvas.sortingOrder = 9998; 

            var scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject imgGo = new GameObject("ColdBlue");
            imgGo.transform.SetParent(root.transform, false);
            image = imgGo.AddComponent<Image>();
            image.color = new Color(0.25f, 0.55f, 1f, 0.5f);

            var rt = image.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            return root;
        }
    }
}
