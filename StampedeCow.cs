using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;

namespace MilkItem
{
    
    
    
    
    
    
    
    public class StampedeCow : PolishCow
    {
        internal const float StampedeSpeed = 26f;   
        private const float GoreRadius = 9f;        
        private const float GoreForce = 42f;        
        private const float GoreTime = 0.4f;        
        private const float GoreCooldown = 1.2f;    

        
        
        

        private readonly Dictionary<Entity, float> goreCooldowns = new Dictionary<Entity, float>();

        public override void Initialize()
        {
            base.Initialize(); 
            LoadRedHotFrames();
            base.navigator.SetSpeed(StampedeSpeed);
            base.navigator.maxSpeed = StampedeSpeed;
            base.behaviorStateMachine.ChangeState(new StampedeState(this));
        }

        
        private void LoadRedHotFrames()
        {
            try
            {
                if (base.spriteRenderer == null || base.spriteRenderer.Length == 0)
                    base.spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
                if (base.spriteRenderer == null || base.spriteRenderer.Length == 0) return;

                frames = new Sprite[21];
                for (int i = 0; i <= 20; i++)
                {
                    frames[i] = AssetLoader.SpriteFromMod(
                        Plugin.Instance,
                        Vector2.one / 2f,
                        25f,
                        "npc/cow/redhot/StampedeCow_" + i + ".png");
                }
                if (frames[0] != null) base.spriteRenderer[0].sprite = frames[0];
            }
            catch (System.Exception) { }
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (ec == null) return;

            
            var player = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
            if (player != null && player.plm != null && player.plm.Entity != null) TryGore(player.plm.Entity);

            
            if (ec.Npcs != null)
            {
                foreach (NPC npc in ec.Npcs)
                {
                    if (npc == null || npc == this) continue;
                    if (npc is PolishCow) continue;
                    if (npc.Navigator == null || npc.Navigator.Entity == null) continue;
                    TryGore(npc.Navigator.Entity);
                }
            }
        }

        private void TryGore(Entity target)
        {
            if (target == null) return;
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist > GoreRadius) return;

            float last;
            if (goreCooldowns.TryGetValue(target, out last) && Time.time - last < GoreCooldown) return;
            goreCooldowns[target] = Time.time;

            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0f;
            if (dir == Vector3.zero) dir = transform.forward;
            dir.Normalize();

            ActivityModifier am = target.ExternalActivity;
            if (am == null) return;
            MovementModifier mod = new MovementModifier(dir * GoreForce, 1f, 95);
            mod.forceTrigger = true;
            am.moveMods.Add(mod);
            StartCoroutine(RemoveMod(am, mod, GoreTime));

            if (hitSource != null && hitSource.clip != null) hitSource.PlayOneShot(hitSource.clip);
            
        }

        private IEnumerator RemoveMod(ActivityModifier am, MovementModifier mod, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (am != null && am.moveMods != null) am.moveMods.Remove(mod);
        }
    }

    
    internal class StampedeState : NpcState
    {
        public StampedeState(NPC npc) : base(npc) { }

        public override void Enter()
        {
            base.Enter();
            npc.Navigator.SetSpeed(StampedeCow.StampedeSpeed);
            npc.Navigator.maxSpeed = StampedeCow.StampedeSpeed;
            ChargeToRandom();
        }

        public override void DestinationEmpty()
        {
            ChargeToRandom();
        }

        private void ChargeToRandom()
        {
            Vector3 pos = npc.transform.position;
            float ang = (float)(new System.Random().NextDouble() * System.Math.PI * 2.0);
            Vector3 dir = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang));
            Vector3 target = pos + dir * 60f;
            if (npc.ec != null)
            {
                target.x = Mathf.Clamp(target.x, 10f, npc.ec.levelSize.x * 10f);
                target.z = Mathf.Clamp(target.z, 10f, npc.ec.levelSize.z * 10f);
            }
            ChangeNavigationState(new NavigationState_TargetPosition(npc, 0, target));
        }
    }
}
