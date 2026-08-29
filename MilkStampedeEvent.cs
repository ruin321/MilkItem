using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;

namespace MilkItem
{
    
    
    
    
    
    
    
    public class MilkStampedeEvent : RandomEvent
    {
        private readonly List<NPC> spawnedCows = new List<NPC>();

        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            base.Initialize(controller, rng);
            eventType = Plugin.StampedeEventType; 
            minEventTime = 60f;                   
            maxEventTime = 90f;
        }

        public override void Begin()
        {
            try
            {
                base.Begin();
            }
            catch (System.Exception )
            {
                
                
                active = true;
                StartCoroutine(FallbackTimer());
            }
            SpawnCows();
            
            try { Plugin.PlayMilkDrinkSound(); }
            catch (System.Exception ) { }
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
                    
                    var g = new GameObject("MilkStampede_EventAudio");
                    var s = g.AddComponent<AudioSource>();
                    s.clip = clip; s.spatialBlend = 0f; s.volume = 1f;
                    Plugin.RouteToMixer(s, Plugin.MilkMixerRoute.Voice);
                    s.Play();
                    UnityEngine.Object.Destroy(g, clip.length + 0.5f);
                }
            }
            catch (System.Exception) { }
        }

        public override void End()
        {
            foreach (var cow in spawnedCows)
            {
                if (cow == null) continue;
                
                
                Plugin.RemoveNpcFromEnvironment(ec, cow);
                UnityEngine.Object.Destroy(cow.gameObject);
            }
            spawnedCows.Clear();
            
            try { Plugin.RemoveSelfFromEcActiveEvents(this); } catch { }
            base.End();
        }

        private IEnumerator FallbackTimer()
        {
            yield return new WaitForSeconds(90f);
            End();
        }

        private void SpawnCows()
        {
            try
            {
                if (ec == null || Plugin.StampedeCowPrefab == null)
                {
                    
                    return;
                }
                int count = 5 + crng.Next(0, 4); 
                var cells = ec.mainHall != null ? ec.mainHall.cells : null;
                if (cells == null || cells.Count == 0)
                {
                    
                    return;
                }
                var used = new HashSet<IntVector2>();
                int attempts = 0;
                while (spawnedCows.Count < count && attempts < 300)
                {
                    attempts++;
                    var cell = cells[crng.Next(0, cells.Count)];
                    if (cell == null || !used.Add(cell.position)) continue;
                    NPC n = ec.SpawnNPC(Plugin.StampedeCowPrefab, cell.position);
                    if (n == null) continue;
                    spawnedCows.Add(n);
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }
    }
}
