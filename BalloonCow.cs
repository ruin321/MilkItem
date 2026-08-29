using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;

namespace MilkItem
{
    
    
    
    public class BalloonCow : NPC
    {
        private const float SpinSpeed = 9999f;      
        private static readonly string[] FrameFiles =
        {
            "Bulloon.png",
            "BulloonBlue.png",
            "BulloonGreen.png",
            "BulloonOrange.png",
            "BulloonPurple.png",
            "BulloonYellow.png"
        };

        private Sprite[] frames;         
        private int frameIndex = 0;
        private const float animInterval = 0.2f;   
        private AudioSource musicSource;           
        private SpriteRenderer sr;                 

        public override void Initialize()
        {
            base.Initialize();

            
            if (base.spriteRenderer == null || base.spriteRenderer.Length == 0)
                base.spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
            sr = (base.spriteRenderer.Length > 0) ? base.spriteRenderer[0] : null;

            
            var list = new List<Sprite>();
            foreach (var f in FrameFiles)
            {
                try
                {
                    var s = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, "npc/BalloonCow/" + f);
                    if (s != null) list.Add(s);
                }
                catch (System.Exception) { }
            }
            frames = list.ToArray();
            if (frames.Length > 0 && sr != null)
                sr.sprite = frames[0];

            
            try
            {
                AudioClip moo = AssetLoader.AudioClipFromMod(Plugin.Instance, "npc/cow/mus_Cow.wav");
                if (moo != null)
                {
                    AudioSource src = gameObject.AddComponent<AudioSource>();
                    src.clip = moo;
                    src.loop = true;
                    src.spatialBlend = 1f;
                    src.minDistance = 10f;
                    src.maxDistance = 40f;
                    src.playOnAwake = false;
                    src.volume = 0.7f;
                    Plugin.RouteToMixer(src, Plugin.MilkMixerRoute.Effect);
                    src.Play();
                    musicSource = src;
                }
            }
            catch (System.Exception) { }

            
            base.navigator.SetSpeed(0f);
            base.navigator.maxSpeed = 0f;

            StartCoroutine(Spin());
            StartCoroutine(Animate());

            
        }

        
        
        
        
        
        
        
        private IEnumerator Spin()
        {
            float roll = 0f;
            while (true)
            {
                yield return null;
                if (sr == null) continue;
                Camera cam = Camera.main;
                Vector3 toCam = (cam != null) ? (sr.transform.position - cam.transform.position) : Vector3.forward;
                if (toCam == Vector3.zero) toCam = Vector3.forward;
                
                Quaternion facing = Quaternion.LookRotation(toCam);
                
                roll += UnityEngine.Random.Range(-1f, 1f) * (SpinSpeed * Time.deltaTime);
                sr.transform.rotation = facing * Quaternion.Euler(0f, 0f, roll);
            }
        }

        
        private IEnumerator Animate()
        {
            while (true)
            {
                yield return new WaitForSeconds(animInterval);
                if (frames == null || frames.Length == 0 || sr == null) continue;
                frameIndex = (frameIndex + 1) % frames.Length;
                if (frames[frameIndex] != null)
                    sr.sprite = frames[frameIndex];
            }
        }
    }
}