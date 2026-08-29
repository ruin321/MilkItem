using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;

namespace MilkItem
{
    
    
    
    
    public class FakeBlackSalesman : NPC
    {
        internal const float WanderSpeed = 16f;     
        internal const float ChaseSpeed = 26f;      
        internal const float ProximityRange = 25f;  
        internal const float ContactRange = 3f;     
        internal const float HearRange = 45f;       
        internal const float ChaseDuration = 20f;    
        internal const float NoSightAdvanceTime = 3f; 

        private Sprite[] _blackSprites;             
        private AudioSource _noiseSource;
        private bool _killing = false;

        
        
        
        internal Vector3[] _soundLocations = new Vector3[128];
        internal int _currentSoundVal = 0;

        
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hAfter, int x, int y, int cx, int cy, uint flags);
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hWnd, out RECT r);
        [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int idx);
        [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int idx, int val);
        [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint key, byte alpha, uint flags);
        
        [DllImport("user32.dll")] private static extern bool EnumWindows(EnumWindowsProc cb, IntPtr lParam);
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const uint LWA_ALPHA = 2;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private struct RECT { public int left, top, right, bottom; }

        private IntPtr _hWnd = IntPtr.Zero;
        private int _ox, _oy, _ow, _oh;
        private bool _winGot = false;

        private void EnsureWindow()
        {
            if (_winGot) return;
            try
            {
                _hWnd = FindGameWindow();
                if (_hWnd != IntPtr.Zero && GetWindowRect(_hWnd, out RECT r))
                {
                    _ox = r.left; _oy = r.top; _ow = r.right - r.left; _oh = r.bottom - r.top;
                    int ex = GetWindowLong(_hWnd, GWL_EXSTYLE);
                    SetWindowLong(_hWnd, GWL_EXSTYLE, ex | WS_EX_LAYERED);
                    _winGot = true;
                }
            }
            catch { }
        }

        
        
        private static IntPtr FindGameWindow()
        {
            uint curPid = 0;
            try { curPid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id; } catch { return IntPtr.Zero; }
            IntPtr result = IntPtr.Zero;
            try
            {
                EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
                {
                    uint pid;
                    GetWindowThreadProcessId(hWnd, out pid);
                    if (pid != curPid) return true;   
                    if (!IsWindowVisible(hWnd)) return true;
                    if (GetWindowTextLength(hWnd) > 0)
                    {
                        result = hWnd;
                        return false;                 
                    }
                    return true;
                }, IntPtr.Zero);
            }
            catch { }

            if (result == IntPtr.Zero)
            {
                try
                {
                    var p = System.Diagnostics.Process.GetCurrentProcess();
                    if (p != null && p.MainWindowHandle != IntPtr.Zero) result = p.MainWindowHandle;
                }
                catch { }
            }
            return result;
        }

        private void SetAlpha(byte a)
        {
            if (_hWnd != IntPtr.Zero)
                SetLayeredWindowAttributes(_hWnd, 0, a, LWA_ALPHA);
        }

        public override void Initialize()
        {
            base.Initialize();
            if (base.spriteRenderer == null || base.spriteRenderer.Length == 0)
                base.spriteRenderer = GetComponentsInChildren<SpriteRenderer>(true);

            
            _blackSprites = LoadBlackSalesmanSprites();
            if (_blackSprites != null && _blackSprites.Length > 0 && base.spriteRenderer.Length > 0)
            {
                
                for (int i = 0; i < base.spriteRenderer.Length; i++)
                {
                    if (base.spriteRenderer[i] != null)
                    {
                        int frameIdx = Mathf.Min(i, _blackSprites.Length - 1);
                        base.spriteRenderer[i].sprite = _blackSprites[frameIdx];
                        base.spriteRenderer[i].color = Color.white;
                    }
                }
            }

            
            _noiseSource = gameObject.AddComponent<AudioSource>();
            _noiseSource.clip = MakeNoiseClip();
            _noiseSource.loop = true;
            _noiseSource.volume = 0f;
            _noiseSource.spatialBlend = 1f;
            _noiseSource.minDistance = 1f;
            _noiseSource.maxDistance = ProximityRange;
            _noiseSource.playOnAwake = false;
            Plugin.RouteToMixer(_noiseSource, Plugin.MilkMixerRoute.Effect);
            _noiseSource.Play();

            base.navigator.SetSpeed(WanderSpeed);
            base.navigator.maxSpeed = ChaseSpeed;
            base.behaviorStateMachine.ChangeState(new FakeSalesman_WanderState(this));
        }

        
        private Sprite[] LoadBlackSalesmanSprites()
        {
            try
            {
                string[] spriteFiles = new string[]
                {
                    "npc/Milksalesman/Milk salesman.png",
                    "npc/Milksalesman/Milk salesman_Talking.png",
                    "npc/Milksalesman/Milk salesman_Talking_1.png",
                    "npc/Milksalesman/Milk salesman_No.png",
                };
                Sprite[] result = new Sprite[spriteFiles.Length];
                for (int i = 0; i < spriteFiles.Length; i++)
                {
                    try
                    {
                        Sprite orig = AssetLoader.SpriteFromMod(
                            Plugin.Instance,
                            Vector2.one / 2f,
                            38f,
                            spriteFiles[i]);
                        if (orig != null)
                        {
                            result[i] = MakeBlackSprite(orig);
                        }
                    }
                    catch { result[i] = null; }
                }
                return result;
            }
            catch { return null; }
        }

        
        private Sprite MakeBlackSprite(Sprite orig)
        {
            try
            {
                Texture2D origTex = orig.texture;
                Rect srcRect = orig.rect;
                int x0 = Mathf.RoundToInt(srcRect.x);
                int y0 = Mathf.RoundToInt(srcRect.y);
                int w = Mathf.RoundToInt(srcRect.width);
                int h = Mathf.RoundToInt(srcRect.height);
                if (w <= 0 || h <= 0) return null;

                Color[] px = origTex.GetPixels(x0, y0, w, h);
                for (int i = 0; i < px.Length; i++)
                {
                    
                    px[i] = new Color(0f, 0f, 0f, px[i].a);
                }
                Texture2D blackTex = new Texture2D(w, h, TextureFormat.ARGB32, false);
                blackTex.SetPixels(px);
                blackTex.Apply();
                blackTex.wrapMode = TextureWrapMode.Clamp;

                Vector2 pivotRatio = new Vector2(
                    Mathf.Clamp01((orig.pivot.x - srcRect.x) / srcRect.width),
                    Mathf.Clamp01((orig.pivot.y - srcRect.y) / srcRect.height));
                return Sprite.Create(blackTex, new Rect(0, 0, w, h), pivotRatio, orig.pixelsPerUnit);
            }
            catch { return null; }
        }

        
        private AudioClip MakeNoiseClip()
        {
            try
            {
                const int sr = 44100;
                const int len = sr;
                float[] d = new float[len];
                for (int i = 0; i < len; i++)
                    d[i] = (UnityEngine.Random.value * 2f - 1f) * 0.5f;
                AudioClip c = AudioClip.Create("FakeSalesmanNoise", len, 1, sr, false);
                c.SetData(d, 0);
                return c;
            }
            catch { return null; }
        }

        
        
        public override void Hear(GameObject source, Vector3 position, int value)
        {
            if (_killing) return;
            _soundLocations[value] = position;
            if (value >= _currentSoundVal)
            {
                if (behaviorStateMachine.CurrentState is FakeSalesman_ChaseState cs)
                {
                    
                    cs.PursueNextSound();
                }
                else
                {
                    behaviorStateMachine.ChangeState(new FakeSalesman_ChaseState(this));
                }
            }
        }

        
        internal void ClearSoundMemory()
        {
            for (int i = 0; i < _soundLocations.Length; i++) _soundLocations[i] = Vector3.zero;
            _currentSoundVal = 0;
        }

        
        internal bool HasSoundMemory()
        {
            for (int i = 0; i < _soundLocations.Length; i++) if (_soundLocations[i] != Vector3.zero) return true;
            return false;
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (_killing) return;

            var pm = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
            if (pm == null) return;

            float dist = Vector3.Distance(transform.position, pm.transform.position);

            
            if (dist < ProximityRange && !IsPlayerHidden(pm))
            {
                float t = 1f - (dist / ProximityRange);
                EnsureWindow();
                byte alpha = (byte)Mathf.RoundToInt(Mathf.Lerp(255f, 80f, t));
                SetAlpha(alpha);
                if (_noiseSource != null)
                    _noiseSource.volume = Mathf.Lerp(0f, 0.8f, t);
            }
            else
            {
                if (_winGot) SetAlpha(255);
                if (_noiseSource != null) _noiseSource.volume = 0f;
            }

            
            if (dist < ContactRange && !IsPlayerHidden(pm))
            {
                _killing = true;
                StartCoroutine(KillSequence(pm));
            }
        }

        
        private IEnumerator KillSequence(PlayerManager pm)
        {
            try { if (pm.plm != null && pm.plm.Entity != null) pm.plm.Entity.SetFrozen(true); } catch { }

            EnsureWindow();
            if (_noiseSource != null) _noiseSource.volume = 1f;

            float elapsed = 0f;
            float duration = 8f;
            int shakeX = 0, shakeY = 0;

            while (elapsed < duration)
            {
                
                if (IsPlayerHidden(pm))
                {
                    try { if (pm.plm != null && pm.plm.Entity != null) pm.plm.Entity.SetFrozen(false); } catch { }
                    try { if (_hWnd != IntPtr.Zero) SetAlpha(255); } catch { }
                    try { if (_hWnd != IntPtr.Zero) SetWindowPos(_hWnd, IntPtr.Zero, _ox, _oy, _ow, _oh, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE); } catch { }
                    try { if (_noiseSource != null) _noiseSource.volume = 0f; } catch { }
                    _killing = false;
                    yield break;
                }

                elapsed += Time.deltaTime;
                float frac = elapsed / duration;

                int maxShake = Mathf.RoundToInt(Mathf.Lerp(2f, 40f, frac));
                shakeX = UnityEngine.Random.Range(-maxShake, maxShake + 1);
                shakeY = UnityEngine.Random.Range(-maxShake, maxShake + 1);

                byte alpha = (byte)UnityEngine.Random.Range(
                    Mathf.RoundToInt(Mathf.Lerp(120f, 30f, frac)),
                    256);
                SetAlpha(alpha);

                if (_noiseSource != null)
                    _noiseSource.volume = UnityEngine.Random.Range(0.7f, 1f);

                if (_hWnd != IntPtr.Zero)
                {
                    SetWindowPos(_hWnd, IntPtr.Zero,
                        _ox + shakeX, _oy + shakeY, _ow, _oh,
                        SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
                }

                if (frac > 0.75f)
                {
                    shakeX *= 2;
                    shakeY *= 2;
                    if (_hWnd != IntPtr.Zero)
                        SetWindowPos(_hWnd, IntPtr.Zero,
                            _ox + shakeX, _oy + shakeY, _ow, _oh,
                            SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
                }

                yield return null;
            }

            if (_hWnd != IntPtr.Zero)
            {
                SetAlpha(255);
                SetWindowPos(_hWnd, IntPtr.Zero, _ox, _oy, _ow, _oh,
                    SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
            }
            Application.Quit();
        }

        
        
        
        
        internal class FakeSalesman_WanderState : NpcState
        {
            public FakeSalesman_WanderState(NPC npc) : base(npc) { }
            public override void Enter()
            {
                base.Enter();
                npc.Navigator.SetSpeed(WanderSpeed);
                npc.Navigator.maxSpeed = ChaseSpeed;
                try { ChangeNavigationState(new NavigationState_WanderRandom(npc, 0)); } catch { }
            }
            public override void Update()
            {
                var pm = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
                if (pm == null) return;
                if (IsPlayerHidden(pm)) return; 
                
                if (npc.looker != null && npc.looker.PlayerInSight(pm))
                {
                    npc.behaviorStateMachine.ChangeState(new FakeSalesman_ChaseState(npc));
                }
            }
            public override void DestinationEmpty()
            {
                
                try { ChangeNavigationState(new NavigationState_WanderRandom(npc, 0)); } catch { }
            }
        }

        
        
        
        
        internal class FakeSalesman_ChaseState : NpcState
        {
            private FakeBlackSalesman _fbs;
            private float _noSightTimer = 0f;   
            private float _chaseClock = 0f;     
            public FakeSalesman_ChaseState(NPC npc) : base(npc)
            {
                _fbs = (FakeBlackSalesman)npc;
            }
            public override void Enter()
            {
                base.Enter();
                npc.Navigator.SetSpeed(ChaseSpeed);
                npc.Navigator.maxSpeed = ChaseSpeed;
                _noSightTimer = 0f;
                _chaseClock = 0f;
                
                var pm = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
                if (pm != null && !IsPlayerHidden(pm) && npc.looker != null && npc.looker.PlayerInSight(pm))
                {
                    try { ChangeNavigationState(new NavigationState_TargetPlayer(npc, 63, pm.transform.position)); } catch { }
                }
                else
                {
                    
                    if (!PursueNextSound())
                        npc.behaviorStateMachine.ChangeState(new FakeSalesman_WanderState(npc));
                }
            }
            public override void Update()
            {
                var pm = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
                if (pm == null) return;

                
                if (IsPlayerHidden(pm))
                {
                    _fbs.ClearSoundMemory();
                    npc.behaviorStateMachine.ChangeState(new FakeSalesman_WanderState(npc));
                    return;
                }

                _chaseClock += Time.deltaTime;
                bool seen = npc.looker != null && npc.looker.PlayerInSight(pm);

                if (seen)
                {
                    
                    _noSightTimer = 0f;
                    _fbs.ClearSoundMemory();
                    try
                    {
                        if (!(npc.behaviorStateMachine.CurrentNavigationState is NavigationState_TargetPlayer))
                            ChangeNavigationState(new NavigationState_TargetPlayer(npc, 63, pm.transform.position));
                        else
                            npc.behaviorStateMachine.CurrentNavigationState.UpdatePosition(pm.transform.position);
                    }
                    catch { }
                    return;
                }

                
                _noSightTimer += Time.deltaTime;
                if (_noSightTimer >= NoSightAdvanceTime)
                {
                    _noSightTimer = 0f;
                    if (!PursueNextSound())
                        npc.behaviorStateMachine.ChangeState(new FakeSalesman_WanderState(npc));
                }

                
                if (_chaseClock >= ChaseDuration && !_fbs.HasSoundMemory())
                {
                    npc.behaviorStateMachine.ChangeState(new FakeSalesman_WanderState(npc));
                }
            }
            public override void DestinationEmpty()
            {
                
                if (!PursueNextSound())
                    npc.behaviorStateMachine.ChangeState(new FakeSalesman_WanderState(npc));
            }

            
            
            public bool PursueNextSound()
            {
                try
                {
                    for (int i = 127; i >= 0; i--)
                    {
                        if (_fbs._soundLocations[i] != Vector3.zero)
                        {
                            Vector3 target = _fbs._soundLocations[i];
                            _fbs._soundLocations[i] = Vector3.zero;
                            _fbs._currentSoundVal = i;
                            if (i == 127)
                                ChangeNavigationState(new NavigationState_TargetPlayer(npc, 63, target));
                            else
                                ChangeNavigationState(new NavigationState_TargetPosition(npc, 0, target));
                            return true;
                        }
                    }
                }
                catch (System.Exception) { }
                _fbs._currentSoundVal = 0;
                return false;
            }
        }

        
        
        
        private static bool IsPlayerHidden(PlayerManager pm)
        {
            try { if (pm.Invisible) return true; } catch { }
            try { if (pm.plm != null && pm.plm.Entity != null && pm.plm.Entity.Hidden) return true; } catch { }
            
            try
            {
                var locker = PatchLockerTrack.ActiveLocker;
                if (locker != null)
                {
                    if (locker.playerInside) return true;
                    
                    if (locker.gameObject == null)
                        PatchLockerTrack.ActiveLocker = null;
                }
            }
            catch { PatchLockerTrack.ActiveLocker = null; }
            return false;
        }
    }

    
    
    [HarmonyPatch(typeof(HideableLocker), "Clicked")]
    public static class PatchLockerTrack
    {
        public static HideableLocker ActiveLocker; 

        static void Postfix(HideableLocker __instance, int player)
        {
            if (player == 0)
                ActiveLocker = __instance;
        }
    }
}
