using UnityEngine;
using UnityEngine.UI;

namespace MilkItem
{
    
    
    
    public class HotRedOverlay : MonoBehaviour
    {
        public static readonly System.Collections.Generic.HashSet<int> ActivePlayers =
            new System.Collections.Generic.HashSet<int>();

        private static Canvas _canvas;
        private static Image _image;

        private static void EnsureCanvas()
        {
            if (_canvas != null) return;
            GameObject go = new GameObject("HotRedOverlayCanvas");
            _canvas = go.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay; 
            _canvas.sortingOrder = 9997; 
            GameObject imgGo = new GameObject("HotRedImage");
            imgGo.transform.SetParent(_canvas.transform, false);
            _image = imgGo.AddComponent<Image>();
            
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _image.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            _image.color = new Color(1f, 0.25f, 0.15f, 0.3f);
            RectTransform rt = _image.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            _image.enabled = false; 
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        void Update()
        {
            EnsureCanvas();
            if (_image != null)
            {
                bool on = ActivePlayers.Count > 0;
                _image.enabled = on;
                if (on)
                {
                    
                    float a = 0.28f + Mathf.Sin(Time.time * 2.2f) * 0.06f;
                    _image.color = new Color(1f, 0.25f, 0.15f, a);
                }
            }
        }
    }
}
