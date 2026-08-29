using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;

namespace MilkItem
{
    
    
    
    
    
    
    
    
    
    
    
    
    public class MilkSalesman : NPC
    {
        internal const float WanderSpeed = 10f;      
        private const float OfferRange = 13f;        
        private const float LeaveRange = 17f;        
        private const float CooldownTime = 15f;      

        
        private const int Price150 = 150;
        private const int Price100 = 100;
        private const float WaitBuffer = 0.25f;      
        private const float TurnSmoothSpeed = 5f;    

        
        private Sprite idleSprite;
        private Sprite talkSprite0;
        private Sprite talkSprite1;
        private Sprite noSprite;

        
        private AudioManager audMan;

        
        private bool talking = false;
        
        private float cooldown = 0f;
        
        private bool frozen = false;
        
        private PlayerManager sellPlayer;
        
        private Quaternion? _preSellPlayerRot = null;
        private Quaternion? _preSellCameraRot = null;
        
        private bool playerLeft = false;

        
        private AudioClip helloClip, costClip, price150Clip, price100Clip,
                          walkawayClip, nomoneyClip, freegiveClip, noSpaceClip;

        
        
        private static readonly ItemObject[] GoodMilks =
        {
            Plugin.ChocolateMilkItemObject,
            Plugin.MilkSodaItemObject,
            Plugin.CompressedMilkItemObject,
            Plugin.AppleMilkItemObject,
        };
        
        private static readonly ItemObject[] BadMilks =
        {
            Plugin.MilkItemObject,
            Plugin.MiItemObject,
            Plugin.LkItemObject,
            Plugin.LostBilkItemObject,
            Plugin.RottenMilkItemObject,
        };

        
        private const string HelloLine = "Hi! Would you buy some milk?";
        private const string CostLine = "It's only cost...";
        private const string WalkAwayLine = "Oh, forget about it...";
        private const string NoMoneyLine = "Oh, looks like you don't have enough money to buy my milk.";
        private const string FreeGiveLine = "Ok, I'll give you a free one.";
        private const string NoSpaceLine = "Looks like your pockets are full. Come back when you have room.";

        public override void Initialize()
        {
            base.Initialize();

            
            if (base.spriteRenderer == null || base.spriteRenderer.Length == 0)
            {
                base.spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
            }

            
            idleSprite = Load("Milk salesman.png");
            talkSprite0 = Load("Milk salesman_Talking.png");
            talkSprite1 = Load("Milk salesman_Talking_1.png");
            noSprite = Load("Milk salesman_Talking_No.png");
            SetSprite(idleSprite);

            
            

            
            base.navigator.SetSpeed(WanderSpeed);
            base.navigator.maxSpeed = WanderSpeed;
            base.behaviorStateMachine.ChangeState(new MilkSalesman_WanderState(this));

            
            StartCoroutine(TalkAnim());

            
        }

        private Sprite Load(string fileName)
        {
            try
            {
                
                
                return AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 38f, "npc/Milksalesman/" + fileName);
            }
            catch (System.Exception )
            {
                
                return null;
            }
        }

        private void SetSprite(Sprite s)
        {
            if (s != null && base.spriteRenderer != null && base.spriteRenderer.Length > 0)
            {
                base.spriteRenderer[0].sprite = s;
            }
        }

        
        private AudioManager EnsureAudMan()
        {
            if (audMan == null)
            {
                try
                {
                    
                    
                    
                    
                    
                    GameObject host = new GameObject("SalesmanAudMan");
                    host.SetActive(false);                     
                    host.transform.SetParent(transform, false);
                    AudioSource src = host.AddComponent<AudioSource>();
                    Plugin.RouteToMixer(src, Plugin.MilkMixerRoute.Voice); 
                    audMan = host.AddComponent<AudioManager>();
                    audMan.audioDevice = src;                  
                    
                    
                    
                    audMan.positional = false;
                    host.SetActive(true);                      
                }
                catch { audMan = null; }
            }
            return audMan;
        }

        
        private AudioClip GetClip(string fileName)
        {
            try
            {
                return AssetLoader.AudioClipFromMod(Plugin.Instance, "npc/Milksalesman/" + fileName);
            }
            catch (System.Exception )
            {
                
                return null;
            }
        }

