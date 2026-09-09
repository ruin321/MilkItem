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


        private Sprite fullSprite;
        private Sprite emptySprite;

        
        
        
        private const float TextBaseHeight = 4.8f; 

        
        private const int NormalPoints = 50;
        private const int BonusPoints = 100;

        private void Awake()
        {
            
            
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            
            
            
            GameObject textObj = new GameObject("CounterBillboard");
            textObj.transform.SetParent(transform);
            textObj.transform.localPosition = new Vector3(0f, TextBaseHeight, 0f);
            counterText = textObj.AddComponent<TextMeshPro>();
            
            
            try
            {
                counterText.font = TMP_Settings.defaultFontAsset;
            }
            catch { }
            counterText.fontSize = 14;
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
                Plugin.DeduplicateExteriorSigns(this);
                RegisterOcclusion();
            }
            catch (System.Exception )
            {
                
            }
        }





        private void RegisterOcclusion()
        {
            try
            {
                if (spriteRenderer == null || room == null || room.ec == null) return;
                Cell cell = room.ec.CellFromPosition(transform.position);
                if (cell == null) return;
                if (!cell.renderers.Contains(spriteRenderer))
                    cell.AddRenderer(spriteRenderer);
                if (cell.HasChunk)
                    spriteRenderer.enabled = cell.Chunk.Rendering;
            }
            catch (System.Exception)
            {
            }
        }

        public void Init(Sprite machineSprite, int initialCount)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            spriteRenderer.sprite = machineSprite;
        }

        public void SetStateSprites(Sprite full, Sprite empty)
        {
            fullSprite = full;
            emptySprite = empty;
            if (spriteRenderer != null && spriteRenderer.sprite == null) spriteRenderer.sprite = full ?? empty;
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

            Camera cam = Camera.main;
            if (cam != null)
            {
                Quaternion face = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
                
                if (spriteRenderer != null) spriteRenderer.transform.rotation = face;
                
                Transform t = counterText.transform;
                t.rotation = face;
                Vector3 lp = t.localPosition;
                t.localPosition = new Vector3(lp.x, TextBaseHeight, lp.z);
            }
        }

        
        
        
        
        
        private void UpdatePowerVisual()
        {
            if (room == null) return;
            bool hasPower = room.Powered;
            if (spriteRenderer != null)
            {
                Sprite stateSprite = machineActive ? fullSprite : emptySprite;
                if (stateSprite != null) spriteRenderer.sprite = stateSprite;
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
