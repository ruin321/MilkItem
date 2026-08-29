using System.Collections.Generic;
using UnityEngine;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;

namespace MilkItem
{
    
    
    public class IceMilkComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();
            Plugin.StopMilkRandomEvents();
            if (player.ec != null)
                IceZoneController.Create(player.ec, player.transform.position);
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.IceMilkItemObject);
        }
    }

    public class IceZoneController : MonoBehaviour
    {
        public const float Radius = 8f;        
        public const float Duration = 120f;    
        const float SlowFactor = 0.5f;         
        const float TintStrength = 0.55f;      
        static readonly Color IceTint = new Color(0.6f, 0.82f, 1f);

        const int SnowCount = 22;              
        const float SnowCeilingY = 5f;         
        const float SnowFloorY = 0.2f;         
        const float SnowFallSpeed = 1.6f;      
        const float SnowSway = 0.6f;           
        const float SnowSwaySpeed = 1.2f;      
        const int SnowSortingOrder = 100;      

        EnvironmentController ec;
        Vector3 center;
        float remain;
        float checkTimer;

        Sprite snowSprite;
        readonly List<Snowflake> snowflakes = new List<Snowflake>();
        readonly Dictionary<NPC, NpcState> affected = new Dictionary<NPC, NpcState>();

        class Snowflake
        {
            public Transform t;
            public float baseX;
            public float swayPhase;
            public float speed;
        }

        class NpcState
        {
            public float origMaxSpeed;
            public Color[] origColors;
        }

        public static IceZoneController Create(EnvironmentController ec, Vector3 pos)
        {
            if (ec == null) return null;
            var go = new GameObject("IceMilkZone");
            var z = go.AddComponent<IceZoneController>();
            z.ec = ec;
            z.center = new Vector3(pos.x, 0f, pos.z);
            z.remain = Duration;
            z.InitSnow();
            return z;
        }

        void InitSnow()
        {
            try { snowSprite = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 128f, "snow.png"); }
            catch (System.Exception) { }
            if (snowSprite == null) return;
            for (int i = 0; i < SnowCount; i++)
            {
                var go = new GameObject("IceSnow");
                go.transform.SetParent(transform, false);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = snowSprite;
                sr.sortingOrder = SnowSortingOrder;
                var f = new Snowflake
                {
                    t = go.transform,
                    swayPhase = UnityEngine.Random.value * Mathf.PI * 2f,
                    speed = SnowFallSpeed * (0.7f + UnityEngine.Random.value * 0.6f)
                };
                PlaceAtTop(f);
                snowflakes.Add(f);
            }
        }

        void PlaceAtTop(Snowflake f)
        {
            float ang = UnityEngine.Random.value * Mathf.PI * 2f;
            float rr = Radius * Mathf.Sqrt(UnityEngine.Random.value);
            f.baseX = center.x + Mathf.Cos(ang) * rr;
            float zz = center.z + Mathf.Sin(ang) * rr;
            f.t.position = new Vector3(f.baseX, center.y + SnowCeilingY, zz);
        }

        void Update()
        {
            remain -= Time.deltaTime;
            UpdateSnow();
            ApplyOngoingSlow();
            checkTimer -= Time.deltaTime;
            if (checkTimer <= 0f)
            {
                checkTimer = 0.2f;
                UpdateAffectedNpcs();
            }
            if (remain <= 0f)
            {
                Cleanup();
                Destroy(gameObject);
            }
        }

        void UpdateSnow()
        {
            foreach (Snowflake f in snowflakes)
            {
                if (f.t == null) continue;
                Vector3 p = f.t.position;
                f.swayPhase += SnowSwaySpeed * Time.deltaTime;
                p.y -= f.speed * Time.deltaTime;
                p.x = f.baseX + Mathf.Sin(f.swayPhase) * SnowSway;
                if (p.y <= center.y + SnowFloorY) PlaceAtTop(f);
                else f.t.position = p;
            }
        }

        bool InZone(Vector3 p)
        {
            float dx = p.x - center.x, dz = p.z - center.z;
            return dx * dx + dz * dz <= Radius * Radius;
        }

        
        void ApplyOngoingSlow()
        {
            foreach (KeyValuePair<NPC, NpcState> kv in affected)
            {
                NPC n = kv.Key;
                if (n == null || n.Navigator == null) continue;
                float slow = Mathf.Max(kv.Value.origMaxSpeed * SlowFactor, 0.01f);
                n.Navigator.maxSpeed = slow;
                if (n.Navigator.speed > slow) n.Navigator.SetSpeed(slow);
            }
        }

        void UpdateAffectedNpcs()
        {
            
            List<NPC> toLeave = null;
            foreach (KeyValuePair<NPC, NpcState> kv in affected)
            {
                NPC n = kv.Key;
                if (n == null || n.gameObject == null || !InZone(n.transform.position))
                {
                    if (toLeave == null) toLeave = new List<NPC>();
                    toLeave.Add(n);
                }
            }
            if (toLeave != null)
                foreach (NPC n in toLeave) RestoreNpc(n);

            
            if (ec == null || ec.Npcs == null) return;
            foreach (NPC n in ec.Npcs)
            {
                if (n == null || n.gameObject == null) continue;
                if (affected.ContainsKey(n)) continue;
                if (InZone(n.transform.position)) AffectNpc(n);
            }
        }

        void AffectNpc(NPC n)
        {
            var st = new NpcState();
            Navigator nav = n.Navigator;
            if (nav != null) st.origMaxSpeed = nav.maxSpeed;

            if (n.spriteRenderer != null && n.spriteRenderer.Length > 0)
            {
                st.origColors = new Color[n.spriteRenderer.Length];
                for (int i = 0; i < n.spriteRenderer.Length; i++)
                {
                    SpriteRenderer sr = n.spriteRenderer[i];
                    if (sr == null) continue;
                    st.origColors[i] = sr.color;
                    sr.color = Color.Lerp(sr.color, IceTint, TintStrength);
                }
            }

            if (nav != null)
            {
                nav.maxSpeed = Mathf.Max(st.origMaxSpeed * SlowFactor, 0.01f);
                if (nav.speed > nav.maxSpeed) nav.SetSpeed(nav.maxSpeed);
            }
            affected[n] = st;
        }

        void RestoreNpc(NPC n)
        {
            NpcState st;
            if (!affected.TryGetValue(n, out st)) return;

            Navigator nav = n.Navigator;
            if (nav != null && st.origMaxSpeed > 0f) nav.maxSpeed = st.origMaxSpeed;

            if (n.spriteRenderer != null && st.origColors != null)
            {
                for (int i = 0; i < n.spriteRenderer.Length && i < st.origColors.Length; i++)
                {
                    SpriteRenderer sr = n.spriteRenderer[i];
                    if (sr == null) continue;
                    sr.color = st.origColors[i];
                }
            }
            affected.Remove(n);
        }

        void Cleanup()
        {
            List<NPC> ks = new List<NPC>(affected.Keys);
            foreach (NPC n in ks) RestoreNpc(n);
            affected.Clear();
        }
    }
}
