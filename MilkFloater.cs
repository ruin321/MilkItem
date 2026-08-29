using System.Collections.Generic;
using UnityEngine;
using MTM101BaldAPI;

namespace MilkItem
{
    
    
    
    public class MilkFloater : MonoBehaviour
    {
        private float bobSpeed;
        private float baseHeight;
        private float bobAmplitude = 0.4f;
        private float moveSpeed;
        private readonly List<Vector3> targets = new List<Vector3>();
        private int tIndex = -1;
        private readonly float minDist = 0.8f;
        private readonly float driftRadius = 8f;   

        public static MilkFloater Attach(GameObject target, List<Vector3> safePoints, float height)
        {
            MilkFloater f = target.GetComponent<MilkFloater>();
            if (f != null) return f;
            f = target.AddComponent<MilkFloater>();
            f.baseHeight = height;
            f.bobSpeed = Random.Range(0.8f, 1.6f);
            f.moveSpeed = Random.Range(1.2f, 2.4f);
            f.PickTargets(safePoints);
            return f;
        }

        
        private void PickTargets(List<Vector3> safePoints)
        {
            if (safePoints == null || safePoints.Count == 0) return;
            Vector3 start = transform.position;
            List<Vector3> local = new List<Vector3>();
            foreach (var p in safePoints)
            {
                Vector3 flat = new Vector3(p.x, 0f, p.z);
                Vector3 sflat = new Vector3(start.x, 0f, start.z);
                if (Vector3.Distance(flat, sflat) <= driftRadius) local.Add(p);
            }
            if (local.Count == 0) local = safePoints; 
            
            for (int i = local.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                Vector3 tmp = local[i];
                local[i] = local[j];
                local[j] = tmp;
            }
            int take = Mathf.Min(5, local.Count);
            for (int i = 0; i < take; i++) targets.Add(local[i]);
        }

        private void Update()
        {
            Vector3 p = transform.position;
            if (targets.Count > 0)
            {
                if (tIndex < 0) tIndex = 0;
                Vector3 dest = new Vector3(targets[tIndex].x, baseHeight, targets[tIndex].z);
                Vector3 to = dest - p; to.y = 0f;
                if (to.sqrMagnitude < minDist * minDist)
                {
                    
                    tIndex = (tIndex + 1) % targets.Count;
                    transform.position = new Vector3(p.x, baseHeight + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude, p.z);
                    return;
                }
                Vector3 np = Vector3.MoveTowards(p, dest, moveSpeed * Time.deltaTime);
                np.y = baseHeight + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude; 
                transform.position = np;
                return;
            }
            
            transform.position = new Vector3(p.x, baseHeight + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude, p.z);
        }
    }
}