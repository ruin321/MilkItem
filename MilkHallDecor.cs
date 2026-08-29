using UnityEngine;

namespace MilkItem
{
    
    
    
    public class MilkHallDecor : MonoBehaviour
    {
        private const float FloatAmplitude = 0.08f;
        private const float FloatSpeed = 1.2f;

        private Vector3 baseLocalPos;

        
        public static MilkHallDecor Create(Transform parent, Vector3 worldPosition, Sprite sprite)
        {
            GameObject go = new GameObject("MilkHallDecor");
            go.transform.SetParent(parent, true); 
            go.transform.position = worldPosition;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 5;

            MilkHallDecor d = go.AddComponent<MilkHallDecor>();
            d.baseLocalPos = go.transform.localPosition;
            return d;
        }

        private void Awake()
        {
            baseLocalPos = transform.localPosition;
        }

        private void Update()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                
                transform.rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
            }

            float y = baseLocalPos.y + Mathf.Sin(Time.unscaledTime * FloatSpeed) * FloatAmplitude;
            Vector3 lp = transform.localPosition;
            transform.localPosition = new Vector3(lp.x, y, lp.z);
        }
    }
}
