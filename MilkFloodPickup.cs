using UnityEngine;

namespace MilkItem
{
    
    
    
    
    public class MilkFloodPickup : MonoBehaviour
    {
        
        public enum FloodMilkType
        {
            Normal,     
            DeepGreen,  
            Green,      
            Black,      
        }

        public MilkFloodEvent owner;
        public FloodMilkType type = FloodMilkType.Normal;

        private float fallSpeed = 0f;
        private const float MaxFallSpeed = 30f;
        private const float FallAccel = 60f;
        private const float DestroyY = -30f;
        private bool falling = false;

        public static MilkFloodPickup Attach(GameObject target, MilkFloodEvent owner, FloodMilkType type = FloodMilkType.Normal)
        {
            MilkFloodPickup tag = target.GetComponent<MilkFloodPickup>();
            if (tag == null)
            {
                tag = target.AddComponent<MilkFloodPickup>();
            }
            tag.owner = owner;
            tag.type = type;
            return tag;
        }

        
        
        
        public void StartFallAndDestroy()
        {
            if (falling) return;
            falling = true;

            
            var floater = GetComponent<MilkFloater>();
            if (floater != null) floater.enabled = false;

            if (owner != null) owner.OnFloodMilkFalling(this);
        }

        private void Update()
        {
            if (!falling) return;
            fallSpeed += FallAccel * Time.deltaTime;
            if (fallSpeed > MaxFallSpeed) fallSpeed = MaxFallSpeed;
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            if (transform.position.y < DestroyY)
            {
                Destroy(gameObject);
            }
        }
    }
}