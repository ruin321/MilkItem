using System.Collections.Generic;
using UnityEngine;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;

namespace MilkItem
{
    
    
    
    
    
    public class SnowZone : MonoBehaviour
    {
        
        private const float halfXCells = 5f;
        private const float halfZCells = 5f;
        private const int snowCount = 180;
        private const float snowTop = 24f;     
        private const float snowBottom = 0.5f;  
        private const float fallSpeedMin = 4f;
        private const float fallSpeedMax = 10f;
        private const float flakeScale = 1.5f;   
        private const float windDrift = 1.5f;   

        private Sprite snowSprite;
        private readonly List<GameObject> flakes = new List<GameObject>();
        private readonly List<float> speeds = new List<float>();
        private readonly List<float> driftX = new List<float>(); 
        private readonly List<float> driftZ = new List<float>();
        private readonly List<GameObject> visuals = new List<GameObject>(); 
        private float regionHalfX; 
        private float regionHalfZ;
        
        
        
        private float regionCenterX;
        private float regionCenterZ;

        
        private static Texture2D _whiteTex;
        private static Texture2D WhiteTex
        {
            get
            {
                if (_whiteTex == null)
                {
                    _whiteTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    _whiteTex.SetPixel(0, 0, Color.white);
                    _whiteTex.Apply();
                }
                return _whiteTex;
            }
        }

        private void Start()
        {
            
            bool inGameplay = false;
            try
            {
                inGameplay = Application.isPlaying && Singleton<CoreGameManager>.Instance != null;
            }
            catch (System.Exception) { inGameplay = false; }

            
            Vector3 userScale = transform.localScale;
            regionHalfX = halfXCells * 10f * Mathf.Max(0.01f, userScale.x);
            regionHalfZ = halfZCells * 10f * Mathf.Max(0.01f, userScale.z);

            
            transform.localScale = Vector3.one;

            
            regionCenterX = 0f;
            regionCenterZ = 0f;

            if (inGameplay)
            {
                
                TryConstrainToRoom();

                
                try
                {
                    snowSprite = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 64f, "snow.png");
                }
                catch (System.Exception )
                {
                    
                }

                SpawnFlakes();
            }
            else
            {
                
                CreateEditorRegionVisual();
            }
        }

        
        
        private void TryConstrainToRoom()
        {
            EnvironmentController ec = null;
            try
            {
                var gm = Singleton<CoreGameManager>.Instance;
                ec = gm?.GetComponent<EnvironmentController>();
            }
            catch (System.Exception )
            {
                
            }
            if (ec == null || ec.rooms == null) return;

            Vector3 here = transform.position;
            RoomController room = null;
            try
            {
                foreach (var r in ec.rooms)
                {
                    if (r != null && r.containsPosition(here))
                    {
                        room = r;
                        break;
                    }
                }
            }
            catch (System.Exception )
            {
                
                return;
            }

            if (room == null)
            {
                
                return;
            }

            
            Vector3 mn = ec.RealRoomMin(room);
            Vector3 mx = ec.RealRoomMax(room);
            float inset = 2f; 
            
            
            regionCenterX = (mn.x + mx.x) * 0.5f - transform.position.x;
            regionCenterZ = (mn.z + mx.z) * 0.5f - transform.position.z;
            regionHalfX = Mathf.Max(0.5f, (mx.x - mn.x) * 0.5f - inset);
            regionHalfZ = Mathf.Max(0.5f, (mx.z - mn.z) * 0.5f - inset);
            
        }

        
        
        private void CreateEditorRegionVisual()
        {
            
            var ground = new GameObject("SnowZoneRegion");
            ground.transform.SetParent(transform, false);
            var gsr = ground.AddComponent<SpriteRenderer>();
            
            gsr.sprite = Sprite.Create(WhiteTex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            gsr.color = new Color(0.35f, 0.62f, 1f, 0.42f); 
            gsr.sortingOrder = 5;
            
            
            ground.transform.localPosition = new Vector3(0f, 0.06f, 0f);
            ground.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            ground.transform.localScale = new Vector3(regionHalfX * 2f, regionHalfZ * 2f, 1f);
            visuals.Add(ground);

            
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = "SnowZoneVolume";
            box.transform.SetParent(transform, false);
            var bc = box.GetComponent<Collider>(); if (bc != null) Object.Destroy(bc);
            var mr = box.GetComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("GUI/Text Shader"));
            mr.material.color = new Color(0.45f, 0.7f, 1f, 0.12f);
            box.transform.localPosition = new Vector3(0f, snowTop * 0.5f, 0f);
            box.transform.localScale = new Vector3(regionHalfX * 2f, snowTop, regionHalfZ * 2f);
            visuals.Add(box);
        }

        private void SpawnFlakes()
        {
            if (snowSprite == null) return;
            float hx = regionHalfX;
            float hz = regionHalfZ;
            for (int i = 0; i < snowCount; i++)
            {
                GameObject flake = new GameObject("SnowFlake" + i);
                flake.transform.SetParent(transform, false);
                var sr = flake.AddComponent<SpriteRenderer>();
                sr.sprite = snowSprite;
                sr.sortingOrder = 50;
                flake.transform.localPosition = new Vector3(
                    regionCenterX + Random.Range(-hx, hx),
                    Random.Range(snowBottom, snowTop),
                    regionCenterZ + Random.Range(-hz, hz));
                flake.transform.localScale = Vector3.one * flakeScale;
                flakes.Add(flake);
                speeds.Add(Random.Range(fallSpeedMin, fallSpeedMax));
                driftX.Add(Random.Range(-windDrift, windDrift));
                driftZ.Add(Random.Range(-windDrift, windDrift));
            }
        }

        private Transform cachedCam;
        private bool camReady;

        private void Update()
        {
            if (flakes.Count == 0) return;

            
            if (!camReady)
            {
                var cam = Camera.main;
                if (cam != null) { cachedCam = cam.transform; camReady = true; }
            }

            float hx = regionHalfX;
            float hz = regionHalfZ;

            for (int i = 0; i < flakes.Count; i++)
            {
                var f = flakes[i];
                if (f == null) continue;
                Vector3 lp = f.transform.localPosition;
                lp.y -= speeds[i] * Time.deltaTime;
                lp.x += driftX[i] * Time.deltaTime;
                lp.z += driftZ[i] * Time.deltaTime;
                if (lp.y <= snowBottom)
                {
                    
                    lp = new Vector3(
                        regionCenterX + Random.Range(-hx, hx),
                        snowTop,
                        regionCenterZ + Random.Range(-hz, hz));
                    speeds[i] = Random.Range(fallSpeedMin, fallSpeedMax);
                    driftX[i] = Random.Range(-windDrift, windDrift);
                    driftZ[i] = Random.Range(-windDrift, windDrift);
                }
                
                if (lp.x < regionCenterX - hx) lp.x = regionCenterX - hx;
                else if (lp.x > regionCenterX + hx) lp.x = regionCenterX + hx;
                if (lp.z < regionCenterZ - hz) lp.z = regionCenterZ - hz;
                else if (lp.z > regionCenterZ + hz) lp.z = regionCenterZ + hz;
                f.transform.localPosition = lp;
                if (cachedCam != null) f.transform.LookAt(cachedCam); 
            }
        }

        private void OnDestroy()
        {
            foreach (var f in flakes)
            {
                if (f != null) Destroy(f);
            }
            flakes.Clear();
            speeds.Clear();
            driftX.Clear();
            driftZ.Clear();
            foreach (var v in visuals)
            {
                if (v != null) Destroy(v);
            }
            visuals.Clear();
        }
    }
}
