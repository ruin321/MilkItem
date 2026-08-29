using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;

namespace MilkItem
{
    
    
    
    
    
    
    
    
    
    
    
    public class QuizMachine : Activity, IClickable<int>
    {
        private SpriteRenderer spriteRenderer;
        private TMP_Text counterText;
        private int countRemaining;
        private bool machineActive = true;

        
        private const float TextBaseHeight = 6.1f;
        private const float FloatAmplitude = 0.18f;
        private const float FloatSpeed = 2.2f;

        
        private const int QuizUses = 1;
        private const int QuizPoints = 200;   
        private const int NormalPoints = 50;   

        
        
        public static readonly HashSet<Door> lockedDoors = new HashSet<Door>();

        private RoomController targetRoom;
        private bool quizStarted = false;   
        private bool quizResolved = false;  
        private bool armed = false;         
        private bool quizOver = false;      

        private AudioSource audioSrc;
        private AudioClip tickClip;
        private AudioClip correctClip;
        private AudioClip incorrectClip;
        private Coroutine timerCoroutine;

        private const float QuizFailAnger = 1f; 

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            GameObject textObj = new GameObject("CounterBillboard");
            textObj.transform.SetParent(transform);
            textObj.transform.localPosition = new Vector3(0f, TextBaseHeight, 0f);
            counterText = textObj.AddComponent<TextMeshPro>();
            
            
            try { counterText.font = TMP_Settings.defaultFontAsset; } catch { }
            counterText.fontSize = 28;
            counterText.alignment = TextAlignmentOptions.Center;
            counterText.color = Color.white;
            counterText.outlineColor = Color.black;
            counterText.outlineWidth = 0.45f;
            counterText.enableWordWrapping = false;
            counterText.overflowMode = TextOverflowModes.Overflow;
            MeshRenderer mr = textObj.GetComponent<MeshRenderer>();
            if (mr != null) mr.sortingOrder = 10;
        }

        private void Start()
        {
            ReInit();
            SetupRoomAndAudio();
        }

        
        private void SetupRoomAndAudio()
        {
            
            
            if (this.room != null)
            {
                targetRoom = this.room;
            }
            else
            {
                EnvironmentController ec = null;
                try { ec = Singleton<CoreGameManager>.Instance?.GetComponent<EnvironmentController>(); } catch (System.Exception) { }
                if (ec != null && ec.rooms != null)
                {
                    foreach (var r in ec.rooms)
                    {
                        if (r != null && r.containsPosition(transform.position)) { targetRoom = r; break; }
                    }
                }
            }
            if (targetRoom == null)
                

            
            try { tickClip = AssetLoader.AudioClipFromMod(Plugin.Instance, "tickTockTicktockTickTock.wav"); }
            catch (System.Exception ) {  }
            try { correctClip = AssetLoader.AudioClipFromMod(Plugin.Instance, "Activity_Correct.wav"); }
            catch (System.Exception ) {  }
            try { incorrectClip = AssetLoader.AudioClipFromMod(Plugin.Instance, "Activity_Incorrect.wav"); }
            catch (System.Exception ) {  }

            audioSrc = gameObject.AddComponent<AudioSource>();
            audioSrc.spatialBlend = 0f; 
            audioSrc.loop = false;
            Plugin.RouteToMixer(audioSrc, Plugin.MilkMixerRoute.Effect);
        }

        public override void ReInit()
        {
            if (notebook != null)
            {
                notebook.transform.position = transform.position + Vector3.up * 5f;
                notebook.gameObject.SetActive(false);
            }
            completed = false;
            countRemaining = QuizUses;
            machineActive = true;
            armed = false;
            quizOver = false;

            
            
            if (bonusMode)
            {
                quizStarted = true;
                quizResolved = true;
            }
            else
            {
                quizStarted = false;
                quizResolved = false;
            }

            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            if (counterText != null) counterText.color = Color.white;
            UpdateCounterDisplay();
        }

        public override void Initialize()
        {
            try
            {
                base.Initialize();
            }
            catch (System.Exception )
            {
                
            }
        }

