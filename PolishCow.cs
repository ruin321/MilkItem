using System.Collections;
using UnityEngine;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using BepInEx;

namespace MilkItem
{
    
    
    
    
    
    
    
    public class PolishCow : NPC
    {
        
        
        protected Sprite[] frames;
        private int frameIndex = 0;
        private float animInterval = 0.12f; 

        
        private AudioSource musicSource;
        
        internal AudioSource hitSource;

        
        internal const float BaseSpeed = 10f;     
        private const float LaunchDistance = 35f; 
        private const float LaunchSpeed = 14f;    

        private void Awake()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            
            
            if (base.spriteRenderer == null || base.spriteRenderer.Length == 0)
            {
                base.spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
            }

            
            frames = new Sprite[21];
            for (int i = 0; i <= 20; i++)
            {
                frames[i] = AssetLoader.SpriteFromMod(
                    Plugin.Instance,
                    Vector2.one / 2f,
                    25f,
                    "npc/cow/PolishCow_" + i + ".png");
            }
            if (frames[0] != null)
            {
                base.spriteRenderer[0].sprite = frames[0];
            }

            
            AudioClip clip = AssetLoader.AudioClipFromMod(Plugin.Instance, "npc/cow/mus_Cow.wav");
            if (clip != null)
            {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.clip = clip;
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

            
            AudioClip hitClip = AssetLoader.AudioClipFromMod(Plugin.Instance, "Cow.wav");
            if (hitClip != null)
            {
                hitSource = gameObject.AddComponent<AudioSource>();
                hitSource.clip = hitClip;
                hitSource.spatialBlend = 1f;
                hitSource.minDistance = 10f;
                hitSource.maxDistance = 40f;
                hitSource.playOnAwake = false;
                hitSource.volume = 0.9f;
                Plugin.RouteToMixer(hitSource, Plugin.MilkMixerRoute.Effect);
            }

            
            
            if (GetComponent<Collider>() == null)
            {
                BoxCollider collider = gameObject.AddComponent<BoxCollider>();
                collider.center = new Vector3(0f, 2.5f, 0f);
                collider.size = new Vector3(3f, 5f, 1f);
                collider.isTrigger = false;
            }

            
            base.navigator.SetSpeed(BaseSpeed);
            base.navigator.maxSpeed = BaseSpeed;
            base.behaviorStateMachine.ChangeState(new PolishCow_State(this));

            
            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            while (true)
            {
                yield return new WaitForSeconds(animInterval);
                if (frames == null || frames.Length == 0) continue;
                frameIndex = (frameIndex + 1) % frames.Length;
                if (frames[frameIndex] != null)
                {
                    base.spriteRenderer[0].sprite = frames[frameIndex];
                }
            }
        }

        
        private float leftClickCooldown = 0f;

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();

            if (leftClickCooldown > 0f) leftClickCooldown -= Time.deltaTime;

            
            if (leftClickCooldown <= 0f && Input.GetMouseButtonDown(0))
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && hit.collider != null &&
                        (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform)))
                    {
                        leftClickCooldown = 0.25f; 
                        LaunchFromClick();
                    }
                }
            }

            
        }

        private void LaunchFromClick()
        {
            PlayerManager player = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
            if (player == null) return;

            
            if (hitSource != null && hitSource.clip != null) hitSource.PlayOneShot(hitSource.clip);

            
            TryMilkWithBucket(player);

            
            Vector3 away = transform.position - player.transform.position;
            away.y = 0f;
            if (away == Vector3.zero) away = Vector3.forward; 
            away.Normalize();

            
            Vector3 target = transform.position + away * LaunchDistance;

            
            base.navigator.maxSpeed = LaunchSpeed;
            base.navigator.SetSpeed(LaunchSpeed);
            (base.behaviorStateMachine.CurrentState as PolishCow_State)?.Launch(target);
        }

        
        
        private void TryMilkWithBucket(PlayerManager player)
        {
            try
            {
                ItemManager itm = player?.itm;
                if (itm == null || Plugin.EmptyBucketItemObject == null) return;
                int slot = itm.selectedItem;
                ItemObject held = itm.items[slot];
                if (held == null || held != Plugin.EmptyBucketItemObject) return; 
                ItemObject pick = RandomMilkLoot();
                if (pick == null) return;
                itm.SetItem(pick, slot); 
            }
            catch (System.Exception) { }
        }

        
        
        
        
        private static readonly ItemObject[] MilkLootPool =
        {
            Plugin.ChocolateMilkItemObject,
            Plugin.MilkSodaItemObject,
            Plugin.DietMilkSodaItemObject,
            Plugin.CompressedMilkItemObject,
            Plugin.AppleMilkItemObject,
            Plugin.ReverseMilkItemObject,
            Plugin.QuarterMilkItemObject,
            Plugin.WindowMilkItemObject,
            Plugin.MilkItemObject,
            Plugin.RottenMilkItemObject,
            Plugin.SilentMilkItemObject,
            Plugin.MooMilkItemObject,
            Plugin.IceMilkItemObject,
            Plugin.LostBilkItemObject,
            Plugin.MiItemObject,
            Plugin.LkItemObject,
        };
        private static ItemObject RandomMilkLoot()
        {
            if (MilkLootPool.Length == 0) return null;
            return MilkLootPool[UnityEngine.Random.Range(0, MilkLootPool.Length)];
        }
    }

    
    internal class PolishCow_State : NpcState
    {
        public PolishCow_State(NPC npc) : base(npc) { }

        public override void Enter()
        {
            base.Enter();
            
            npc.Navigator.SetSpeed(PolishCow.BaseSpeed);
            npc.Navigator.maxSpeed = PolishCow.BaseSpeed;
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }

        public override void DestinationEmpty()
        {
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }

        
        public void Launch(Vector3 target)
        {
            ChangeNavigationState(new NavigationState_TargetPosition(npc, 0, target));
        }
    }
}