        private AudioClip EnsureClip(ref AudioClip cache, string fileName)
        {
            if (cache == null) cache = GetClip(fileName);
            return cache;
        }

        
        private void Speak(string line, AudioClip clip)
        {
            if (clip == null) return;
            AudioManager m = EnsureAudMan();
            if (m == null) return;
            SoundObject so = ObjectCreators.CreateSoundObject(clip, line, SoundType.Voice, Color.white, clip.length);
            
            
            m.PlaySingle(so);
        }

        
        private IEnumerator TalkAnim()
        {
            bool flip = false;
            while (true)
            {
                yield return new WaitForSeconds(0.25f);
                if (!talking) continue;
                flip = !flip;
                SetSprite(flip ? talkSprite0 : talkSprite1);
            }
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();

            
            if (cooldown > 0f)
            {
                cooldown -= Time.deltaTime;
                return;
            }

            
            if (base.behaviorStateMachine.CurrentState is MilkSalesman_SellState) return;

            PlayerManager player = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
            if (player == null) return;
            
            
            if (player.plm == null || player.plm.Entity == null || player.plm.Entity.Frozen) return;

            float dist = Vector3.Distance(transform.position, player.transform.position);
            
            
            
            
            
            if (dist <= OfferRange && IsPlayerClear(player))
            {
                base.behaviorStateMachine.ChangeState(new MilkSalesman_SellState(this));
            }
        }

        
        
        
        
        private bool IsPlayerClear(PlayerManager player)
        {
            try
            {
                if (player == null || player.plm == null || player.plm.Entity == null) return false;
                
                if (!player.plm.Entity.Visible) return false;
                
                Vector3 origin = transform.position + Vector3.up * 1.2f;
                Vector3 target = player.transform.position + Vector3.up * 1.2f;
                Vector3 dir = target - origin;
                float dist = dir.magnitude;
                if (dist <= 0.01f) return true;
                dir /= dist;
                if (dist > OfferRange) return false; 
                foreach (var hit in UnityEngine.Physics.RaycastAll(origin, dir, dist, UnityEngine.Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider == null) continue;
                    Transform root = hit.collider.transform.root;
                    if (root == player.transform.root) continue;  
                    if (root == transform.root) continue;         
                    if (root.CompareTag("NPC")) continue;         
                    return false;                                 
                }
                return true;
            }
            catch (System.Exception)
            {
                return false; 
            }
        }

        
        public void BeginSell()
        {
            sellPlayer = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
            
            if (sellPlayer != null)
            {
                _preSellPlayerRot = sellPlayer.transform.rotation;
                if (sellPlayer.cameraBase != null)
                    _preSellCameraRot = sellPlayer.cameraBase.rotation;
                else
                    _preSellCameraRot = null;
            }
            else
            {
                _preSellPlayerRot = null;
                _preSellCameraRot = null;
            }
            StartCoroutine(SellFlow());
        }

