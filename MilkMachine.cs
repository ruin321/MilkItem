using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MTM101BaldAPI.AssetTools;

namespace MilkItem
{
    
    
    
    
    
    
    
    
    
    
    
    
    
    public class MilkMachine : Activity, IClickable<int>
    {
        private SpriteRenderer spriteRenderer;
        private TMP_Text counterText;
        private int countRemaining;
        private bool machineActive = true;

        
        
        
        private const float TextBaseHeight = 6.1f;
        private const float FloatAmplitude = 0.18f; 
        private const float FloatSpeed = 2.2f;      

        
        

        
        private const int NormalPoints = 50;
        private const int BonusPoints = 100;

        private void Awake()
        {
            
            
            spriteRenderer = GetComponent<SpriteRenderer>();

            
            
            
            GameObject textObj = new GameObject("CounterBillboard");
            textObj.transform.SetParent(transform);
            textObj.transform.localPosition = new Vector3(0f, TextBaseHeight, 0f);
            counterText = textObj.AddComponent<TextMeshPro>();
            
            
            try
            {
                counterText.font = TMP_Settings.defaultFontAsset;
            }
            catch { }
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
        }

        
        
        public override void ReInit()
        {
            if (notebook != null)
            {
                notebook.transform.position = transform.position + Vector3.up * 5f;
                notebook.gameObject.SetActive(false);
            }
            completed = false;

            
            countRemaining = UnityEngine.Random.Range(1, 16);
            machineActive = true;
            
            UpdatePowerVisual();
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
            
            if (!machineActive || room == null || !room.Powered) return;

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
            UpdatePowerVisual(); 

            if (countRemaining <= 0)
            {
                machineActive = false;
                UpdatePowerVisual(); 

                
                
                Singleton<CoreGameManager>.Instance.AddPoints(bonusMode ? BonusPoints : NormalPoints, playerNumber, playAnimation: true);

                
                
                
                Completed(playerNumber, true);
            }
        }

        
        
        
        
        public override void SetBonusMode(bool val)
        {
            base.SetBonusMode(val);
            if (val)
            {
                ReInit();
            }
        }

        
        protected override void VirtualUpdate()
        {
            
            UpdatePowerVisual();

            if (counterText == null) return;

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

        
        
        
        
        
        private void UpdatePowerVisual()
        {
            if (room == null) return;
            bool hasPower = room.Powered;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = (hasPower && machineActive) ? Color.white : Color.gray;
            }
            if (counterText != null)
            {
                if (hasPower && machineActive)
                {
                    counterText.enabled = true;
                    counterText.color = Color.white;
                    counterText.text = countRemaining.ToString();
                }
                else
                {
                    counterText.enabled = false;
                }
            }
        }

        public void ClickableSighted(int player) { }
        public void ClickableUnsighted(int player) { }
        public bool ClickableHidden() => false;
        public bool ClickableRequiresNormalHeight() => false;
    }
}