        public void Init(Sprite machineSprite, int initialCount)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            spriteRenderer.sprite = machineSprite;
        }

        public void Clicked(int playerNumber)
        {
            
            if (quizOver || !machineActive || !powered) return;

            PlayerManager player = Singleton<CoreGameManager>.Instance?.GetPlayer(playerNumber);
            if (player == null || player.itm == null) return;

            ItemObject heldItem = player.itm.items[player.itm.selectedItem];
            if (heldItem == null || heldItem != Plugin.EmptyBucketItemObject)
            {
                return;
            }

            
            player.itm.RemoveItem(player.itm.selectedItem);
            player.itm.AddItem(Plugin.MilkItemObject);

            countRemaining--;
            UpdateCounterDisplay();

            if (countRemaining <= 0)
            {
                spriteRenderer.color = Color.gray;
                machineActive = false;
                if (counterText != null) counterText.color = Color.gray;

                if (armed && !quizOver)
                {
                    
                    armed = false;
                    quizOver = true;
                    ResolveQuiz(true, playerNumber);
                    Completed(playerNumber, true);
                }
                else
                {
                    
                    Singleton<CoreGameManager>.Instance.AddPoints(NormalPoints, playerNumber, playAnimation: true);
                    Completed(playerNumber, true);
                }
            }
        }

        public override void SetBonusMode(bool val)
        {
            base.SetBonusMode(val);
            if (val) ReInit();
        }

        protected override void VirtualUpdate()
        {
            if (counterText == null) return;

            
            
            if (armed && !quizResolved && room != null && !room.Powered)
            {
                armed = false;
                quizStarted = false;
                quizResolved = false;
                quizOver = false;
                if (timerCoroutine != null) { StopCoroutine(timerCoroutine); timerCoroutine = null; }
                if (audioSrc != null && audioSrc.isPlaying) audioSrc.Stop();
                UnlockDoors();
                
            }

            
            
            if (armed && !quizResolved && !quizOver && audioSrc != null && audioSrc.isPlaying)
            {
                ForceCloseDoors();
            }

            
            if (!quizStarted && !quizResolved)
            {
                PlayerManager p = null;
                try { p = Singleton<CoreGameManager>.Instance?.GetPlayer(0); } catch (System.Exception) { }
                if (p != null && targetRoom == null)
                {
                    
                    EnvironmentController ec = null;
                    try { ec = Singleton<CoreGameManager>.Instance?.GetComponent<EnvironmentController>(); } catch (System.Exception) { }
                    if (ec != null && ec.rooms != null)
                    {
                        foreach (var r in ec.rooms)
                        {
                            if (r != null && r.containsPosition(transform.position)) { targetRoom = r; break; }
                        }
                    }
                }
                if (p != null && targetRoom != null && targetRoom.containsPosition(p.transform.position))
                    StartQuiz(p);
            }

            
            Transform t = counterText.transform;
            Camera cam = Camera.main;
            if (cam != null)
            {
                t.rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
            }
            float y = TextBaseHeight + Mathf.Sin(Time.unscaledTime * FloatSpeed) * FloatAmplitude;
            Vector3 lp = t.localPosition;
            t.localPosition = new Vector3(lp.x, y, lp.z);
        }

        
        private void StartQuiz(PlayerManager p)
        {
            
            if (room == null || !room.Powered) return;

            quizStarted = true;
            armed = true;
            quizOver = false;
            countRemaining = QuizUses;
            machineActive = true;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            if (counterText != null) counterText.color = Color.white;
            UpdateCounterDisplay();

            
            if (targetRoom.doors != null)
            {
                foreach (var d in targetRoom.doors)
                {
                    if (d != null) { d.Lock(false); lockedDoors.Add(d); }
                }
            }

            
            if (tickClip != null && audioSrc != null)
            {
                audioSrc.clip = tickClip;
                audioSrc.time = 0f;
                if (audioSrc.isPlaying) audioSrc.Stop();
                audioSrc.volume = 1f;
                audioSrc.enabled = true;
                audioSrc.Play();
            }
            float dur = (tickClip != null) ? tickClip.length : 30f;
            timerCoroutine = StartCoroutine(QuizTimer(dur));

            
        }

        private IEnumerator QuizTimer(float dur)
        {
            yield return new WaitForSeconds(dur);
            if (!quizResolved)
                ResolveQuiz(false, 0);
        }

        
        public void ResolveQuiz(bool success, int player)
        {
            if (quizResolved) return;
            quizResolved = true;

            
            if (audioSrc != null && audioSrc.isPlaying) audioSrc.Stop();

            
            if (targetRoom != null && targetRoom.doors != null)
            {
                foreach (var d in targetRoom.doors) lockedDoors.Remove(d);
            }

            if (success)
            {
                if (correctClip != null && audioSrc != null) audioSrc.PlayOneShot(correctClip);
                Singleton<CoreGameManager>.Instance.AddPoints(QuizPoints, player, playAnimation: true);
                try { AchievementHelper.UnlockAchievement("quiz_success"); } catch (System.Exception) { }
                
                
                
                
                try
                {
                    if (targetRoom != null && targetRoom.ec != null)
                        targetRoom.ec.GetBaldi()?.Praise(30f, false);
                }
                catch (System.Exception) { }
            }
            else
            {
                if (incorrectClip != null && audioSrc != null) audioSrc.PlayOneShot(incorrectClip);
                
                try
                {
                    if (targetRoom != null && targetRoom.ec != null)
                        targetRoom.ec.GetBaldi()?.GetAngry(QuizFailAnger);
                }
                catch (System.Exception) { }
                
                try { SpawnNotebook(); }
                catch (System.Exception ) {  }
                
                machineActive = false;
                armed = false;
                quizOver = true;
                if (spriteRenderer != null) spriteRenderer.color = Color.gray;
                if (counterText != null) counterText.color = Color.gray;
                try { AchievementHelper.UnlockAchievement("quiz_fail"); } catch (System.Exception) { }
                
            }

            
            UnlockDoors();
        }

        private void UnlockDoors()
        {
            if (targetRoom == null || targetRoom.doors == null) return;
            foreach (var d in targetRoom.doors)
            {
                if (d != null)
                {
                    try { d.Unlock(); }
                    catch (System.Exception ) {  }
                }
            }
        }

        
        
        
        private void ForceCloseDoors()
        {
            if (targetRoom == null || targetRoom.doors == null) return;
            foreach (var d in targetRoom.doors)
            {
                if (d == null) continue;
                try { d.Lock(false); lockedDoors.Add(d); } catch (System.Exception) { }
                try { d.Open(false, false); } catch (System.Exception) { }
            }
        }

        private void OnDestroy()
        {
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            if (targetRoom != null && targetRoom.doors != null)
                foreach (var d in targetRoom.doors) lockedDoors.Remove(d);
        }

        private void UpdateCounterDisplay()
        {
            if (counterText != null && machineActive)
            {
                counterText.text = countRemaining.ToString();
            }
        }

        public void ClickableSighted(int player) { }
        public void ClickableUnsighted(int player) { }
        public bool ClickableHidden() => false;
        public bool ClickableRequiresNormalHeight() => false;
    }
}