        private IEnumerator SellFlow()
        {
            PlayerManager player = sellPlayer;
            if (player == null)
            {
                FinishSell();
                yield break;
            }

            
            talking = true;
            SetFrozen(player, true);
            AudioClip hello = EnsureClip(ref helloClip, "Salesman_Hello.wav");
            Speak(HelloLine, hello);
            float helloLen = (hello != null ? hello.length : 1.9f);
            float t = 0f;
            while (t < helloLen)
            {
                t += Time.deltaTime;
                FacePlayerToMe(player);
                yield return null;
            }

            
            SetFrozen(player, false);
            talking = false;

            bool high = UnityEngine.Random.value < 0.5f;
            int price = high ? Price150 : Price100;

            AudioClip cost = EnsureClip(ref costClip, "Salesman_Cost.wav");
            Speak(CostLine, cost);
            yield return StayWait(player, (cost != null ? cost.length : 1.4f) + WaitBuffer);
            if (playerLeft)
            {
                StartCoroutine(FinishWalkAway(player));
                yield break;
            }

            AudioClip priceClip = high
                ? EnsureClip(ref price150Clip, "Salesman_Price150.mp3")
                : EnsureClip(ref price100Clip, "Salesman_Price100.mp3");
            Speak(price + " YTPs.", priceClip);
            yield return StayWait(player, (priceClip != null ? priceClip.length : 1.4f) + WaitBuffer);
            if (playerLeft)
            {
                StartCoroutine(FinishWalkAway(player));
                yield break;
            }

            
            
            talking = false;
            SetSprite(idleSprite);
            float confirmTimer = 0f;
            const float ConfirmTimeout = 12f;
            bool confirmed = false;
            while (confirmTimer < ConfirmTimeout)
            {
                confirmTimer += Time.deltaTime;
                if (IsGone(player))
                {
                    StartCoroutine(FinishWalkAway(player));
                    yield break;
                }
                if (IsSoldierClicked()) { confirmed = true; break; }
                yield return null;
            }
            if (!confirmed)
            {
                StartCoroutine(FinishWalkAway(player));
                yield break;
            }

            
            if (player.itm != null && player.itm.InventoryFull())
            {
                talking = true;
                AudioClip ns = EnsureClip(ref noSpaceClip, "Salesman_NoSpace.wav");
                if (ns != null) Speak(NoSpaceLine, ns);
                yield return StayWait(player, (ns != null ? ns.length : 1.6f) + WaitBuffer);
                talking = false;
                SetSprite(idleSprite);
                FinishSell();
                yield break;
            }

            
            int money = GetMoney(player);
            if (money >= price)
            {
                
                Deduct(player, price);
                GiveMilk(player, GoodMilks);
                SetSprite(idleSprite);
                
                FinishSell();
            }
            else
            {
                
                talking = true;
                AudioClip nm = EnsureClip(ref nomoneyClip, "Salesman_NoMoney.wav");
                Speak(NoMoneyLine, nm);
                yield return StayWait(player, (nm != null ? nm.length : 2.6f) + WaitBuffer);
                talking = false;

                AudioClip fg = EnsureClip(ref freegiveClip, "Salesman_FreeGive.wav");
                Speak(FreeGiveLine, fg);
                GiveMilk(player, BadMilks);
                SetSprite(idleSprite);
                
                FinishSell();
            }
        }

        
        
        
        
        
        private void FacePlayerToMe(PlayerManager player)
        {
            if (player == null) return;
            Vector3 dir = transform.position - player.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;
            Quaternion target = Quaternion.LookRotation(dir.normalized);
            float k = Mathf.Clamp01(Time.deltaTime * TurnSmoothSpeed);
            try { player.transform.rotation = Quaternion.Slerp(player.transform.rotation, target, k); } catch (System.Exception) { }          
            if (player.cameraBase != null)
            {
                try { player.cameraBase.rotation = Quaternion.Slerp(player.cameraBase.rotation, target, k); } catch (System.Exception) { }    
            }
        }

        
        private void SetFrozen(PlayerManager player, bool freeze)
        {
            try
            {
                if (player == null || player.plm == null || player.plm.Entity == null) return;
                var e = player.plm.Entity;
                if (freeze)
                {
                    if (!frozen) { e.SetFrozen(true); frozen = true; }
                }
                else
                {
                    if (frozen) { if (e.Frozen) e.SetFrozen(false); frozen = false; }
                }
            }
            catch (System.Exception )
            {
                frozen = freeze;
                
            }
        }

        
        private bool IsGone(PlayerManager player)
        {
            if (player == null) return true;
            return Vector3.Distance(transform.position, player.transform.position) > LeaveRange;
        }

        
        
        
        private bool IsSoldierClicked()
        {
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) return false;
            Camera cam = Camera.main;
            if (cam == null) return false;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            return Physics.Raycast(ray, out hit) && hit.collider != null &&
                   (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform));
        }

        
        private IEnumerator StayWait(PlayerManager player, float seconds)
        {
            playerLeft = false;
            float t = seconds;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                if (IsGone(player)) { playerLeft = true; yield break; }
                yield return null;
            }
        }

        private int GetMoney(PlayerManager player)
        {
            try { return Singleton<CoreGameManager>.Instance.GetPoints(player.playerNumber); }
            catch (System.Exception )
            {
                
                return 0;
            }
        }

        private void Deduct(PlayerManager player, int amount)
        {
            try
            {
                Singleton<CoreGameManager>.Instance.AddPoints(-amount, player.playerNumber, playAnimation: true);
            }
            catch (System.Exception )
            {
                
            }
        }

        
        private void GiveMilk(PlayerManager player, ItemObject[] pool)
        {
            try
            {
                if (player == null || player.itm == null) return;
                if (player.itm.InventoryFull()) return;
                var valid = new System.Collections.Generic.List<ItemObject>();
                if (pool != null) valid.AddRange(pool);
                valid.RemoveAll(x => x == null);
                if (valid.Count == 0) return;
                var chosen = valid[UnityEngine.Random.Range(0, valid.Count)];
                player.itm.AddItem(chosen);
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        private IEnumerator FinishWalkAway(PlayerManager player)
        {
            talking = false;
            SetFrozen(player, false);
            SetSprite(noSprite);
            AudioClip w = EnsureClip(ref walkawayClip, "Salesman_WalkAway.wav");
            Speak(WalkAwayLine, w);
            float len = (w != null ? w.length : 2.2f);
            float t = 0f;
            while (t < len) { t += Time.deltaTime; yield return null; }
            SetSprite(idleSprite);
            FinishSell();
        }

        
        private void RestorePlayerRotation()
        {
            try
            {
                if (_preSellPlayerRot.HasValue && sellPlayer != null)
                {
                    sellPlayer.transform.rotation = _preSellPlayerRot.Value;
                    if (_preSellCameraRot.HasValue && sellPlayer.cameraBase != null)
                        sellPlayer.cameraBase.rotation = _preSellCameraRot.Value;
                }
            }
            catch { }
            _preSellPlayerRot = null;
            _preSellCameraRot = null;
        }

        private void FinishSell()
        {
            try
            {
                RestorePlayerRotation();
                talking = false;
                SetFrozen(sellPlayer, false);
                SetSprite(idleSprite);
                cooldown = CooldownTime;
                base.behaviorStateMachine.ChangeState(new MilkSalesman_WanderState(this));
            }
            catch (System.Exception )
            {
                RestorePlayerRotation();
                talking = false;
                SetFrozen(sellPlayer, false);
                SetSprite(idleSprite);
                cooldown = CooldownTime;
                try { base.behaviorStateMachine.ChangeState(new MilkSalesman_WanderState(this)); } catch { }
            }
        }
    }

    
    internal class MilkSalesman_WanderState : NpcState
    {
        public MilkSalesman_WanderState(NPC npc) : base(npc) { }

        public override void Enter()
        {
            base.Enter();
            npc.Navigator.SetSpeed(MilkSalesman.WanderSpeed);
            npc.Navigator.maxSpeed = MilkSalesman.WanderSpeed;
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }

        public override void DestinationEmpty()
        {
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }
    }

    
    internal class MilkSalesman_SellState : NpcState
    {
        public MilkSalesman_SellState(NPC npc) : base(npc) { }

        public override void Enter()
        {
            base.Enter();
            npc.Navigator.SetSpeed(0f);
            npc.Navigator.maxSpeed = 0f;
            ((MilkSalesman)npc).BeginSell();
        }

        public override void Exit()
        {
            npc.Navigator.SetSpeed(MilkSalesman.WanderSpeed);
            npc.Navigator.maxSpeed = MilkSalesman.WanderSpeed;
        }
    }
}
