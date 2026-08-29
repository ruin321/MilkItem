using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using HarmonyLib;
using TMPro;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using MTM101BaldAPI;
using MTM101BaldAPI.UI;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusLevelStudio.Editor.Tools;
using PlusStudioLevelFormat;
using PlusStudioLevelLoader;
using CharissasVeryHelpfulHelper;
using UnityEngine;
using UnityEngine.UI;

namespace MilkItem
{
    [BepInPlugin("com.milk.item", "Milk - Clear All Effects", "1.0.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    
    
    
    [BepInDependency("mtm101.rulerp.baldiplus.levelstudioloader")]
    [BepInDependency("mtm101.rulerp.baldiplus.levelstudio")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        public static ManualLogSource Log;   
        
        public static void SilentLog(string message)
        {
            try
            {
                if (MilkSettings.LogOutput != null && MilkSettings.LogOutput.Value)
                    Log?.LogInfo(message);
            }
            catch (System.Exception) { }
        }
        public AssetManager assetMan = new AssetManager();
        public static ItemObject MilkItemObject;
        public static ItemObject EmptyBucketItemObject;
        public static ItemObject ChocolateMilkItemObject;
        public static ItemObject MilkSodaItemObject;
        public static ItemObject DietMilkSodaItemObject;
        public static ItemObject CompressedMilkItemObject;
        public static ItemObject PoisonMilkItemObject;
        public static ItemObject AppleMilkItemObject;
        public static ItemObject ReverseMilkItemObject;
        public static ItemObject MiItemObject;
        public static ItemObject LkItemObject;
        public static ItemObject RottenMilkItemObject;
        public static ItemObject RandomMilkItemObject;
        
        public static ItemObject RandomMilkNoItemItemObject;
        
        public static ItemObject RandomMilk75ItemObject;
        public static ItemObject LostBilkItemObject;
        public static ItemObject MilkYtpsItemObject;
        public static ItemObject WindowMilkItemObject;       
        public static ItemObject NineNineMilkItemObject;      
        public static ItemObject QuarterMilkItemObject;       
        public static ItemObject BusPassMilkItemObject;       
        public static ItemObject SilentMilkItemObject;        
        public static ItemObject MooMilkItemObject;           
        public static ItemObject IceMilkItemObject;            
        public static ItemObject TimeMilkItemObject;           
        public static ItemObject FakeMilkItemObject;           
        public static SoundObject DrinkSound;
        public static SoundObject YtpPickupSound;

        
        
        
        public static ItemObject KeyItemObject = null;                 
        public static bool keySpawnedThisRun = false;                  
        public static bool keySpawnAttemptDone = false;               
        public static WeightedRoomAsset Loaded99Room = null;            
        public static RoomController Last99Room = null;                 
        public static bool key99SpawnedThisRun = false;                
        public static bool nineNineTriggeredThisRun = false;          
        public static bool nineNineDoorUnlockedByPlayer = false;      

        
        
        
        
        public static SceneObject MooSceneObject = null;   
        public static bool MooArmed = false;               
        public static bool MooEntryTriggered = false;      
        public static int MooPhase = 0;                    
        public static bool MooPh1Started = false;          
        public static bool MooPh2Started = false;          
        public static bool MooF1Active = false;            
        public static bool MooCreditsPending = false;      
        public static bool F1RestartTriggered = false;     
        public const string MOO_REG_KEY = @"SOFTWARE\RUIN321GAMES\MODS\MOOMILK"; 
        public const string MOO_REG_VALUE = "99";

        
        
        
        
        public const float RedWhiteTotalSeconds = 900f;      
        public static bool MooRedWhiteActive = false;       
        public static float sodaDrinkNoRuleBreakUntil = 0f; 
        public static int MooRedWhiteFloor = 0;             
        public static bool MooRedWhiteFloorReady = false;   
        public static float MooRedWhiteCountdown = 0f;      
        public static bool MooRedWhiteFailed = false;       
        public static int MooNotebookSlot = 0;              

        
        
        
        
        
        
        
        public static Sticker BilkSticker;
        public static Sticker BaldishhSticker;
        
        public static AudioManager SilencedBaldiAudMan = null;
        public static Sticker PolishCowSticker;
        public static Sticker AngryPolishCowSticker;
        public static Sticker MilkBonusSticker;
        
        public static int MilkBonusDrinks = 0;
        public static bool MilkBonusPaid = false;
        public static bool StickersReady = false;
        
        public static Structure_SpawnPolishCows PolishCowSpawnStructure = null;
        
        public static Structure_SpawnStampedeCows AngryPolishCowSpawnStructure = null;

        
        
        public static RandomEventType StampedeEventType = EnumExtensions.ExtendEnum<RandomEventType>("MilkStampede");
        public static NPC StampedeCowPrefab = null;                 
        public static MilkStampedeEvent StampedeEventTemplate = null; 
        public static NPC MilkSalesmanPrefab = null;                
        public static NPC BalloonCowPrefab = null;                  
        public static NPC FakeBlackSalesmanPrefab = null;            

        
        public static RandomEventType MilkFloodEventType = EnumExtensions.ExtendEnum<RandomEventType>("MilkFlood");
        public static MilkFloodEvent MilkFloodEventTemplate = null; 

        
        
        public static float DistanceToNearestCow(EnvironmentController ec, Vector3 pos, float maxDist = 40f)
        {
            try
            {
                if (ec == null || ec.Npcs == null) return maxDist;
                float nearest = maxDist;
                foreach (NPC n in ec.Npcs)
                {
                    if (n == null || !(n is PolishCow)) continue;
                    if (n.transform == null) continue;
                    float d = Vector3.Distance(n.transform.position, pos);
                    if (d < nearest) nearest = d;
                }
                return nearest;
            }
            catch (System.Exception)
            {
                return maxDist;
            }
        }

        
        public static LevelObject FactoryLevelObject = null;     
        public static SceneObject FactorySceneObject = null;    
        public static LevelType MilkFactory = EnumExtensions.ExtendEnum<LevelType>("MilkFactory"); 
        
        
        
        public static int factoryReplaceLevelNo = -1;  
        public static int ranchReplaceLevelNo = -1;    
        
        
        public static bool factoryPlanRolled = false;

        
        public static Texture2D RanchGrassTex = null;   
        public static Texture2D RanchFenceTex = null;   
        public static Texture2D RanchEdgeTex = null;    
        public static bool activeRanchReskin = false;   

        
        
        public static void RollReplacementPlan()
        {
            factoryReplaceLevelNo = -1;
            ranchReplaceLevelNo = -1; 
            
            if (UnityEngine.Random.value < 0.3f) factoryReplaceLevelNo = UnityEngine.Random.Range(1, 5); 
            
        }

        
        
        
        public static bool IsF2MilkFactoryFloor()
        {
            try
            {
                var cgm = Singleton<CoreGameManager>.Instance;
                if (cgm == null || cgm.sceneObject == null) return false;
                int levelNo = cgm.sceneObject.levelNo;
                if (levelNo != 1) return false; 
                if (factoryReplaceLevelNo == levelNo) return true;
                var theme = GetActualTheme(cgm.sceneObject);
                if (theme == null) return false;
                int t = (int)(object)theme.type;
                return t == (int)(object)Plugin.MilkFactory       
                    || t == (int)(object)LevelType.Factory;       
            }
            catch (System.Exception) { return false; }
        }

        
        
        
        
        public static LevelObject GetActualTheme(SceneObject so)
        {
            try
            {
                if (so == null) return null;
                if (so.randomizedLevelObject != null && so.randomizedLevelObject.Length != 0)
                    return GameInitializer.GetControlledRandomLevelData(so);
                return so.levelObject;
            }
            catch (System.Exception) { return null; }
        }

        
        
        public static bool IsFactoryFloor(int levelNo)
        {
            try
            {
                if (factoryReplaceLevelNo == levelNo) return true;
                var cgm = Singleton<CoreGameManager>.Instance;
                if (cgm != null && cgm.sceneObject != null)
                {
                    var theme = GetActualTheme(cgm.sceneObject);
                    if (theme != null && (int)(object)theme.type == (int)(object)Plugin.MilkFactory)
                    {
                        return true; 
                    }
                }
                return false;
            }
            catch (System.Exception) { return false; }
        }

        
        public static bool IsRanchFloor(int levelNo)
        {
            return ranchReplaceLevelNo == levelNo;
        }

        
        public static bool RanchReady()
        {
            return RanchGrassTex != null || RanchFenceTex != null || RanchEdgeTex != null;
        }

        
        
        public static void RegisterRanchTextures()
        {
            try
            {
                RanchGrassTex = AssetLoader.TextureFromMod(Instance, "Grass.png");
                RanchFenceTex = AssetLoader.TextureFromMod(Instance, "fence.png");
                RanchEdgeTex = AssetLoader.TextureFromMod(Instance, "EdgeTexture.png");
                SilentLog("[Ranch] Loaded ranch textures: Grass" + (RanchGrassTex == null ? " MISSING" : " ok")
                    + " / Fence" + (RanchFenceTex == null ? " MISSING" : " ok")
                    + " / EdgeTexture" + (RanchEdgeTex == null ? " MISSING" : " ok"));
            }
            catch (System.Exception ex)
            {
                Log?.LogError("[Ranch] Failed to load ranch textures: " + ex.Message);
            }
        }

        
        
        
        
        
        
        
        
        
        
        private static SceneObject GetReferenceSceneObject()
        {
            try
            {
                var scenes = MTM101BaldiDevAPI.gameLoader.list.scenes;
                if (scenes == null) return null;
                SceneObject fallback = null;
                foreach (var s in scenes)
                {
                    if (s == null || s.manager == null) continue;
                    if (fallback == null) fallback = s;
                    if (s.manager.GetType().Name.Contains("Endless")) return s;
                }
                return fallback;
            }
            catch (System.Exception ) {  return null; }
        }

        
        
        private static RoomGroup[] BuildMilkRoomGroups(WeightedRoomAsset[] halls, int tMin, int tMax)
        {
            if (halls == null || halls.Length == 0) return null;
            int n = halls.Length;
            int maxPer = System.Math.Min(n, 6);                 
            int numGroups = (int)System.Math.Ceiling((double)tMax / (double)maxPer);
            if (numGroups < 2) numGroups = 2;
            if (numGroups > 4) numGroups = 4;
            
            int baseMin = tMin / numGroups, remMin = tMin - baseMin * numGroups;
            int baseMax = tMax / numGroups, remMax = tMax - baseMax * numGroups;
            var groups = new System.Collections.Generic.List<RoomGroup>();
            for (int i = 0; i < numGroups; i++)
            {
                RoomGroup g = new RoomGroup();
                g.name = "MilkRooms_" + i;
                g.potentialRooms = halls;
                int gmin = baseMin + (i < remMin ? 1 : 0);
                int gmax = baseMax + (i < remMax ? 1 : 0);
                if (gmax > maxPer) gmax = maxPer;
                if (gmin > gmax) gmin = gmax;
                if (gmin < 1) gmin = 1;
                g.minRooms = gmin;
                g.maxRooms = gmax;
                groups.Add(g);
            }
            return groups.ToArray();
        }

        public static void LoadFactoryLevelAsset()
        {
            try
            {
                if (FactorySceneObject != null) return;
                
                Plugin.LoadMilkRoomsFromFiles();
                if (Plugin.LoadedMilkRooms.Count == 0)
                {
                    
                    return;
                }
                WeightedRoomAsset[] milkHalls = Plugin.LoadedMilkRooms.ToArray();

                
                
                LevelObject lvl = ScriptableObject.CreateInstance<LevelObject>();
                lvl.name = "MilkFactory_Lvl";
                lvl.type = MilkFactory;
                
                
                
                lvl.roomGroup = BuildMilkRoomGroups(milkHalls, 11, 15);
                if (lvl.roomGroup == null || lvl.roomGroup.Length == 0)
                {
                    RoomGroup group = new RoomGroup();
                    group.name = "MilkRooms";
                    group.potentialRooms = milkHalls;
                    int n = (milkHalls != null ? milkHalls.Length : 0);
                    group.minRooms = System.Math.Min(4, n);
                    group.maxRooms = System.Math.Min(6, n);
                    lvl.roomGroup = new RoomGroup[] { group };
                    
                }
                else
                {
                    int gmin = 0, gmax = 0;
                    foreach (var rg in lvl.roomGroup) { if (rg != null) { gmin += rg.minRooms; gmax += rg.maxRooms; } }
                    
                }
                
                try
                {
                    var aliases = LevelLoaderPlugin.Instance.roomTextureAliases;
                    Texture2D mtW = null, mtF = null, mtC = null;
                    aliases.TryGetValue("MilkRoom_Wall", out mtW);
                    aliases.TryGetValue("MilkRoom_Floor", out mtF);
                    aliases.TryGetValue("MilkRoom_Ceiling", out mtC);
                    if (mtW != null) { WeightedTexture2D wt = new WeightedTexture2D(); wt.selection = mtW; wt.weight = 100; lvl.hallWallTexs = new WeightedTexture2D[] { wt }; }
                    if (mtF != null) { WeightedTexture2D ft = new WeightedTexture2D(); ft.selection = mtF; ft.weight = 100; lvl.hallFloorTexs = new WeightedTexture2D[] { ft }; }
                    if (mtC != null) { WeightedTexture2D ct = new WeightedTexture2D(); ct.selection = mtC; ct.weight = 100; lvl.hallCeilingTexs = new WeightedTexture2D[] { ct }; }
                }
                catch (System.Exception ) {  }
                FactoryLevelObject = lvl;
                

                
                
                
                SceneObject template = GetReferenceSceneObject();
                SceneObject so;
                if (template != null)
                {
                    so = UnityEngine.Object.Instantiate<SceneObject>(template); 
                    
                }
                else
                {
                    
                    
                    
                    
                    
                    return;
                }
                so.name = "MilkFactory";
                so.levelTitle = "Milk Factory";
                so.levelObject = lvl;
                so.randomizedLevelObject = new WeightedLevelObject[0]; 
                so.levelContainer = null;
                so.levelAsset = null;
                so.levelNo = 0;
                so.MarkAsNeverUnload();
                FactorySceneObject = so;
                

                
                
                
            }
            catch (System.Exception ) {  }
        }

        
        
        
        
        
        private static RoomGroup[] _schoolRoomGroupsCache = null;
        public static RoomGroup[] GetSchoolRoomGroups()
        {
            if (_schoolRoomGroupsCache != null) return _schoolRoomGroupsCache;
            try
            {
                var scenes = MTM101BaldiDevAPI.gameLoader.list.scenes;
                if (scenes != null)
                {
                    foreach (var so in scenes)
                    {
                        if (so != null && so.levelObject != null
                            && (int)(object)so.levelObject.type == (int)(object)LevelType.Schoolhouse)
                        {
                            _schoolRoomGroupsCache = so.levelObject.roomGroup;
                            return _schoolRoomGroupsCache;
                        }
                    }
                }
            }
            catch (System.Exception ) {  }
            return null;
        }

        
        
        
        public static System.Collections.Generic.List<RoomGroup> NormalizeFactoryRoomCount(System.Collections.Generic.List<RoomGroup> src, int targetMin, int targetMax)
        {
            if (src == null) return null;
            var groups = new System.Collections.Generic.List<RoomGroup>();
            foreach (var rg in src)
            {
                if (rg == null) { groups.Add(null); continue; }
                RoomGroup c = new RoomGroup();
                var rgt = rg.GetType();
                foreach (var fld in rgt.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    try { fld.SetValue(c, fld.GetValue(rg)); } catch (System.Exception) { }
                }
                groups.Add(c);
            }
            int gcount = 0; foreach (var rg in groups) if (rg != null) gcount++;
            
            foreach (var rg in groups)
            {
                if (rg == null) continue;
                
            }
            int totalMin = 0, totalMax = 0;
            foreach (var rg in groups) { if (rg == null) continue; totalMin += rg.minRooms; totalMax += rg.maxRooms; }
            

            int deficit = targetMin - totalMin;
            if (deficit > 0)
            {
                int i = 0, guard = 0;
                while (deficit > 0 && guard < 9999)
                {
                    var rg = groups[i % groups.Count];
                    if (rg != null)
                    {
                        int capR = (rg.potentialRooms != null) ? rg.potentialRooms.Length : 9999;
                        if (rg.minRooms < capR) { rg.minRooms++; if (rg.maxRooms < rg.minRooms) rg.maxRooms = rg.minRooms; deficit--; }
                    }
                    i++; guard++;
                }
            }
            totalMax = 0; foreach (var rg in groups) if (rg != null) totalMax += rg.maxRooms;
            int mdef = targetMax - totalMax;
            if (mdef > 0)
            {
                int i = 0, guard = 0;
                while (mdef > 0 && guard < 9999)
                {
                    var rg = groups[i % groups.Count];
                    if (rg != null)
                    {
                        int capR = (rg.potentialRooms != null) ? rg.potentialRooms.Length : 9999;
                        if (rg.maxRooms < capR) { rg.maxRooms++; mdef--; }
                    }
                    i++; guard++;
                }
            }
            totalMax = 0; foreach (var rg in groups) if (rg != null) totalMax += rg.maxRooms;
            if (totalMax > targetMax)
            {
                int over = totalMax - targetMax, guard = 0;
                while (over > 0 && guard < 9999)
                {
                    RoomGroup hi = null;
                    foreach (var rg in groups) { if (rg == null) continue; if (hi == null || rg.maxRooms > hi.maxRooms) hi = rg; }
                    if (hi != null && hi.maxRooms > hi.minRooms) { hi.maxRooms--; over--; } else break;
                    guard++;
                }
            }
            totalMin = 0; foreach (var rg in groups) if (rg != null) totalMin += rg.minRooms;
            if (totalMin > targetMax)
            {
                int over = totalMin - targetMax, guard = 0;
                while (over > 0 && guard < 9999)
                {
                    RoomGroup hi = null;
                    foreach (var rg in groups) { if (rg == null) continue; if (hi == null || rg.minRooms > hi.minRooms) hi = rg; }
                    if (hi != null && hi.minRooms > 0) { hi.minRooms--; if (hi.maxRooms < hi.minRooms) hi.maxRooms = hi.minRooms; over--; } else break;
                    guard++;
                }
            }

            totalMin = 0; totalMax = 0;
            foreach (var rg in groups) { if (rg == null) continue; totalMin += rg.minRooms; totalMax += rg.maxRooms; }
            
            return groups;
        }

        
        
        
        
        public static System.Collections.Generic.List<RoomGroup> ShallowReduceRoomGroupPressure(System.Collections.Generic.List<RoomGroup> src, int maxTotal = 10)
        {
            if (src == null) return null;
            var groups = new System.Collections.Generic.List<RoomGroup>();
            bool anyRooms = false;
            foreach (var rg in src)
            {
                if (rg == null) { groups.Add(null); continue; }
                RoomGroup c = new RoomGroup();
                var rgt = rg.GetType();
                foreach (var fld in rgt.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    try { fld.SetValue(c, fld.GetValue(rg)); } catch (System.Exception) { }
                }
                
                int capR = (c.potentialRooms != null) ? c.potentialRooms.Length : 9999;
                if (capR > 0) anyRooms = true;
                if (c.maxRooms > capR) c.maxRooms = System.Math.Max(1, capR);
                if (c.minRooms > c.maxRooms) c.minRooms = c.maxRooms;
                if (c.minRooms < 0) c.minRooms = 0;
                groups.Add(c);
            }

            
            
            
            
            
            
            
            
            var classIdx = new System.Collections.Generic.List<int>();
            for (int i = 0; i < groups.Count; i++)
            {
                var rg = groups[i];
                if (rg == null) continue;
                string nm = rg.name != null ? rg.name.ToLowerInvariant() : "";
                if (nm.Contains("class") || nm.Contains("lesson") || nm.Contains("教室"))
                {
                    if (rg.potentialRooms != null && rg.potentialRooms.Length > 0) classIdx.Add(i);
                }
            }
            int totalMin = 0;
            if (classIdx.Count > 0)
            {
                
                
                int perGroup = maxTotal / classIdx.Count;
                if (perGroup < 2) perGroup = 2;
                foreach (int ci in classIdx)
                {
                    var rg = groups[ci];
                    int cap = rg.potentialRooms.Length;
                    int v = (perGroup > cap) ? cap : perGroup;
                    rg.minRooms = 1;
                    rg.maxRooms = v;
                    totalMin += 1;
                    if (totalMin > maxTotal) { rg.minRooms = 0; rg.maxRooms = 0; totalMin -= 1; }
                }
            }
            
            bool anyNonClassKept = false;
            for (int i = 0; i < groups.Count; i++)
            {
                if (classIdx.Contains(i)) continue;
                var rg = groups[i];
                if (rg == null) continue;
                rg.minRooms = 0;
                rg.maxRooms = (anyRooms && !anyNonClassKept && totalMin < maxTotal) ? 1 : 0;
                if (rg.maxRooms > 0) anyNonClassKept = true;
            }
            
            bool anyRoomKept = false;
            foreach (var rg in groups) if (rg != null && rg.maxRooms > 0) { anyRoomKept = true; break; }
            if (!anyRoomKept)
            {
                foreach (var rg in groups)
                {
                    if (rg != null && rg.potentialRooms != null && rg.potentialRooms.Length > 0)
                    { rg.minRooms = 1; rg.maxRooms = 1; anyRoomKept = true; break; }
                }
            }

            
            return groups;
        }

        
        public static LevelObject GetSchoolLevelObject()
        {
            try
            {
                var scenes = MTM101BaldiDevAPI.gameLoader.list.scenes;
                if (scenes != null)
                {
                    foreach (var so in scenes)
                    {
                        if (so != null && so.levelObject != null
                            && (int)(object)so.levelObject.type == (int)(object)LevelType.Schoolhouse)
                            return so.levelObject;
                    }
                }
            }
            catch (System.Exception ) {  }
            return null;
        }

        
        
        
        
        
        
        
        
        
        private static WeightedRoomAsset[] _bilkClassroomAssets = null;
        public static WeightedRoomAsset[] GetBilkClassroomAssets()
        {
            if (_bilkClassroomAssets != null) return _bilkClassroomAssets;
            _bilkClassroomAssets = new WeightedRoomAsset[0];
            try
            {
                
                LoadMathRoomsFromFiles();
                if (LoadedMathRooms.Count > 0)
                {
                    var list = new System.Collections.Generic.List<WeightedRoomAsset>();
                    foreach (WeightedRoomAsset wra in LoadedMathRooms)
                    {
                        if (wra == null || wra.selection == null) continue;
                        RoomAsset clone = UnityEngine.Object.Instantiate(wra.selection);
                        clone.name = "BILK_QuizRoom_" + wra.selection.name;
                        clone.category = MilkMachineClassroomCategory;
                        clone.type = RoomType.Room;
                        
                        if (!clone.hasActivity && QuizMachinePrefabInstance != null)
                        {
                            clone.hasActivity = true;
                            if (clone.activity == null) clone.activity = new ActivityData();
                            clone.activity.prefab = QuizMachinePrefabInstance;
                            clone.activity.position = Vector3.zero;
                            clone.activity.direction = Direction.North;
                        }
                        WeightedRoomAsset w = new WeightedRoomAsset();
                        w.selection = clone;
                        w.weight = 100;
                        list.Add(w);
                    }
                    if (list.Count > 0)
                    {
                        _bilkClassroomAssets = list.ToArray();
                        
                        return _bilkClassroomAssets;
                    }
                }

                
                LoadMilkRoomsFromFiles();
                if (LoadedMilkRooms.Count == 0)
                {
                    
                    return _bilkClassroomAssets;
                }
                var fallbackList = new System.Collections.Generic.List<WeightedRoomAsset>();
                foreach (WeightedRoomAsset wra in LoadedMilkRooms)
                {
                    if (wra == null || wra.selection == null) continue;
                    RoomAsset clone = UnityEngine.Object.Instantiate(wra.selection);
                    clone.name = "BILK_MilkClassroom_" + wra.selection.name;
                    clone.category = MilkMachineClassroomCategory;
                    clone.type = RoomType.Room;
                    if (QuizMachinePrefabInstance != null)
                    {
                        clone.hasActivity = true;
                        if (clone.activity == null) clone.activity = new ActivityData();
                        clone.activity.prefab = QuizMachinePrefabInstance;
                        clone.activity.position = Vector3.zero;
                        clone.activity.direction = Direction.North;
                    }
                    WeightedRoomAsset w = new WeightedRoomAsset();
                    w.selection = clone;
                    w.weight = 100;
                    fallbackList.Add(w);
                }
                _bilkClassroomAssets = fallbackList.ToArray();
                
            }
            catch (System.Exception )
            {
                
            }
            return _bilkClassroomAssets;
        }

        
        
        
        private static System.Collections.Generic.List<StructureWithParameters> _factoryStructCache = null;
        public static System.Collections.Generic.List<StructureWithParameters> GetBeltAndSteamStructures()
        {
            if (_factoryStructCache != null) return _factoryStructCache;
            _factoryStructCache = new System.Collections.Generic.List<StructureWithParameters>();
            try
            {
                var scenes = MTM101BaldiDevAPI.gameLoader.list.scenes;
                if (scenes == null) return _factoryStructCache;
                bool foundBelt = false, foundSteam = false;
                foreach (var so in scenes)
                {
                    if (so == null || so.levelObject == null) continue;
                    LevelObject lo = so.levelObject;
                    if (lo.potentialStructures != null)
                    {
                        foreach (var ws in lo.potentialStructures)
                        {
                            if (ws == null || ws.selection == null || ws.selection.prefab == null) continue;
                            if (!foundBelt && ws.selection.prefab is Structure_ConveyorBelt)
                            {
                                _factoryStructCache.Add(ws.selection);
                                foundBelt = true;
                            }
                            else if (!foundSteam && ws.selection.prefab is Structure_SteamValves)
                            {
                                _factoryStructCache.Add(ws.selection);
                                foundSteam = true;
                            }
                        }
                    }
                    if (lo.forcedStructures != null)
                    {
                        foreach (var s in lo.forcedStructures)
                        {
                            if (s == null || s.prefab == null) continue;
                            if (!foundBelt && s.prefab is Structure_ConveyorBelt)
                            {
                                _factoryStructCache.Add(s);
                                foundBelt = true;
                            }
                            else if (!foundSteam && s.prefab is Structure_SteamValves)
                            {
                                _factoryStructCache.Add(s);
                                foundSteam = true;
                            }
                        }
                    }
                    if (foundBelt && foundSteam) break;
                }
                
            }
            catch (System.Exception ) {  }
            return _factoryStructCache;
        }

        
        
        
        
        private static RoomAsset _beltRoomAssetCache = null;
        private static bool _beltRoomAssetSearched = false;
        public static RoomAsset GetBeltRoomAsset()
        {
            if (_beltRoomAssetSearched) return _beltRoomAssetCache;
            _beltRoomAssetSearched = true;
            _beltRoomAssetCache = null;
            try
            {
                var scenes = MTM101BaldiDevAPI.gameLoader.list.scenes;
                if (scenes == null) return null;
                foreach (var so in scenes)
                {
                    if (so == null || so.levelObject == null) continue;
                    LevelObject lo = so.levelObject;
                    if (lo.roomGroup != null)
                    {
                        foreach (var rg in lo.roomGroup)
                        {
                            if (rg == null || rg.potentialRooms == null) continue;
                            foreach (var w in rg.potentialRooms)
                            {
                                if (w != null && w.selection != null && w.selection.roomFunction is BeltRoomFunction)
                                { _beltRoomAssetCache = w.selection; break; }
                            }
                            if (_beltRoomAssetCache != null) break;
                        }
                    }
                    if (_beltRoomAssetCache == null && lo.potentialSpecialRooms != null)
                    {
                        foreach (var w in lo.potentialSpecialRooms)
                        {
                            if (w != null && w.selection != null && w.selection.roomFunction is BeltRoomFunction)
                            { _beltRoomAssetCache = w.selection; break; }
                        }
                    }
                    if (_beltRoomAssetCache != null) break;
                }
                
            }
            catch (System.Exception ) {  }
            return _beltRoomAssetCache;
        }

        
        
        
        private static System.Collections.Generic.List<StructureWithParameters> _extraStructCache = null;
        public static System.Collections.Generic.List<StructureWithParameters> GetVentDoorRotoStructures()
        {
            if (_extraStructCache != null) return _extraStructCache;
            _extraStructCache = new System.Collections.Generic.List<StructureWithParameters>();
            try
            {
                bool foundVent = false, foundDoor = false, foundRoto = false;
                var scenes = MTM101BaldiDevAPI.gameLoader.list.scenes;
                if (scenes == null) return _extraStructCache;
                foreach (var so in scenes)
                {
                    if (so == null || so.levelObject == null) continue;
                    LevelObject lo = so.levelObject;
                    if (lo.potentialStructures != null)
                    {
                        foreach (var ws in lo.potentialStructures)
                        {
                            if (ws == null || ws.selection == null || ws.selection.prefab == null) continue;
                            if (!foundVent && ws.selection.prefab is Structure_Vent) { _extraStructCache.Add(ws.selection); foundVent = true; }
                            else if (!foundDoor && ws.selection.prefab is Structure_HallDoor) { _extraStructCache.Add(ws.selection); foundDoor = true; }
                            else if (!foundRoto && ws.selection.prefab is Structure_Rotohalls) { _extraStructCache.Add(ws.selection); foundRoto = true; }
                        }
                    }
                    if (lo.forcedStructures != null)
                    {
                        foreach (var s in lo.forcedStructures)
                        {
                            if (s == null || s.prefab == null) continue;
                            if (!foundVent && s.prefab is Structure_Vent) { _extraStructCache.Add(s); foundVent = true; }
                            else if (!foundDoor && s.prefab is Structure_HallDoor) { _extraStructCache.Add(s); foundDoor = true; }
                            else if (!foundRoto && s.prefab is Structure_Rotohalls) { _extraStructCache.Add(s); foundRoto = true; }
                        }
                    }
                    if (foundVent && foundDoor && foundRoto) break;
                }
                
            }
            catch (System.Exception ) {  }
            return _extraStructCache;
        }

        
        public static StructureWithParameters CloneStructure(StructureWithParameters src)
        {
            StructureWithParameters c = new StructureWithParameters();
            c.prefab = src.prefab;
            StructureParameters sp = new StructureParameters();
            if (src.parameters != null)
            {
                sp.chance = src.parameters.chance != null ? (float[])src.parameters.chance.Clone() : new float[0];
                sp.minMax = src.parameters.minMax != null ? (IntVector2[])src.parameters.minMax.Clone() : new IntVector2[0];
                sp.prefab = src.parameters.prefab != null ? (WeightedGameObject[])src.parameters.prefab.Clone() : new WeightedGameObject[0];
            }
            c.parameters = sp;
            return c;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public static void EnsurePowerLeverCategories(Structure_PowerLever pl)
        {
            try
            {
                if (pl == null) return;
                var fPower = typeof(Structure_PowerLever).GetField("poweredRoomCategories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var poweredList = (System.Collections.Generic.List<RoomCategory>)fPower.GetValue(pl);
                if (poweredList == null) { poweredList = new System.Collections.Generic.List<RoomCategory>(); fPower.SetValue(pl, poweredList); }
                foreach (RoomCategory cat in new RoomCategory[]
                {
                    RoomCategory.Class, RoomCategory.Office, RoomCategory.Faculty,
                    RoomCategory.Hall, RoomCategory.Store, RoomCategory.Special, RoomCategory.Test
                })
                {
                    if (!poweredList.Contains(cat)) poweredList.Add(cat);
                }
                if (Plugin.MilkMachineClassroomCategoryReady && !poweredList.Contains(Plugin.MilkMachineClassroomCategory))
                    poweredList.Add(Plugin.MilkMachineClassroomCategory);

                var fLever = typeof(Structure_PowerLever).GetField("leverRoomCategories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var leverList = (System.Collections.Generic.List<RoomCategory>)fLever.GetValue(pl);
                if (leverList == null) { leverList = new System.Collections.Generic.List<RoomCategory>(); fLever.SetValue(pl, leverList); }
                if (!leverList.Contains(RoomCategory.Hall)) leverList.Add(RoomCategory.Hall);

                var fBreaker = typeof(Structure_PowerLever).GetField("breakerRoomCategories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var breakerList = (System.Collections.Generic.List<RoomCategory>)fBreaker.GetValue(pl);
                if (breakerList == null) { breakerList = new System.Collections.Generic.List<RoomCategory>(); fBreaker.SetValue(pl, breakerList); }
                
                
                
                breakerList.Clear();
                breakerList.Add(RoomCategory.Class);
                if (Plugin.MilkMachineClassroomCategoryReady && !breakerList.Contains(Plugin.MilkMachineClassroomCategory))
                    breakerList.Add(Plugin.MilkMachineClassroomCategory);
            }
            catch (System.Exception )
            {
                
            }
        }

        private static StructureWithParameters _powerLeverStructCache = null;
        private static bool _powerLeverSearched = false;
        public static StructureWithParameters GetPowerLeverStructure()
        {
            if (_powerLeverSearched) return _powerLeverStructCache;
            _powerLeverSearched = true;
            try
            {
                
                var scenes = MTM101BaldiDevAPI.gameLoader.list.scenes;
                if (scenes != null)
                {
                    foreach (var so in scenes)
                    {
                        if (so == null || so.levelObject == null) continue;
                        LevelObject lo = so.levelObject;
                        if (lo.potentialStructures != null)
                        {
                            foreach (var ws in lo.potentialStructures)
                            {
                                if (ws != null && ws.selection != null && ws.selection.prefab is Structure_PowerLever)
                                {
                                    _powerLeverStructCache = CloneStructure(ws.selection);
                                    EnsurePowerLeverCategories(_powerLeverStructCache.prefab as Structure_PowerLever);
                                    
                                    return _powerLeverStructCache;
                                }
                            }
                        }
                        if (lo.forcedStructures != null)
                        {
                            foreach (var s in lo.forcedStructures)
                            {
                                if (s != null && s.prefab is Structure_PowerLever)
                                {
                                    _powerLeverStructCache = CloneStructure(s);
                                    EnsurePowerLeverCategories(_powerLeverStructCache.prefab as Structure_PowerLever);
                                    
                                    return _powerLeverStructCache;
                                }
                            }
                        }
                    }
                }

                
                
                
                Structure_PowerLever[] found = UnityEngine.Resources.FindObjectsOfTypeAll<Structure_PowerLever>();
                if (found != null && found.Length > 0)
                {
                    Structure_PowerLever pl = found[0];
                    
                    
                    EnsurePowerLeverCategories(pl);

                    StructureWithParameters swp = new StructureWithParameters();
                    swp.prefab = pl;
                    StructureParameters sp = new StructureParameters();
                    
                    
                    sp.minMax = new IntVector2[]
                    {
                        new IntVector2(1, 2),
                        new IntVector2(1, 1),
                        new IntVector2(3, 4),
                        new IntVector2(50, 60)
                    };
                    swp.parameters = sp;
                    _powerLeverStructCache = swp;
                    
                    return swp;
                }

                
            }
            catch (System.Exception ) {  }
            return null;
        }

        
        
        
        public static bool LevelHasPowerLever(LevelGenerationParameters lgp)
        {
            if (lgp.forcedStructures != null)
            {
                foreach (var s in lgp.forcedStructures)
                    if (s != null && s.prefab is Structure_PowerLever) return true;
            }
            return false;
        }

        
        
        
        public static void RemovePowerLeverFromPotential(LevelGenerationParameters lgp)
        {
            try
            {
                if (lgp.potentialStructures == null || lgp.potentialStructures.Length == 0) return;
                var kept = new System.Collections.Generic.List<WeightedStructureWithParameters>();
                int removed = 0;
                foreach (var ws in lgp.potentialStructures)
                {
                    if (ws != null && ws.selection != null && ws.selection.prefab is Structure_PowerLever) { removed++; continue; }
                    kept.Add(ws);
                }
                if (removed > 0)
                {
                    lgp.potentialStructures = kept.ToArray();
                    
                }
            }
            catch (System.Exception ) {  }
        }

        
        
        
        
        
        
        private static void TryPatchPineDebug(Harmony harmony)
        {
            try
            {
                System.Reflection.Assembly pdAsm = null;
                foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (a.GetName().Name == "PineDebug") { pdAsm = a; break; }
                }
                if (pdAsm == null) {  return; }
                System.Type pdType = pdAsm.GetType("PineDebug.PineDebugManager");
                if (pdType == null) {  return; }
                
                System.Reflection.MethodInfo initUi = pdType.GetMethod("InitUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (initUi != null)
                {
                    var postfix = new HarmonyMethod(typeof(Plugin).GetMethod(nameof(PineDebugInitUIPostfix), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
                    harmony.Patch(initUi, postfix: postfix);
                }
                
                
                System.Reflection.MethodInfo updateM = pdType.GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (updateM != null)
                {
                    var upostfix = new HarmonyMethod(typeof(Plugin).GetMethod(nameof(PineDebugUpdatePostfix), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
                    harmony.Patch(updateM, postfix: upostfix);
                }
                
                try
                {
                    var inst = UnityEngine.Object.FindObjectOfType(pdType);
                    if (inst != null) PineDebugInitUIPostfix(inst);
                }
                catch { }
            }
            catch (System.Exception ) {  }
        }

        
        private static void PineDebugUpdatePostfix(object __instance)
        {
            PineDebugInitUIPostfix(__instance);
        }

        
        
        
        private static readonly object pineDebugLock = new object();
        private static bool pineDebugButtonsInjected = false;
        private static void PineDebugInitUIPostfix(object __instance)
        {
            if (pineDebugButtonsInjected) return;
            try
            {
                if (pineDebugButtonsInjected) return;
                lock (pineDebugLock)
                {
                    if (pineDebugButtonsInjected) return;
                    PineDebugInjectLocked(__instance);
                }
                pineDebugButtonsInjected = true; 
            }
            catch (System.Exception ) {  }
        }

        
        private static void PineDebugInjectLocked(object __instance)
        {
            try
            {
                
                if (Plugin.FactorySceneObject == null)
                {
                    Plugin.LoadFactoryLevelAsset();
                    if (Plugin.FactorySceneObject == null) {  return; }
                }
                System.Type pdType = __instance.GetType();
                System.Reflection.Assembly pdAsm = pdType.Assembly;
                System.Reflection.FieldInfo sceneButtonsField = pdType.GetField("sceneButtons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (sceneButtonsField == null) {  return; }
                object sceneButtons = sceneButtonsField.GetValue(__instance);
                if (sceneButtons == null) {  return; }

                
                try
                {
                    var existing = sceneButtons.GetType().GetMethod("GetEnumerator");
                    if (existing != null)
                    {
                        var en = existing.Invoke(sceneButtons, null) as System.Collections.IEnumerator;
                        while (en != null && en.MoveNext())
                        {
                            if (en.Current != null && ((System.Object)en.Current).GetType().Name == "PineButton")
                            {
                                var btnName = en.Current.GetType().GetField("button")?.GetValue(en.Current);
                                
                                break;
                            }
                        }
                    }
                }
                catch { }

                System.Type pbType = pdAsm.GetType("PineDebug.PineDebugManager+PineButton");
                System.Type dataCtorType = pdAsm.GetType("PineDebug.PineDebugManager+PineButtonSceneObjectData");
                System.Type extraDataType = pdAsm.GetType("PineDebug.PineDebugManager+PineButtonExtraData");
                if (pbType == null || dataCtorType == null || extraDataType == null) {  return; }

                object data = null;
                var dataCtor = dataCtorType.GetConstructor(new System.Type[] { typeof(SceneObject), typeof(string) });
                if (dataCtor != null) data = dataCtor.Invoke(new object[] { Plugin.FactorySceneObject, "" });

                Texture2D transparent = null;
                foreach (var t in Resources.FindObjectsOfTypeAll<Texture2D>())
                {
                    if (t.name == "Transparent") { transparent = t; break; }
                }

                System.Reflection.MethodInfo createButton = pdType.GetMethod("CreateButton",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null,
                    new System.Type[] { typeof(string), typeof(UnityEngine.Events.UnityAction), typeof(Texture2D), extraDataType }, null);
                if (createButton == null) {  return; }

                UnityEngine.Events.UnityAction action = () => LoadMilkFactoryFromPineDebug();
                object button = createButton.Invoke(null, new object[] { "MilkFactory", action, transparent, data });
                if (button == null) {  return; }

                System.Reflection.MethodInfo insertText = pdType.GetMethod("InsertTextToButton", new System.Type[] { pbType, typeof(string) });
                if (insertText != null) button = insertText.Invoke(__instance, new object[] { button, Plugin.FactorySceneObject.levelTitle });

                
                System.Action<object, object> insertIntoList = (listObj, btn) =>
                {
                    var addM = listObj.GetType().GetMethod("Add", new System.Type[] { pbType });
                    if (addM != null) addM.Invoke(listObj, new object[] { btn });
                };

                
                object eventButtons = null;
                var eventButtonsField = pdType.GetField("eventButtons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (eventButtonsField != null) eventButtons = eventButtonsField.GetValue(__instance);

                
                
                if (eventButtons != null) insertIntoList(eventButtons, button);
                else insertIntoList(sceneButtons, button);

                
                try
                {
                    object stampedeData = null;
                    var extraCtor = extraDataType.GetConstructor(System.Type.EmptyTypes);
                    if (extraCtor != null) stampedeData = extraCtor.Invoke(null);
                    UnityEngine.Events.UnityAction stampedeAction = () => TriggerStampedeFromPineDebug();
                    object stampedeBtn = createButton.Invoke(null, new object[] { "MilkStampede", stampedeAction, transparent, stampedeData });
                    if (stampedeBtn != null)
                    {
                        if (insertText != null) stampedeBtn = insertText.Invoke(__instance, new object[] { stampedeBtn, "MilkStampede!" });
                        if (eventButtons != null) insertIntoList(eventButtons, stampedeBtn);
                        
                    }
                }
                catch (System.Exception ) {  }

                
                try
                {
                    object floodData = null;
                    var floodExtraCtor = extraDataType.GetConstructor(System.Type.EmptyTypes);
                    if (floodExtraCtor != null) floodData = floodExtraCtor.Invoke(null);
                    UnityEngine.Events.UnityAction floodAction = () => TriggerMilkFloodFromPineDebug();
                    object floodBtn = createButton.Invoke(null, new object[] { "MilkFlood", floodAction, transparent, floodData });
                    if (floodBtn != null)
                    {
                        if (insertText != null) floodBtn = insertText.Invoke(__instance, new object[] { floodBtn, "MilkFlood!" });
                        if (eventButtons != null) insertIntoList(eventButtons, floodBtn);
                        
                    }
                }
                catch (System.Exception ) {  }

                
                
                try
                {
                    object tpData = null;
                    var tpExtraCtor = extraDataType.GetConstructor(System.Type.EmptyTypes);
                    if (tpExtraCtor != null) tpData = tpExtraCtor.Invoke(null);
                    UnityEngine.Events.UnityAction tpAction = () => TeleportTo99RoomFromPineDebug();
                    object tpBtn = createButton.Invoke(null, new object[] { "Teleport99Room", tpAction, transparent, tpData });
                    if (tpBtn != null)
                    {
                        if (insertText != null) tpBtn = insertText.Invoke(__instance, new object[] { tpBtn, "Teleport to 99 Room" });
                        
                        if (eventButtons != null) insertIntoList(eventButtons, tpBtn);
                        else insertIntoList(sceneButtons, tpBtn);
                        
                    }
                }
                catch (System.Exception ) {  }
            }
            catch (System.Exception ) {  }
        }

        
        
        
        private static void LoadMilkFactoryFromPineDebug()
        {
            try
            {
                var cgm = Singleton<CoreGameManager>.Instance;
                var bgm = Singleton<BaseGameManager>.Instance;
                if (cgm == null || bgm == null || Plugin.FactorySceneObject == null) return;
                Singleton<GlobalCam>.Instance.Transition((UiTransition)0, 0.01666667f);
                bgm.StopAllCoroutines();
                bgm.Ec.ResetEvents();
                Time.timeScale = 0f;
                cgm.readyToStart = false;
                cgm.disablePause = true;
                PropagatedAudioManager.paused = true;
                var elevatorScreenField = AccessTools.DeclaredField(typeof(BaseGameManager), "elevatorScreen");
                var elevatorScreenPreField = AccessTools.DeclaredField(typeof(BaseGameManager), "elevatorScreenPre");
                var prepareToLoad = AccessTools.Method(typeof(BaseGameManager), "PrepareToLoad", null, null);
                var es = UnityEngine.Object.Instantiate((ElevatorScreen)elevatorScreenPreField.GetValue(bgm));
                elevatorScreenField.SetValue(bgm, es);
                es.OnLoadReady += () =>
                {
                    prepareToLoad.Invoke(bgm, System.Array.Empty<object>());
                    cgm.PrepareForReload();
                    cgm.SetLives(2, true);
                    cgm.tripPlayed = false;
                    Singleton<SubtitleManager>.Instance.DestroyAll();
                    cgm.sceneObject = Plugin.FactorySceneObject;
                    Singleton<AdditiveSceneManager>.Instance.LoadScene("Game");
                };
                es.Initialize();
                
            }
            catch (System.Exception ) {  }
        }

        
        
        private static void TriggerStampedeFromPineDebug()
        {
            try
            {
                var bgm = Singleton<BaseGameManager>.Instance;
                if (bgm == null || bgm.Ec == null) {  return; }
                if (Plugin.StampedeEventTemplate == null) {  return; }
                EnvironmentController ec = bgm.Ec;
                MilkStampedeEvent evt = UnityEngine.Object.Instantiate(Plugin.StampedeEventTemplate, ec.transform);
                evt.Initialize(ec, new System.Random(System.Environment.TickCount));
                evt.Begin();
                
            }
            catch (System.Exception ) {  }
        }

        
        
        public static void PlayBaldiSpeech(string audioRelativePath, string soundKey)
        {
            try
            {
                var ec = Singleton<BaseGameManager>.Instance?.Ec;
                if (ec == null) return;
                Baldi b = ec.GetBaldi();
                if (b == null) return;
                AudioClip clip = AssetLoader.AudioClipFromMod(Instance, audioRelativePath);
                if (clip == null) return;
                var f = AccessTools.DeclaredField(typeof(Baldi), "audMan");
                if (f == null) return;
                AudioManager audMan = f.GetValue(b) as AudioManager;
                if (audMan == null) return;
                SoundObject so = ObjectCreators.CreateSoundObject(clip, soundKey, SoundType.Voice, Color.green, clip.length);
                so.subtitle = true; 
                audMan.QueueAudio(so);
            }
            catch (System.Exception ) {  }
        }

        
        
        
        public static SoundObject MakeEventIntroSound(string audioRelativePath, string soundKey)
        {
            try
            {
                AudioClip clip = AssetLoader.AudioClipFromMod(Instance, audioRelativePath);
                if (clip == null) return null;
                SoundObject so = ObjectCreators.CreateSoundObject(clip, soundKey, SoundType.Voice, Color.green, clip.length);
                so.subtitle = true; 
                return so;
            }
            catch (System.Exception ) { return null; }
        }

        
        private static void TriggerMilkFloodFromPineDebug()
        {
            try
            {
                var bgm = Singleton<BaseGameManager>.Instance;
                if (bgm == null || bgm.Ec == null) {  return; }
                if (Plugin.MilkFloodEventTemplate == null) {  return; }
                EnvironmentController ec = bgm.Ec;
                MilkFloodEvent evt = UnityEngine.Object.Instantiate(Plugin.MilkFloodEventTemplate, ec.transform);
                evt.Initialize(ec, new System.Random(System.Environment.TickCount));
                evt.SetEventTime(new System.Random(System.Environment.TickCount));
                evt.Begin();
            }
            catch (System.Exception ) {  }
        }

        
        
        private static void TeleportTo99RoomFromPineDebug()
        {
            try
            {
                var bgm = Singleton<BaseGameManager>.Instance;
                if (bgm == null || bgm.Ec == null) {  return; }
                if (Plugin.Last99Room == null) {  return; }
                RoomController room = Plugin.Last99Room;
                if (room.cells == null || room.cells.Count == 0) {  return; }
                var center = room.cells[room.cells.Count / 2];
                Vector3 dest = center.FloorWorldPosition; 
                var pm = Singleton<CoreGameManager>.Instance.GetPlayer(0);
                if (pm == null) {  return; }
                pm.transform.position = dest;
                
            }
            catch (System.Exception ) {  }
        }

        
        
        
        private void InjectMilkEventsToFloor(string floorName, int floorNumber, SceneObject sceneObject)
        {
            try
            {
                Plugin.SilentLog($"[Event] Addend callback hit: floor={floorName}({floorNumber}) scene={(sceneObject != null ? sceneObject.name : "null")} stampedeTpl={(StampedeEventTemplate != null)} floodTpl={(MilkFloodEventTemplate != null)}");
                if (sceneObject == null) return;
                if (StampedeEventTemplate == null && MilkFloodEventTemplate == null) return;
                CustomLevelObject[] customLevels = sceneObject.GetCustomLevelObjects();
                if (customLevels == null || customLevels.Length == 0) return;
                foreach (CustomLevelObject cl in customLevels)
                {
                    if (cl == null || cl.IsModifiedByMod(Info)) continue;
                    if (StampedeEventTemplate != null)
                    {
                        bool has = false;
                        foreach (var we in cl.randomEvents) if (we != null && we.selection == StampedeEventTemplate) { has = true; break; }
                        if (!has)
                        {
                            WeightedRandomEvent ev = new WeightedRandomEvent();
                            ev.selection = StampedeEventTemplate;
                            ev.weight = 70;
                            cl.randomEvents.Add(ev);
                        }
                    }
                    if (MilkFloodEventTemplate != null)
                    {
                        bool has = false;
                        foreach (var we in cl.randomEvents) if (we != null && we.selection == MilkFloodEventTemplate) { has = true; break; }
                        if (!has)
                        {
                            WeightedRandomEvent ev = new WeightedRandomEvent();
                            ev.selection = MilkFloodEventTemplate;
                            ev.weight = 70;
                            cl.randomEvents.Add(ev);
                        }
                    }
                    cl.MarkAsModifiedByMod(Info);
                    Plugin.SilentLog($"[Event] InjectMilkEventsToFloor added to {cl.name}: randomEvents={cl.randomEvents.Count} stampedeTpl={(StampedeEventTemplate != null)} floodTpl={(MilkFloodEventTemplate != null)}");
                }
            }
            catch (System.Exception ) {  }
        }

        
        
        
        

        
        
        internal static void PlayMilkDrinkSound()
        {
            if (DrinkSound == null) return;
            var cgm = Singleton<CoreGameManager>.Instance;
            if (cgm != null && cgm.audMan != null)
                cgm.audMan.PlaySingle(DrinkSound);
        }

        
        
        
        
        internal static void StopMilkRandomEvents()
        {
            try
            {
                foreach (var evt in UnityEngine.Object.FindObjectsOfType<MilkFloodEvent>())
                    if (evt != null && evt.IsActive) evt.End();
                foreach (var evt in UnityEngine.Object.FindObjectsOfType<MilkStampedeEvent>())
                    if (evt != null && IsEventActive(evt)) evt.End();
            }
            catch (System.Exception) { }
        }

        
        
        
        
        
        internal static void RemoveNpcFromEnvironment(EnvironmentController ec, NPC npc)
        {
            try
            {
                if (ec == null || npc == null) return;
                System.Type colNpc = typeof(ICollection<NPC>);
                System.Type colEnt = typeof(ICollection<Entity>);
                var fields = typeof(EnvironmentController).GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                foreach (var f in fields)
                {
                    if (f.IsLiteral || f.IsInitOnly) continue;
                    object val = null;
                    try { val = f.GetValue(ec); } catch { continue; }
                    if (val == null) continue;
                    try
                    {
                        if (colNpc.IsAssignableFrom(val.GetType()))
                        {
                            colNpc.GetMethod("Remove").Invoke(val, new object[] { npc });
                        }
                        else if (colEnt.IsAssignableFrom(val.GetType()))
                        {
                            colEnt.GetMethod("Remove").Invoke(val, new object[] { npc });
                        }
                    }
                    catch { }
                }
                
                foreach (var f in fields)
                {
                    if (f.FieldType != typeof(NPC)) continue;
                    object cur = null;
                    try { cur = f.GetValue(ec); } catch { continue; }
                    if (object.ReferenceEquals(cur, npc)) { try { f.SetValue(ec, null); } catch { } }
                }
            }
            catch (System.Exception) { }
        }

        
        
        
        
        
        
        
        internal static void RemoveSelfFromEcActiveEvents(RandomEvent self)
        {
            try
            {
                if (self == null) return;
                EnvironmentController ec = (EnvironmentController)typeof(RandomEvent).GetField("ec",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(self);
                if (ec == null) return;
                System.Reflection.FieldInfo[] fields = typeof(EnvironmentController).GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                System.Type targetCol = typeof(ICollection<RandomEvent>);
                foreach (var f in fields)
                {
                    if (f.IsLiteral || f.IsInitOnly) continue;
                    object val = null;
                    try { val = f.GetValue(ec); } catch { continue; }
                    if (val == null) continue;
                    if (targetCol.IsAssignableFrom(val.GetType()))
                    {
                        try { targetCol.GetMethod("Remove").Invoke(val, new object[] { self }); } catch { }
                    }
                }
                
                foreach (var f in fields)
                {
                    if (f.FieldType != typeof(RandomEvent)) continue;
                    object cur = null;
                    try { cur = f.GetValue(ec); } catch { continue; }
                    if (object.ReferenceEquals(cur, self))
                    {
                        try { f.SetValue(ec, null); } catch { }
                    }
                }
            }
            catch (System.Exception) { }
        }

        
        private static bool IsEventActive(RandomEvent evt)
        {
            try
            {
                if (evt == null) return false;
                var f = typeof(RandomEvent).GetField("active", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return f != null && (bool)f.GetValue(evt);
            }
            catch (System.Exception) { return false; }
        }

        
        
        internal static bool ConsumeMilkToEmptyBucket(PlayerManager player, ItemObject selfObj)
        {
            if (player == null || player.itm == null || EmptyBucketItemObject == null || selfObj == null)
                return true;
            int slot = player.itm.selectedItem;
            if (slot < 0 || slot > player.itm.maxItem) return true;
            if (player.itm.items[slot] == selfObj)
            {
                try { player.itm.SetItem(EmptyBucketItemObject, slot); return false; }
                catch (System.Exception )
                {
                    
                    return true;
                }
            }
            return true; 
        }
        
        
        internal static readonly System.Collections.Generic.HashSet<Pickup> _randomMilkPickups = new System.Collections.Generic.HashSet<Pickup>();

        
        public static MilkMachine MilkMachinePrefabInstance;
        
        public static QuizMachine QuizMachinePrefabInstance;

        public static SoundObject AppleMilkSound;

        
        public static Sprite AppleMilkBaldiSprite;    
        public static Sprite AppleMilkBaldiSprite1;   
        public static float AppleMilkAudioLength = 0f;
        
        
        public const float AppleMilkFreezeScale = 1.6f;
        private const string EnumName = "ITM_Milk";
        private const string EmptyBucketEnumName = "ITM_EmptyBucket";
        private const string ChocolateMilkEnumName = "ITM_ChocolateMilk";
        private const string MilkSodaEnumName = "ITM_MilkSoda";
        private const string DietMilkSodaEnumName = "ITM_DietMilkSoda";
        private const string CompressedMilkEnumName = "ITM_CompressedMilk";
        private const string PoisonMilkEnumName = "ITM_PoisonMilk";
        private const string AppleMilkEnumName = "ITM_AppleMilk";
        private const string ReverseMilkEnumName = "ITM_ReverseMilk";
        private const string WindowMilkEnumName = "ITM_WindowMilk";
        private const string NineNineMilkEnumName = "ITM_99Milk";
        private const string QuarterMilkEnumName = "ITM_QuarterMilk";
        private const string BusPassMilkEnumName = "ITM_BusPassMilk";
        private const string MiEnumName = "ITM_Mi";
        private const string LkEnumName = "ITM_Lk";
        private const string RottenMilkEnumName = "ITM_RottenMilk";
        
        
        private const string FakeMilkEnumName = "ITM_FakeMilk";
        
        private const string SilentMilkEnumName = "ITM_SilentMilk";
        
        private const string MooMilkEnumName = "ITM_MooMilk";
        
        private const string IceMilkEnumName = "ITM_IceMilk";
        
        private const string TimeMilkEnumName = "ITM_TimeMilk";
        private const string RandomMilkEnumName = "ITM_RandomMilk";
        
        private const string RandomMilkNoItemEnumName = "ITM_RandomMilkNoItem";
        
        private const string RandomMilk75EnumName = "ITM_RandomMilk75";
        
        
        public const float RandomMilkNoItemAirChance = 0.5f;
        
        public const float RandomMilk75AirChance = 0.75f;
        
        public const float RandomPosterNoShowChance = 0.5f;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            
            try { MilkSettings.Init(Config); } catch (System.Exception) { }

            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilkRoom, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterColdRoom, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, Register99Room, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterHotRoom, LoadingEventOrder.Pre);
            
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilkMachineClassroom, LoadingEventOrder.Pre);
            
            
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilkPosters, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilkVendingMachine, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterEmptyBucket, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterChocolateMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilkSoda, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterDietMilkSoda, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilkSodaVendingMachine, LoadingEventOrder.Pre);
                LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterDietMilkSodaVendingMachine, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterCompressedMilk, LoadingEventOrder.Pre);
            
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterAppleMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterReverseMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterWindowMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, Register99Milk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterSilentMilk, LoadingEventOrder.Pre);
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMooMilk, LoadingEventOrder.Pre);
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterIceMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterTimeMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, delegate { AchievementHelper.RegisterAllAchievements(); }, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterQuarterMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterBusPassMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterWeakMilks, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterRottenMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterFakeMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterRandomMilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterRandomMilkNoItem, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterRandomMilk75, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterLostBilk, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilkYtps, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterRandomMilkVendingMachine, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterKey, LoadingEventOrder.Pre);
            
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilkContent, LoadingEventOrder.Post);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, LoadFactoryLevelAsset, LoadingEventOrder.Post);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterRanchTextures, LoadingEventOrder.Post);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, SetMilkWindowTitle, LoadingEventOrder.Post);
            
            
            if (!(MilkSettings.Remove20sLoading != null && MilkSettings.Remove20sLoading.Value))
            {
                LoadingEvents.RegisterOnAssetsLoaded(Info, TrollLoading(), LoadingEventOrder.Post);
            }
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterStickers, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, InjectStickersToScenePool, LoadingEventOrder.Post);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, InjectMilkToShops, LoadingEventOrder.Post);
            
            
            
            
            
            
            
            
            
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterMilkMainMenu, LoadingEventOrder.Pre);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, InjectChineseViaBBPC, LoadingEventOrder.Post);
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, LoadMooScene, LoadingEventOrder.Post);
            
            
            LoadingEvents.RegisterOnAssetsLoaded(Info, KitchenStoveCompat.Register, LoadingEventOrder.Post);

            
            
            
            
            GeneratorManagement.Register((BaseUnityPlugin)(object)this, GenerationModType.Addend, (Action<string, int, SceneObject>)InjectMilkEventsToFloor);

            
            Harmony harmony = new Harmony(Info.Metadata.GUID);
            
            
            
            
            PatchAllSkipBroken(harmony);

            
            
            
            PatchRCPackCarterHudSafe(harmony);

            
            
            
            PatchStudentSpawnerGuard(harmony);

            
            
            PatchRestoreMapGuard(harmony);

            
            
            
            SetupFastLoaderStallSuppress(harmony);

            
            
            SetupBaldishhCaptionSuppress(harmony);
            

            
            try { MooCreditsPending = MooReadFlag(); }
            catch (System.Exception ) {  }

            
            
            TryPatchPineDebug(harmony);

            
        }

        
        
        
        private static void PatchAllSkipBroken(Harmony harmony)
        {
            int ok = 0, skip = 0;
            try
            {
                foreach (var type in typeof(Plugin).Assembly.GetTypes())
                {
                    object[] attrs = null;
                    try { attrs = type.GetCustomAttributes(typeof(HarmonyPatch), true); } catch { }
                    if (attrs == null || attrs.Length == 0) continue;
                    try
                    {
                        harmony.CreateClassProcessor(type).Patch();
                        ok++;
                    }
                    catch (System.Exception ex)
                    {
                        skip++;
                        Log?.LogWarning("[Harmony] skip broken patch " + type.FullName + ": " + ex.Message);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log?.LogWarning("[Harmony] PatchAllSkipBroken sweep failed: " + ex.Message);
            }
            if (skip > 0) SilentLog("[Harmony] PatchAllSkipBroken: 成功 " + ok + "，跳过失效 " + skip);
        }

        
        
        private static void PatchRCPackCarterHudSafe(Harmony harmony)
        {
            try
            {
                System.Type cart = null;
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    var t = asm.GetType("UncertainLuei.BaldiPlus.RecommendedChars.CarterHudManager");
                    if (t != null) { cart = t; break; }
                }
                if (cart == null) return;
                var bf = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                var aw = cart.GetMethod("Awake", bf) ?? cart.GetMethod("Start", bf);
                if (aw == null) return;
                harmony.Patch(aw, finalizer: new HarmonyMethod(typeof(Plugin).GetMethod(nameof(CarterAwakeGuardFinalizer),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)));
                SilentLog("[RCPack] CarterHudManager.Awake 已挂异常守卫");
            }
            catch (System.Exception) { }
        }

        
        private static void CarterAwakeGuardFinalizer(ref System.Exception __exception)
        {
            if (__exception == null) return;
            if (__exception is System.Collections.Generic.KeyNotFoundException) __exception = null;
        }

        
        
        private static void PatchStudentSpawnerGuard(Harmony harmony)
        {
            try
            {
                
                var st = HarmonyLib.AccessTools.TypeByName("Structure_StudentSpawner");
                if (st == null) return;
                var m = HarmonyLib.AccessTools.Method(st, "SpawnStudents");
                if (m == null) return;
                harmony.Patch(m, finalizer: new HarmonyMethod(typeof(Plugin).GetMethod(nameof(StudentSpawnerGuardFinalizer),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)));
                SilentLog("[StudentSpawner] 异常守卫已挂");
            }
            catch (System.Exception) { }
        }

        private static void StudentSpawnerGuardFinalizer(ref System.Exception __exception)
        {
            if (__exception == null) return;
            if (__exception is NullReferenceException) __exception = null;
        }

        
        
        private static void PatchRestoreMapGuard(Harmony harmony)
        {
            try
            {
                var cgmType = HarmonyLib.AccessTools.TypeByName("CoreGameManager");
                if (cgmType == null) return;
                var m = HarmonyLib.AccessTools.Method(cgmType, "RestoreMap", new[] { typeof(Map), typeof(EnvironmentController) });
                if (m == null) return;
                harmony.Patch(m, finalizer: new HarmonyMethod(typeof(Plugin).GetMethod(nameof(RestoreMapGuardFinalizer),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)));
                SilentLog("[RestoreMap] 越界守卫已挂");
            }
            catch (System.Exception) { }
        }

        private static void RestoreMapGuardFinalizer(ref System.Exception __exception)
        {
            if (__exception == null) return;
            if (__exception is IndexOutOfRangeException) __exception = null;
        }

        
        
        
        private static void SetupFastLoaderStallSuppress(Harmony harmony)
        {
            try
            {
                var t = HarmonyLib.AccessTools.TypeByName("FastLoader.LevelBuilderPatches");
                if (t == null)
                {
                    SilentLog("[FastLoader] 未检测到，跳过 STALL dump 抑制");
                    return;
                }
                System.Reflection.MethodInfo prefix = typeof(Plugin).GetMethod(nameof(FastLoaderStallSuppressPrefix),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                int patched = 0;
                foreach (var mName in new[] { "DumpStallStack", "DumpRoomPosterInfo", "DumpPlotExpansionInfo" })
                {
                    var m = HarmonyLib.AccessTools.Method(t, mName);
                    if (m == null) continue;
                    harmony.Patch(m, prefix: new HarmonyMethod(prefix));
                    patched++;
                }
                SilentLog($"[FastLoader] STALL 诊断 dump 已抑制 ({patched}/3)");
            }
            catch (System.Exception) { }
        }

        private static bool FastLoaderStallSuppressPrefix()
        {
            return false; 
        }

        
        
        
        private static void SetupBaldishhCaptionSuppress(Harmony harmony)
        {
            try
            {
                System.Reflection.MethodInfo prefix = typeof(Plugin).GetMethod(nameof(BaldishhNoCaptionPrefix),
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (prefix == null) return;
                var hm = new HarmonyMethod(prefix);
                foreach (var m in typeof(AudioManager).GetMethods(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    if (m.IsSpecialName) continue;
                    if (m.Name != "PlaySingle" && m.Name != "PlayQueue") continue;
                    try { harmony.Patch(m, prefix: hm); }
                    catch (System.Exception) { }
                }
            }
            catch (System.Exception) { }
        }

        
        private static bool BaldishhNoCaptionPrefix(AudioManager __instance)
        {
            try
            {
                if (!StickersReady) return true;
                if (SilencedBaldiAudMan == null || !object.ReferenceEquals(__instance, SilencedBaldiAudMan)) return true;
                if (Singleton<StickerManager>.Instance != null
                    && Singleton<StickerManager>.Instance.StickerValue(BaldishhSticker) > 0)
                    return false;
            }
            catch (System.Exception) { }
            return true;
        }

        
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetWindowTextW(System.IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        private static extern System.IntPtr GetActiveWindow();

        internal static void SetWindowTitle(string title)
        {
            try
            {
                System.IntPtr hWnd = GetActiveWindow();
                if (hWnd != System.IntPtr.Zero)
                {
                    SetWindowTextW(hWnd, title);
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        private static void SetMilkWindowTitle()
        {
            SetWindowTitle("Baldi's Basics Milk! (Millkk)");
        }

        
        
        
        
        
        private static IEnumerator TrollLoading()
        {
            
            if (MilkSettings.Remove20sLoading != null && MilkSettings.Remove20sLoading.Value)
            {
                yield return 1;
                yield break;
            }
            yield return 20; 
            yield return "Ah nothing, just wanna steal a sec of your time. Wait 20 seconds...";
            for (int s = 19; s >= 1; s--)
            {
                yield return new WaitForSecondsRealtime(1f);
                yield return "Ah nothing, just wanna steal a sec of your time. Wait " + s + (s == 1 ? " second..." : " seconds...");
            }
            yield return new WaitForSecondsRealtime(1f);
            yield return "Ah nothing, just wanna steal a sec of your time. Okay, you may enter now!";
        }

        
        
        
        
        
        
        
        private static void RegisterMilkMainMenu()
        {
            try
            {
                Sprite menuSprite = AssetLoader.SpriteFromMod(
                    Instance, new Vector2(0.5f, 0.5f), 50f, "mainMenu.png");
                if (menuSprite == null)
                {
                    
                    return;
                }

                
                Type menuObjType = Type.GetType("CustomMainMenusAPI.MainMenuObject, CustomMainMenusAPI");
                if (menuObjType == null)
                {
                    
                    return;
                }
                MethodInfo createMethod = menuObjType.GetMethod(
                    "CreateMenuObject",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(string), typeof(Sprite) },
                    null);
                if (createMethod == null)
                {
                    
                    return;
                }

                createMethod.Invoke(null, new object[] { "Ed_MilkMenu", menuSprite });
                MainMenuPatch.UseCustomMenuAPI = true;
                
            }
            catch (System.Exception )
            {
                
            }
        }

        private void RegisterMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(EnumName))
                {
                    
                    MilkItemObject = LevelLoaderPlugin.Instance.itemObjects[EnumName];
                    return;
                }

                
                Sprite milkSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Milk_Small.png");
                Sprite milkLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Milk_Large.png");

                var milkItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription("ITM_Milk", "ITM_Milk_Desc")
                    .SetEnum(EnumName)
                    .SetShopPrice(250)
                    .SetGeneratorCost(50)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(milkSmall, milkLarge)
                    .SetItemComponent<MilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(EnumName, milkItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(EnumName, assetMan.Get<ItemObject>(EnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(EnumName);
                MilkItemObject = milkItem;

                
                try
                {
                    var all = ItemMetaStorage.Instance.All();
                    var found = ItemMetaStorage.Instance.FindByEnum(milkItem.itemType);
                    
                }
                catch (System.Exception )
                {
                    
                }

                
                try
                {
                    AudioClip drinkClip = AssetLoader.AudioClipFromMod(Instance, "Drink.wav");
                    if (drinkClip != null)
                    {
                        DrinkSound = ObjectCreators.CreateSoundObject(
                            drinkClip,
                            "Vfx_Drink",
                            SoundType.Voice,
                            Color.white);
                        assetMan.Add<SoundObject>("MilkDrink", DrinkSound);
                        
                    }
                }
                catch (System.Exception )
                {
                    
                }

                
                try
                {
                    string locPath = Path.Combine(AssetLoader.GetModPath(Instance), "Localization.json");
                    if (File.Exists(locPath))
                    {
                        AssetLoader.LocalizationFromFile(locPath, (Language)0);
                    }
                }
                catch (System.Exception )
                {
                    
                }

                

                
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        
        
        
        private void InjectChineseViaBBPC()
        {
            try
            {
                System.Reflection.Assembly bbpcAsm = null;
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.GetName().Name == "BBPC")
                    {
                        bbpcAsm = asm;
                        break;
                    }
                }
                if (bbpcAsm == null)
                {
                    
                    return;
                }
                System.Type bbpcPlugin = bbpcAsm.GetType("BBPC.Plugin");
                if (bbpcPlugin == null)
                {
                    
                    return;
                }
                var instanceProp = bbpcPlugin.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                object bbpcInstance = instanceProp?.GetValue(null);
                if (bbpcInstance == null)
                {
                    
                    return;
                }
                
                
                
                
                
                
                
                string lang = "";
                try
                {
                    const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance;
                    
                    
                    System.Type cfgMgr = bbpcAsm.GetType("BBPC.API.ConfigManager")
                        ?? bbpcAsm.GetType("BBPC.ConfigManager");
                    
                    object cfgEntry = null;
                    
                    var prop = cfgMgr?.GetProperty("currect_lang", flags);
                    
                    if (prop != null)
                    {
                        cfgEntry = prop.GetValue(null) ?? prop.GetValue(bbpcInstance);
                    }
                    
                    
                    if (cfgEntry == null)
                    {
                        var backing = cfgMgr?.GetField("<currect_lang>k__BackingField", flags);
                        
                        cfgEntry = backing?.GetValue(null) ?? backing?.GetValue(bbpcInstance);
                    }
                    
                    if (cfgEntry == null)
                    {
                        var field = cfgMgr?.GetField("currect_lang", flags);
                        
                        cfgEntry = field?.GetValue(null) ?? field?.GetValue(bbpcInstance);
                    }
                    lang = (string)cfgEntry?.GetType().GetProperty("Value")?.GetValue(cfgEntry) ?? "";
                    
                }
                catch (System.Exception )
                {
                    
                }
                if (lang != "SChinese" && lang != "TChinese")
                {
                    
                    return;
                }
                
                
                string zhPath;
                if (lang == "TChinese")
                {
                    zhPath = Path.Combine(AssetLoader.GetModPath(Instance), "Localization.ChineseTraditional.json");
                }
                else
                {
                    zhPath = Path.Combine(AssetLoader.GetModPath(Instance), "Localization.Chinese.json");
                }
                if (File.Exists(zhPath))
                {
                    AssetLoader.LocalizationFromFile(zhPath, (Language)0);
                    
                }
                else
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterEmptyBucket()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(EmptyBucketEnumName))
                {
                    
                    EmptyBucketItemObject = LevelLoaderPlugin.Instance.itemObjects[EmptyBucketEnumName];
                    return;
                }

                Sprite bucketSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Bucket_Small.png");
                Sprite bucketLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Bucket_Large.png");

                var bucketItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(EmptyBucketEnumName, EmptyBucketEnumName + "_Desc")
                    .SetEnum(EmptyBucketEnumName)
                    .SetShopPrice(0)
                    .SetGeneratorCost(0)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(bucketSmall, bucketLarge)
                    .SetItemComponent<EmptyBucketComponent>()
                    .Build();

                assetMan.Add<ItemObject>(EmptyBucketEnumName, bucketItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(EmptyBucketEnumName, assetMan.Get<ItemObject>(EmptyBucketEnumName));
                EmptyBucketItemObject = bucketItem;

                

                
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterChocolateMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(ChocolateMilkEnumName))
                {
                    
                    ChocolateMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[ChocolateMilkEnumName];
                    return;
                }

                Sprite chocoSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Chocolatemilk_Small.png");
                Sprite chocoLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Chocolatemilk_Large.png");

                
                var chocoItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(ChocolateMilkEnumName, ChocolateMilkEnumName + "_Desc")
                    .SetEnum(ChocolateMilkEnumName)
                    .SetShopPrice(400) 
                    .SetGeneratorCost(80)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(chocoSmall, chocoLarge)
                    .SetItemComponent<MilkComponent>()
                    .Build();

                
                var chocoComponent = chocoItem.item.GetComponent<MilkComponent>();
                if (chocoComponent != null) chocoComponent.Variant = MilkVariant.Chocolate;

                assetMan.Add<ItemObject>(ChocolateMilkEnumName, chocoItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(ChocolateMilkEnumName, assetMan.Get<ItemObject>(ChocolateMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(ChocolateMilkEnumName);
                ChocolateMilkItemObject = chocoItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterMilkSoda()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(MilkSodaEnumName))
                {
                    
                    MilkSodaItemObject = LevelLoaderPlugin.Instance.itemObjects[MilkSodaEnumName];
                    return;
                }

                Sprite sodaSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "MilkSodaIcon_Small.png");
                Sprite sodaLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "MilkSodaIcon_Large.png");

                
                var sodaItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(MilkSodaEnumName, MilkSodaEnumName + "_Desc")
                    .SetEnum(MilkSodaEnumName)
                    .SetShopPrice(400) 
                    .SetGeneratorCost(80)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(sodaSmall, sodaLarge)
                    .SetItemComponent<MilkComponent>()
                    .Build();

                
                var sodaComponent = sodaItem.item.GetComponent<MilkComponent>();
                if (sodaComponent != null) sodaComponent.Variant = MilkVariant.MilkSoda;

                assetMan.Add<ItemObject>(MilkSodaEnumName, sodaItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(MilkSodaEnumName, assetMan.Get<ItemObject>(MilkSodaEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(MilkSodaEnumName);
                MilkSodaItemObject = sodaItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        private void RegisterDietMilkSoda()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(DietMilkSodaEnumName))
                {
                    
                    DietMilkSodaItemObject = LevelLoaderPlugin.Instance.itemObjects[DietMilkSodaEnumName];
                    return;
                }

                Sprite dietSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "DietMilkSodaIcon_Small.png");
                Sprite dietLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "DietMilkSodaIcon_Large.png");

                
                var dietItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(DietMilkSodaEnumName, DietMilkSodaEnumName + "_Desc")
                    .SetEnum(DietMilkSodaEnumName)
                    .SetShopPrice(200) 
                    .SetGeneratorCost(40)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(dietSmall, dietLarge)
                    .SetItemComponent<MilkComponent>()
                    .Build();

                
                var dietComponent = dietItem.item.GetComponent<MilkComponent>();
                if (dietComponent != null) dietComponent.Variant = MilkVariant.DietMilkSoda;

                assetMan.Add<ItemObject>(DietMilkSodaEnumName, dietItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(DietMilkSodaEnumName, assetMan.Get<ItemObject>(DietMilkSodaEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(DietMilkSodaEnumName);
                DietMilkSodaItemObject = dietItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        private void RegisterMilkRoom()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.roomSettings.ContainsKey("milk_room"))
                {
                    
                    return;
                }

                
                RoomCategory milkRoomCat = EnumExtensions.ExtendEnum<RoomCategory>("MilkRoom");
                MilkRoomCategory = milkRoomCat;
                MilkRoomCategoryReady = true;
                

                
                Texture2D doorClosed = AssetLoader.TextureFromMod(Instance, "MilkDoor_Closed.png");
                Texture2D doorOpened = AssetLoader.TextureFromMod(Instance, "MilkDoor_Opened.png");
                StandardDoorMats doorMats = ObjectCreators.CreateDoorDataObject("MilkDoorMats", doorOpened, doorClosed);

                
                Texture2D wallTex = AssetLoader.TextureFromMod(Instance, "MilkRoom_Wall.png");
                Texture2D floorTex = AssetLoader.TextureFromMod(Instance, "MilkRoom_Floor.png");
                Texture2D ceilingTex = AssetLoader.TextureFromMod(Instance, "MilkRoom_Ceiling.png");

                
                RoomSettings settings = new RoomSettings(milkRoomCat, (RoomType)2, Color.white, doorMats);
                LevelLoaderPlugin.Instance.roomSettings.Add("milk_room", settings);

                
                LevelStudioPlugin.Instance.defaultRoomTextures.Add("milk_room",
                    new TextureContainer("MilkRoom_Floor", "MilkRoom_Wall", "MilkRoom_Ceiling"));

                
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("MilkRoom_Wall", wallTex);
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("MilkRoom_Floor", floorTex);
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("MilkRoom_Ceiling", ceilingTex);

                
                LevelStudioPlugin.Instance.selectableTextures.Add("MilkRoom_Wall");
                LevelStudioPlugin.Instance.selectableTextures.Add("MilkRoom_Floor");
                LevelStudioPlugin.Instance.selectableTextures.Add("MilkRoom_Ceiling");

                

                
                try
                {
                    Sprite roomIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "MilkRoom_Icon.png");
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "rooms", new RoomTool("milk_room", roomIcon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        
        
        public static RoomCategory MilkRoomCategory;
        public static bool MilkRoomCategoryReady = false;
        public static RoomCategory ColdRoomCategory;
        public static bool ColdRoomCategoryReady = false;

        
        public static RoomCategory MilkMachineClassroomCategory;
        public static bool MilkMachineClassroomCategoryReady = false;
        private void RegisterColdRoom()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.roomSettings.ContainsKey("cold_room"))
                {
                    
                    return;
                }

                
                RoomCategory coldRoomCat = EnumExtensions.ExtendEnum<RoomCategory>("ColdRoom");
                ColdRoomCategory = coldRoomCat;
                ColdRoomCategoryReady = true;
                

                
                Texture2D doorClosed = AssetLoader.TextureFromMod(Instance, "ColdDoor_Closed.png");
                Texture2D doorOpened = AssetLoader.TextureFromMod(Instance, "ColdDoor_Opened.png");
                StandardDoorMats doorMats = ObjectCreators.CreateDoorDataObject("ColdRoomDoorMats", doorOpened, doorClosed);

                
                Texture2D wallTex = AssetLoader.TextureFromMod(Instance, "ColdRoom_Wall.png");
                Texture2D floorTex = AssetLoader.TextureFromMod(Instance, "ColdRoom_Floor.png");
                Texture2D ceilingTex = AssetLoader.TextureFromMod(Instance, "ColdRoom_Ceiling.png");

                
                RoomSettings settings = new RoomSettings(coldRoomCat, (RoomType)2, Color.white, doorMats);
                LevelLoaderPlugin.Instance.roomSettings.Add("cold_room", settings);

                
                LevelStudioPlugin.Instance.defaultRoomTextures.Add("cold_room",
                    new TextureContainer("ColdRoom_Floor", "ColdRoom_Wall", "ColdRoom_Ceiling"));

                
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("ColdRoom_Wall", wallTex);
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("ColdRoom_Floor", floorTex);
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("ColdRoom_Ceiling", ceilingTex);

                
                LevelStudioPlugin.Instance.selectableTextures.Add("ColdRoom_Wall");
                LevelStudioPlugin.Instance.selectableTextures.Add("ColdRoom_Floor");
                LevelStudioPlugin.Instance.selectableTextures.Add("ColdRoom_Ceiling");

                

                
                try
                {
                    
                    Sprite roomIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Coldroom_Icon.png");
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "rooms", new RoomTool("cold_room", roomIcon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        public static RoomCategory Room99Category;
        public static bool Room99CategoryReady = false;

        private void Register99Room()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.roomSettings.ContainsKey("99room"))
                {
                    
                    return;
                }

                
                RoomCategory room99Cat = EnumExtensions.ExtendEnum<RoomCategory>("Room99");
                Room99Category = room99Cat;
                Room99CategoryReady = true;
                

                
                Texture2D doorClosed = AssetLoader.TextureFromMod(Instance, "99Door_Closed.png");
                Texture2D doorOpened = AssetLoader.TextureFromMod(Instance, "99Door_Opened.png");
                StandardDoorMats doorMats = ObjectCreators.CreateDoorDataObject("Room99DoorMats", doorOpened, doorClosed);

                
                Texture2D wallTex = AssetLoader.TextureFromMod(Instance, "99_Wall.png");
                Texture2D floorTex = AssetLoader.TextureFromMod(Instance, "99_Floor.png");
                Texture2D ceilingTex = AssetLoader.TextureFromMod(Instance, "99_Ceiling.png");

                
                RoomSettings settings = new RoomSettings(room99Cat, (RoomType)2, Color.white, doorMats);
                LevelLoaderPlugin.Instance.roomSettings.Add("99room", settings);

                
                LevelStudioPlugin.Instance.defaultRoomTextures.Add("99room",
                    new TextureContainer("99room_Floor", "99room_Wall", "99room_Ceiling"));

                
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("99room_Wall", wallTex);
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("99room_Floor", floorTex);
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("99room_Ceiling", ceilingTex);

                
                LevelStudioPlugin.Instance.selectableTextures.Add("99room_Wall");
                LevelStudioPlugin.Instance.selectableTextures.Add("99room_Floor");
                LevelStudioPlugin.Instance.selectableTextures.Add("99room_Ceiling");

                

                
                try
                {
                    Sprite roomIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "99Room_Icon.png");
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "rooms", new RoomTool("99room", roomIcon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        public static RoomCategory HotRoomCategory;
        public static bool HotRoomCategoryReady = false;

        private void RegisterHotRoom()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.roomSettings.ContainsKey("hot_room"))
                {
                    
                    return;
                }

                
                RoomCategory hotRoomCat = EnumExtensions.ExtendEnum<RoomCategory>("HotRoom");
                HotRoomCategory = hotRoomCat;
                HotRoomCategoryReady = true;
                

                
                Texture2D doorClosed = AssetLoader.TextureFromMod(Instance, "HotDoor_Closed.png");
                Texture2D doorOpened = AssetLoader.TextureFromMod(Instance, "HotDoor_Opened.png");
                StandardDoorMats doorMats = ObjectCreators.CreateDoorDataObject("HotRoomDoorMats", doorOpened, doorClosed);

                
                Texture2D wallTex = AssetLoader.TextureFromMod(Instance, "HotRoom_Wall.png");
                Texture2D floorTex = AssetLoader.TextureFromMod(Instance, "HotRoom_Floor.png");
                Texture2D ceilingTex = AssetLoader.TextureFromMod(Instance, "HotRoom_Ceiling.png");

                
                RoomSettings settings = new RoomSettings(hotRoomCat, (RoomType)2, Color.white, doorMats);
                LevelLoaderPlugin.Instance.roomSettings.Add("hot_room", settings);

                
                LevelStudioPlugin.Instance.defaultRoomTextures.Add("hot_room",
                    new TextureContainer("HotRoom_Floor", "HotRoom_Wall", "HotRoom_Ceiling"));

                
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("HotRoom_Wall", wallTex);
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("HotRoom_Floor", floorTex);
                LevelLoaderPlugin.Instance.roomTextureAliases.Add("HotRoom_Ceiling", ceilingTex);

                
                LevelStudioPlugin.Instance.selectableTextures.Add("HotRoom_Wall");
                LevelStudioPlugin.Instance.selectableTextures.Add("HotRoom_Floor");
                LevelStudioPlugin.Instance.selectableTextures.Add("HotRoom_Ceiling");

                

                
                try
                {
                    Sprite roomIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "HotRoom_Icon.png");
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "rooms", new RoomTool("hot_room", roomIcon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        
        
        private void RegisterMilkMachineClassroom()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.roomSettings.ContainsKey("milk_machine_classroom"))
                {
                    
                    return;
                }

                
                RoomCategory cat = EnumExtensions.ExtendEnum<RoomCategory>("MilkMachineClassroom");
                MilkMachineClassroomCategory = cat;
                MilkMachineClassroomCategoryReady = true;
                

                
                Texture2D doorClosed = AssetLoader.TextureFromMod(Instance, "MilkDoor_Closed.png");
                Texture2D doorOpened = AssetLoader.TextureFromMod(Instance, "MilkDoor_Opened.png");
                StandardDoorMats doorMats = ObjectCreators.CreateDoorDataObject("MilkClassroomDoorMats", doorOpened, doorClosed);

                
                Texture2D wallTex = AssetLoader.TextureFromMod(Instance, "MilkRoom_Wall.png");
                Texture2D floorTex = AssetLoader.TextureFromMod(Instance, "MilkRoom_Floor.png");
                Texture2D ceilingTex = AssetLoader.TextureFromMod(Instance, "MilkRoom_Ceiling.png");

                
                RoomSettings settings = new RoomSettings(cat, (RoomType)2, Color.white, doorMats);
                LevelLoaderPlugin.Instance.roomSettings.Add("milk_machine_classroom", settings);

                
                LevelStudioPlugin.Instance.defaultRoomTextures.Add("milk_machine_classroom",
                    new TextureContainer("MilkRoom_Floor", "MilkRoom_Wall", "MilkRoom_Ceiling"));

                
                if (!LevelLoaderPlugin.Instance.roomTextureAliases.ContainsKey("MilkRoom_Wall"))
                    LevelLoaderPlugin.Instance.roomTextureAliases.Add("MilkRoom_Wall", wallTex);
                if (!LevelLoaderPlugin.Instance.roomTextureAliases.ContainsKey("MilkRoom_Floor"))
                    LevelLoaderPlugin.Instance.roomTextureAliases.Add("MilkRoom_Floor", floorTex);
                if (!LevelLoaderPlugin.Instance.roomTextureAliases.ContainsKey("MilkRoom_Ceiling"))
                    LevelLoaderPlugin.Instance.roomTextureAliases.Add("MilkRoom_Ceiling", ceilingTex);

                
                if (!LevelStudioPlugin.Instance.selectableTextures.Contains("MilkRoom_Wall"))
                    LevelStudioPlugin.Instance.selectableTextures.Add("MilkRoom_Wall");
                if (!LevelStudioPlugin.Instance.selectableTextures.Contains("MilkRoom_Floor"))
                    LevelStudioPlugin.Instance.selectableTextures.Add("MilkRoom_Floor");
                if (!LevelStudioPlugin.Instance.selectableTextures.Contains("MilkRoom_Ceiling"))
                    LevelStudioPlugin.Instance.selectableTextures.Add("MilkRoom_Ceiling");

                

                
                try
                {
                    Sprite roomIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "MilkRoom_Icon.png");
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "rooms", new RoomTool("milk_machine_classroom", roomIcon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        
        private static Sprite cachedHallBucket = null;

        public static void SpawnMilkHallDecor(RoomController room)
        {
            try
            {
                if (room == null || room.ec == null || room.objectObject == null) return;

                
                if (cachedHallBucket == null)
                {
                    cachedHallBucket = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Bucket_Large.png");
                    if (cachedHallBucket == null)
                    {
                        
                        return;
                    }
                }
                Sprite bucket = cachedHallBucket;

                
                var candidates = new System.Collections.Generic.List<Cell>();
                if (room.entitySafeCells != null && room.entitySafeCells.Count > 0)
                {
                    foreach (IntVector2 ip in room.entitySafeCells)
                    {
                        Cell c = room.ec.CellFromPosition(ip);
                        if (c != null) candidates.Add(c);
                    }
                }
                if (candidates.Count == 0)
                {
                    foreach (Cell c in room.cells) candidates.Add(c);
                }
                if (candidates.Count == 0) return;

                
                int count = UnityEngine.Random.Range(3, 8);
                for (int i = 0; i < count; i++)
                {
                    Cell c = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                    Vector3 worldPos = c.TileTransform.position + Vector3.up * 1.5f;
                    MilkHallDecor.Create(room.objectObject.transform, worldPos, bucket);
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        public static System.Collections.Generic.List<WeightedRoomAsset> LoadedMilkRooms = new System.Collections.Generic.List<WeightedRoomAsset>();

        
        
        public static System.Collections.Generic.List<WeightedRoomAsset> LoadedMathRooms = new System.Collections.Generic.List<WeightedRoomAsset>();

        
        
        public static int MaxMathRoomsPerFloor(int levelNo)
        {
            int cap = 12 + levelNo;
            if (cap > 16) cap = 16;
            return cap;
        }

        public static void LoadMilkRoomsFromFiles()
        {
            try
            {
                if (LoadedMilkRooms.Count > 0) return; 

                string roomsPath = Path.Combine(AssetLoader.GetModPath(Plugin.Instance), "Rooms");
                
                if (!Directory.Exists(roomsPath))
                {
                    
                    return;
                }

                string[] files = Directory.GetFiles(roomsPath, "*.rbpl");
                

                
                string fallbackType = null;
                try { foreach (var k in LevelLoaderPlugin.Instance.roomSettings.Keys) { fallbackType = k; break; } }
                catch (System.Exception) { fallbackType = null; }

                foreach (string file in files)
                {
                    try
                    {
                        
                        BaldiRoomAsset baldiRoom;
                        using (var ms = new MemoryStream(File.ReadAllBytes(file)))
                        using (var br = new BinaryReader(ms))
                        {
                            baldiRoom = BaldiRoomAsset.Read(br);
                        }
                        if (baldiRoom == null)
                        {
                            
                            continue;
                        }

                        
                        
                        
                        if (!LevelLoaderPlugin.Instance.roomSettings.ContainsKey(baldiRoom.type))
                        {
                            
                            if (fallbackType != null) baldiRoom.type = fallbackType;
                        }

                        
                        RoomAsset room = LevelImporter.CreateRoomAsset(baldiRoom);
                        if (room == null)
                        {
                            
                            continue;
                        }

                        
                        Transform lightPre;
                        if (LevelLoaderPlugin.Instance.lightTransforms.TryGetValue("standardhanging", out lightPre))
                        {
                            room.lightPre = lightPre;
                        }

                        
                        WeightedRoomAsset wra = new WeightedRoomAsset();
                        wra.selection = room;
                        wra.weight = 100;
                        LoadedMilkRooms.Add(wra);

                        
                    }
                    catch (System.Exception )
                    {
                        
                    }
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        public static void Load99RoomAsset()
        {
            try
            {
                if (Loaded99Room != null) return;
                string path = Path.Combine(AssetLoader.GetModPath(Plugin.Instance), "99", "99.rbpl");
                if (!File.Exists(path))
                {
                    
                    return;
                }
                string fallbackType = null;
                try { foreach (var k in LevelLoaderPlugin.Instance.roomSettings.Keys) { fallbackType = k; break; } }
                catch (System.Exception) { fallbackType = null; }

                BaldiRoomAsset baldiRoom;
                using (var ms = new MemoryStream(File.ReadAllBytes(path)))
                using (var br = new BinaryReader(ms))
                {
                    baldiRoom = BaldiRoomAsset.Read(br);
                }
                if (baldiRoom == null)
                {
                    
                    return;
                }
                if (!LevelLoaderPlugin.Instance.roomSettings.ContainsKey(baldiRoom.type))
                {
                    
                    if (fallbackType != null) baldiRoom.type = fallbackType;
                }
                RoomAsset room = LevelImporter.CreateRoomAsset(baldiRoom);
                if (room == null)
                {
                    
                    return;
                }
                
                Transform lightPre;
                if (LevelLoaderPlugin.Instance.lightTransforms.TryGetValue("standardhanging", out lightPre))
                {
                    room.lightPre = lightPre;
                }
                
                
                int badLights = (room.lights != null) ? room.lights.Count : 0;
                if (badLights > 0 && room.lights != null)
                {
                    room.lights.Clear();
                    
                }
                WeightedRoomAsset w = new WeightedRoomAsset();
                w.selection = room;
                w.weight = 9999; 
                Loaded99Room = w;
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        public static void Inject99RoomIntoPool(LevelGenerationParameters ld)
        {
            try
            {
                Plugin.Load99RoomAsset();
                if (Plugin.Loaded99Room == null) return;
                
                
                
                var specials = new System.Collections.Generic.List<WeightedRoomAsset>(ld.potentialSpecialRooms ?? new WeightedRoomAsset[0]);
                specials.Add(new WeightedRoomAsset { selection = Plugin.Loaded99Room.selection, weight = 99999 });
                ld.potentialSpecialRooms = specials.ToArray();
                
                
                
                ld.minSpecialRooms = 1;
                ld.maxSpecialRooms = 1;
                
            }
            catch (System.Exception ) {  }
        }

        
        
        
        public static void LoadMathRoomsFromFiles()
        {
            try
            {
                if (LoadedMathRooms.Count > 0) return; 

                string roomsPath = Path.Combine(AssetLoader.GetModPath(Plugin.Instance), "MathRooms");
                
                if (!Directory.Exists(roomsPath))
                {
                    
                    return;
                }

                string[] files = Directory.GetFiles(roomsPath, "*.rbpl");
                

                string fallbackType = null;
                try { foreach (var k in LevelLoaderPlugin.Instance.roomSettings.Keys) { fallbackType = k; break; } }
                catch (System.Exception) { fallbackType = null; }

                foreach (string file in files)
                {
                    try
                    {
                        BaldiRoomAsset baldiRoom;
                        using (var ms = new MemoryStream(File.ReadAllBytes(file)))
                        using (var br = new BinaryReader(ms))
                        {
                            baldiRoom = BaldiRoomAsset.Read(br);
                        }
                        if (baldiRoom == null)
                        {
                            
                            continue;
                        }

                        
                        
                        
                        
                        
                        string originalType = baldiRoom.type;
                        string targetType = "class";
                        if (LevelLoaderPlugin.Instance.roomSettings.ContainsKey(targetType))
                        {
                            baldiRoom.type = targetType;
                        }
                        else if (!LevelLoaderPlugin.Instance.roomSettings.ContainsKey(baldiRoom.type))
                        {
                            
                            if (fallbackType != null) baldiRoom.type = fallbackType;
                        }

                        
                        
                        
                        if (baldiRoom.textureContainer != null)
                        {
                            string oldT = baldiRoom.textureContainer.wall;
                            if (oldT != "MilkRoom_Wall")
                                
                            baldiRoom.textureContainer.floor = "MilkRoom_Floor";
                            baldiRoom.textureContainer.wall = "MilkRoom_Wall";
                            baldiRoom.textureContainer.ceiling = "MilkRoom_Ceiling";
                        }

                        RoomAsset room = LevelImporter.CreateRoomAsset(baldiRoom);
                        if (room == null)
                        {
                            
                            continue;
                        }

                        Transform lightPre;
                        if (LevelLoaderPlugin.Instance.lightTransforms.TryGetValue("standardhanging", out lightPre))
                        {
                            room.lightPre = lightPre;
                        }

                        WeightedRoomAsset wra = new WeightedRoomAsset();
                        wra.selection = room;
                        
                        
                        PosterObject milkPoster14 = GetMilkPoster14();
                        if (milkPoster14 != null)
                        {
                            if (room.posters == null) room.posters = new System.Collections.Generic.List<WeightedPosterObject>();
                            room.posters.Add(new WeightedPosterObject { selection = milkPoster14, weight = 60 });
                        }
                        
                        PosterObject milkPoster15 = GetMilkPoster15();
                        if (milkPoster15 != null)
                        {
                            if (room.posters == null) room.posters = new System.Collections.Generic.List<WeightedPosterObject>();
                            room.posters.Add(new WeightedPosterObject { selection = milkPoster15, weight = 60 });
                        }
                        
                        PosterObject milkPoster16 = GetMilkPoster16();
                        if (milkPoster16 != null)
                        {
                            if (room.posters == null) room.posters = new System.Collections.Generic.List<WeightedPosterObject>();
                            room.posters.Add(new WeightedPosterObject { selection = milkPoster16, weight = 60 });
                        }
                        
                        PosterObject milkPoster17 = GetMilkPoster17();
                        if (milkPoster17 != null)
                        {
                            if (room.posters == null) room.posters = new System.Collections.Generic.List<WeightedPosterObject>();
                            room.posters.Add(new WeightedPosterObject { selection = milkPoster17, weight = 60 });
                        }
                        
                        PosterObject chalkMilk1 = GetChalkMilk1();
                        if (chalkMilk1 != null)
                        {
                            if (room.posters == null) room.posters = new System.Collections.Generic.List<WeightedPosterObject>();
                            room.posters.Add(new WeightedPosterObject { selection = chalkMilk1, weight = 60 });
                        }
                        PosterObject chalkMilk2 = GetChalkMilk2();
                        if (chalkMilk2 != null)
                        {
                            if (room.posters == null) room.posters = new System.Collections.Generic.List<WeightedPosterObject>();
                            room.posters.Add(new WeightedPosterObject { selection = chalkMilk2, weight = 60 });
                        }
                        wra.weight = 100;
                        LoadedMathRooms.Add(wra);

                        
                    }
                    catch (System.Exception )
                    {
                        
                    }
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }


        
        
        private void RegisterMilkPosters()
        {
            try
            {
                RegisterPoster("MilkPoster", "MilkPoster.png", "MilkPoster_Icon.png", "Milk Poster");
                RegisterPoster("MilkPoster2", "MilkPoster2.png", "MilkPoster_Icon.png", "Milk Poster 2");
                RegisterPoster("MilkPoster3", "MilkPoster3.png", "MilkPoster_Icon.png", "Milk Poster 3");
                RegisterPoster("MilkPoster4", "MilkPoster4.png", "MilkPoster_Icon.png", "Milk Poster 4");
                RegisterPoster("MilkPoster5", "MilkPoster5.png", "MilkPoster_Icon.png", "Milk Poster 5");
                RegisterPoster("MilkPoster6", "MilkPoster6.png", "MilkPoster_Icon.png", "Milk Poster 6");
                RegisterPoster("MilkPoster7", "MilkPoster7.png", "MilkPoster_Icon.png", "Milk Poster 7");
                RegisterPoster("MilkPoster8", "MilkPoster8.png", "MilkPoster_Icon.png", "Milk Poster 8");
                RegisterPoster("MilkPoster9", "MilkPoster9.png", "MilkPoster_Icon.png", "Milk Poster 9");
                
                RegisterPoster("MilkPoster10", "MilkPoster10.png", "MilkPoster_Icon.png", "Milk Poster 10");
                RegisterPoster("MilkPoster11", "MilkPoster11.png", "MilkPoster_Icon.png", "Milk Poster 11");
                RegisterPoster("MilkPoster12", "MilkPoster12.png", "MilkPoster_Icon.png", "Milk Poster 12");
                
                RegisterPoster("MilkPoster13", "MilkPoster13.png", "MilkPoster_Icon.png", "Milk Poster 13");
                
                RegisterPoster("MilkPoster14", "MilkPoster14.png", "MilkPoster_Icon.png", "Milk Poster 14");
                
                RegisterPoster("MilkPoster15", "MilkPoster15.png", "MilkPoster_Icon.png", "Milk Poster 15");
                
                RegisterPoster("MilkPoster16", "MilkPoster16.png", "MilkPoster_Icon.png", "Milk Poster 16");
                
                RegisterPoster("MilkPoster17", "MilkPoster17.png", "MilkPoster_Icon.png", "Milk Poster 17");
                
                RegisterPoster("MilkPoster19", "MilkPoster19.png", "MilkPoster_Icon.png", "Milk Poster 19");
                
                RegisterPoster("ChalkMilk1", "ChalkMilk_1.png", "MilkPoster_Icon.png", "Chalk Milk 1");
                RegisterPoster("ChalkMilk2", "ChalkMilk_2.png", "MilkPoster_Icon.png", "Chalk Milk 2");
                
                RegisterPoster("PolishCowPoster", "PolishCowPoster.png", "PolishCowPoster_Icon.png", "The Polishcow");
                
                RegisterPoster("MilkPoster_Random", "MilkPoster_Random.png", "MilkPoster_Random_Icon.png", "Milk Poster Random");
                
                RegisterPoster("MilkPoster_RandomNoItem", "MilkPoster_RandomNoItem.png", "MilkPoster_RandomNoItem_Icon.png", "Milk Poster Random (Chance of no poster)");
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        
        
        
        
        
        
        private static readonly Dictionary<string, string[]> _posterFilesMap = new Dictionary<string, string[]>();

        public static void ReloadModdedContent()
        {
            try { ReloadPosterTextures(); } catch (System.Exception) { }
            try { ReloadLocalization(); } catch (System.Exception) { }
            try { ReloadItemSprites(); } catch (System.Exception) { }
            try { ReloadStickerSprites(); } catch (System.Exception) { }
            try { ReloadRanchTextures(); } catch (System.Exception) { }
            try { ReloadAudio(); } catch (System.Exception) { }
            try { ReloadRooms(); } catch (System.Exception) { }
        }

        
        
        private static void ReloadRooms()
        {
            if (LoadedMilkRooms.Count > 0) LoadedMilkRooms.Clear();
            if (LoadedMathRooms.Count > 0) LoadedMathRooms.Clear();
            if (Loaded99Room != null) Loaded99Room = null;
            _bilkClassroomAssets = null; 
            LoadMilkRoomsFromFiles();
            LoadMathRoomsFromFiles();
            Load99RoomAsset();
        }

        
        
        private static void ReloadItemSprites()
        {
            ReloadItemSpritesFor(MilkItemObject, "Milk_Small.png", "Milk_Large.png");
            ReloadItemSpritesFor(EmptyBucketItemObject, "Bucket_Small.png", "Bucket_Large.png");
            ReloadItemSpritesFor(ChocolateMilkItemObject, "Chocolatemilk_Small.png", "Chocolatemilk_Large.png");
            ReloadItemSpritesFor(MilkSodaItemObject, "MilkSodaIcon_Small.png", "MilkSodaIcon_Large.png");
            ReloadItemSpritesFor(DietMilkSodaItemObject, "DietMilkSodaIcon_Small.png", "DietMilkSodaIcon_Large.png");
            ReloadItemSpritesFor(CompressedMilkItemObject, "CompressedMilk_Small.png", "CompressedMilk_Large.png");
            ReloadItemSpritesFor(AppleMilkItemObject, "AppleMilk_Small.png", "AppleMilk_Large.png");
            ReloadItemSpritesFor(ReverseMilkItemObject, "ReverseMilk_Small.png", "ReverseMilk_Large.png");
            ReloadItemSpritesFor(WindowMilkItemObject, "windowmilk_Small.png", "windowmilk_Large.png");
            ReloadItemSpritesFor(SilentMilkItemObject, "shhmilk_Small.png", "shhmilk_Large.png");
            ReloadItemSpritesFor(LostBilkItemObject, "Lost_BILK_Small.png", "Lost_BILK_Large.png");
            ReloadItemSpritesFor(MilkYtpsItemObject, "Milk_Ytps_Small.png", "Milk_Ytps_Large.png");
            ReloadItemSpritesFor(KeyItemObject, "KEY_Small.png", "KEY_Large.png");
        }

        private static void ReloadItemSpritesFor(ItemObject it, string smallFile, string largeFile)
        {
            try
            {
                if (it == null) return;
                Sprite s = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, smallFile);
                Sprite l = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, largeFile);
                if (s != null) it.itemSpriteSmall = s;
                if (l != null) it.itemSpriteLarge = l;
            }
            catch (System.Exception) { }
        }

        
        private static void ReloadStickerSprites()
        {
            var sm = Singleton<StickerManager>.Instance;
            if (sm == null) return;
            var field = typeof(StickerManager).GetField("stickerData",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null) return;
            var stickerData = field.GetValue(sm) as StickerData[];
            if (stickerData == null) return;
            TryReloadStickerSprite(stickerData, BilkSticker, "Stickers", "BILK.png");
            TryReloadStickerSprite(stickerData, BaldishhSticker, "Stickers", "baldishh.png");
            TryReloadStickerSprite(stickerData, PolishCowSticker, "Stickers", "PolishCow.png");
            TryReloadStickerSprite(stickerData, AngryPolishCowSticker, "Stickers", "AngryPolishCow.png");
        }

        private static void TryReloadStickerSprite(StickerData[] stickerData, Sticker st, params string[] path)
        {
            try
            {
                int idx = (int)st;
                if (idx < 0 || idx >= stickerData.Length) return;
                if (stickerData[idx] == null) return;
                Sprite sp = AssetLoader.SpriteFromMod(Instance, new Vector2(0.5f, 0.5f), 50f, path);
                if (sp != null) stickerData[idx].sprite = sp;
            }
            catch (System.Exception) { }
        }

        
        private static void ReloadRanchTextures()
        {
            RanchGrassTex = AssetLoader.TextureFromMod(Instance, "Grass.png");
            RanchFenceTex = AssetLoader.TextureFromMod(Instance, "fence.png");
            RanchEdgeTex = AssetLoader.TextureFromMod(Instance, "EdgeTexture.png");
        }

        
        private static void ReloadAudio()
        {
            ReloadSoundObjectClip(DrinkSound, "Drink.wav");
            ReloadSoundObjectClip(YtpPickupSound, "YTPPickup_1.wav");
            ReloadSoundObjectClip(AppleMilkSound, "BAL_DrinkAppleMilk.wav");
        }

        private static void ReloadSoundObjectClip(SoundObject so, string file)
        {
            try
            {
                if (so == null) return;
                AudioClip clip = AssetLoader.AudioClipFromMod(Instance, file);
                if (clip != null) so.soundClip = clip;
            }
            catch (System.Exception) { }
        }

        
        private static void ReloadPosterTextures()
        {
            foreach (var kv in _posterFilesMap)
            {
                string alias = kv.Key;
                string png = kv.Value[0];
                string icon = kv.Value.Length > 1 ? kv.Value[1] : "MilkPoster_Icon.png";
                try
                {
                    Texture2D tex = AssetLoader.TextureFromMod(Instance, png);
                    if (tex == null) continue;
                    var aliases = LevelLoaderPlugin.Instance.posterAliases;
                    if (aliases.TryGetValue(alias, out PosterObject po) && po != null)
                    {
                        po.baseTexture = tex; 
                    }
                    else if (!aliases.ContainsKey(alias))
                    {
                        
                        RegisterPoster(alias, png, icon, alias);
                    }
                }
                catch (System.Exception) { }
            }
        }

        
        private static void ReloadLocalization()
        {
            var mgr = Singleton<LocalizationManager>.Instance;
            if (mgr == null) return;
            var field = typeof(LocalizationManager).GetField("localizedText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null) return;
            var dict = field.GetValue(mgr) as Dictionary<string, string>;
            if (dict == null) return;
            
            LoadLocaFileInto(dict, "Localization.json");
            
            string lang = DetectCurrentLanguage();
            if (lang == "SChinese") LoadLocaFileInto(dict, "Localization.Chinese.json");
            else if (lang == "TChinese") LoadLocaFileInto(dict, "Localization.ChineseTraditional.json");
        }

        private static void LoadLocaFileInto(Dictionary<string, string> dict, string file)
        {
            try
            {
                string path = Path.Combine(AssetLoader.GetModPath(Instance), file);
                if (!File.Exists(path)) return;
                var data = JsonUtility.FromJson<LocalizationData>(File.ReadAllText(path));
                if (data == null || data.items == null) return;
                foreach (var item in data.items)
                {
                    if (item == null || string.IsNullOrEmpty(item.key)) continue;
                    dict[item.key] = item.value;
                }
            }
            catch (System.Exception) { }
        }

        
        private static string DetectCurrentLanguage()
        {
            try
            {
                System.Reflection.Assembly bbpcAsm = null;
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.GetName().Name == "BBPC") { bbpcAsm = asm; break; }
                }
                if (bbpcAsm == null) return "";
                System.Type bbpcPlugin = bbpcAsm.GetType("BBPC.Plugin");
                if (bbpcPlugin == null) return "";
                var instanceProp = bbpcPlugin.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                object bbpcInstance = instanceProp?.GetValue(null);
                if (bbpcInstance == null) return "";
                const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance;
                System.Type cfgMgr = bbpcAsm.GetType("BBPC.API.ConfigManager") ?? bbpcAsm.GetType("BBPC.ConfigManager");
                object cfgEntry = null;
                var prop = cfgMgr?.GetProperty("currect_lang", flags);
                if (prop != null) cfgEntry = prop.GetValue(null) ?? prop.GetValue(bbpcInstance);
                if (cfgEntry == null)
                {
                    var backing = cfgMgr?.GetField("<currect_lang>k__BackingField", flags);
                    cfgEntry = backing?.GetValue(null) ?? backing?.GetValue(bbpcInstance);
                }
                if (cfgEntry == null)
                {
                    var field = cfgMgr?.GetField("currect_lang", flags);
                    cfgEntry = field?.GetValue(null) ?? field?.GetValue(bbpcInstance);
                }
                return (string)cfgEntry?.GetType().GetProperty("Value")?.GetValue(cfgEntry) ?? "";
            }
            catch (System.Exception) { return ""; }
        }

        
        private static PosterObject _cachedPoster14 = null;
        private static PosterObject GetMilkPoster14()
        {
            try
            {
                if (_cachedPoster14 != null) return _cachedPoster14;
                if (LevelLoaderPlugin.Instance.posterAliases.TryGetValue("MilkPoster14", out PosterObject p)) { _cachedPoster14 = p; return p; }
                Texture2D tex = AssetLoader.TextureFromMod(Plugin.Instance, "MilkPoster14.png");
                if (tex == null) return null;
                _cachedPoster14 = ObjectCreators.CreatePosterObject(tex, new PosterTextData[0]);
                return _cachedPoster14;
            }
            catch (System.Exception) { return null; }
        }

        
        private static PosterObject _cachedPoster15 = null;
        private static PosterObject GetMilkPoster15()
        {
            try
            {
                if (_cachedPoster15 != null) return _cachedPoster15;
                if (LevelLoaderPlugin.Instance.posterAliases.TryGetValue("MilkPoster15", out PosterObject p)) { _cachedPoster15 = p; return p; }
                Texture2D tex = AssetLoader.TextureFromMod(Plugin.Instance, "MilkPoster15.png");
                if (tex == null) return null;
                _cachedPoster15 = ObjectCreators.CreatePosterObject(tex, new PosterTextData[0]);
                return _cachedPoster15;
            }
            catch (System.Exception) { return null; }
        }

        
        private static PosterObject _cachedPoster16 = null;
        private static PosterObject GetMilkPoster16()
        {
            try
            {
                if (_cachedPoster16 != null) return _cachedPoster16;
                if (LevelLoaderPlugin.Instance.posterAliases.TryGetValue("MilkPoster16", out PosterObject p)) { _cachedPoster16 = p; return p; }
                Texture2D tex = AssetLoader.TextureFromMod(Plugin.Instance, "MilkPoster16.png");
                if (tex == null) return null;
                _cachedPoster16 = ObjectCreators.CreatePosterObject(tex, new PosterTextData[0]);
                return _cachedPoster16;
            }
            catch (System.Exception) { return null; }
        }

        
        private static PosterObject _cachedPoster17 = null;
        private static PosterObject GetMilkPoster17()
        {
            try
            {
                if (_cachedPoster17 != null) return _cachedPoster17;
                if (LevelLoaderPlugin.Instance.posterAliases.TryGetValue("MilkPoster17", out PosterObject p)) { _cachedPoster17 = p; return p; }
                Texture2D tex = AssetLoader.TextureFromMod(Plugin.Instance, "MilkPoster17.png");
                if (tex == null) return null;
                _cachedPoster17 = ObjectCreators.CreatePosterObject(tex, new PosterTextData[0]);
                return _cachedPoster17;
            }
            catch (System.Exception) { return null; }
        }

        
        private static PosterObject _cachedChalkMilk1 = null;
        private static PosterObject GetChalkMilk1()
        {
            try
            {
                if (_cachedChalkMilk1 != null) return _cachedChalkMilk1;
                if (LevelLoaderPlugin.Instance.posterAliases.TryGetValue("ChalkMilk1", out PosterObject p)) { _cachedChalkMilk1 = p; return p; }
                Texture2D tex = AssetLoader.TextureFromMod(Plugin.Instance, "ChalkMilk_1.png");
                if (tex == null) return null;
                _cachedChalkMilk1 = ObjectCreators.CreatePosterObject(tex, new PosterTextData[0]);
                return _cachedChalkMilk1;
            }
            catch (System.Exception) { return null; }
        }

        
        private static PosterObject _cachedChalkMilk2 = null;
        private static PosterObject GetChalkMilk2()
        {
            try
            {
                if (_cachedChalkMilk2 != null) return _cachedChalkMilk2;
                if (LevelLoaderPlugin.Instance.posterAliases.TryGetValue("ChalkMilk2", out PosterObject p)) { _cachedChalkMilk2 = p; return p; }
                Texture2D tex = AssetLoader.TextureFromMod(Plugin.Instance, "ChalkMilk_2.png");
                if (tex == null) return null;
                _cachedChalkMilk2 = ObjectCreators.CreatePosterObject(tex, new PosterTextData[0]);
                return _cachedChalkMilk2;
            }
            catch (System.Exception) { return null; }
        }

        
        private static void RegisterPoster(string alias, string pngFile, string iconFile, string displayName)
        {
            try
            {
                _posterFilesMap[alias] = new string[] { pngFile, iconFile }; 
                if (LevelLoaderPlugin.Instance.posterAliases.ContainsKey(alias))
                {
                    
                    return;
                }

                Texture2D posterTex = AssetLoader.TextureFromMod(Instance, pngFile);
                if (posterTex == null)
                {
                    
                    return;
                }

                
                PosterObject poster = ObjectCreators.CreatePosterObject(posterTex, new PosterTextData[0]);
                ((UnityEngine.Object)poster).name = alias;

                LevelLoaderPlugin.Instance.posterAliases.Add(alias, poster);
                

                
                try
                {
                    Sprite posterIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, iconFile);
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        
                        
                        
                        
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "posters", new PosterTool(alias, posterIcon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterMilkVendingMachine()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.basicObjects.ContainsKey("MilkVendingMachine"))
                {
                    
                    return;
                }

                if (MilkItemObject == null)
                {
                    
                    return;
                }

                
                SodaMachine baseMachine = Resources.FindObjectsOfTypeAll<SodaMachine>()
                    .FirstOrDefault(x => x.name == "ZestyMachine");
                if (baseMachine == null)
                {
                    
                    return;
                }

                
                ItemMetaStorage metaStorage = ItemMetaStorage.Instance;
                ItemObject quarterItem = null;
                try
                {
                    Items quarterEnum = (Items)System.Enum.Parse(typeof(Items), "Quarter");
                    quarterItem = metaStorage.FindByEnum(quarterEnum).value;
                }
                catch (System.Exception )
                {
                    
                }
                if (quarterItem == null)
                {
                    
                    return;
                }

                
                SodaMachine machine = UnityEngine.Object.Instantiate<SodaMachine>(baseMachine, MTM101BaldiDevAPI.prefabTransform);
                machine.gameObject.ConvertToPrefab(false);
                ((UnityEngine.Object)machine).name = "MilkVendingMachine";
                
                machine.gameObject.AddComponent<MilkVendingMarker>();

                
                FieldInfo requiredItemField = typeof(SodaMachine).GetField("requiredItem",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (requiredItemField != null)
                {
                    requiredItemField.SetValue(machine, quarterItem);
                }
                else
                {
                    
                }

                
                FieldInfo potentialItemsField = typeof(SodaMachine).GetField("potentialItems",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (potentialItemsField != null)
                {
                    WeightedItemObject milkOut = new WeightedItemObject
                    {
                        selection = MilkItemObject,
                        weight = 100
                    };
                    potentialItemsField.SetValue(machine, new WeightedItemObject[] { milkOut });
                }
                else
                {
                    
                    return;
                }

                
                try
                {
                    Texture2D faceTex = AssetLoader.TextureFromMod(Instance, "MilkVendingMachine.png");
                    Texture2D faceOutTex = AssetLoader.TextureFromMod(Instance, "MilkVendingMachine_Out.png");
                    MeshRenderer renderer = machine.GetComponent<MeshRenderer>();

                    if (renderer != null && renderer.sharedMaterials.Length > 1 && faceTex != null)
                    {
                        
                        Material[] mats = renderer.sharedMaterials;
                        
                        Material faceMat = new Material(mats[1]) { mainTexture = faceTex };
                        mats[1] = faceMat;
                        renderer.sharedMaterials = mats;
                        

                        
                        
                        
                        if (faceOutTex != null)
                        {
                            Material outMat = new Material(faceMat) { mainTexture = faceOutTex };
                            FieldInfo outMatField = typeof(SodaMachine).GetField("outOfStockMat",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            outMatField?.SetValue(machine, outMat);

                            
                            FieldInfo rendererField = typeof(SodaMachine).GetField("meshRenderer",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            rendererField?.SetValue(machine, renderer);
                        }
                    }
                    else
                    {
                        
                    }
                }
                catch (System.Exception )
                {
                    
                }

                
                GameObject machineGo = machine.gameObject;
                LevelLoaderPlugin.Instance.basicObjects.Add("MilkVendingMachine", machineGo);

                
                try
                {
                    EditorInterface.AddObjectVisual(
                        "MilkVendingMachine",
                        LevelLoaderPlugin.Instance.basicObjects["MilkVendingMachine"],
                        useRegularColliderAsEditorHitbox: true);
                    

                    
                    Sprite vendingIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "MilkVendingMachine_Icon.png");
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "objects", new ObjectTool("MilkVendingMachine", vendingIcon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        private void RegisterMilkSodaVendingMachine()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.basicObjects.ContainsKey("MilkSodaVendingMachine"))
                {
                    
                    return;
                }

                if (MilkSodaItemObject == null)
                {
                    
                    return;
                }

                
                SodaMachine baseMachine = Resources.FindObjectsOfTypeAll<SodaMachine>()
                    .FirstOrDefault(x => x.name == "ZestyMachine");
                if (baseMachine == null)
                {
                    
                    return;
                }

                
                ItemMetaStorage metaStorage = ItemMetaStorage.Instance;
                ItemObject quarterItem = null;
                try
                {
                    Items quarterEnum = (Items)System.Enum.Parse(typeof(Items), "Quarter");
                    quarterItem = metaStorage.FindByEnum(quarterEnum).value;
                }
                catch (System.Exception )
                {
                    
                }
                if (quarterItem == null)
                {
                    
                    return;
                }

                
                SodaMachine machine = UnityEngine.Object.Instantiate<SodaMachine>(baseMachine, MTM101BaldiDevAPI.prefabTransform);
                machine.gameObject.ConvertToPrefab(false);
                ((UnityEngine.Object)machine).name = "MilkSodaVendingMachine";
                
                machine.gameObject.AddComponent<MilkVendingMarker>();

                
                FieldInfo requiredItemField = typeof(SodaMachine).GetField("requiredItem",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (requiredItemField != null)
                {
                    requiredItemField.SetValue(machine, quarterItem);
                }
                else
                {
                    
                }

                
                FieldInfo potentialItemsField = typeof(SodaMachine).GetField("potentialItems",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (potentialItemsField != null)
                {
                    WeightedItemObject sodaOut = new WeightedItemObject
                    {
                        selection = MilkSodaItemObject,
                        weight = 100
                    };
                    potentialItemsField.SetValue(machine, new WeightedItemObject[] { sodaOut });
                }
                else
                {
                    
                    return;
                }

                
                try
                {
                    Texture2D faceTex = AssetLoader.TextureFromMod(Instance, "MilkSodaVendingMachine.png");
                    Texture2D faceOutTex = AssetLoader.TextureFromMod(Instance, "MilkSodaVendingMachine_Out.png");
                    MeshRenderer renderer = machine.GetComponent<MeshRenderer>();

                    if (renderer != null && renderer.sharedMaterials.Length > 1 && faceTex != null)
                    {
                        Material[] mats = renderer.sharedMaterials;
                        Material faceMat = new Material(mats[1]) { mainTexture = faceTex };
                        mats[1] = faceMat;
                        renderer.sharedMaterials = mats;
                        

                        if (faceOutTex != null)
                        {
                            Material outMat = new Material(faceMat) { mainTexture = faceOutTex };
                            FieldInfo outMatField = typeof(SodaMachine).GetField("outOfStockMat",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            outMatField?.SetValue(machine, outMat);

                            FieldInfo rendererField = typeof(SodaMachine).GetField("meshRenderer",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            rendererField?.SetValue(machine, renderer);
                        }
                    }
                }
                catch (System.Exception )
                {
                    
                }

                
                LevelLoaderPlugin.Instance.basicObjects.Add("MilkSodaVendingMachine", machine.gameObject);

                
                try
                {
                    EditorInterface.AddObjectVisual(
                        "MilkSodaVendingMachine",
                        LevelLoaderPlugin.Instance.basicObjects["MilkSodaVendingMachine"],
                        useRegularColliderAsEditorHitbox: true);
                    

                    Sprite vendingIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "MilkSodaVendingMachine_Icon.png");
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "objects", new ObjectTool("MilkSodaVendingMachine", vendingIcon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        private WeightedItemObject[] MilkMachinePotentialItems()
        {
            var list = new System.Collections.Generic.List<WeightedItemObject>();
            void Add(ItemObject it, int w) { if (it != null && w > 0) list.Add(new WeightedItemObject { selection = it, weight = w }); }
            Add(MilkItemObject, 100);                 
            Add(MiItemObject, 80);                    
            Add(LkItemObject, 80);                    
            Add(MilkSodaItemObject, 60);              
            Add(DietMilkSodaItemObject, 55);          
            Add(CompressedMilkItemObject, 55);        
            Add(ChocolateMilkItemObject, 45);         
            Add(ReverseMilkItemObject, 45);           
            Add(MilkYtpsItemObject, 35);              
            Add(WindowMilkItemObject, 35);            
            Add(QuarterMilkItemObject, 35);           
            Add(AppleMilkItemObject, 30);             
            Add(RottenMilkItemObject, 30);            
            Add(MooMilkItemObject, 30);               
            Add(IceMilkItemObject, 30);               
            Add(LostBilkItemObject, 30);              
            Add(SilentMilkItemObject, 25);            
            Add(NineNineMilkItemObject, 8);           
            return list.ToArray();
        }

        
        
        
        private void RegisterDietMilkSodaVendingMachine()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.basicObjects.ContainsKey("DietMilkSodaVendingMachine"))
                    return;
                if (DietMilkSodaItemObject == null) return;

                
                SodaMachine baseMachine = Resources.FindObjectsOfTypeAll<SodaMachine>()
                    .FirstOrDefault(x => x.name == "ZestyMachine");
                if (baseMachine == null) return;

                
                ItemObject quarterItem = null;
                try
                {
                    quarterItem = ItemMetaStorage.Instance.FindByEnum(
                        (Items)System.Enum.Parse(typeof(Items), "Quarter")).value;
                }
                catch (System.Exception) { }
                if (quarterItem == null) return;

                
                SodaMachine machine = UnityEngine.Object.Instantiate<SodaMachine>(baseMachine, MTM101BaldiDevAPI.prefabTransform);
                machine.gameObject.ConvertToPrefab(false);
                ((UnityEngine.Object)machine).name = "DietMilkSodaVendingMachine";
                machine.gameObject.AddComponent<MilkVendingMarker>();

                
                FieldInfo requiredItemField = typeof(SodaMachine).GetField("requiredItem",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (requiredItemField != null)
                    requiredItemField.SetValue(machine, quarterItem);

                
                FieldInfo potentialItemsField = typeof(SodaMachine).GetField("potentialItems",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (potentialItemsField != null)
                {
                    potentialItemsField.SetValue(machine, new WeightedItemObject[]
                    {
                        new WeightedItemObject { selection = DietMilkSodaItemObject, weight = 100 }
                    });
                }

                
                try
                {
                    Texture2D faceTex = AssetLoader.TextureFromMod(Instance, "DietMilkSodaVendingMachine.png");
                    Texture2D faceOutTex = AssetLoader.TextureFromMod(Instance, "DietSodaVendingMachine_Out.png");
                    MeshRenderer renderer = machine.GetComponent<MeshRenderer>();
                    if (renderer != null && renderer.sharedMaterials.Length > 1 && faceTex != null)
                    {
                        Material[] mats = renderer.sharedMaterials;
                        Material faceMat = new Material(mats[1]) { mainTexture = faceTex };
                        mats[1] = faceMat;
                        renderer.sharedMaterials = mats;
                        if (faceOutTex != null)
                        {
                            Material outMat = new Material(faceMat) { mainTexture = faceOutTex };
                            typeof(SodaMachine).GetField("outOfStockMat", BindingFlags.NonPublic | BindingFlags.Instance)
                                ?.SetValue(machine, outMat);
                            typeof(SodaMachine).GetField("meshRenderer", BindingFlags.NonPublic | BindingFlags.Instance)
                                ?.SetValue(machine, renderer);
                        }
                    }
                }
                catch (System.Exception) { }

                
                LevelLoaderPlugin.Instance.basicObjects.Add("DietMilkSodaVendingMachine", machine.gameObject);

                
                try
                {
                    Sprite vendingIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "DietMilkSodaVendingMachine_Icon.png");
                    EditorInterface.AddObjectVisual(
                        "DietMilkSodaVendingMachine",
                        LevelLoaderPlugin.Instance.basicObjects["DietMilkSodaVendingMachine"],
                        useRegularColliderAsEditorHitbox: true);
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(mode, "objects",
                            new ObjectTool("DietMilkSodaVendingMachine", vendingIcon),
                            addCategoryIfDoesntExist: true);
                    });
                }
                catch (System.Exception) { }
            }
            catch (System.Exception) { }
        }

        
        
        
        private void RegisterRandomMilkVendingMachine()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.basicObjects.ContainsKey("RandomMilkVendingMachine"))
                {
                    
                    return;
                }

                if (RandomMilkItemObject == null)
                {
                    
                    return;
                }

                
                SodaMachine baseMachine = Resources.FindObjectsOfTypeAll<SodaMachine>()
                    .FirstOrDefault(x => x.name == "ZestyMachine");
                if (baseMachine == null)
                {
                    
                    return;
                }

                
                ItemMetaStorage metaStorage = ItemMetaStorage.Instance;
                ItemObject quarterItem = null;
                try
                {
                    Items quarterEnum = (Items)System.Enum.Parse(typeof(Items), "Quarter");
                    quarterItem = metaStorage.FindByEnum(quarterEnum).value;
                }
                catch (System.Exception )
                {
                    
                }
                if (quarterItem == null)
                {
                    
                    return;
                }

                
                SodaMachine machine = UnityEngine.Object.Instantiate<SodaMachine>(baseMachine, MTM101BaldiDevAPI.prefabTransform);
                machine.gameObject.ConvertToPrefab(false);
                ((UnityEngine.Object)machine).name = "RandomMilkVendingMachine";
                machine.gameObject.AddComponent<MilkVendingMarker>();

                
                FieldInfo requiredItemField = typeof(SodaMachine).GetField("requiredItem",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (requiredItemField != null)
                {
                    requiredItemField.SetValue(machine, quarterItem);
                }
                else
                {
                    
                }

                
                FieldInfo potentialItemsField = typeof(SodaMachine).GetField("potentialItems",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (potentialItemsField != null)
                {
                    potentialItemsField.SetValue(machine, MilkMachinePotentialItems());
                }
                else
                {
                    
                    return;
                }

                
                try
                {
                    Texture2D faceTex = AssetLoader.TextureFromMod(Instance, "RandomMilkVendingMachine.png");
                    Texture2D faceOutTex = AssetLoader.TextureFromMod(Instance, "RandomMilkVendingMachine_Out.png");
                    MeshRenderer renderer = machine.GetComponent<MeshRenderer>();

                    if (renderer != null && renderer.sharedMaterials.Length > 1 && faceTex != null)
                    {
                        Material[] mats = renderer.sharedMaterials;
                        Material faceMat = new Material(mats[1]) { mainTexture = faceTex };
                        mats[1] = faceMat;
                        renderer.sharedMaterials = mats;
                        

                        if (faceOutTex != null)
                        {
                            Material outMat = new Material(faceMat) { mainTexture = faceOutTex };
                            FieldInfo outMatField = typeof(SodaMachine).GetField("outOfStockMat",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            outMatField?.SetValue(machine, outMat);

                            FieldInfo rendererField = typeof(SodaMachine).GetField("meshRenderer",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            rendererField?.SetValue(machine, renderer);
                        }
                    }
                }
                catch (System.Exception )
                {
                    
                }

                
                LevelLoaderPlugin.Instance.basicObjects.Add("RandomMilkVendingMachine", machine.gameObject);

                
                try
                {
                    EditorInterface.AddObjectVisual(
                        "RandomMilkVendingMachine",
                        LevelLoaderPlugin.Instance.basicObjects["RandomMilkVendingMachine"],
                        useRegularColliderAsEditorHitbox: true);
                    

                    Sprite vendingIcon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "RandomMilkVendingMachine_Icon.png");
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "objects", new ObjectTool("RandomMilkVendingMachine", vendingIcon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        private static bool polishCowEditorRegistered = false;

        
        private static bool milkItemEditorToolsRegistered = false;

        
        private static void RegisterPolishCowEditorTools(EditorMode mode, bool vanillaCompliant)
        {
            try
            {
                if (!LevelLoaderPlugin.Instance.npcAliases.ContainsKey("PolishCow"))
                {
                    return;
                }
                Sprite icon = AssetLoader.SpriteFromMod(
                    Instance, Vector2.one / 2f, 25f, "npc/cow/PolishCow_0.png");
                var list = new System.Collections.Generic.List<EditorTool>
                {
                    new NPCTool("PolishCow", icon)
                };
                EditorInterfaceModes.AddToolsToCategory(mode, "npcs", list, false);
                

                
                try
                {
                    if (LevelLoaderPlugin.Instance.npcAliases.ContainsKey("BalloonCow"))
                    {
                        Sprite balloonIcon = AssetLoader.SpriteFromMod(
                            Instance, Vector2.one / 2f, 25f, "npc/BalloonCow/Bulloon.png");
                        var bcList = new System.Collections.Generic.List<EditorTool>
                        {
                            new NPCTool("BalloonCow", balloonIcon)
                        };
                        EditorInterfaceModes.AddToolsToCategory(mode, "npcs", bcList, false);
                        
                    }
                }
                catch (System.Exception ) {  }

                
                try
                {
                    if (LevelLoaderPlugin.Instance.npcAliases.ContainsKey("MilkSalesman"))
                    {
                        Sprite msIcon = AssetLoader.SpriteFromMod(
                            Instance, Vector2.one / 2f, 38f, "npc/Milksalesman/Milk salesman.png");
                        var msList = new System.Collections.Generic.List<EditorTool>
                        {
                            new NPCTool("MilkSalesman", msIcon)
                        };
                        EditorInterfaceModes.AddToolsToCategory(mode, "npcs", msList, false);
                        
                    }
                }
                catch (System.Exception ) {  }

                
                
                
                
                try
                {
                    Sprite machineIcon = AssetLoader.SpriteFromMod(
                        Instance, Vector2.one / 2f, 50f, "machine/empbucket2nbVendingMachine_Icon.png");
                    var actList = new System.Collections.Generic.List<EditorTool>
                    {
                        new ActivityTool("MilkMachine", machineIcon, 5f)
                    };
                    EditorInterfaceModes.AddToolsToCategory(mode, "activities", actList, false);
                    

                    
                    try
                    {
                        Sprite quizIcon = AssetLoader.SpriteFromMod(
                            Instance, Vector2.one / 2f, 50f, "machine/empbucket2nbQuik_VendingMachine_Icon.png");
                        var quizList = new System.Collections.Generic.List<EditorTool>
                        {
                            new ActivityTool("QuizMachine", quizIcon, 5f)
                        };
                        EditorInterfaceModes.AddToolsToCategory(mode, "activities", quizList, false);
                        
                    }
                    catch (System.Exception )
                    {
                        
                    }
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        
        
        
        
        
        
        private static Animator TryCreateWallSignPrefab(string prefabName, string leftFile, string rightFile)
        {
            try
            {
                Animator baseSign = null;
                Animator[] allAnims = Resources.FindObjectsOfTypeAll<Animator>();
                foreach (var a in allAnims)
                {
                    if (a != null && a.gameObject != null)
                    {
                        string nm = a.gameObject.name;
                        if (nm != null && nm.IndexOf("ActivityExteriorSign", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            baseSign = a;
                            break;
                        }
                    }
                }
                if (baseSign == null) return null;
                Animator obj = UnityEngine.Object.Instantiate<Animator>(baseSign);
                obj.gameObject.name = prefabName;
                SpriteRenderer[] srs = obj.GetComponentsInChildren<SpriteRenderer>();
                if (srs == null || srs.Length < 2)
                {
                    UnityEngine.Object.Destroy(obj.gameObject);
                    return null;
                }
                float ppu = (srs[0].sprite != null) ? srs[0].sprite.pixelsPerUnit : 10f;
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                Sprite right = AssetLoader.SpriteFromMod(Instance, pivot, ppu, rightFile);
                Sprite left = AssetLoader.SpriteFromMod(Instance, pivot, ppu, leftFile);
                if (right == null || left == null)
                {
                    UnityEngine.Object.Destroy(obj.gameObject);
                    return null;
                }
                srs[0].sprite = right;
                srs[1].sprite = left;
                obj.gameObject.ConvertToPrefab(true);
                return obj;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        
        private static void AssignWallSign(Activity activity, Animator signPrefab)
        {
            if (activity == null || signPrefab == null) return;
            try
            {
                System.Reflection.FieldInfo f = typeof(Activity).GetField("exteriorSignPrefab",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (f != null) f.SetValue(activity, signPrefab);
            }
            catch (System.Exception) { }
        }

        
        
        
        
        private void RegisterMilkContent()
        {
            try
            {
                
                
                try
                {
                    
                    PosterObject cowPoster = ScriptableObject.CreateInstance<PosterObject>();
                    cowPoster.textData = new PosterTextData[]
                    {
                        new PosterTextData { textKey = "Milk_PolishCow_Title" },
                        new PosterTextData { textKey = "Milk_PolishCow_Desc" }
                    };

                    PolishCow polishCow = new NPCBuilder<PolishCow>(Info)
                        .SetName("PolishCow")
                        .SetEnum("PolishCow")
                        .SetPoster(cowPoster)
                        .AddLooker()
                        .IgnorePlayerOnSpawn()
                        .Build();

                    
                    assetMan.Add<NPC>("PolishCow", polishCow);
                    NPC cowFromMan = assetMan.Get<NPC>("PolishCow");


                    
                    try
                    {
                        LevelLoaderPlugin.Instance.npcAliases["PolishCow"] = cowFromMan;
                        EditorInterface.AddNPCVisual("PolishCow", cowFromMan);

                        
                        if (LevelStudioPlugin.Instance.npcDisplays.TryGetValue("PolishCow", out GameObject vis) && vis != null)
                        {
                            vis.SetActive(true);
                            
                            try
                            {
                                Sprite cowSprite = AssetLoader.SpriteFromMod(
                                    Instance, Vector2.one / 2f, 25f, "npc/cow/PolishCow_0.png");
                                if (cowSprite != null)
                                {
                                    var sr = vis.GetComponentInChildren<SpriteRenderer>();
                                    if (sr != null) sr.sprite = cowSprite;
                                }
                            }
                            catch { }
                        }

                        
                        
                        
                        
                    }
                    catch (System.Exception )
                    {
                        
                    }

                    
                    
                    
                    try
                    {
                        StampedeCow stampedeCow = new NPCBuilder<StampedeCow>(Info)
                            .SetName("StampedeCow")
                            .SetEnum("StampedeCow")
                            .AddLooker()
                            .IgnorePlayerOnSpawn()
                            .Build();
                        assetMan.Add<NPC>("StampedeCow", stampedeCow);
                        StampedeCowPrefab = assetMan.Get<NPC>("StampedeCow");
                        
                    }
                    catch (System.Exception )
                    {
                        
                    }

                    
                    
                    
                    try
                    {
                        if (MilkSalesmanPrefab == null)
                        {
                            PosterObject salesmanPoster = ScriptableObject.CreateInstance<PosterObject>();
                            salesmanPoster.textData = new PosterTextData[]
                            {
                                new PosterTextData { textKey = "Milk_Salesman_Title" },
                                new PosterTextData { textKey = "Milk_Salesman_Desc" }
                            };

                            MilkSalesman salesman = new NPCBuilder<MilkSalesman>(Info)
                                .SetName("MilkSalesman")
                                .SetEnum("MilkSalesman")
                                .SetPoster(salesmanPoster)
                                .AddLooker()
                                .IgnorePlayerOnSpawn()
                                .Build();

                            assetMan.Add<NPC>("MilkSalesman", salesman);
                            MilkSalesmanPrefab = assetMan.Get<NPC>("MilkSalesman");
                        }

                        
                        try
                        {
                            if (LevelLoaderPlugin.Instance != null)
                            {
                                LevelLoaderPlugin.Instance.npcAliases["MilkSalesman"] = MilkSalesmanPrefab;
                                try { EditorInterface.AddNPCVisual("MilkSalesman", MilkSalesmanPrefab); } catch (System.Exception) { }
                                if (LevelStudioPlugin.Instance != null && LevelStudioPlugin.Instance.npcDisplays != null
                                    && LevelStudioPlugin.Instance.npcDisplays.TryGetValue("MilkSalesman", out GameObject msVis) && msVis != null)
                                {
                                    msVis.SetActive(true);
                                    try
                                    {
                                        Sprite msSprite = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 38f, "npc/Milksalesman/Milk salesman.png");
                                        if (msSprite != null)
                                        {
                                            var msr = msVis.GetComponentInChildren<SpriteRenderer>();
                                            if (msr != null) msr.sprite = msSprite;
                                        }
                                    }
                                    catch { }
                                }
                                
                            }
                        }
                        catch (System.Exception )
                        {
                            
                        }
                    }
                    catch (System.Exception )
                    {
                        
                    }

                    
                    
                    try
                    {
                        if (BalloonCowPrefab == null)
                        {
                            BalloonCow balloonCow = new NPCBuilder<BalloonCow>(Info)
                                .SetName("BalloonCow")
                                .SetEnum("BalloonCow")
                                .AddLooker()
                                .IgnorePlayerOnSpawn()
                                .Build();
                            assetMan.Add<NPC>("BalloonCow", balloonCow);
                            BalloonCowPrefab = assetMan.Get<NPC>("BalloonCow");
                        }

                        
                        try
                        {
                            if (LevelLoaderPlugin.Instance != null)
                            {
                                LevelLoaderPlugin.Instance.npcAliases["BalloonCow"] = BalloonCowPrefab;
                                try { EditorInterface.AddNPCVisual("BalloonCow", BalloonCowPrefab); } catch (System.Exception) { }
                                if (LevelStudioPlugin.Instance != null && LevelStudioPlugin.Instance.npcDisplays != null
                                    && LevelStudioPlugin.Instance.npcDisplays.TryGetValue("BalloonCow", out GameObject bcVis) && bcVis != null)
                                {
                                    bcVis.SetActive(true);
                                    try
                                    {
                                        Sprite bcSprite = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "npc/BalloonCow/Bulloon.png");
                                        if (bcSprite != null)
                                        {
                                            var bsr = bcVis.GetComponentInChildren<SpriteRenderer>();
                                            if (bsr != null) bsr.sprite = bcSprite;
                                        }
                                    }
                                    catch { }
                                }
                                
                            }
                        }
                        catch (System.Exception )
                        {
                            
                        }
                    }
                    catch (System.Exception )
                    {
                        
                    }

                    
                    
                    
                    
                    try
                    {
                        if (FakeBlackSalesmanPrefab == null)
                        {
                            FakeBlackSalesman fbs = new NPCBuilder<FakeBlackSalesman>(Info)
                                .SetName("FakeBlackSalesman")
                                .SetEnum("FakeBlackSalesman")
                                .AddLooker()
                                .IgnorePlayerOnSpawn()
                                .Build();
                            assetMan.Add<NPC>("FakeBlackSalesman", fbs);
                            FakeBlackSalesmanPrefab = assetMan.Get<NPC>("FakeBlackSalesman");
                        }
                    }
                    catch (System.Exception )
                    {
                        
                    }

                    
                    
                    
                    try
                    {
                        if (StampedeEventTemplate == null)
                        {
                            SoundObject stampedeIntro = MakeEventIntroSound("mooout.wav", "Baldi_MooOut");
                            var stampedeBuilder = new RandomEventBuilder<MilkStampedeEvent>(Info)
                                .SetEnum(StampedeEventType)
                                .SetMinMaxTime(60f, 90f)
                                .SetName("MilkStampede");
                            if (stampedeIntro != null) stampedeBuilder = stampedeBuilder.SetSound(stampedeIntro);
                            StampedeEventTemplate = stampedeBuilder.Build();
                            assetMan.Add<RandomEvent>("MilkStampede", StampedeEventTemplate);
                            try { LevelLoaderPlugin.Instance.randomEventAliases.Add("milkstampede", StampedeEventTemplate); } catch { }
                            Plugin.SilentLog("[Event] MilkStampede template built: " + (StampedeEventTemplate != null));
                            
                        }
                    }
                    catch (System.Exception )
                    {
                        
                    }

                    
                    try
                    {
                        if (MilkFloodEventTemplate == null)
                        {
                            SoundObject floodIntro = MakeEventIntroSound("lotofmilk.wav", "Baldi_LotsOfMilk");
                            var floodBuilder = new RandomEventBuilder<MilkFloodEvent>(Info)
                                .SetEnum(MilkFloodEventType)
                                .SetMinMaxTime(60f, 60f)
                                .SetName("MilkFlood");
                            if (floodIntro != null) floodBuilder = floodBuilder.SetSound(floodIntro);
                            MilkFloodEventTemplate = floodBuilder.Build();
                            assetMan.Add<RandomEvent>("MilkFlood", MilkFloodEventTemplate);
                            try { LevelLoaderPlugin.Instance.randomEventAliases.Add("milkflood", MilkFloodEventTemplate); } catch { }
                            Plugin.SilentLog("[Event] MilkFlood template built: " + (MilkFloodEventTemplate != null));
                            
                        }
                    }
                    catch (System.Exception )
                    {
                        
                    }

                    
                    
                    
                    

                    
                    try
                    {
                        Plugin.Load99RoomAsset();
                    }
                    catch (System.Exception )
                    {
                        
                    }

                    
                    try
                    {
                        
                        
                        
                        
                        
                        
                        
                        
                        GameObject machinePrefab = new GameObject("MilkMachine");
                        machinePrefab.SetActive(true);
                        MilkMachine machine = machinePrefab.AddComponent<MilkMachine>();
                        
                        
                        
                        Sprite machineSprite = AssetLoader.SpriteFromMod(
                            Instance, new Vector2(0.5f, 0f), 50f, "machine", "milkmachine.png");
                        SpriteRenderer sr = machinePrefab.AddComponent<SpriteRenderer>();
                        sr.sprite = machineSprite;
                        int initCount = UnityEngine.Random.Range(1, 10); 
                        machine.Init(machineSprite, initCount);

                        
                        
                        
                        
                        
                        if (machinePrefab.GetComponent<BoxCollider>() == null)
                        {
                            BoxCollider collider = machinePrefab.AddComponent<BoxCollider>();
                            collider.center = new Vector3(0f, 2.56f, 0f);
                            collider.size = new Vector3(3f, 5.12f, 0.5f);
                            collider.isTrigger = false;
                        }
                        
                        
                        
                        int clickLayer = LayerMask.NameToLayer("ClickableCollideable");
                        if (clickLayer >= 0)
                        {
                            machinePrefab.layer = clickLayer; 
                        }

                        
                        
                        try
                        {
                            Animator wallSign = TryCreateWallSignPrefab("ActivityExteriorSign_MilkMachine",
                                "WallSign_MilkMachine_left.png", "WallSign_MilkMachine_right.png");
                            AssignWallSign(machine, wallSign);
                        }
                        catch (System.Exception ) {  }

                        
                        
                        
                        
                        
                        
                        
                        machinePrefab.ConvertToPrefab(true);
                        assetMan.Add<GameObject>("MilkMachinePrefab", machinePrefab);
                        assetMan.Add<MilkMachine>("MilkMachine", machine);
                        MilkMachinePrefabInstance = machine; 
                        
                        MilkMachine registered = assetMan.Get<MilkMachine>("MilkMachine");
                        LevelLoaderPlugin.Instance.activityAliases["MilkMachine"] = registered;
                        

                        
                        try
                        {
                            EditorInterface.AddActivityVisual("MilkMachine", machinePrefab);

                            
                            if (LevelStudioPlugin.Instance.activityDisplays.TryGetValue("MilkMachine", out GameObject actVis) && actVis != null)
                            {
                                actVis.SetActive(true);
                                var visSr = actVis.GetComponentInChildren<SpriteRenderer>();
                                if (visSr != null && machineSprite != null)
                                {
                                    visSr.sprite = machineSprite;
                                }
                                
                                
                                GameObject actVisual = actVis.transform.Find("Visual")?.gameObject ?? actVis;
                                if (actVisual.GetComponent<Collider>() == null)
                                {
                                    BoxCollider bc = actVisual.AddComponent<BoxCollider>();
                                    bc.center = new Vector3(0f, 2.5f, 0f);
                                    bc.size = new Vector3(3f, 5f, 0.5f);
                                }
                            }
                            
                        }
                        catch (System.Exception )
                        {
                            
                        }
                    }
                    catch (System.Exception )
                    {
                        
                    }
                }
                catch (System.Exception )
                {
                    
                }

                
                try { RegisterSnowZone(); }
                catch (System.Exception ) {  }

                
                try { RegisterQuizMachine(); }
                catch (System.Exception ) {  }

                
                
                
                try
                {
                    if (!polishCowEditorRegistered)
                    {
                        EditorInterfaceModes.AddModeCallback(RegisterPolishCowEditorTools);
                        polishCowEditorRegistered = true;
                    }
                }
                catch (System.Exception ) {  }

                
                
                
                
                
                try
                {
                    if (!milkItemEditorToolsRegistered)
                    {
                        EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode2, bool vanillaCompat2)
                        {
                            try
                            {
                                if (LevelLoaderPlugin.Instance == null) return;
                                var list = new System.Collections.Generic.List<EditorTool>();
                                foreach (var kv in LevelLoaderPlugin.Instance.itemObjects)
                                {
                                    if (string.IsNullOrEmpty(kv.Key)) continue;
                                    try { list.Add(new ItemTool(kv.Key)); } catch (System.Exception) { }
                                }
                                if (list.Count > 0)
                                {
                                    EditorInterfaceModes.AddToolsToCategory(mode2, "items", list, true);
                                    
                                }
                            }
                            catch (System.Exception ) {  }
                        });
                        milkItemEditorToolsRegistered = true;
                    }
                }
                catch (System.Exception ) {  }

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        private void RegisterKey()
        {
            try
            {
                if (KeyItemObject != null) return;
                Sprite keySmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "KEY_Small.png");
                Sprite keyLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "KEY_Large.png");
                var keyItem = new ItemBuilder(Info)
                    .SetNameAndDescription("ITM_KEY", "ITM_KEY_Desc")
                    .SetEnum("KEY")
                    .SetShopPrice(99999)
                    .SetGeneratorCost(0) 
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(keySmall, keyLarge)
                    .SetItemComponent<KeyComponent>()
                    .Build();
                assetMan.Add<ItemObject>("KEY", keyItem);
                LevelLoaderPlugin.Instance.itemObjects.Add("KEY", keyItem);
                LevelStudioPlugin.Instance.selectableShopItems.Add("KEY");
                KeyItemObject = keyItem;
                
                try
                {
                    var keyMeta = ItemMetaStorage.Instance != null ? ItemMetaStorage.Instance.FindByEnum(keyItem.itemType) : null;
                    
                }
                catch (System.Exception ) {  }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterLostBilk()
        {
            try
            {
                const string LostBilkEnumName = "ITM_LostBILK";
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(LostBilkEnumName))
                {
                    
                    LostBilkItemObject = LevelLoaderPlugin.Instance.itemObjects[LostBilkEnumName];
                    return;
                }

                
                Sprite lostSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Lost_BILK_Small.png");
                Sprite lostLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Lost_BILK_Large.png");

                var lostItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription("ITM_LostBILK", "ITM_LostBILK_Desc")
                    .SetEnum(LostBilkEnumName)
                    .SetShopPrice(0)
                    .SetGeneratorCost(0)
                    .SetMeta(ItemFlags.None, new string[] { "lost_item" })
                    .SetSprites(lostSmall, lostLarge)
                    
                    
                    .SetItemComponent<LostBilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(LostBilkEnumName, lostItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(LostBilkEnumName, assetMan.Get<ItemObject>(LostBilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(LostBilkEnumName);
                LostBilkItemObject = lostItem;

                

                
                try
                {
                    CharissaHelpfulMod.MakeLostItem(lostItem);
                    
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        
        
        
        
        
        

        
        
        
        
        
        
        
        internal static bool IsMilkBonusActive()
        {
            try
            {
                if (MilkBonusSticker == null) return false;
                var sm = Singleton<StickerManager>.Instance;
                return sm != null && sm.StickerValue(MilkBonusSticker) > 0; 
            }
            catch (System.Exception) { return false; }
        }

        internal static void MilkBonusNoteMilkDrink()
        {
            try { if (IsMilkBonusActive()) MilkBonusDrinks++; } catch (System.Exception) { }
        }

        
        private static HashSet<ItemObject> _milkBonusItemSet = null;
        private static bool _milkBonusSetBuilt = false;
        private static ItemObject[] MilkItemObjectsAll()
        {
            return new ItemObject[]
            {
                MilkItemObject,       
                ChocolateMilkItemObject,
                MilkSodaItemObject,
                DietMilkSodaItemObject,
                CompressedMilkItemObject,
                AppleMilkItemObject,
                ReverseMilkItemObject,
                MiItemObject,
                LkItemObject,
                RottenMilkItemObject,
                LostBilkItemObject,
                MilkYtpsItemObject,
                WindowMilkItemObject,
                NineNineMilkItemObject,
                QuarterMilkItemObject,
                BusPassMilkItemObject,
                SilentMilkItemObject,
                MooMilkItemObject,
                IceMilkItemObject,
                TimeMilkItemObject,
                PoisonMilkItemObject,
            };
        }
        internal static bool IsMilkItemObject(ItemObject it)
        {
            try
            {
                if (it == null) return false;
                if (!_milkBonusSetBuilt)
                {
                    _milkBonusItemSet = new HashSet<ItemObject>();
                    foreach (var o in MilkItemObjectsAll()) if (o != null) _milkBonusItemSet.Add(o);
                    _milkBonusSetBuilt = true;
                }
                return _milkBonusItemSet.Contains(it);
            }
            catch (System.Exception) { return false; }
        }

        
        private void RegisterStickers()
        {
            try
            {
                if (StickersReady) return;

                Sprite bilkSprite = AssetLoader.SpriteFromMod(Instance, new Vector2(0.5f, 0.5f), 50f, "Stickers", "BILK.png");
                Sprite baldishhSprite = AssetLoader.SpriteFromMod(Instance, new Vector2(0.5f, 0.5f), 50f, "Stickers", "baldishh.png");
                Sprite polishCowSprite = AssetLoader.SpriteFromMod(Instance, new Vector2(0.5f, 0.5f), 50f, "Stickers", "PolishCow.png");
                Sprite angryPolishCowSprite = AssetLoader.SpriteFromMod(Instance, new Vector2(0.5f, 0.5f), 50f, "Stickers", "AngryPolishCow.png");

                var bilkData = new StickerBuilder<ExtendedStickerData>(Info)
                    .SetEnum("BILK")
                    .SetDuplicateOddsMultiplier(0.75f)
                    .SetTagsArray(new[] { "milk_item" })
                    .SetAsAffectingGenerator()
                    .SetAsBonusSticker()
                    .SetSprite(bilkSprite)
                    .Build();
                BilkSticker = bilkData.sticker;

                var baldishhData = new StickerBuilder<ExtendedStickerData>(Info)
                    .SetEnum("baldishh")
                    .SetDuplicateOddsMultiplier(0.75f)
                    .SetTagsArray(new[] { "milk_item" })
                    .SetSprite(baldishhSprite)
                    .Build();
                BaldishhSticker = baldishhData.sticker;

                var polishCowData = new StickerBuilder<ExtendedStickerData>(Info)
                    .SetEnum("PolishCow")
                    .SetDuplicateOddsMultiplier(0.75f)
                    .SetTagsArray(new[] { "milk_item", "cow" })
                    .SetAsAffectingGenerator()
                    .SetSprite(polishCowSprite)
                    .Build();
                PolishCowSticker = polishCowData.sticker;

                var angryPolishCowData = new StickerBuilder<ExtendedStickerData>(Info)
                    .SetEnum("AngryPolishCow")
                    .SetDuplicateOddsMultiplier(0.75f)
                    .SetTagsArray(new[] { "milk_item", "cow" })
                    .SetAsAffectingGenerator()
                    .SetSprite(angryPolishCowSprite)
                    .Build();
                AngryPolishCowSticker = angryPolishCowData.sticker;

                
                
                Sprite milkBonusSprite = AssetLoader.SpriteFromMod(Instance, new Vector2(0.5f, 0.5f), 50f, "Stickers", "MilkBonus.png");
                var milkBonusData = new StickerBuilder<ExtendedStickerData>(Info)
                    .SetEnum("MilkBonus")
                    .SetDuplicateOddsMultiplier(0.75f)
                    .SetTagsArray(new[] { "milk_item" })
                    .SetAsBonusSticker()
                    .SetSprite(milkBonusSprite)
                    .Build();
                MilkBonusSticker = milkBonusData.sticker;

                StickersReady = true;
                

                
                
                
                
                InjectStickersToScenePool();

                
                
                try
                {
                    if (PolishCowSpawnStructure == null)
                    {
                        GameObject go = new GameObject("Structure_SpawnPolishCows");
                        go.ConvertToPrefab(true);
                        PolishCowSpawnStructure = go.AddComponent<Structure_SpawnPolishCows>();
                        
                    }
                }
                catch (System.Exception )
                {
                    
                }

                
                try
                {
                    if (AngryPolishCowSpawnStructure == null)
                    {
                        GameObject go = new GameObject("Structure_SpawnStampedeCows");
                        go.ConvertToPrefab(true);
                        AngryPolishCowSpawnStructure = go.AddComponent<Structure_SpawnStampedeCows>();
                        
                    }
                }
                catch (System.Exception )
                {
                    
                }

                
                try
                {
                    if (Instance != null && Instance.gameObject.GetComponent<PolishCowStickerAudio>() == null)
                    {
                        Instance.gameObject.AddComponent<PolishCowStickerAudio>();
                    }
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private static bool _stickerPoolInjected = false;
        public static void InjectStickersToScenePool()
        {
            try
            {
                if (_stickerPoolInjected || !StickersReady) return;
                var scenes = MTM101BaldiDevAPI.gameLoader?.list?.scenes;
                if (scenes == null)
                {
                    
                    return;
                }
                int added = 0;
                foreach (var so in scenes)
                {
                    if (so == null || so.levelObject == null) continue;
                    if (so == FactorySceneObject) continue;
                    CharissaHelpfulMod.AddSticker(so, BilkSticker, 120);
                    CharissaHelpfulMod.AddSticker(so, BaldishhSticker, 140);
                    CharissaHelpfulMod.AddSticker(so, PolishCowSticker, 130);
                    CharissaHelpfulMod.AddSticker(so, AngryPolishCowSticker, 120);
                    
                    
                    CharissaHelpfulMod.AddSticker(so, MilkBonusSticker, 60);
                    added++;
                }
                _stickerPoolInjected = true;
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        private static bool _milkShopInjected = false;
        public static void InjectMilkToShops()
        {
            try
            {
                if (_milkShopInjected) return;
                var scenes = MTM101BaldiDevAPI.gameLoader?.list?.scenes;
                if (scenes == null)
                {
                    
                    return;
                }
                
                var shopItems = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<ItemObject, int>>();
                void Add(ItemObject it, int w) { if (it != null) shopItems.Add(new System.Collections.Generic.KeyValuePair<ItemObject, int>(it, w)); }
                Add(MilkItemObject, 100);            
                Add(MiItemObject, 50);               
                Add(LkItemObject, 50);               
                Add(ChocolateMilkItemObject, 40);    
                Add(MilkSodaItemObject, 40);         
                Add(RandomMilkItemObject, 40);       
                Add(CompressedMilkItemObject, 30);   
                Add(ReverseMilkItemObject, 30);      
                Add(WindowMilkItemObject, 30);       
                Add(QuarterMilkItemObject, 30);      
                Add(AppleMilkItemObject, 25);        
                Add(SilentMilkItemObject, 25);       
                Add(MooMilkItemObject, 20);          
                Add(IceMilkItemObject, 20);          
                
                Add(RottenMilkItemObject, 20);       
                Add(FakeMilkItemObject, 25);         
                Add(LostBilkItemObject, 20);         
                
                if (shopItems.Count == 0)
                {
                    
                    return;
                }

                int scenesDone = 0;
                foreach (var so in scenes)
                {
                    if (so == null || so.levelObject == null) continue;
                    if (so == FactorySceneObject) continue;
                    foreach (var kv in shopItems)
                    {
                        CharissaHelpfulMod.AddItemtoShop(so, kv.Key, kv.Value);
                    }
                    scenesDone++;
                }
                _milkShopInjected = true;
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterMilkYtps()
        {
            try
            {
                const string YtpsEnumName = "ITM_MilkYtps";
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(YtpsEnumName))
                {
                    
                    MilkYtpsItemObject = LevelLoaderPlugin.Instance.itemObjects[YtpsEnumName];
                    return;
                }

                Sprite ytpsSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Milk_Ytps_Small.png");
                Sprite ytpsLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Milk_Ytps_Large.png");

                var ytpsItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription("ITM_MilkYtps", "ITM_MilkYtps_Desc")
                    .SetEnum(YtpsEnumName)
                    .SetShopPrice(200)
                    .SetGeneratorCost(45)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(ytpsSmall, ytpsLarge)
                    .SetItemComponent<MilkYtpsComponent>()
                    .Build();

                
                
                ytpsItem.addToInventory = false;

                assetMan.Add<ItemObject>(YtpsEnumName, ytpsItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(YtpsEnumName, assetMan.Get<ItemObject>(YtpsEnumName));
                
                MilkYtpsItemObject = ytpsItem;

                
                try
                {
                    AudioClip ytpsClip = AssetLoader.AudioClipFromMod(Instance, "YTPPickup_1.wav");
                    if (ytpsClip != null)
                    {
                        YtpPickupSound = ObjectCreators.CreateSoundObject(
                            ytpsClip,
                            "Vfx_YTPPickup",
                            SoundType.Voice,
                            Color.white);
                        assetMan.Add<SoundObject>("MilkYtpPickup", YtpPickupSound);
                        ytpsItem.audPickupOverride = YtpPickupSound; 
                        
                    }
                }
                catch (System.Exception )
                {
                    
                }

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterQuizMachine()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.activityAliases.ContainsKey("QuizMachine"))
                {
                    
                    return;
                }

                GameObject machinePrefab = new GameObject("QuizMachine");
                machinePrefab.SetActive(true);
                QuizMachine machine = machinePrefab.AddComponent<QuizMachine>();
                
                Sprite machineSprite = AssetLoader.SpriteFromMod(
                    Instance, new Vector2(0.5f, 0f), 50f, "machine", "milkmachine.png");
                SpriteRenderer sr = machinePrefab.AddComponent<SpriteRenderer>();
                sr.sprite = machineSprite;
                machine.Init(machineSprite, 1);

                if (machinePrefab.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider collider = machinePrefab.AddComponent<BoxCollider>();
                    collider.center = new Vector3(0f, 2.56f, 0f);
                    collider.size = new Vector3(3f, 5.12f, 0.5f);
                    collider.isTrigger = false;
                }
                int clickLayer = LayerMask.NameToLayer("ClickableCollideable");
                if (clickLayer >= 0)
                {
                    machinePrefab.layer = clickLayer;
                }

                
                
                try
                {
                    Animator wallSign = TryCreateWallSignPrefab("ActivityExteriorSign_QuickMilkMachine",
                        "WallSign_QuickMilkMachine_left.png", "WallSign_QuickMilkMachine_right.png");
                    AssignWallSign(machine, wallSign);
                }
                catch (System.Exception ) {  }

                machinePrefab.ConvertToPrefab(true);
                assetMan.Add<GameObject>("QuizMachinePrefab", machinePrefab);
                assetMan.Add<QuizMachine>("QuizMachine", machine);
                QuizMachine registered = assetMan.Get<QuizMachine>("QuizMachine");
                QuizMachinePrefabInstance = machine; 
                LevelLoaderPlugin.Instance.activityAliases["QuizMachine"] = registered;
                

                try
                {
                    EditorInterface.AddActivityVisual("QuizMachine", machinePrefab);
                    if (LevelStudioPlugin.Instance.activityDisplays.TryGetValue("QuizMachine", out GameObject actVis) && actVis != null)
                    {
                        actVis.SetActive(true);
                        var visSr = actVis.GetComponentInChildren<SpriteRenderer>();
                        if (visSr != null && machineSprite != null)
                        {
                            visSr.sprite = machineSprite;
                        }
                        GameObject actVisual = actVis.transform.Find("Visual")?.gameObject ?? actVis;
                        if (actVisual.GetComponent<Collider>() == null)
                        {
                            BoxCollider bc = actVisual.AddComponent<BoxCollider>();
                            bc.center = new Vector3(0f, 2.5f, 0f);
                            bc.size = new Vector3(3f, 5f, 0.5f);
                        }
                    }
                    
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        [HarmonyPatch(typeof(Door), "Unlock")]
        static class PatchDoorUnlock
        {
            static bool Prefix(Door __instance)
            {
                if (QuizMachine.lockedDoors.Contains(__instance))
                    return false; 
                return true;
            }
        }

        private void RegisterSnowZone()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.basicObjects.ContainsKey("SnowZone"))
                {
                    
                    return;
                }

                
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = "SnowZone";
                go.transform.localScale = new Vector3(4f, 4f, 4f);
                go.AddComponent<SnowZone>();
                go.ConvertToPrefab(false);

                
                LevelLoaderPlugin.Instance.basicObjects.Add("SnowZone", go);

                
                try
                {
                    EditorInterface.AddObjectVisual("SnowZone", go, useRegularColliderAsEditorHitbox: true);
                    

                    
                    Sprite icon = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 64f, "snow.png");
                    EditorInterfaceModes.AddModeCallback(delegate (EditorMode mode, bool vanillaCompat)
                    {
                        EditorInterfaceModes.AddToolToCategory(
                            mode, "objects", new ObjectTool("SnowZone", icon), addCategoryIfDoesntExist: true);
                    });
                    
                }
                catch (System.Exception )
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterCompressedMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(CompressedMilkEnumName))
                {
                    
                    CompressedMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[CompressedMilkEnumName];
                    return;
                }

                Sprite compSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "CompressedMilk_Small.png");
                Sprite compLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "CompressedMilk_Large.png");

                var compItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(CompressedMilkEnumName, CompressedMilkEnumName + "_Desc")
                    .SetEnum(CompressedMilkEnumName)
                    .SetShopPrice(450) 
                    .SetGeneratorCost(60)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(compSmall, compLarge)
                    .SetItemComponent<MilkComponent>()
                    .Build();

                var compComponent = compItem.item.GetComponent<MilkComponent>();
                if (compComponent != null) compComponent.Variant = MilkVariant.Compressed;

                assetMan.Add<ItemObject>(CompressedMilkEnumName, compItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(CompressedMilkEnumName, assetMan.Get<ItemObject>(CompressedMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(CompressedMilkEnumName);
                CompressedMilkItemObject = compItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        

        
        
        private void RegisterAppleMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(AppleMilkEnumName))
                {
                    
                    AppleMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[AppleMilkEnumName];
                    return;
                }

                
                Sprite appleSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "AppleMilk_Small.png");
                Sprite appleLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "AppleMilk_Large.png");
                AudioClip appleClip = AssetLoader.AudioClipFromMod(Instance, "BAL_DrinkAppleMilk.wav");

                
                AppleMilkBaldiSprite = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "BaldiApple.png");
                AppleMilkBaldiSprite1 = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "BaldiApple1.png");
                if (appleClip != null) AppleMilkAudioLength = appleClip.length;

                var appleItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(AppleMilkEnumName, AppleMilkEnumName + "_Desc")
                    .SetEnum(AppleMilkEnumName)
                    .SetShopPrice(650)
                    .SetGeneratorCost(75)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(appleSmall, appleLarge)
                    .SetItemComponent<AppleMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(AppleMilkEnumName, appleItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(AppleMilkEnumName, assetMan.Get<ItemObject>(AppleMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(AppleMilkEnumName);
                AppleMilkItemObject = appleItem;

                
                if (appleClip != null)
                {
                    AppleMilkSound = ObjectCreators.CreateSoundObject(
                        appleClip,
                        "Baldi_AppleMilk", 
                        SoundType.Voice,
                        Color.white);
                    assetMan.Add<SoundObject>("BaldiAppleMilk", AppleMilkSound);
                }

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        public static void UnsquishPlayer(PlayerManager player)
        {
            try
            {
                if (player.plm == null)
                {
                    
                    return;
                }
                var entity = player.plm.Entity;
                if (entity == null)
                {
                    
                    return;
                }

                
                if (player.gameObject.GetComponent<CompressedSquishMarker>() != null)
                {
                    
                    return;
                }

                if (entity.Squished)
                {
                    entity.Unsquish();
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterReverseMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(ReverseMilkEnumName))
                {
                    
                    ReverseMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[ReverseMilkEnumName];
                    return;
                }

                
                Sprite reverseSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "ReverseMilk_Small.png");
                Sprite reverseLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "ReverseMilk_Large.png");

                var reverseItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(ReverseMilkEnumName, ReverseMilkEnumName + "_Desc")
                    .SetEnum(ReverseMilkEnumName)
                    .SetShopPrice(110) 
                    .SetGeneratorCost(40)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(reverseSmall, reverseLarge)
                    .SetItemComponent<ReverseMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(ReverseMilkEnumName, reverseItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(ReverseMilkEnumName, assetMan.Get<ItemObject>(ReverseMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(ReverseMilkEnumName);
                ReverseMilkItemObject = reverseItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterWindowMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(WindowMilkEnumName))
                {
                    
                    WindowMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[WindowMilkEnumName];
                    return;
                }

                Sprite wSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "windowmilk_Small.png");
                Sprite wLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "windowmilk_Large.png");

                var wItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(WindowMilkEnumName, WindowMilkEnumName + "_Desc")
                    .SetEnum(WindowMilkEnumName)
                    .SetShopPrice(80) 
                    .SetGeneratorCost(30)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(wSmall, wLarge)
                    .SetItemComponent<WindowMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(WindowMilkEnumName, wItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(WindowMilkEnumName, assetMan.Get<ItemObject>(WindowMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(WindowMilkEnumName);
                WindowMilkItemObject = wItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        private void RegisterSilentMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(SilentMilkEnumName))
                {
                    SilentMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[SilentMilkEnumName];
                    return;
                }

                Sprite sSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "shhmilk_Small.png");
                Sprite sLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "shhmilk_Large.png");

                
                int wd40Price = 400;
                try
                {
                    var wd40Meta = ItemMetaStorage.Instance?.FindByEnum(Items.Wd40);
                    if (wd40Meta != null && wd40Meta.value != null)
                        wd40Price = wd40Meta.value.price;
                }
                catch (System.Exception) { }
                int shopPrice = wd40Price + 1;

                var silentItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(SilentMilkEnumName, SilentMilkEnumName + "_Desc")
                    .SetEnum(SilentMilkEnumName)
                    .SetShopPrice(shopPrice)
                    .SetGeneratorCost(40)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(sSmall, sLarge)
                    .SetItemComponent<SilentMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(SilentMilkEnumName, silentItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(SilentMilkEnumName, assetMan.Get<ItemObject>(SilentMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(SilentMilkEnumName);
                SilentMilkItemObject = silentItem;
            }
            catch (System.Exception) { }
        }

        
        
        
        private void RegisterMooMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(MooMilkEnumName))
                {
                    MooMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[MooMilkEnumName];
                    return;
                }

                Sprite mooSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "moomilk_Small.png");
                Sprite mooLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "moomilk_Large.png");

                var mooItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(MooMilkEnumName, MooMilkEnumName + "_Desc")
                    .SetEnum(MooMilkEnumName)
                    .SetShopPrice(180)
                    .SetGeneratorCost(45)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(mooSmall, mooLarge)
                    .SetItemComponent<MooMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(MooMilkEnumName, mooItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(MooMilkEnumName, assetMan.Get<ItemObject>(MooMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(MooMilkEnumName);
                MooMilkItemObject = mooItem;
            }
            catch (System.Exception) { }
        }

        
        
        
        private void RegisterIceMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(IceMilkEnumName))
                {
                    IceMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[IceMilkEnumName];
                    return;
                }

                Sprite iceSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "IceMilk_Small.png");
                Sprite iceLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "IceMilk_Large.png");

                var iceItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(IceMilkEnumName, IceMilkEnumName + "_Desc")
                    .SetEnum(IceMilkEnumName)
                    .SetShopPrice(200)
                    .SetGeneratorCost(50)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(iceSmall, iceLarge)
                    .SetItemComponent<IceMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(IceMilkEnumName, iceItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(IceMilkEnumName, assetMan.Get<ItemObject>(IceMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(IceMilkEnumName);
                IceMilkItemObject = iceItem;
            }
            catch (System.Exception) { }
        }

        
        
        
        
        
        private void RegisterTimeMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(TimeMilkEnumName))
                {
                    TimeMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[TimeMilkEnumName];
                    return;
                }

                
                Sprite tSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "TimeMilk_Small.png");
                Sprite tLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "TimeMilk_Large.png");

                var tItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(TimeMilkEnumName, TimeMilkEnumName + "_Desc")
                    .SetEnum(TimeMilkEnumName)
                    .SetShopPrice(140)
                    .SetGeneratorCost(40)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(tSmall, tLarge)
                    .SetItemComponent<TimeMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(TimeMilkEnumName, tItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(TimeMilkEnumName, assetMan.Get<ItemObject>(TimeMilkEnumName));
                TimeMilkItemObject = tItem;
            }
            catch (System.Exception) { }
        }
        private void Register99Milk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(NineNineMilkEnumName))
                {
                    
                    NineNineMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[NineNineMilkEnumName];
                    return;
                }

                Sprite sSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "99milk_Small.png");
                Sprite sLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "99milk_Large.png");

                var sItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(NineNineMilkEnumName, NineNineMilkEnumName + "_Desc")
                    .SetEnum(NineNineMilkEnumName)
                    .SetShopPrice(0)
                    .SetGeneratorCost(999)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(sSmall, sLarge)
                    .SetItemComponent<NineNineMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(NineNineMilkEnumName, sItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(NineNineMilkEnumName, assetMan.Get<ItemObject>(NineNineMilkEnumName));
                
                NineNineMilkItemObject = sItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterQuarterMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(QuarterMilkEnumName))
                {
                    
                    QuarterMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[QuarterMilkEnumName];
                    return;
                }

                Sprite qSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Quartermilk_Small.png");
                Sprite qLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Quartermilk_Large.png");

                var qItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(QuarterMilkEnumName, QuarterMilkEnumName + "_Desc")
                    .SetEnum(QuarterMilkEnumName)
                    .SetShopPrice(140) 
                    .SetGeneratorCost(35)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(qSmall, qLarge)
                    .SetItemComponent<QuarterMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(QuarterMilkEnumName, qItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(QuarterMilkEnumName, assetMan.Get<ItemObject>(QuarterMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(QuarterMilkEnumName);
                QuarterMilkItemObject = qItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        private void RegisterBusPassMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(BusPassMilkEnumName))
                {
                    
                    BusPassMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[BusPassMilkEnumName];
                    return;
                }

                Sprite bSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "BUSPASSmilk_Small.png");
                Sprite bLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "BUSPASSmilk_Large.png");

                var bItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(BusPassMilkEnumName, BusPassMilkEnumName + "_Desc")
                    .SetEnum(BusPassMilkEnumName)
                    .SetShopPrice(0)
                    .SetGeneratorCost(0)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(bSmall, bLarge)
                    .SetItemComponent<BusPassMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(BusPassMilkEnumName, bItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(BusPassMilkEnumName, assetMan.Get<ItemObject>(BusPassMilkEnumName));
                
                BusPassMilkItemObject = bItem;

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterWeakMilks()
        {
            try
            {
                RegisterSingleWeakMilk(MiEnumName, "mi_Small.png", "mi_Large.png", "Mi", (obj) => MiItemObject = obj);
                RegisterSingleWeakMilk(LkEnumName, "lk_Small.png", "lk_Large.png", "Lk", (obj) => LkItemObject = obj);
            }
            catch (System.Exception )
            {
                
            }
        }

        private void RegisterSingleWeakMilk(string enumName, string smallFile, string largeFile, string logTag, System.Action<ItemObject> setField)
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(enumName))
                {
                    
                    setField(LevelLoaderPlugin.Instance.itemObjects[enumName]);
                    return;
                }

                Sprite weakSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, smallFile);
                Sprite weakLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, largeFile);

                var weakItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(enumName, enumName + "_Desc")
                    .SetEnum(enumName)
                    .SetShopPrice(40) 
                    .SetGeneratorCost(15)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(weakSmall, weakLarge)
                    .SetItemComponent<MilkComponent>()
                    .Build();

                
                var comp = weakItem.item as MilkComponent;
                if (comp != null) comp.Variant = MilkVariant.Weak;

                assetMan.Add<ItemObject>(enumName, weakItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(enumName, assetMan.Get<ItemObject>(enumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(enumName);
                setField(weakItem);

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        private void RegisterRottenMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(RottenMilkEnumName))
                {
                    
                    RottenMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[RottenMilkEnumName];
                    return;
                }

                Sprite rottenSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "RottenMilk_Small.png");
                Sprite rottenLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "RottenMilk_Large.png");

                var rottenItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(RottenMilkEnumName, RottenMilkEnumName + "_Desc")
                    .SetEnum(RottenMilkEnumName)
                    .SetShopPrice(90) 
                    .SetGeneratorCost(35)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(rottenSmall, rottenLarge)
                    .SetItemComponent<RottenMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(RottenMilkEnumName, rottenItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(RottenMilkEnumName, assetMan.Get<ItemObject>(RottenMilkEnumName));
                LevelStudioPlugin.Instance.selectableShopItems.Add(RottenMilkEnumName);
                RottenMilkItemObject = rottenItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        
        
        
        private void RegisterFakeMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(FakeMilkEnumName))
                {
                    FakeMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[FakeMilkEnumName];
                    return;
                }

                
                Sprite fakeSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Milk_Small.png");
                Sprite fakeLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Milk_Large.png");

                var fakeItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(FakeMilkEnumName, FakeMilkEnumName + "_Desc")
                    .SetEnum(FakeMilkEnumName)
                    .SetShopPrice(250) 
                    .SetGeneratorCost(40)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(fakeSmall, fakeLarge)
                    .SetItemComponent<FakeMilkComponent>()
                    .Build();

                
                assetMan.Add<ItemObject>(FakeMilkEnumName, fakeItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(FakeMilkEnumName, assetMan.Get<ItemObject>(FakeMilkEnumName));
                FakeMilkItemObject = fakeItem;
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterRandomMilk()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(RandomMilkEnumName))
                {
                    
                    RandomMilkItemObject = LevelLoaderPlugin.Instance.itemObjects[RandomMilkEnumName];
                    return;
                }

                Sprite randomSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Randommilk_Small.png");
                Sprite randomLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Randommilk_Large.png");

                var randomItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(RandomMilkEnumName, RandomMilkEnumName + "_Desc")
                    .SetEnum(RandomMilkEnumName)
                    .SetShopPrice(120) 
                    .SetGeneratorCost(30)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(randomSmall, randomLarge)
                    .SetItemComponent<RandomMilkComponent>()
                    .Build();

                assetMan.Add<ItemObject>(RandomMilkEnumName, randomItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(RandomMilkEnumName, assetMan.Get<ItemObject>(RandomMilkEnumName));
                
                RandomMilkItemObject = randomItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterRandomMilkNoItem()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(RandomMilkNoItemEnumName))
                {
                    
                    RandomMilkNoItemItemObject = LevelLoaderPlugin.Instance.itemObjects[RandomMilkNoItemEnumName];
                    return;
                }

                
                Sprite noItemSmall = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Randommilk_Small.png");
                Sprite noItemLarge = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Randommilk_Large.png");

                var noItemItem = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(RandomMilkNoItemEnumName, RandomMilkNoItemEnumName + "_Desc")
                    .SetEnum(RandomMilkNoItemEnumName)
                    .SetShopPrice(60)
                    .SetGeneratorCost(20)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(noItemSmall, noItemLarge)
                    .SetItemComponent<RandomMilkNoItemComponent>()
                    .Build();

                assetMan.Add<ItemObject>(RandomMilkNoItemEnumName, noItemItem);
                LevelLoaderPlugin.Instance.itemObjects.Add(RandomMilkNoItemEnumName, assetMan.Get<ItemObject>(RandomMilkNoItemEnumName));
                
                RandomMilkNoItemItemObject = noItemItem;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void RegisterRandomMilk75()
        {
            try
            {
                if (LevelLoaderPlugin.Instance.itemObjects.ContainsKey(RandomMilk75EnumName))
                {
                    
                    RandomMilk75ItemObject = LevelLoaderPlugin.Instance.itemObjects[RandomMilk75EnumName];
                    return;
                }

                
                Sprite r75Small = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 25f, "Randommilk_Small.png");
                Sprite r75Large = AssetLoader.SpriteFromMod(Instance, Vector2.one / 2f, 50f, "Randommilk_Large.png");

                var r75Item = new ItemBuilder(Instance.Info)
                    .SetNameAndDescription(RandomMilk75EnumName, RandomMilk75EnumName + "_Desc")
                    .SetEnum(RandomMilk75EnumName)
                    .SetShopPrice(40)
                    .SetGeneratorCost(15)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(r75Small, r75Large)
                    .SetItemComponent<RandomMilk75Component>()
                    .Build();

                assetMan.Add<ItemObject>(RandomMilk75EnumName, r75Item);
                LevelLoaderPlugin.Instance.itemObjects.Add(RandomMilk75EnumName, assetMan.Get<ItemObject>(RandomMilk75EnumName));
                
                RandomMilk75ItemObject = r75Item;

                

                
            }
            catch (System.Exception )
            {
                
            }
        }


        
        
        private static GameObject CreatePoisonFogOverlay(GameCamera gameCam)
        {
            if (gameCam == null || gameCam.canvasCam == null) return null;

            
            GameObject root = new GameObject("PoisonMilkFogCanvas");
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = gameCam.canvasCam;
            canvas.sortingOrder = 9999;

            
            var scaler = root.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            
            var imgGo = new GameObject("BlindOverlay");
            imgGo.transform.SetParent(root.transform, false);
            var img = imgGo.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0f, 0f, 0f, 0.98f); 

            
            var rt = img.rectTransform;
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            return root;
        }

        
        
        
        
        
        
        
        
        
        
        
        public enum MilkMixerRoute { Effect, Voice, Music }

        public static void RouteToMixer(AudioSource src, MilkMixerRoute route)
        {
            if (src == null) return;
            try
            {
                UnityEngine.Audio.AudioMixerGroup group = null;
                switch (route)
                {
                    case MilkMixerRoute.Effect: group = AudioSourceManager.sfxMixerGroup; break;
                    case MilkMixerRoute.Voice: group = AudioSourceManager.vfxMixerGroup; break;
                    case MilkMixerRoute.Music: group = AudioSourceManager.mscMixerGroup; break;
                }
                if (group == null)
                {
                    try
                    {
                        var pfm = Singleton<PlayerFileManager>.Instance;
                        if (pfm != null && pfm.mixer != null && pfm.mixer.Length > 2)
                        {
                            switch (route)
                            {
                                case MilkMixerRoute.Effect: group = pfm.mixer[1]; break;
                                case MilkMixerRoute.Voice: group = pfm.mixer[0]; break;
                                case MilkMixerRoute.Music: group = pfm.mixer[2]; break;
                            }
                        }
                    }
                    catch (System.Exception) { }
                }
                src.outputAudioMixerGroup = group;
            }
            catch (System.Exception) { }
        }

        
        
        private static AudioSource CreateTinnitusSource()
        {
            
            int sampleRate = 44100;
            int samples = sampleRate * 1;
            AudioClip clip = AudioClip.Create("PoisonTinnitus", samples, 1, sampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                data[i] = Mathf.Sin(2f * Mathf.PI * 8000f * i / sampleRate) * 0.6f;
            }
            clip.SetData(data, 0);

            GameObject go = new GameObject("PoisonTinnitus");
            go.hideFlags = HideFlags.HideAndDontSave;
            var src = go.AddComponent<AudioSource>();
            src.clip = clip;
            src.loop = true;
            src.volume = 0.4f;
            src.spatialBlend = 0f; 
            RouteToMixer(src, MilkMixerRoute.Effect);
            src.Play();
            return src;
        }

        
        
        public static System.Collections.IEnumerator PoisonMilkPlayerEffectCoroutine(PlayerManager player, float duration)
        {
            if (player == null) yield break;
            var entity = player.plm != null ? player.plm.Entity : null;
            if (entity != null) entity.SetFrozen(true);
            
            float origWalk = 0f, origRun = 0f;
            if (player.plm != null)
            {
                try { origWalk = player.plm.walkSpeed; origRun = player.plm.runSpeed;
                      player.plm.walkSpeed = 0f; player.plm.runSpeed = 0f; } catch (System.Exception) { }
            }

            
            Transform camBase = null;
            Vector3 origPos = Vector3.zero;
            Quaternion origRot = Quaternion.identity;
            float roll = UnityEngine.Random.Range(-25f, 25f); 
            try
            {
                if (player.cameraBase != null)
                {
                    camBase = player.cameraBase;
                    origPos = camBase.localPosition;
                    origRot = camBase.localRotation;
                    camBase.localPosition = new Vector3(origPos.x, 0.4f, origPos.z); 
                    camBase.localRotation = Quaternion.Euler(82f, 0f, roll); 
                }
            }
            catch (System.Exception) { }

            

            float elapsed = 0f;
            while (elapsed < duration && player != null)
            {
                elapsed += Time.deltaTime;
                
                if (camBase != null)
                {
                    camBase.localPosition = new Vector3(origPos.x, 0.4f, origPos.z);
                    camBase.localRotation = Quaternion.Euler(82f, 0f, roll);
                }
                yield return null;
            }

            
            if (entity != null && entity.Frozen) entity.SetFrozen(false);
            if (player.plm != null)
            {
                try { player.plm.walkSpeed = origWalk; player.plm.runSpeed = origRun; } catch (System.Exception) { }
            }
            if (camBase != null)
            {
                camBase.localPosition = origPos;
                camBase.localRotation = origRot;
            }
            
        }

        
        
        public static System.Collections.IEnumerator PoisonMilkNPCEffectCoroutine(NPC npc, float duration)
        {
            if (npc == null) yield break;

            var entity = npc.Entity;
            if (entity != null) entity.SetFrozen(true);

            
            PoisonMilkDownTracker.downed.Add(npc);

            

            float elapsed = 0f;
            while (elapsed < duration && npc != null)
            {
                elapsed += Time.deltaTime;
                
                if (npc.spriteBase != null)
                {
                    npc.spriteBase.transform.localEulerAngles = new Vector3(90f, 0f, 0f); 
                    npc.spriteBase.transform.localScale = new Vector3(1f, 0.35f, 1f);       
                }
                yield return null;
            }

            
            if (npc != null)
            {
                if (entity != null && entity.Frozen) entity.SetFrozen(false);
                PoisonMilkDownTracker.downed.Remove(npc);
                if (npc.spriteBase != null)
                {
                    npc.spriteBase.transform.localEulerAngles = Vector3.zero;
                    npc.spriteBase.transform.localScale = Vector3.one;
                }
                
            }
        }

        
        public void ThrowPoisonMilkProjectile(PlayerManager player)
        {
            if (player == null) return;

            
            GameObject projectile = new GameObject("PoisonMilkProjectile");
            projectile.transform.position = player.transform.position + Vector3.up * 1.5f;
            projectile.transform.forward = player.transform.forward;

            var pmp = projectile.AddComponent<PoisonMilkProjectile>();
            PoisonMilkProjectile.player_ref = player;
            pmp.Initialize(player);

            
        }

        
        
        public System.Collections.IEnumerator PoisonMilkThrowMonitor(PlayerManager player)
        {
            int lastSelected = -1; 
            while (player != null)
            {
                if (player.itm != null && player.itm.maxItem >= 0 && player.itm.items != null)
                {
                    int curSelected = player.itm.selectedItem;
                    
                    bool hasPoisonMilk = false;
                    try
                    {
                        var sel = player.itm.items[curSelected];
                        
                        hasPoisonMilk = sel != null && PoisonMilkItemObject != null &&
                            sel == PoisonMilkItemObject;
                    }
                    catch (System.Exception) { }

                    
                    
                    bool leftUp = false;
                    try { leftUp = UnityEngine.Input.GetMouseButtonUp(0); }
                    catch (System.Exception) { }

                    
                    bool justSelected = (curSelected != lastSelected);

                    if (hasPoisonMilk && leftUp && !justSelected)
                    {
                        
                        ThrowPoisonMilkProjectile(player);
                        
                        try
                        {
                            if (Plugin.EmptyBucketItemObject != null && player.itm != null)
                            {
                                player.itm.SetItem(Plugin.EmptyBucketItemObject, player.itm.selectedItem);
                            }
                            else
                            {
                                player.itm.RemoveItem(player.itm.selectedItem);
                            }
                        }
                        catch (System.Exception )
                        {
                            
                        }
                        yield return new WaitForSeconds(0.3f); 
                    }
                    lastSelected = curSelected;
                }
                yield return null;
            }
        }

        
        
        
        
        
        
        

        
        private static System.Reflection.FieldInfo GetSObjectFieldAny(System.Type t, string name)
        {
            while (t != null && t != typeof(object))
            {
                System.Reflection.FieldInfo f = t.GetField(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                if (f != null) return f;
                t = t.BaseType;
            }
            return null;
        }
        private static void SetSObjectFieldAny(object obj, string name, object val)
        {
            System.Reflection.FieldInfo f = GetSObjectFieldAny(obj.GetType(), name);
            if (f != null) f.SetValue(obj, val);
        }

        
        
        internal static void LoadMooScene()
        {
            try
            {
                if (MooSceneObject != null) return;
                string path = System.IO.Path.Combine(AssetLoader.GetModPath(Instance), "Floors", "moo.bpl");
                if (!System.IO.File.Exists(path)) {  return; }
                byte[] data = System.IO.File.ReadAllBytes(path);
                SceneObject shell = null;
                using (var ms = new System.IO.MemoryStream(data))
                using (var br = new System.IO.BinaryReader(ms))
                {
                    var lvl = PlusStudioLevelFormat.BaldiLevel.Read(br);
                    shell = LevelImporter.CreateSceneObject(lvl);
                }
                if (shell == null) return;
                
                
                
                
                
                
                SceneObject template = GetReferenceSceneObject();
                if (template == null || template.manager == null) return; 
                SceneObject so = UnityEngine.Object.Instantiate<SceneObject>(template);
                const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                var fields = typeof(SceneObject).GetFields(flags);
                foreach (var f in fields)
                {
                    if (f.IsStatic) continue;
                    object sv = f.GetValue(shell);
                    if (sv != null) f.SetValue(so, sv); 
                }
                
                try { so.levelObject = shell.levelObject; } catch (System.Exception ) {  }
                SetSObjectFieldAny(so, "levelAsset", shell.levelAsset);
                SetSObjectFieldAny(so, "levelContainer", shell.levelContainer);
                SetSObjectFieldAny(so, "randomizedLevelObject",
                    GetSObjectFieldAny(shell.GetType(), "randomizedLevelObject")?.GetValue(shell)
                    ?? System.Array.CreateInstance(typeof(object), 0));
                so.levelNo = shell.levelNo;
                so.name = "MooScene";
                so.levelTitle = "???";
                so.MarkAsNeverUnload();
                MooSceneObject = so;
            }
            catch (System.Exception ) {  }
        }

        
        
        private static void LoadSceneObjectInline(SceneObject so, string who)
        {
            try
            {
                var cgm = Singleton<CoreGameManager>.Instance;
                var bgm = Singleton<BaseGameManager>.Instance;
                if (cgm == null || bgm == null || so == null) {  return; }
                Singleton<GlobalCam>.Instance.Transition((UiTransition)0, 0.01666667f);
                bgm.StopAllCoroutines();
                bgm.Ec.ResetEvents();
                Time.timeScale = 0f;
                cgm.readyToStart = false;
                cgm.disablePause = true;
                PropagatedAudioManager.paused = true;
                var elevatorScreenPreField = AccessTools.DeclaredField(typeof(BaseGameManager), "elevatorScreenPre");
                var prepareToLoad = AccessTools.Method(typeof(BaseGameManager), "PrepareToLoad", null, null);
                var es = UnityEngine.Object.Instantiate((ElevatorScreen)elevatorScreenPreField.GetValue(bgm));
                AccessTools.DeclaredField(typeof(BaseGameManager), "elevatorScreen").SetValue(bgm, es);
                es.OnLoadReady += () =>
                {
                    prepareToLoad.Invoke(bgm, System.Array.Empty<object>());
                    cgm.PrepareForReload();
                    cgm.SetLives(2, true);
                    cgm.tripPlayed = false;
                    Singleton<SubtitleManager>.Instance.DestroyAll();
                    cgm.sceneObject = so;
                    Singleton<AdditiveSceneManager>.Instance.LoadScene("Game");
                };
                es.Initialize();
                
            }
            catch (System.Exception ) {  }
        }

        
        internal static void MooStartEntry()
        {
            try
            {
                LoadMooScene();
                if (MooSceneObject == null) {  return; }
                MooPhase = 1;
                MooPh1Started = false;
                MooPh2Started = false;
                MooF1Active = false;
                try { AchievementHelper.UnlockAchievement("milk_moomystery"); } catch (System.Exception) { }   
                LoadSceneObjectInline(MooSceneObject, "moo.bpl");
            }
            catch (System.Exception ) {  }
        }

        
        
        
        
        
        
        

        
        
        private static SceneObject BuildRedWhiteScene()
        {
            try
            {
                var scenes = MTM101BaldiDevAPI.gameLoader.list.scenes;
                if (scenes == null) return null;
                SceneObject pick = null;
                int bestScore = -1;
                foreach (var s in scenes)
                {
                    if (s == null || s.levelObject == null || s.manager == null) continue;
                    if ((object)s == (object)FactorySceneObject) continue;
                    if ((object)s == (object)MooSceneObject) continue;
                    if ((int)(object)s.levelObject.type == (int)(object)MilkFactory) continue;
                    if ((int)(object)s.levelObject.type == (int)(object)LevelType.Factory) continue;
                    string n = (s.name ?? "") + "|" + (s.levelTitle ?? "");
                    int score = 0;
                    if (n.Contains("School") || n.Contains("school")) score += 100;
                    if ((int)(object)s.levelObject.type == 0) score += 40;
                    if (s.levelNo >= 1 && s.levelNo <= 4) score += 20;
                    if (score > bestScore) { bestScore = score; pick = s; }
                }
                if (pick == null)
                {
                    foreach (var s in scenes)
                    {
                        if (s == null || s.levelObject == null || s.manager == null) continue;
                        if ((object)s == (object)FactorySceneObject) continue;
                        if ((object)s == (object)MooSceneObject) continue;
                        if ((int)(object)s.levelObject.type == (int)(object)MilkFactory) continue;
                        if ((int)(object)s.levelObject.type == (int)(object)LevelType.Factory) continue;
                        pick = s; break;
                    }
                }
                if (pick == null) return null;
                SceneObject clone = UnityEngine.Object.Instantiate<SceneObject>(pick);
                SetSObjectFieldAny(clone, "randomizedLevelObject", System.Array.CreateInstance(
                    GetSObjectFieldAny(clone.GetType(), "randomizedLevelObject")?.FieldType.GetElementType() ?? typeof(object), 0));
                SetSObjectFieldAny(clone, "levelContainer", null);
                try { clone.levelNo = 3 - MooRedWhiteFloor; } catch (System.Exception) { }  
                clone.name = "RedWhiteF" + (3 - MooRedWhiteFloor);
                return clone;
            }
            catch (System.Exception ) { return null; }
        }

        
        private static void MooRedWhiteLoadFloor()
        {
            try
            {
                SceneObject f = BuildRedWhiteScene();
                if (f == null) { MooRedWhiteActive = false; return; }   
                
                if (MooRedWhiteCountdown <= 0f) MooRedWhiteCountdown = RedWhiteTotalSeconds;
                MooRedWhiteFailed = false;
                MooRedWhiteFloorReady = false;
                ensureRedWhiteComponent();
                LoadSceneObjectInline(f, "redwhite");
            }
            catch (System.Exception ) { }
        }

        
        internal static void MooRedWhiteAdvance()
        {
            try
            {
                if (!MooRedWhiteFloorReady)
                {
                    
                    MooRedWhiteLoadFloor();
                    return;
                }
                if (MooRedWhiteFloor >= 2)
                {
                    
                    MooRedWhiteActive = false;
                    MooRedWhiteFloorReady = false;
                    try { var cgm = Singleton<CoreGameManager>.Instance; if (cgm != null) cgm.disablePause = false; } catch (System.Exception) { }   
                    MooStartEntry();
                    return;
                }
                MooRedWhiteFloor += 1;        
                MooRedWhiteLoadFloor();
            }
            catch (System.Exception ) { }
        }

        
        internal static void MooRedWhiteStage(LevelBuilder lb)
        {
            try
            {
                if (!MooRedWhiteActive || MooRedWhiteFloorReady) return;
                MooArmed = true;            
                MooEntryTriggered = false;
                ensureRedWhiteComponent();
                StartRedWhiteMusic();
                if (lb != null) lb.StartCoroutine(RedWhitePostGen(lb));
                
                
                
                if (_rwMode != null) _rwMode.StartCoroutine(_rwMode.IntroSequence(Plugin.MooRedWhiteFloor == 0));
                else MooRedWhiteFloorReady = true;
            }
            catch (System.Exception) { }
        }

        
        private static System.Collections.IEnumerator RedWhitePostGen(LevelBuilder lb)
        {
            while (lb != null && lb.levelInProgress) yield return null;
            try { RedWhiteLights(); } catch (System.Exception) { }
            
            
            try { lb.StartCoroutine(SpawnFakeSalesmanWhenReady(lb)); } catch (System.Exception) { }
            
            yield break;
        }

        
        
        private static System.Collections.IEnumerator SpawnFakeSalesmanWhenReady(LevelBuilder lb)
        {
            float wait = 0f;
            EnvironmentController ec = null;
            while (wait < 60f)
            {
                if (lb != null && lb.Ec != null && !lb.levelInProgress
                    && lb.Ec.rooms != null && lb.Ec.rooms.Count > 0)
                {
                    ec = lb.Ec;
                    for (int k = 0; k < 10; k++) yield return null;
                    break;
                }
                wait += Time.deltaTime;
                yield return null;
            }
            if (ec == null || Plugin.FakeBlackSalesmanPrefab == null) yield break;
            
            
            if (Plugin.nineNineTriggeredThisRun && !Plugin.MooRedWhiteActive) yield break;
            
            if (MilkSettings.NoFakeMilkSalesman != null && MilkSettings.NoFakeMilkSalesman.Value)
            {
                Plugin.SilentLog("[FakeSalesman] Disabled by setting NoFakeMilkSalesman; skip.");
                yield break;
            }
            
            try
            {
                int lvl = Singleton<CoreGameManager>.Instance != null
                    && Singleton<CoreGameManager>.Instance.sceneObject != null
                    ? Singleton<CoreGameManager>.Instance.sceneObject.levelNo : -1;
                if (lvl == 0)
                {
                    Plugin.SilentLog("[FakeSalesman] Weird F1 floor (levelNo==0): skip spawning fake salesman.");
                    yield break;
                }
            }
            catch (System.Exception) { }
            try { SpawnRedWhiteBlackSalesman(ec); }
            catch (System.Exception e) { Plugin.SilentLog("[FakeSalesman] ERROR: " + e.Message); }
        }

        
        private static void SpawnRedWhiteBlackSalesman(EnvironmentController ec)
        {
            if (ec == null || Plugin.FakeBlackSalesmanPrefab == null) return;
            try
            {
                
                
                Vector3 anchor = ec.spawnPoint;
                try
                {
                    var pm = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
                    Vector3 playerPos = (pm != null && pm.transform != null) ? pm.transform.position : anchor;
                    anchor = (playerPos + ec.spawnPoint) * 0.5f;
                }
                catch { }

                Plugin.SilentLog("[FakeSalesman] Spawning... anchor=" + anchor + " rooms=" + (ec.rooms != null ? ec.rooms.Count : -1));

                
                var hallCells = new System.Collections.Generic.List<Cell>();
                if (ec.mainHall != null && ec.mainHall.cells != null) hallCells.AddRange(ec.mainHall.cells);
                if (ec.rooms != null)
                {
                    foreach (RoomController room in ec.rooms)
                    {
                        if (room != null && room.type == RoomType.Hall && room.cells != null)
                            hallCells.AddRange(room.cells);
                    }
                }

                
                Cell c = null;
                float bestD = -1f;
                Vector3 anchorD = anchor;
                for (int i = 0; i < hallCells.Count; i++)
                {
                    var cellT = hallCells[i];
                    if (cellT == null) continue;
                    float d = Vector3.Distance(cellT.FloorWorldPosition, anchorD);
                    if (d > bestD) { bestD = d; c = cellT; }
                }

                
                if (c == null || bestD < 25f)
                {
                    if (c != null) Plugin.SilentLog($"[FakeSalesman] Hall farthest only {bestD}; falling back to random rooms");
                    float maxDist = bestD; 
                    Cell alt = c;
                    if (ec.rooms != null)
                    {
                        for (int tries = 0; tries < 100; tries++)
                        {
                            var room = ec.rooms[UnityEngine.Random.Range(0, ec.rooms.Count)];
                            if (room == null) continue;
                            try
                            {
                                RoomCategory rc = room.category;
                                if (rc == RoomCategory.Special || rc == RoomCategory.Store
                                    || rc == RoomCategory.Mystery || rc == RoomCategory.FieldTrip) continue;
                            }
                            catch { }
                            var sc = room.RandomEntitySafeCellNoGarbage();
                            if (sc == null) continue;
                            float dR = Vector3.Distance(sc.FloorWorldPosition, anchorD);
                            if (dR > maxDist) { maxDist = dR; alt = sc; }
                        }
                    }
                    if (alt != null) c = alt;
                }

                if (c == null) return;
                float finalDist = Vector3.Distance(c.FloorWorldPosition, anchorD);

                Plugin.SilentLog("[FakeSalesman] Spawn dist=" + finalDist + " cells");

                
                if (finalDist < 10f)
                {
                    Plugin.SilentLog("[FakeSalesman] Farthest cell only " + finalDist + " < 10; skip spawning this floor.");
                    return;
                }

                
                NPC npc = ec.SpawnNPC(Plugin.FakeBlackSalesmanPrefab, c.position);
                Plugin.SilentLog("[FakeSalesman] Spawned at " + c.position + " npc=" + (npc != null));
            }
            catch (System.Exception e) { Plugin.SilentLog("[FakeSalesman] Spawn ERROR: " + e.Message); }
        }

        private static void RedWhiteLights()
        {
            try
            {
                var ec = Singleton<BaseGameManager>.Instance?.Ec;
                if (ec != null && ec.cells != null)
                {
                    Color redCol = new Color(1f, 0.12f, 0.08f, 1f);
                    Cell[,] cells = ec.cells;
                    for (int x = 0; x < cells.GetLength(0); x++)
                    {
                        for (int z = 0; z < cells.GetLength(1); z++)
                        {
                            Cell cell = cells[x, z];
                            if (cell == null) continue;
                            cell.lightColor = redCol;
                            ec.UpdateLightingAtCell(cell);
                        }
                    }
                    Shader.SetGlobalColor("_SkyboxColor", redCol);
                }
                RenderSettings.ambientLight = new Color(0.85f, 0.07f, 0.05f);
                foreach (Light l in UnityEngine.Object.FindObjectsOfType<Light>())
                {
                    if (l == null) continue;
                    try { l.color = Color.Lerp(l.color, new Color(1f, 0.20f, 0.12f), 0.75f); l.intensity = Mathf.Max(l.intensity, 1.1f); } catch (System.Exception) { }
                }
            }
            catch (System.Exception) { }
        }

        
        private static AudioSource _rwMusic = null;
        
        internal static AudioSource RedWhiteMusicSource { get { return _rwMusic; } }
        internal static void StartRedWhiteMusic()
        {
            try
            {
                if (_rwMusic != null) { if (!_rwMusic.isPlaying) _rwMusic.Play(); return; }
                AudioClip clip = AssetLoader.AudioClipFromMod(Instance, "espace.wav");
                if (clip == null) return;
                var g = new GameObject("RedWhiteMusic");
                UnityEngine.Object.DontDestroyOnLoad(g);
                var s = g.AddComponent<AudioSource>();
                s.clip = clip; s.spatialBlend = 0f; s.volume = 1f; s.loop = true; s.playOnAwake = false;
                RouteToMixer(s, MilkMixerRoute.Music);
                s.Play();
                _rwMusic = s;
            }
            catch (System.Exception) { }
        }
        private static void StopRedWhiteMusic()
        {
            try { if (_rwMusic != null) { _rwMusic.Stop(); } } catch (System.Exception) { }
        }

        
        private static GameObject _rwSfxRoot = null;
        private static AudioSource _rwHumSrc = null;     
        private static AudioSource _rwAlarmSrc = null;   
        private static AudioSource _rwOneShotSrc = null; 
        private static AudioClip _rwHumClip, _rwAlarmClip, _rwHeartClip, _rwThumpClip, _rwZapClip;

        internal static void StartRedWhiteSfx()
        {
            try
            {
                if (_rwSfxRoot == null)
                {
                    var g = new GameObject("RedWhiteSfx");
                    UnityEngine.Object.DontDestroyOnLoad(g);
                    _rwHumSrc = g.AddComponent<AudioSource>();
                    _rwAlarmSrc = g.AddComponent<AudioSource>();
                    _rwOneShotSrc = g.AddComponent<AudioSource>();
                    foreach (var s in new[] { _rwHumSrc, _rwAlarmSrc, _rwOneShotSrc })
                    { s.spatialBlend = 0f; s.playOnAwake = false; s.volume = 1f; RouteToMixer(s, MilkMixerRoute.Effect); }
                    _rwSfxRoot = g;
                }
                if (_rwHumClip == null) _rwHumClip = MakeRwHum();
                if (_rwHumClip != null)
                {
                    _rwHumSrc.clip = _rwHumClip; _rwHumSrc.loop = true;
                    if (!_rwHumSrc.isPlaying) _rwHumSrc.Play();
                }
            }
            catch (System.Exception) { }
        }

        internal static void SetRedWhiteAlarm(bool on, float vol)
        {
            try
            {
                if (_rwAlarmSrc == null) return;
                if (on)
                {
                    if (_rwAlarmClip == null) _rwAlarmClip = MakeRwAlarm();
                    if (_rwAlarmClip != null)
                    {
                        _rwAlarmSrc.clip = _rwAlarmClip; _rwAlarmSrc.loop = true; _rwAlarmSrc.volume = vol;
                        if (!_rwAlarmSrc.isPlaying) _rwAlarmSrc.Play();
                    }
                }
                else if (_rwAlarmSrc.isPlaying) _rwAlarmSrc.Stop();
            }
            catch (System.Exception) { }
        }

        internal static void RwOneShot(string kind)
        {
            try
            {
                if (_rwOneShotSrc == null) return;
                AudioClip c = null;
                if (kind == "heart") { if (_rwHeartClip == null) _rwHeartClip = MakeRwHeart(); c = _rwHeartClip; }
                else if (kind == "thump") { if (_rwThumpClip == null) _rwThumpClip = MakeRwThump(); c = _rwThumpClip; }
                else if (kind == "zap") { if (_rwZapClip == null) _rwZapClip = MakeRwZap(); c = _rwZapClip; }
                if (c != null) _rwOneShotSrc.PlayOneShot(c);
            }
            catch (System.Exception) { }
        }

        internal static void StopRedWhiteSfx()
        {
            try { if (_rwHumSrc != null) _rwHumSrc.Stop(); } catch (System.Exception) { }
            try { if (_rwAlarmSrc != null) _rwAlarmSrc.Stop(); } catch (System.Exception) { }
        }

        
        private static AudioClip MakeRwHum()
        {
            try
            {
                const int sr = 44100; const int len = sr * 2;
                var d = new float[len];
                for (int i = 0; i < len; i++)
                {
                    float t = (float)i / sr;
                    float v = Mathf.Sin(t * Mathf.PI * 2f * 55f) * 0.5f
                            + Mathf.Sin(t * Mathf.PI * 2f * 110f) * 0.3f
                            + Mathf.Sin(t * Mathf.PI * 2f * 165f) * 0.2f;
                    d[i] = v * 0.045f + (UnityEngine.Random.value * 2f - 1f) * 0.012f;
                }
                var c = AudioClip.Create("RwHum", len, 1, sr, false);
                c.SetData(d, 0);
                return c;
            }
            catch (System.Exception) { return null; }
        }

        
        private static AudioClip MakeRwAlarm()
        {
            try
            {
                const int sr = 44100; const int len = sr * 2;
                var d = new float[len];
                float phase = 0f;
                for (int i = 0; i < len; i++)
                {
                    float t = (float)i / sr;
                    float sweep = Mathf.PingPong(t * 0.7f, 1f);
                    float f = Mathf.Lerp(180f, 280f, sweep);
                    phase += f / sr;
                    d[i] = Mathf.Sin(phase * Mathf.PI * 2f) * 0.14f * (0.85f + Mathf.Sin(t * Mathf.PI * 2f) * 0.15f);
                }
                var c = AudioClip.Create("RwAlarm", len, 1, sr, false);
                c.SetData(d, 0);
                return c;
            }
            catch (System.Exception) { return null; }
        }

        
        private static AudioClip MakeRwHeart()
        {
            try
            {
                const int sr = 44100; const int len = (int)(sr * 0.22f);
                var d = new float[len];
                for (int i = 0; i < len; i++)
                {
                    float t = (float)i / sr;
                    d[i] = Mathf.Sin(t * Mathf.PI * 2f * 52f) * 0.38f * Mathf.Exp(-t * 22f);
                }
                var c = AudioClip.Create("RwHeart", len, 1, sr, false);
                c.SetData(d, 0);
                return c;
            }
            catch (System.Exception) { return null; }
        }

        
        private static AudioClip MakeRwThump()
        {
            try
            {
                const int sr = 44100; const int len = (int)(sr * 0.45f);
                var d = new float[len];
                for (int i = 0; i < len; i++)
                {
                    float t = (float)i / sr;
                    float hit = (t < 0.03f) ? (UnityEngine.Random.value * 2f - 1f) * 0.35f : 0f;
                    d[i] = Mathf.Sin(t * Mathf.PI * 2f * 44f) * 0.8f * Mathf.Exp(-t * 9f) + hit;
                }
                var c = AudioClip.Create("RwThump", len, 1, sr, false);
                c.SetData(d, 0);
                return c;
            }
            catch (System.Exception) { return null; }
        }

        
        private static AudioClip MakeRwZap()
        {
            try
            {
                const int sr = 44100; const int len = (int)(sr * 0.2f);
                var d = new float[len];
                for (int i = 0; i < len; i++)
                {
                    float t = (float)i / sr;
                    d[i] = (UnityEngine.Random.value * 2f - 1f) * 0.3f * Mathf.Exp(-t * 18f);
                }
                var c = AudioClip.Create("RwZap", len, 1, sr, false);
                c.SetData(d, 0);
                return c;
            }
            catch (System.Exception) { return null; }
        }

        
        private static RedWhiteMode _rwMode = null;
        internal static void ensureRedWhiteComponent()
        {
            try
            {
                if (_rwMode != null) { _rwMode.enabled = true; return; }
                var go = new GameObject("MooRedWhiteMode");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _rwMode = go.AddComponent<RedWhiteMode>();
                UnityEngine.Object.DontDestroyOnLoad(_rwMode);
            }
            catch (System.Exception) { }
        }

        
        internal static System.Collections.Generic.HashSet<IntVector2> RedWhiteModeErodedCells()
        {
            try { return RedWhiteMode.GetErodedCells(); }
            catch { return null; }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public class RedWhiteMode : MonoBehaviour
        {
            private static Canvas _cvs;
            private static TextMeshProUGUI _timeText;   
            private static TextMeshProUGUI _noteText;   
            private static TMP_FontAsset _font;

            
            private static System.Collections.Generic.List<Renderer> _surf;
            private static System.Collections.Generic.List<int> _surfKind;   
            private static int _morphDone = 0;
            private static float _morphTimer = 0f;
            
            private static System.Collections.Generic.HashSet<IntVector2> _erodedMapCells = new System.Collections.Generic.HashSet<IntVector2>();
            internal static System.Collections.Generic.HashSet<IntVector2> GetErodedCells() { return _erodedMapCells; }
            private static int _lastFloor = -1;
            private static Texture2D _t99Wall, _t99Floor, _t99Ceil;

            
            private static Canvas _introCvs;
            private static UnityEngine.UI.Image _blackImg;   
            private static UnityEngine.UI.Image _flashImg;   
            private static TextMeshProUGUI _introText;       
            private static bool _introRunning = false;       

            
            private static System.Collections.Generic.List<Light> _lights;
            private static System.Collections.Generic.List<float> _lightBase;
            private static int _bkState = 0;      
            private static float _bkT = 0f, _bkNext = 11f;

            
            private class GlitchEntry { public Renderer r; public float until; public Texture tex; public Color col; }
            private static System.Collections.Generic.List<GlitchEntry> _glitching = new System.Collections.Generic.List<GlitchEntry>();
            private static Texture2D _tNoiseA, _tNoiseB;
            private static float _glitchTick = 0f;

            
            private static float _heartT = 0f;
            private static float _thumpT = 0f;

            
            private static System.IntPtr _hWnd = System.IntPtr.Zero;
            private static int _ox = 0, _oy = 0;
            private static int _ow = 800, _oh = 600;    
            private static bool _winGot = false;

            
            private static string _origTitle = null;     
            private static bool _titleCorrupted = false; 
            private static float _titleTimer = 0f;       
            private static bool _layeredSet = false;     
            private static float _bkAlphaT = 0f;         

            
            private static int _shakeDx = 0, _shakeDy = 0;
            
            private static float[] _beatBuf = new float[128];
            private static float _bounceSmooth = 0f;
            private static bool _winOffsetApplied = false;   

            
            private static float _driftX = 0f, _driftY = 0f;        
            private static float _driftTgtX = 0f, _driftTgtY = 0f;  
            private static float _driftTimer = 0f;                  

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern System.IntPtr GetForegroundWindow();
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern bool GetWindowRect(System.IntPtr hWnd, out RECT lpRect);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            private static extern int GetWindowText(System.IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            private static extern bool SetWindowText(System.IntPtr hWnd, string lpString);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern int GetWindowLong(System.IntPtr hWnd, int nIndex);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern int SetWindowLong(System.IntPtr hWnd, int nIndex, int dwNewLong);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern bool SetLayeredWindowAttributes(System.IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
            private struct FLASHWINFO { public uint cbSize; public System.IntPtr hwnd; public uint dwFlags; public uint uCount; public uint dwTimeout; }
            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
            private struct RECT { public int left; public int top; public int right; public int bottom; }

            
            internal static void ResetRun()
            {
                _introRunning = false;
                _bkState = 0; _bkNext = 11f; _bkT = 0f; _bkAlphaT = 0f;
                _lights = null; _lightBase = null;
                try { _glitching.Clear(); } catch (System.Exception) { }
                _heartT = 0f; _thumpT = 0f;
                _shakeDx = 0; _shakeDy = 0;
                _titleTimer = 0f; _titleCorrupted = false;
                _driftX = _driftY = _driftTgtX = _driftTgtY = 0f; _driftTimer = 0f;   
                
                
                _winGot = false; _origTitle = null; _ow = 1366; _oh = 768;
            }

            void Awake()
            {
                try { EnsureHud(); } catch (System.Exception) { }
                try { EnsureWindowHandle(); } catch (System.Exception) { }
            }

            private void EnsureHud()
            {
                if (_cvs != null) return;
                var go = new GameObject("RedWhiteHudCanvas");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _cvs = go.AddComponent<Canvas>();
                _cvs.renderMode = RenderMode.ScreenSpaceOverlay;
                _cvs.sortingOrder = 9996;   

                
                var tgo = new GameObject("RWTime");
                tgo.transform.SetParent(go.transform, false);
                _timeText = tgo.AddComponent<TextMeshProUGUI>();
                _timeText.font = GetFont();
                _timeText.fontSize = 150f;
                _timeText.fontStyle = FontStyles.Bold;
                _timeText.alignment = TextAlignmentOptions.Center;
                _timeText.color = new Color(0.1f, 1f, 0.15f, 0.25f);
                var trt = _timeText.rectTransform;
                trt.anchorMin = new Vector2(0.5f, 0.5f);
                trt.anchorMax = new Vector2(0.5f, 0.5f);
                trt.pivot = new Vector2(0.5f, 0.5f);
                trt.sizeDelta = new Vector2(1200f, 340f);
                trt.anchoredPosition = new Vector2(0f, 110f);

                
                var ngo = new GameObject("RWNotes");
                ngo.transform.SetParent(go.transform, false);
                _noteText = ngo.AddComponent<TextMeshProUGUI>();
                _noteText.font = GetFont();
                _noteText.fontSize = 52f;
                _noteText.fontStyle = FontStyles.Bold;
                _noteText.alignment = TextAlignmentOptions.Center;
                _noteText.color = new Color(1f, 1f, 1f, 0.92f);
                var nrt = _noteText.rectTransform;
                nrt.anchorMin = new Vector2(0.5f, 0f);
                nrt.anchorMax = new Vector2(0.5f, 0f);
                nrt.pivot = new Vector2(0.5f, 0f);
                nrt.sizeDelta = new Vector2(1000f, 90f);
                nrt.anchoredPosition = new Vector2(0f, 26f);
            }

            private static TMP_FontAsset GetFont()
            {
                if (_font != null) return _font;
                try { if (TMP_Settings.defaultFontAsset != null) _font = TMP_Settings.defaultFontAsset; } catch (System.Exception) { }
                if (_font == null)
                {
                    try
                    {
                        var all = UnityEngine.Object.FindObjectsOfType<TMP_Text>(true);
                        foreach (var t in all) { if (t != null && t.font != null) { _font = t.font; break; } }
                    }
                    catch (System.Exception) { }
                }
                return _font;
            }

            private static void EnsureWindowHandle()
            {
                if (_winGot) return;
                try
                {
                    _hWnd = GetForegroundWindow();
                    RECT r; r.left = r.top = r.right = r.bottom = 0;
                    if (_hWnd != System.IntPtr.Zero && GetWindowRect(_hWnd, out r))
                    {
                        _ox = r.left; _oy = r.top;
                        _ow = r.right - r.left; _oh = r.bottom - r.top;   
                        _winGot = true;
                        
                        try
                        {
                            var sb = new System.Text.StringBuilder(512);
                            if (GetWindowText(_hWnd, sb, 512) > 0) _origTitle = sb.ToString();
                        }
                        catch (System.Exception) { }
                    }
                }
                catch (System.Exception) { }
            }

            
            private void SetWinAlpha(byte a)
            {
                try
                {
                    
                    if (MilkSettings.WindowEffects != null && !MilkSettings.WindowEffects.Value) return;
                    if (!_winGot || _hWnd == System.IntPtr.Zero) return;
                    if (!_layeredSet)
                    {
                        int ex = GetWindowLong(_hWnd, -20);   
                        SetWindowLong(_hWnd, -20, ex | 0x00080000);   
                        _layeredSet = true;
                    }
                    SetLayeredWindowAttributes(_hWnd, 0, a, 2);   
                }
                catch (System.Exception) { }
            }

            
            private static void FlashTaskbar()
            {
                try
                {
                    if (MilkSettings.WindowEffects != null && !MilkSettings.WindowEffects.Value) return;
                    if (!_winGot || _hWnd == System.IntPtr.Zero) return;
                    var fi = new FLASHWINFO();
                    fi.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(FLASHWINFO));
                    fi.hwnd = _hWnd;
                    fi.dwFlags = 3;   
                    fi.uCount = 3;
                    fi.dwTimeout = 0;
                    FlashWindowEx(ref fi);
                }
                catch (System.Exception) { }
            }

            
            private void CorruptTitle(float frac)
            {
                try
                {
                    if (_origTitle == null || _origTitle.Length == 0) return;
                    _titleTimer -= Time.deltaTime;
                    if (_titleTimer > 0f) return;
                    _titleTimer = UnityEngine.Random.Range(0.35f, 1.0f);
                    float eaten = 1f - Mathf.Clamp01(frac);
                    float corrupt = Mathf.Clamp01((eaten - 0.12f) / 0.62f);
                    if (corrupt <= 0.02f) return;
                    var sb = new System.Text.StringBuilder(_origTitle.Length + 8);
                    foreach (char c in _origTitle)
                    {
                        if (c == ' ') { sb.Append(c); continue; }
                        sb.Append(UnityEngine.Random.value < corrupt
                            ? MojibakePool[UnityEngine.Random.Range(0, MojibakePool.Length)]
                            : c);
                    }
                    SetWindowText(_hWnd, sb.ToString());
                    _titleCorrupted = true;
                }
                catch (System.Exception) { }
            }

            
            
            public void ShakeOnce()
            {
                try { StartCoroutine(ShakeCoroutine()); } catch (System.Exception) { }
            }
            private System.Collections.IEnumerator ShakeCoroutine()
            {
                try
                {
                    
                    if (MilkSettings.WindowEffects != null && !MilkSettings.WindowEffects.Value) yield break;
                }
                catch (System.Exception) { yield break; }
                try { EnsureWindowHandle(); } catch (System.Exception) { }
                if (!_winGot || _hWnd == System.IntPtr.Zero) yield break;
                int n = 16;
                for (int i = 0; i < n; i++)
                {
                    int amp = Mathf.RoundToInt(13f * (1f - (float)i / (float)n));
                    _shakeDx = UnityEngine.Random.Range(-1, 2) * amp;
                    _shakeDy = UnityEngine.Random.Range(-1, 2) * amp;
                    yield return null;
                }
                _shakeDx = 0; _shakeDy = 0;
            }

            
            
            private int MusicBounceOffset()
            {
                try
                {
                    var m = Plugin.RedWhiteMusicSource;
                    if (m == null || !m.isPlaying) { _bounceSmooth = Mathf.Lerp(_bounceSmooth, 0f, 0.25f); return Mathf.RoundToInt(_bounceSmooth * 80f); }
                    m.GetOutputData(_beatBuf, 0);
                    float peak = 0f;
                    for (int i = 0; i < _beatBuf.Length; i++)
                    {
                        float a = _beatBuf[i] < 0f ? -_beatBuf[i] : _beatBuf[i];
                        if (a > peak) peak = a;
                    }
                    
                    _bounceSmooth = Mathf.Lerp(_bounceSmooth, Mathf.Clamp01(peak * 3f), 0.45f);
                    return Mathf.RoundToInt(_bounceSmooth * 80f);   
                }
                catch (System.Exception) { return 0; }
            }

            
            
            
            
            private void ApplyWindowOffset(float frac)
            {
                try
                {
                    
                    if (MilkSettings.WindowEffects != null && !MilkSettings.WindowEffects.Value)
                    {
                        _winOffsetApplied = false;
                        return;
                    }
                    if (!_winGot) { try { EnsureWindowHandle(); } catch (System.Exception) { } }   
                    if (!_winGot || _hWnd == System.IntPtr.Zero) return;
                    if (_ow < 100 || _oh < 100) return;   
                    int bounce = MusicBounceOffset();
                    DriveDrift(frac);

                    
                    float remain = Plugin.MooRedWhiteCountdown;
                    float panic = (remain <= 60f && remain > 0f && !Plugin.MooRedWhiteFailed)
                        ? Mathf.Clamp01(1f - remain / 60f) : 0f;

                    
                    int jx = 0, jy = 0;
                    if (panic > 0f)
                    {
                        int amp = Mathf.RoundToInt(panic * 24f);
                        jx = UnityEngine.Random.Range(-amp, amp + 1);
                        jy = UnityEngine.Random.Range(-amp, amp + 1);
                        
                        bounce = Mathf.RoundToInt(bounce * (1f + panic * 0.7f));
                    }

                    
                    float sizeGrow = 0.15f;    
                    float sizeShrink = 0.09f;   
                    float scale = 1f - sizeShrink + _bounceSmooth * (sizeGrow + sizeShrink);
                    int w = Mathf.RoundToInt(_ow * scale);
                    int h = Mathf.RoundToInt(_oh * scale);
                    int sink = Mathf.RoundToInt((1f - Mathf.Clamp01(frac)) * 130f);

                    int x = _ox + _shakeDx + jx + Mathf.RoundToInt(_driftX) - (w - _ow) / 2;                       
                    int y = _oy + _shakeDy + jy - bounce - (h - _oh) / 2 + sink + Mathf.RoundToInt(_driftY);      

                    
                    try
                    {
                        int scrW = Screen.currentResolution.width, scrH = Screen.currentResolution.height;
                        x = Mathf.Clamp(x, -(w - 260), Mathf.Max(0, scrW - 260));
                        y = Mathf.Clamp(y, 0, Mathf.Max(0, scrH - 130));
                    }
                    catch (System.Exception) { }

                    SetWindowPos(_hWnd, System.IntPtr.Zero, x, y, w, h, 0x0004 | 0x0010);   
                    _winOffsetApplied = true;

                    CorruptTitle(frac);   
                }
                catch (System.Exception) { }
            }

            
            
            private void DriveDrift(float frac)
            {
                try
                {
                    float eaten = 1f - Mathf.Clamp01(frac);
                    float radius = Mathf.Lerp(0f, 250f, Mathf.Pow(eaten, 1.15f));
                    float remain = Plugin.MooRedWhiteCountdown;
                    bool panic = remain <= 60f && remain > 0f && !Plugin.MooRedWhiteFailed;
                    if (panic) radius = 250f;   

                    _driftTimer -= Time.deltaTime;
                    if (_driftTimer <= 0f)
                    {
                        _driftTimer = panic ? UnityEngine.Random.Range(0.7f, 1.3f) : UnityEngine.Random.Range(1.6f, 3.2f);
                        float ang = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                        float r = UnityEngine.Random.Range(radius * 0.45f, radius);
                        _driftTgtX = Mathf.Cos(ang) * r;
                        _driftTgtY = Mathf.Sin(ang) * r * 0.72f;   
                    }
                    float k = Mathf.Clamp01(Time.deltaTime * (panic ? 3.2f : 1.5f));
                    _driftX = Mathf.Lerp(_driftX, _driftTgtX, k);
                    _driftY = Mathf.Lerp(_driftY, _driftTgtY, k);
                }
                catch (System.Exception) { }
            }

            
            private void RestoreWindowOnce()
            {
                try
                {
                    if (!_winOffsetApplied && !_titleCorrupted) return;
                    if (_winGot && _hWnd != System.IntPtr.Zero)
                    {
                        SetWindowPos(_hWnd, System.IntPtr.Zero, _ox, _oy, _ow, _oh, 0x0004 | 0x0010);
                        try { SetWinAlpha(255); } catch (System.Exception) { }
                        try
                        {
                            if (_titleCorrupted && _origTitle != null) { SetWindowText(_hWnd, _origTitle); }
                        }
                        catch (System.Exception) { }
                    }
                    _titleCorrupted = false;
                    _winOffsetApplied = false;
                    _bounceSmooth = 0f;
                }
                catch (System.Exception) { }
            }

            
            private void EnsureIntroCanvas()
            {
                if (_introCvs != null) return;
                var go = new GameObject("RwIntroCanvas");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _introCvs = go.AddComponent<Canvas>();
                _introCvs.renderMode = RenderMode.ScreenSpaceOverlay;
                _introCvs.sortingOrder = 99999;   

                var bgo = new GameObject("Black");
                bgo.transform.SetParent(go.transform, false);
                _blackImg = bgo.AddComponent<UnityEngine.UI.Image>();
                var brt = _blackImg.rectTransform;
                brt.anchorMin = Vector2.zero; brt.anchorMax = Vector2.one;
                brt.offsetMin = Vector2.zero; brt.offsetMax = Vector2.zero;
                _blackImg.color = Color.black;
                _blackImg.enabled = false;

                var fgo = new GameObject("Flash");
                fgo.transform.SetParent(go.transform, false);
                _flashImg = fgo.AddComponent<UnityEngine.UI.Image>();
                var frt = _flashImg.rectTransform;
                frt.anchorMin = Vector2.zero; frt.anchorMax = Vector2.one;
                frt.offsetMin = Vector2.zero; frt.offsetMax = Vector2.zero;
                _flashImg.color = Color.white;
                _flashImg.enabled = false;

                var tgo = new GameObject("IntroText");
                tgo.transform.SetParent(go.transform, false);
                _introText = tgo.AddComponent<TextMeshProUGUI>();
                _introText.font = GetFont();
                _introText.fontSize = 100f;
                _introText.fontStyle = FontStyles.Bold;
                _introText.alignment = TextAlignmentOptions.Center;
                _introText.color = new Color(1f, 0.1f, 0.08f, 0.95f);
                var trt = _introText.rectTransform;
                trt.anchorMin = new Vector2(0.5f, 0.5f);
                trt.anchorMax = new Vector2(0.5f, 0.5f);
                trt.pivot = new Vector2(0.5f, 0.5f);
                trt.sizeDelta = new Vector2(1500f, 700f);
                trt.anchoredPosition = Vector2.zero;
                _introText.enabled = false;
            }

            
            private const string MojibakePool =
                "ÃÂâ¬ÎÐÆØ×Ù¤§¥¦¨©ª«®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÄÅÈÉÊËÌÍÏÒÓÔÕÖÙÚÛÜÝßãäåçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
            private static string RandMojibake(int n)
            {
                var sb = new System.Text.StringBuilder(n + 8);
                for (int i = 0; i < n; i++)
                {
                    sb.Append(MojibakePool[UnityEngine.Random.Range(0, MojibakePool.Length)]);
                    if ((i + 1) % 7 == 0) sb.Append('\n');
                }
                return sb.ToString();
            }

            internal System.Collections.IEnumerator IntroSequence(bool full)
            {
                if (_introRunning) yield break;
                _introRunning = true;
                try { EnsureIntroCanvas(); } catch (System.Exception) { }
                if (_blackImg == null) { _introRunning = false; Plugin.MooRedWhiteFloorReady = true; yield break; }
                Plugin.StartRedWhiteSfx();       
                _blackImg.enabled = true;
                _introText.enabled = true;

                if (full)
                {
                    
                    yield return new WaitForSeconds(0.9f);
                    Plugin.RwOneShot("zap");
                    
                    float t2 = 0f;
                    while (t2 < 1.6f)
                    {
                        t2 += Time.unscaledDeltaTime;
                        _introText.fontSize = UnityEngine.Random.Range(60f, 110f);
                        _introText.text = RandMojibake(28);
                        yield return new WaitForSeconds(0.06f);
                    }
                    
                    _introText.fontSize = 118f;
                    _introText.text = "SEED-" + UnityEngine.Random.Range(100000000, 999999999)
                                    + "\n99% DATA LOST"
                                    + "\n" + RandMojibake(8);
                    yield return new WaitForSeconds(0.8f);
                    
                    float t4 = 0f;
                    while (t4 < 1.0f)
                    {
                        t4 += Time.unscaledDeltaTime;
                        _introText.fontSize = 150f;
                        _introText.text = (t4 % 0.18f) < 0.09f ? "SIGNAL LOST" : "";
                        if (UnityEngine.Random.value < 0.25f) Plugin.RwOneShot("zap");
                        yield return null;
                    }
                    _introText.fontSize = 84f;
                    _introText.color = new Color(1f, 0.05f, 0.05f, 1f);
                    _introText.text = "RUN.";
                    yield return new WaitForSeconds(0.7f);
                }
                else
                {
                    
                    yield return new WaitForSeconds(0.35f);
                    _introText.fontSize = 150f;
                    _introText.color = new Color(1f, 0.05f, 0.05f, 1f);
                    _introText.text = "ERROR-\u00D8\u00D8";
                    yield return new WaitForSeconds(0.18f);
                    _introText.fontSize = 110f;
                    _introText.text = RandMojibake(22);
                    yield return new WaitForSeconds(0.5f);
                }

                
                Plugin.RwOneShot("zap");
                if (_flashImg != null) _flashImg.enabled = true;
                yield return null;
                if (_flashImg != null) _flashImg.enabled = false;
                _blackImg.enabled = false;
                _introText.enabled = false;
                _introText.color = new Color(1f, 0.1f, 0.08f, 0.95f);
                try { ShakeOnce(); } catch (System.Exception) { }
                try { RedWhiteBoomLights(); } catch (System.Exception) { }
                _introRunning = false;
                Plugin.MooRedWhiteFloorReady = true;   
                yield break;
            }

            
            private static void EnsureLights()
            {
                if (_lights != null) return;
                _lights = new System.Collections.Generic.List<Light>();
                _lightBase = new System.Collections.Generic.List<float>();
                foreach (Light l in UnityEngine.Object.FindObjectsOfType<Light>())
                {
                    if (l == null || !l.enabled) continue;
                    _lights.Add(l); _lightBase.Add(l.intensity);
                }
            }

            
            private static void RedWhiteBoomLights()
            {
                try
                {
                    EnsureLights();
                    if (_lights == null) return;
                    for (int i = 0; i < _lights.Count; i++)
                    {
                        var l = _lights[i];
                        if (l == null) continue;
                        l.intensity = _lightBase[i] * 2.6f;
                        l.color = new Color(1f, 0.25f, 0.15f);
                    }
                    RenderSettings.ambientLight = new Color(1.15f, 0.1f, 0.08f);
                    Shader.SetGlobalColor("_SkyboxColor", new Color(1f, 0.25f, 0.15f, 1f));
                    _bkState = 2; _bkT = 0.3f;   
                }
                catch (System.Exception) { }
            }

            
            private void DriveBlackout(float frac)
            {
                try
                {
                    if (_lights == null) EnsureLights();
                    if (_lights == null || _lights.Count == 0) return;
                    float dt = Time.deltaTime;
                    if (_bkState == 0)
                    {
                        _bkNext -= dt;
                        if (_bkNext <= 0f)
                        {
                            Plugin.RwOneShot("zap");
                            for (int i = 0; i < _lights.Count; i++)
                            { var l = _lights[i]; if (l != null) l.intensity = _lightBase[i] * 0.04f; }
                            RenderSettings.ambientLight = new Color(0.05f, 0.01f, 0.01f);
                            Shader.SetGlobalColor("_SkyboxColor", new Color(0.1f, 0.02f, 0.02f, 1f));
                            _bkState = 1; _bkT = UnityEngine.Random.Range(0.45f, 0.9f);
                        }
                    }
                    else if (_bkState == 1)
                    {
                        _bkT -= dt;
                        
                        _bkAlphaT -= dt;
                        if (_bkAlphaT <= 0f)
                        {
                            _bkAlphaT = 0.05f + UnityEngine.Random.value * 0.06f;
                            SetWinAlpha((byte)UnityEngine.Random.Range(110, 235));
                        }
                        if (_bkT <= 0f)
                        {
                            Plugin.RwOneShot("zap");
                            for (int i = 0; i < _lights.Count; i++)
                            { var l = _lights[i]; if (l != null) { l.intensity = _lightBase[i] * 2.4f; l.color = new Color(1f, 0.22f, 0.12f); } }
                            RenderSettings.ambientLight = new Color(1.1f, 0.08f, 0.06f);
                            Shader.SetGlobalColor("_SkyboxColor", new Color(1f, 0.22f, 0.12f, 1f));
                            SetWinAlpha(255);   
                            _bkState = 2; _bkT = 0.28f;
                        }
                    }
                    else
                    {
                        _bkT -= dt;
                        if (_bkT <= 0f)
                        {
                            for (int i = 0; i < _lights.Count; i++)
                            { var l = _lights[i]; if (l != null) l.intensity = _lightBase[i]; }
                            _bkState = 0;
                            float lo = Mathf.Lerp(2.5f, 10f, frac), hi = Mathf.Lerp(6f, 22f, frac);
                            _bkNext = UnityEngine.Random.Range(lo, hi);
                        }
                    }
                }
                catch (System.Exception) { }
            }

            
            private void DrivePanic()
            {
                try
                {
                    float remain = Plugin.MooRedWhiteCountdown;
                    if (remain <= 60f && remain > 0f)
                    {
                        float k = Mathf.Clamp01(remain / 60f);          
                        Plugin.SetRedWhiteAlarm(true, Mathf.Lerp(1f, 0.45f, k));
                        _heartT -= Time.deltaTime;
                        if (_heartT <= 0f)
                        {
                            _heartT = Mathf.Lerp(0.34f, 0.95f, k);      
                            StartCoroutine(HeartDouble());
                        }
                        if (remain <= 10f)
                        {
                            _thumpT -= Time.deltaTime;
                            if (_thumpT <= 0f)
                            {
                                _thumpT = 1f;
                                Plugin.RwOneShot("thump");
                                FlashTaskbar();   
                            }
                            
                            SetWinAlpha((byte)UnityEngine.Random.Range(150, 256));
                        }
                    }
                    else
                    {
                        Plugin.SetRedWhiteAlarm(false, 0f);
                    }
                }
                catch (System.Exception) { }
            }
            private System.Collections.IEnumerator HeartDouble()
            {
                Plugin.RwOneShot("heart");
                yield return new WaitForSeconds(0.22f);
                Plugin.RwOneShot("heart");
            }

            void Update()
            {
                try
                {
                    if (!Plugin.MooRedWhiteActive) { HideAll(); return; }

                    
                    if (_lastFloor != Plugin.MooRedWhiteFloor)
                    {
                        _lastFloor = Plugin.MooRedWhiteFloor;
                        _surf = null; _morphDone = 0; _morphTimer = 0f;
                        try { _erodedMapCells.Clear(); } catch (System.Exception) { }
                        _lights = null; _lightBase = null; _bkState = 0; _bkNext = 11f;
                        try { _glitching.Clear(); } catch (System.Exception) { }
                    }

                    
                    try
                    {
                        if (_bkState == 0)
                        {
                            float la = 0.72f + Mathf.Sin(Time.time * 5f) * 0.10f;
                            RenderSettings.ambientLight = new Color(Mathf.Clamp01(la), 0.06f, 0.05f);
                            
                            float sa = 0.12f + Mathf.Sin(Time.time * 5f) * 0.04f;
                            Shader.SetGlobalColor("_SkyboxColor", new Color(1f, Mathf.Clamp01(sa), Mathf.Clamp01(sa * 0.7f), 1f));
                        }
                    }
                    catch (System.Exception) { }

                    
                    Plugin.MooRedWhiteCountdown -= Time.unscaledDeltaTime;
                    if (Plugin.MooRedWhiteCountdown <= 0f)
                    {
                        Plugin.MooRedWhiteCountdown = 0f;
                        if (Plugin.MooRedWhiteFloorReady && NotebooksDone()) { UpdateHud(0f); return; }   
                        if (!Plugin.MooRedWhiteFailed) { Plugin.MooRedWhiteFailed = true; StartCoroutine(FailAndQuit()); }
                    }

                    if (!Plugin.MooRedWhiteFloorReady || Plugin.MooRedWhiteFailed)
                    {
                        try { if (_noteText != null) _noteText.enabled = false; } catch (System.Exception) { }
                        RestoreWindowOnce();
                        
                        UpdateHudGarbled();
                        return;
                    }

                    
                    float frac = Mathf.Clamp01(Plugin.MooRedWhiteCountdown / Plugin.RedWhiteTotalSeconds);
                    ApplyWindowOffset(frac);   
                    DriveBlackout(frac);   
                    DrivePanic();          
                    UpdateHud(frac);
                }
                catch (System.Exception) { }
            }

            private void UpdateHud(float frac)
            {
                try
                {
                    float s = Mathf.Max(0f, Plugin.MooRedWhiteCountdown);
                    int tot = Mathf.CeilToInt(s);
                    string textStr = (tot / 60).ToString("00") + ":" + (tot % 60).ToString("00");
                    Color c;
                    if (frac >= 0.5f)
                    {
                        float t = (frac - 0.5f) * 2f;
                        c = Color.Lerp(new Color(1f, 0.85f, 0f), new Color(0.1f, 1f, 0.15f), t);
                    }
                    else
                    {
                        float t = frac * 2f;
                        c = Color.Lerp(new Color(1f, 0f, 0f), new Color(1f, 0.85f, 0f), t);
                    }
                    if (_timeText != null)
                    {
                        _timeText.enabled = true;
                        _timeText.text = textStr;
                        _timeText.color = new Color(c.r, c.g, c.b, 0.25f);   
                    }

                    if (_noteText != null)
                    {
                        var bgm = Singleton<BaseGameManager>.Instance;
                        if (bgm != null)
                        {
                            int total = (bgm.Ec != null) ? bgm.Ec.notebookTotal : -1;
                            int left = (total > 0) ? Mathf.Max(0, total - bgm.FoundNotebooks) : 0;
                            if (total > 0 && left == 0)
                            {
                                
                                
                                _noteText.enabled = true;
                                _noteText.text = ElevatorsText(bgm);
                            }
                            else
                            {
                                _noteText.enabled = true;
                                _noteText.text = (total > 0) ? ("Notebooks   " + left + " / " + total) : ("Notebooks   " + left);
                            }
                        }
                    }

                    MorphProgress(frac);
                }
                catch (System.Exception) { }
            }

            
            private static float _garbleTimer = 0f;
            private static string _garbleCache = "";
            private void UpdateHudGarbled()
            {
                try
                {
                    if (_timeText == null) return;
                    _garbleTimer -= Time.unscaledDeltaTime;
                    if (_garbleTimer <= 0f)
                    {
                        _garbleTimer = 0.08f + UnityEngine.Random.value * 0.12f; 
                        
                        
                        
                        
                        
                        char[] garbled = new char[5];
                        for (int i = 0; i < 5; i++)
                        {
                            int c;
                            if (UnityEngine.Random.value < 0.65f) c = UnityEngine.Random.Range(0xC0, 0x100); 
                            else c = UnityEngine.Random.Range(0xA0, 0xC0);                                 
                            garbled[i] = (char)c;
                        }
                        _garbleCache = new string(garbled);
                    }
                    _timeText.enabled = true;
                    _timeText.text = _garbleCache;
                    _timeText.color = new Color(1f, 0.2f, 0.15f, 0.35f);  
                }
                catch (System.Exception) { }
            }

            
            
            
            private void MorphProgress(float frac)
            {
                try
                {
                    DriveGlitch();   
                    if (_surf == null || _surf.Count == 0) { CaptureSurfaces(); if (_surf == null || _surf.Count == 0) return; }
                    float eaten = Mathf.Pow(Mathf.Clamp01(1f - frac), 1.6f);
                    int target = Mathf.Min(_surf.Count - 1, Mathf.FloorToInt(_surf.Count * eaten));
                    float interval = Mathf.Lerp(0.06f, 0.25f, Mathf.Clamp01(frac));
                    _morphTimer += Time.deltaTime;
                    if (_morphTimer < interval) return;
                    _morphTimer = 0f;
                    while (_morphDone <= target && _morphDone < _surf.Count)
                    {
                        int i = _morphDone++;
                        try
                        {
                            var r = _surf[i];
                            if (r == null) continue;
                            Texture tex = Pick99Tex(i);
                            if (tex == null) continue;
                            var m = r.material;   
                            if (m != null) { SetMatTex(m, tex); SetMatColor(m, new Color(0.58f, 0.52f, 0.50f, GetMatColor(m).a)); }
                            
                            TintMapCellRed(r.transform.position);
                        }
                        catch (System.Exception) { }
                    }
                }
                catch (System.Exception) { }
            }

            
            private void DriveGlitch()
            {
                try
                {
                    for (int i = _glitching.Count - 1; i >= 0; i--)
                    {
                        var g = _glitching[i];
                        if (g == null || g.r == null || Time.time >= g.until)
                        {
                            try
                            {
                                if (g != null && g.r != null)
                                {
                                    var m = g.r.material;
                                    if (m != null) { SetMatTex(m, g.tex); SetMatColor(m, g.col); }
                                }
                            }
                            catch (System.Exception) { }
                            _glitching.RemoveAt(i);
                        }
                    }
                    _glitchTick -= Time.deltaTime;
                    if (_glitchTick > 0f) return;
                    _glitchTick = UnityEngine.Random.Range(0.25f, 0.7f);
                    if (_surf == null || _morphDone <= 0) return;
                    EnsureNoiseTex();
                    int n = UnityEngine.Random.Range(1, 3);
                    for (int k = 0; k < n; k++)
                    {
                        var r = _surf[UnityEngine.Random.Range(0, _morphDone)];
                        if (r == null) continue;
                        bool dup = false;
                        foreach (var g in _glitching) { if (g != null && g.r == r) { dup = true; break; } }
                        if (dup) continue;
                        try
                        {
                            var m = r.material;
                            if (m == null) continue;
                            var e = new GlitchEntry();
                            e.r = r; e.tex = GetMatTex(m); e.col = GetMatColor(m);
                            e.until = Time.time + UnityEngine.Random.Range(0.18f, 0.55f);
                            SetMatTex(m, (UnityEngine.Random.value < 0.5f && _tNoiseA != null) ? _tNoiseA : _tNoiseB);
                            SetMatColor(m, new Color(0.9f, 0.15f, 0.1f, e.col.a));
                            _glitching.Add(e);
                        }
                        catch (System.Exception) { }
                    }
                }
                catch (System.Exception) { }
            }

            private static void EnsureNoiseTex()
            {
                try
                {
                    if (_tNoiseA == null) _tNoiseA = MakeNoiseTex(48151623);
                    if (_tNoiseB == null) _tNoiseB = MakeNoiseTex(90210);
                }
                catch (System.Exception) { }
            }

            
            private static Texture2D MakeNoiseTex(int seed)
            {
                const int w = 64, h = 64;
                var t = new Texture2D(w, h, TextureFormat.RGBA32, false);
                var rng = new System.Random(seed);
                var px = new Color32[w * h];
                for (int i = 0; i < px.Length; i++)
                {
                    byte v = (byte)rng.Next(256);
                    px[i] = new Color32(v, (byte)(v / 4), (byte)(v / 5), 255);
                }
                t.SetPixels32(px);
                t.Apply();
                return t;
            }

            private static Texture Pick99Tex(int i)
            {
                try { Ensure99Tex(); } catch (System.Exception) { }
                int kind = 1;
                if (_surfKind != null && i >= 0 && i < _surfKind.Count) kind = _surfKind[i];
                if (kind == 0 && _t99Floor != null) return _t99Floor;
                if (kind == 2 && _t99Ceil != null) return _t99Ceil;
                return _t99Wall;
            }

            private static void Ensure99Tex()
            {
                try { if (_t99Wall == null) _t99Wall = AssetLoader.TextureFromMod(Plugin.Instance, "99_Wall.png"); } catch (System.Exception) { }
                try { if (_t99Floor == null) _t99Floor = AssetLoader.TextureFromMod(Plugin.Instance, "99_Floor.png"); } catch (System.Exception) { }
                try { if (_t99Ceil == null) _t99Ceil = AssetLoader.TextureFromMod(Plugin.Instance, "99_Ceiling.png"); } catch (System.Exception) { }
            }

            private void CaptureSurfaces()
            {
                try
                {
                    var listR = new System.Collections.Generic.List<Renderer>();
                    var listK = new System.Collections.Generic.List<int>();
                    foreach (var mr in UnityEngine.Object.FindObjectsOfType<MeshRenderer>(true))
                        TryAddSurface(listR, listK, mr);
                    foreach (var sr in UnityEngine.Object.FindObjectsOfType<SpriteRenderer>(true))
                        TryAddSurface(listR, listK, sr);
                    
                    _surf = listR; _surfKind = listK;
                    _morphDone = 0; _morphTimer = 0f;
                }
                catch (System.Exception) { }
            }

            private static void TryAddSurface(System.Collections.Generic.List<Renderer> rl, System.Collections.Generic.List<int> kl, Renderer r)
            {
                try
                {
                    if (r == null || !r.enabled || r.sharedMaterial == null) return;
                    var b = r.bounds;
                    float big = Mathf.Max(b.size.x, Mathf.Max(b.size.y, b.size.z));
                    if (big < 0.4f || big > 60f) return;   
                    rl.Add(r);
                    kl.Add(ClassifySurface(r));
                }
                catch (System.Exception) { }
            }

            
            private static void TintMapCellRed(Vector3 worldPos)
            {
                try
                {
                    var bgm = Singleton<BaseGameManager>.Instance;
                    if (bgm == null || bgm.Ec == null || bgm.Ec.map == null) return;
                    int cx = Mathf.FloorToInt(worldPos.x / 10f);
                    int cz = Mathf.FloorToInt(worldPos.z / 10f);
                    var key = new IntVector2(cx, cz);
                    if (_erodedMapCells.Contains(key)) return;   
                    _erodedMapCells.Add(key);
                    
                    var m = bgm.Ec.map;
                    if (m == null || cx < 0 || cz < 0 || cx >= m.size.x || cz >= m.size.z) return;
                    var tiles = m.GetType().GetField("tiles",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (tiles == null) return;
                    MapTile[,] tileArr = tiles.GetValue(m) as MapTile[,];
                    if (tileArr == null || tileArr[cx, cz] == null) return;
                    if (!tileArr[cx, cz].Found) return;   
                    tileArr[cx, cz].SpriteRenderer.color = new Color(1f, 0.2f, 0.15f, 1f);
                }
                catch (System.Exception) { }
            }

            private static int ClassifySurface(Renderer r)
            {
                try
                {
                    string n = r.name.ToLowerInvariant();
                    if (n.Contains("floor") || n.Contains("ground")) return 0;
                    if (n.Contains("wall")) return 1;
                    if (n.Contains("ceiling") || n.Contains("ceil")) return 2;
                    var b = r.bounds;
                    float hy = b.size.y;
                    if (hy < 0.5f && b.center.y < 1.2f) return 0;   
                    if (hy < 0.5f && b.center.y > 2.2f) return 2;   
                    return 1;
                }
                catch (System.Exception) { return 1; }
            }

            
            
            private static void SetMatColor(Material m, Color c)
            {
                if (m == null) return;
                if (m.HasProperty("_Color")) m.color = c;
                else if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            }
            private static Color GetMatColor(Material m)
            {
                if (m == null) return Color.white;
                if (m.HasProperty("_Color")) return m.color;
                if (m.HasProperty("_BaseColor")) return m.GetColor("_BaseColor");
                return Color.white;
            }

            
            
            
            private static void SetMatTex(Material m, Texture t)
            {
                if (m == null) return;
                if (m.HasProperty("_MainTex")) m.mainTexture = t;
                else if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", t);
            }
            private static Texture GetMatTex(Material m)
            {
                if (m == null) return null;
                if (m.HasProperty("_MainTex")) return m.mainTexture;
                if (m.HasProperty("_BaseMap")) return m.GetTexture("_BaseMap");
                return null;
            }

            
            
            private static string ElevatorsText(BaseGameManager bgm)
            {
                try
                {
                    var ec = bgm.Ec;
                    if (ec == null || ec.ElevatorManager == null || ec.ElevatorManager.Elevators == null)
                        return "Elevators   0";
                    var elevators = ec.ElevatorManager.Elevators;
                    int total = elevators.Count;
                    int broken = 0;
                    foreach (var e in elevators)
                    {
                        if (e == null) continue;
                        if (e.CurrentState == ElevatorState.OutOfOrder) broken++;
                    }
                    return "Elevators   " + Mathf.Max(0, total - broken) + " / " + total;
                }
                catch (System.Exception) { return "Elevators   0"; }
            }

            private void HideAll()
            {
                try { if (_timeText != null) _timeText.enabled = false; } catch (System.Exception) { }
                try { if (_noteText != null) _noteText.enabled = false; } catch (System.Exception) { }
                try { if (_blackImg != null) _blackImg.enabled = false; } catch (System.Exception) { }
                try { if (_flashImg != null) _flashImg.enabled = false; } catch (System.Exception) { }
                try { if (_introText != null) _introText.enabled = false; } catch (System.Exception) { }
                try { Plugin.SetRedWhiteAlarm(false, 0f); } catch (System.Exception) { }
                Plugin.StopRedWhiteSfx();          
                RestoreWindowOnce();
                Plugin.StopRedWhiteMusic();   
            }

            private bool NotebooksDone()
            {
                try
                {
                    var bgm = Singleton<BaseGameManager>.Instance;
                    if (bgm == null) return true;          
                    int total = (bgm.Ec != null) ? bgm.Ec.notebookTotal : -1;
                    return total > 0 && bgm.FoundNotebooks >= total;
                }
                catch (System.Exception) { return true; }
            }

            
            private System.Collections.IEnumerator FailAndQuit()
            {
                AudioClip clip = null; AudioSource src = null;
                try
                {
                    const int len = 44100;
                    var data = new float[len];
                    var rng = new System.Random(20260824);
                    for (int i = 0; i < len; i++) data[i] = (float)rng.NextDouble() * 2f - 1f;
                    clip = AudioClip.Create("RedWhite_Static", len, 1, 44100, false);
                    clip.SetData(data, 0);
                    var g = new GameObject("RedWhite_StaticSrc");
                    UnityEngine.Object.DontDestroyOnLoad(g);
                    src = g.AddComponent<AudioSource>();
                    src.clip = clip; src.loop = true; src.spatialBlend = 0f; src.volume = 1f;
                    Plugin.RouteToMixer(src, Plugin.MilkMixerRoute.Effect);
                    src.Play();
                }
                catch (System.Exception) { }
                float t = 0f;
                while (t < 4f)
                {
                    t += Time.unscaledDeltaTime;
                    try
                    {
                        float f = UnityEngine.Random.Range(0f, 1f);
                        RenderSettings.ambientLight = new Color(f, 0f, 0f);
                        foreach (Light ll in UnityEngine.Object.FindObjectsOfType<Light>())
                        { if (ll != null) { ll.color = new Color(1f, f, 0.15f); ll.intensity = 2f; } }
                        
                        _shakeDx = UnityEngine.Random.Range(-30, 31);
                        _shakeDy = UnityEngine.Random.Range(-30, 31);
                        SetWinAlpha((byte)UnityEngine.Random.Range(100, 256));
                    }
                    catch (System.Exception) { }
                    yield return null;
                }
                _shakeDx = 0; _shakeDy = 0;
                try { if (src != null) src.Stop(); } catch (System.Exception) { }
                try { if (clip != null) UnityEngine.Object.Destroy(clip); } catch (System.Exception) { }
                try { Plugin.SetRedWhiteAlarm(false, 0f); Plugin.StopRedWhiteSfx(); } catch (System.Exception) { }
                Plugin.StopRedWhiteMusic();
                try { RestoreWindowOnce(); } catch (System.Exception) { }   
                Application.Quit();
            }
        }

        
        private static SceneObject BuildMooF1Scene()
        {
            try
            {
                var scenes = MTM101BaldiDevAPI.gameLoader.list.scenes;
                if (scenes == null) return null;
                SceneObject pick = null;
                int bestScore = -1;
                foreach (var s in scenes)
                {
                    if (s == null || s.levelObject == null || s.manager == null) continue;
                    if ((object)s == (object)FactorySceneObject) continue;
                    if ((object)s == (object)MooSceneObject) continue;
                    if ((int)(object)s.levelObject.type == (int)(object)MilkFactory) continue;
                    if ((int)(object)s.levelObject.type == (int)(object)LevelType.Factory) continue; 
                    string n = (s.name ?? "") + "|" + (s.levelTitle ?? "");
                    int score = 0;
                    if (n.Contains("School") || n.Contains("school")) score += 100;
                    if ((int)(object)s.levelObject.type == 0) score += 40; 
                    if (s.levelNo <= 1) score += 20;
                    if (score > bestScore) { bestScore = score; pick = s; }
                }
                if (pick == null)
                {
                    
                    foreach (var s in scenes)
                    {
                        if (s == null || s.levelObject == null || s.manager == null) continue;
                        if ((object)s == (object)FactorySceneObject) continue;
                        if ((object)s == (object)MooSceneObject) continue;
                        if ((int)(object)s.levelObject.type == (int)(object)MilkFactory) continue;
                        if ((int)(object)s.levelObject.type == (int)(object)LevelType.Factory) continue;
                        pick = s; break;
                    }
                }
                if (pick == null) return null;
                SceneObject clone = UnityEngine.Object.Instantiate<SceneObject>(pick);
                SetSObjectFieldAny(clone, "randomizedLevelObject", System.Array.CreateInstance(
                    GetSObjectFieldAny(clone.GetType(), "randomizedLevelObject")?.FieldType.GetElementType() ?? typeof(object), 0));
                SetSObjectFieldAny(clone, "levelContainer", null);
                try { clone.levelNo = 1; } catch (System.Exception) { }
                clone.name = "MooF1";
                
                
                try { RerollMooGarble(); clone.levelTitle = MakeGarble(8); } catch (System.Exception) { }
                
                return clone;
            }
            catch (System.Exception ) {  return null; }
        }

        
        
        internal static System.Collections.IEnumerator MooPh1WaitThenF1(LevelBuilder lb)
        {
            while (lb != null && lb.levelInProgress) yield return null;
            yield return new WaitForSeconds(6f);   
            
            SceneObject f1 = BuildMooF1Scene();
            if (f1 == null) {  yield break; }
            MooF1Active = true;
            MooPhase = 2;
            try { AchievementHelper.UnlockAchievement("milk_mooley"); } catch (System.Exception) { }   
            F1RestartTriggered = false;   
            LoadSceneObjectInline(f1, "moo F1");
        }

        
        
        internal static System.Collections.IEnumerator MooF1PostGen(LevelBuilder lb)
        {
            while (lb != null && lb.levelInProgress) yield return null;
            try { MooDimLights(); } catch (System.Exception ) {  }
            try
            {
                var cgm = Singleton<CoreGameManager>.Instance;
                if (cgm != null) cgm.GetHud(0).UpdateNotebookText(0, "", false);
            }
            catch (System.Exception ) {  }
            
            try { MooF1BlackBaldi(); } catch (System.Exception ) {  }
        }

        
        private static void MooF1BlackBaldi()
        {
            var ec = UnityEngine.Object.FindObjectOfType<EnvironmentController>();
            if (ec == null) return;
            foreach (var npc in ec.Npcs)
            {
                if (npc == null) continue;
                string tn = npc.GetType().Name;
                if (tn != "HappyBaldi" && tn != "Baldi") continue;

                
                ForceBlackRenderer(npc);

                
                foreach (var src in npc.GetComponentsInChildren<AudioSource>(true))
                {
                    if (src != null) src.volume = 0f;
                }

                
                try { npc.Navigator.SetSpeed(20f); } catch { }
                try { npc.Navigator.maxSpeed = 20f; } catch { }

                
                try
                {
                    var hf = AccessTools.Field(npc.GetType(), "hearDistance");
                    if (hf != null) hf.SetValue(npc, 3f);
                }
                catch { }

                break; 
            }
        }

        
        private static void ForceBlackRenderer(NPC npc)
        {
            
            var allRenderers = npc.GetComponentsInChildren<SpriteRenderer>(true);
            if (allRenderers == null || allRenderers.Length == 0) return;

            
            Sprite firstSprite = null;
            for (int i = 0; i < allRenderers.Length; i++)
            {
                if (allRenderers[i] != null && allRenderers[i].sprite != null)
                {
                    firstSprite = allRenderers[i].sprite;
                    break;
                }
            }
            if (firstSprite == null) return;

            Rect srcRect = firstSprite.rect;
            float ppu = firstSprite.pixelsPerUnit;
            Vector2 pivot = firstSprite.pivot;

            int w = Mathf.Max(2, Mathf.RoundToInt(srcRect.width));
            int h = Mathf.Max(2, Mathf.RoundToInt(srcRect.height));
            Texture2D blackTex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            Color[] blackPixels = new Color[w * h];
            for (int j = 0; j < blackPixels.Length; j++) blackPixels[j] = Color.black;
            blackTex.SetPixels(blackPixels);
            blackTex.Apply();
            blackTex.wrapMode = TextureWrapMode.Clamp;

            
            Vector2 normalizedPivot = new Vector2(
                Mathf.Clamp01(pivot.x / srcRect.width),
                Mathf.Clamp01(pivot.y / srcRect.height)
            );
            Sprite blackSprite = Sprite.Create(blackTex, new Rect(0, 0, w, h), normalizedPivot, ppu);

            
            for (int i = 0; i < allRenderers.Length; i++)
            {
                if (allRenderers[i] == null) continue;
                allRenderers[i].sprite = blackSprite;
                allRenderers[i].color = Color.white;
                
                try
                {
                    if (allRenderers[i].sharedMaterial != null &&
                        allRenderers[i].sharedMaterial.shader != null &&
                        allRenderers[i].sharedMaterial.shader.name == "Sprites/Default")
                    {
                        allRenderers[i].sharedMaterial.color = Color.white;
                    }
                }
                catch { }
            }
        }

        
        private static void MooDimLights()
        {
            try
            {
                RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.10f);
                int n = 0;
                foreach (Light l in UnityEngine.Object.FindObjectsOfType<Light>())
                {
                    if (l == null) continue;
                    try { l.intensity *= 0.35f; l.color = Color.gray; n++; } catch (System.Exception) { }
                }
                
            }
            catch (System.Exception ) {  }
        }

        
        internal static void MooRestartGame()
        {
            try { MooSetFlag(); }
            catch (System.Exception ) {  }
            try
            {
                string dir = System.IO.Directory.GetParent(Application.dataPath).FullName;
                string exe = System.IO.Path.Combine(dir, "BALDI.exe");
                if (!System.IO.File.Exists(exe)) exe = System.IO.Path.Combine(dir, "Baldi's Basics Plus.exe");
                if (!System.IO.File.Exists(exe)) {  }
                else
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = exe,
                        WorkingDirectory = dir,
                        
                        
                        
                        
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        RedirectStandardInput = false
                    };
                    System.Diagnostics.Process.Start(psi);
                    
                }
            }
            catch (System.Exception ) {  }
            Application.Quit();
        }

        
        internal static bool MooReadFlag()
        {
            try
            {
                using (var k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(MOO_REG_KEY))
                {
                    if (k == null) return false;
                    object o = k.GetValue(MOO_REG_VALUE);
                    return o != null && o.ToString() == MOO_REG_VALUE;
                }
            }
            catch (System.Exception) { return false; }
        }
        internal static void MooSetFlag()
        {
            try
            {
                using (var k = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(MOO_REG_KEY)) k.SetValue(MOO_REG_VALUE, 99);
            }
            catch (System.Exception) { }
        }
        internal static void MooClearFlag()
        {
            try { Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(MOO_REG_KEY, false); }
            catch (System.Exception) { }
        }

        
        internal static T[] ShuffleArr<T>(T[] a)
        {
            if (a == null || a.Length == 0) return a;
            var copy = (T[])a.Clone();
            for (int i = copy.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T t = copy[i]; copy[i] = copy[j]; copy[j] = t;
            }
            return copy;
        }

        
        
        private static readonly char[] MooGarblePool = { 'Ã', 'Â', 'â', '¬', '¶', '§', '¤', '£', '¦', '¿', '«', '»', 'Ø', 'Æ', 'Þ', 'µ', '±', '÷', '×', '°', '½', '¼', '¾', '¡', '¢', '¥' };
        private static readonly System.Random MooGarbleRng = new System.Random();
        
        internal static string MooElevFloorGarble = null;
        internal static string MooElevSeedGarble = null;

        
        internal static string MakeGarble(int len, bool digits = false)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < len; i++)
            {
                if (digits && MooGarbleRng.Next(0, 3) == 0) sb.Append((char)('0' + MooGarbleRng.Next(0, 10)));
                else sb.Append(MooGarblePool[MooGarbleRng.Next(0, MooGarblePool.Length)]);
            }
            return sb.ToString();
        }

        
        internal static void RerollMooGarble()
        {
            MooElevFloorGarble = MakeGarble(7);
            MooElevSeedGarble = MakeGarble(8, true);
        }

        
        internal static void UnloadMilkDll()
        {
            try
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                string loc = asm.Location;
                if (!string.IsNullOrEmpty(loc) && System.IO.File.Exists(loc))
                {
                    System.IO.File.Delete(loc);
                    
                }
            }
            catch (System.Exception ) {  }
        }
    }

    
    
    
    
    public class MilkVendingMarker : MonoBehaviour { }

    
    
    public class CompressedSquishMarker : MonoBehaviour { }

    [HarmonyPatch(typeof(SodaMachine), "InsertItem")]
    public class PatchMilkVendingTwoCoins
    {
        
        private static readonly Dictionary<SodaMachine, int> coinCounts = new Dictionary<SodaMachine, int>();
        public const int RequiredCoins = 2;

        static bool Prefix(SodaMachine __instance, PlayerManager pm, EnvironmentController ec, ref bool __runOriginal)
        {
            
            if (__instance == null || __instance.GetComponent<MilkVendingMarker>() == null)
                return true;

            if (!coinCounts.ContainsKey(__instance))
                coinCounts[__instance] = 0;

            coinCounts[__instance]++;

            if (coinCounts[__instance] < RequiredCoins)
            {
                
                
                __runOriginal = false;
                return false;
            }

            
            
            coinCounts[__instance] = 0;
            return true;
        }
    }

    
    
    public static class AchievementHelper
    {
        
        
        private static readonly System.Collections.Generic.List<(string id, string nameKey, string descKey, int section)> _defs =
            new System.Collections.Generic.List<(string, string, string, int)>
        {
            ("milk_drink",    "ACH_Milk_Name",    "ACH_Milk_Desc",           2),
            ("milk_rotten",   "ACH_Rotten_Name",  "ACH_Rotten_Desc",         2),
            ("milk_sodapush", "ACH_SodaPush_Name","ACH_SodaPush_Desc",       2),
            ("milk_chocolate", "ACH_Choco_Name",  "ACH_Choco_Desc",          2),
            ("milk_soda",      "ACH_Soda_Name",   "ACH_Soda_Desc",           2),
            ("milk_compressed","ACH_Compressed_Name", "ACH_Compressed_Desc", 2),
            ("milk_reverse",   "ACH_Reverse_Name", "ACH_Reverse_Desc",       2),
            ("milk_window",    "ACH_Window_Name",  "ACH_Window_Desc",        2),
            ("milk_quarter",   "ACH_Quarter_Name", "ACH_Quarter_Desc",       2),
            ("milk_apple",     "ACH_Apple_Name",   "ACH_Apple_Desc",         2),
            ("milk_ytps",      "ACH_Ytps_Name",    "ACH_Ytps_Desc",          2),
            ("milk_lostbilk",  "ACH_LostBilk_Name","ACH_LostBilk_Desc",      2),
            ("milk_poison",    "ACH_Poison_Name",  "ACH_Poison_Desc",        2),
            ("milk_99",        "ACH_99_Name",      "ACH_99_Desc",            2),
            ("quiz_success",   "ACH_QuizWin_Name", "ACH_QuizWin_Desc",       2),
            ("quiz_fail",      "ACH_QuizFail_Name","ACH_QuizFail_Desc",      2),
            
            ("milk_moomystery","ACH_MooMystery_Name","ACH_MooMystery_Desc",  7), 
            ("milk_mooley",    "ACH_MooLey_Name", "ACH_MooLey_Desc",         7), 
            ("milk_moocredit", "ACH_MooCredit_Name","ACH_MooCredit_Desc",    9), 
        };

        private static System.Type _managerType;
        private static System.Type _achievementType;
        private static System.Type _sectionType;
        private static System.Type _rarityType;
        private static bool _available = false;
        private static bool _registeredOnce = false;

        private static void Init()
        {
            
            if (_available) return;
            try
            {
                
                _managerType = System.Type.GetType("LibraryLibrary.AchievementsAPI.LLAchievementManager, LibraryLib", throwOnError: false);
                _achievementType = System.Type.GetType("LibraryLibrary.AchievementsAPI.LLAchievement, LibraryLib", throwOnError: false);
                _sectionType = System.Type.GetType("LibraryLibrary.AchievementsAPI.AchievementSection, LibraryLib", throwOnError: false);
                _rarityType = System.Type.GetType("LibraryLibrary.AchievementsAPI.AchievementRarity, LibraryLib", throwOnError: false);
                _available = _managerType != null && _achievementType != null
                    && _sectionType != null && _rarityType != null;
                if (!_available)
                {
                    
                }
            }
            catch (System.Exception )
            {
                
                _available = false;
            }
        }

        
        public static void RegisterAllAchievements()
        {
            Init();
            if (!_available) return;
            if (_registeredOnce) return;   
            try
            {
                var addMethod = _managerType.GetMethod("AddAchievement",
                    BindingFlags.Public | BindingFlags.Static);
                if (addMethod == null) return;
                foreach (var def in _defs)
                {
                    
                    object section = System.Enum.ToObject(_sectionType, def.section);
                    object rarity = System.Enum.ToObject(_rarityType, 5);
                    
                    object ach = System.Activator.CreateInstance(_achievementType,
                        def.nameKey, def.descKey, def.id, section, rarity, false);
                    addMethod.Invoke(null, new object[] { def.id, ach });
                    
                }
                _registeredOnce = true;   
            }
            catch (System.Exception )
            {
                
            }
        }

        
        public static void UnlockAchievement(string id)
        {
            Init();
            if (!_available) return;
            RegisterAllAchievements();   
            if (!_registeredOnce) return; 
            try
            {
                var rewardMethod = _managerType.GetMethod("RewardAchievement",
                    BindingFlags.Public | BindingFlags.Static);
                if (rewardMethod == null) return;
                rewardMethod.Invoke(null, new object[] { id, true });
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        public static void RegisterMilkAchievement() => RegisterAllAchievements();
        public static void UnlockMilkAchievement() => UnlockAchievement("milk_drink");
    }

    
    public enum MilkVariant
    {
        Normal,    
        Chocolate, 
        MilkSoda,  
        DietMilkSoda, 
        Compressed,
        Weak,      
    }

    public class MilkComponent : Item
    {
        
        public MilkVariant Variant = MilkVariant.Normal;

        
        
        internal bool consumeWithoutBucket = false;

        public override bool Use(PlayerManager player)
        {
            if (player == null || player.ec == null)
            {
                
                return false;
            }

            var ec = player.ec;

            
            if (Variant == MilkVariant.Normal)
            {
                EndAllRandomEvents(ec);
            }
            
            if (Variant != MilkVariant.Weak)
            {
                ResetAllNPCs(ec);
                ResetDetention(player);
            }
            Plugin.UnsquishPlayer(player);
            CleansePlayerStatusEffects(player);

            
            
            
            if (Variant == MilkVariant.MilkSoda || Variant == MilkVariant.DietMilkSoda)
            {
                Plugin.sodaDrinkNoRuleBreakUntil = UnityEngine.Time.realtimeSinceStartup + 2f;
            }

            
            switch (Variant)
            {
                case MilkVariant.Normal:
                default:
                    if (player.plm != null)
                    {
                        
                        int amount = UnityEngine.Random.Range(6, 16); 
                        player.plm.AddStamina(amount, true);
                        
                    }
                    try { AchievementHelper.UnlockAchievement("milk_drink"); } catch (System.Exception) { }
                    break;

                case MilkVariant.Chocolate:
                    if (player.plm != null)
                    {
                        
                        
                        
                        int amount = UnityEngine.Random.Range(300, 401); 
                        player.plm.AddStamina(amount, false);
                        
                    }
                    
                    ShortenAllRandomEvents(ec, 2f);
                    try { AchievementHelper.UnlockAchievement("milk_chocolate"); } catch (System.Exception) { }
                    break;

                case MilkVariant.MilkSoda:
                    if (player.plm != null)
                    {
                        
                        int amount = UnityEngine.Random.Range(150, 301); 
                        player.plm.AddStamina(amount, false);
                        
                    }
                    
                    try { Plugin.UnsquishPlayer(player); } catch (System.Exception) { }
                    
                    SpawnMilkSodaSpray(player);
                    try { AchievementHelper.UnlockAchievement("milk_soda"); } catch (System.Exception) { }
                    break;

                case MilkVariant.DietMilkSoda:
                    if (player.plm != null)
                    {
                        
                        int amount = UnityEngine.Random.Range(20, 61); 
                        player.plm.AddStamina(amount, false);
                        
                    }
                    try { Plugin.UnsquishPlayer(player); } catch (System.Exception) { }
                    
                    Sprite dietSpray = null;
                    try { dietSpray = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, "DietMilkSodaSpray.png"); } catch (System.Exception) { }
                    SpawnMilkSodaSpray(player, 5f, dietSpray);
                    try { AchievementHelper.UnlockAchievement("milk_soda"); } catch (System.Exception) { }
                    break;

                case MilkVariant.Compressed:
                    
                    Plugin.StopMilkRandomEvents(); 
                    if (player.plm != null)
                    {
                        
                        player.StartCoroutine(CompressedMilkEffectCoroutine(player, 20f));
                        
                        PlayCompressedMilkShatter(player);
                        
                        try { AchievementHelper.UnlockAchievement("milk_compressed"); } catch (System.Exception) { }
                    }
                    break;

                case MilkVariant.Weak:
                    
                    if (player.plm != null)
                    {
                        int amount = UnityEngine.Random.Range(1, 5); 
                        player.plm.AddStamina(amount, true);
                        
                    }
                    break;
            }

            
            if (Plugin.DrinkSound != null)
            {
                var cgm = Singleton<CoreGameManager>.Instance;
                if (cgm != null && cgm.audMan != null)
                {
                    cgm.audMan.PlaySingle(Plugin.DrinkSound);
                }
            }

            
            AchievementHelper.RegisterMilkAchievement();

            
            
            
            
            if (consumeWithoutBucket)
            {
                
                return true;
            }
            if (Variant == MilkVariant.MilkSoda || Variant == MilkVariant.DietMilkSoda || Variant == MilkVariant.Compressed)
            {
                if (Variant == MilkVariant.MilkSoda || Variant == MilkVariant.DietMilkSoda)
                {
                    try
                    {
                        DropMilkSodaCanToGround(player);
                    }
                    catch (System.Exception )
                    {
                        
                    }
                    
                }
                else
                {
                    
                    
                }
                return true; 
            }

            
            try
            {
                if (Plugin.EmptyBucketItemObject != null && player.itm != null)
                {
                    player.itm.SetItem(Plugin.EmptyBucketItemObject, player.itm.selectedItem);
                    
                }
                else
                {
                    
                    return true; 
                }
            }
            catch (System.Exception )
            {
                
                return true; 
            }

            
            return false;
        }

        private void CleansePlayerStatusEffects(PlayerManager player)
        {
            
            try { FakeMilkNauseaManager.Cure(player); } catch (System.Exception) { }

            
            
            
            
            try
            {
                int cleansed = 0;

                
                if (player.jumpropes != null && player.jumpropes.Count > 0)
                {
                    while (player.jumpropes.Count > 0)
                    {
                        player.jumpropes[0].End(false);
                    }
                    
                    cleansed++;
                }

                
                if (Gum.playerGum != null && Gum.playerGum.Count > 0)
                {
                    foreach (var gum in new List<Gum>(Gum.playerGum))
                    {
                        gum.Cut();
                    }
                    
                    cleansed++;
                }

                
                if (player.plm != null && player.plm.Entity != null)
                {
                    var entity = player.plm.Entity;
                    if (entity.Frozen)
                    {
                        entity.SetFrozen(false);

                        cleansed++;
                    }
                    if (entity.InteractionDisabled)
                    {
                        entity.SetInteractionState(true);

                        cleansed++;
                    }
                }

                
                
                try
                {
                    if (player.Disobeying)
                    {
                        player.ClearGuilt();

                        cleansed++;
                    }
                }
                catch (System.Exception ) { }

                
                
                try
                {
                    if (player.Invisible)
                    {
                        player.SetHidden(false);

                        cleansed++;
                    }
                }
                catch (System.Exception ) { }

                
                try
                {
                    if (player.plm != null && !player.plm.enabled)
                    {
                        player.plm.enabled = true;

                        cleansed++;
                    }
                }
                catch (System.Exception ) { }

                
                if (player.plm != null && player.plm.Entity != null)
                {
                    try
                    {
                        var ent = player.plm.Entity;
                        var lockedProp = ent.GetType().GetProperty("Locked");
                        var lockedField = ent.GetType().GetField("Locked");
                        bool locked = false;
                        if (lockedProp != null)
                        {
                            try { locked = (bool)lockedProp.GetValue(ent); } catch { }
                        }
                        else if (lockedField != null)
                        {
                            try { locked = (bool)lockedField.GetValue(ent); } catch { }
                        }
                        if (locked)
                        {
                            var m = ent.GetType().GetMethod("SetLocked",
                                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (m != null)
                            {
                                try { m.Invoke(ent, new object[] { false }); cleansed++; } catch { }
                            }
                            else if (lockedProp != null && lockedProp.CanWrite)
                            {
                                try { lockedProp.SetValue(ent, false); cleansed++; } catch { }
                            }
                        }
                    }
                    catch (System.Exception ) { }
                }

                if (cleansed == 0)
                {
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        private void EndAllRandomEvents(EnvironmentController ec)
        {
            try
            {
                
                FieldInfo currentEventsField = null;
                Type t = typeof(EnvironmentController);
                while (t != null && currentEventsField == null)
                {
                    currentEventsField = t.GetField("currentEvents",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    t = t.BaseType;
                }

                if (currentEventsField == null)
                {
                    
                    return;
                }

                var events = currentEventsField.GetValue(ec) as List<RandomEvent>;
                if (events == null)
                {
                    
                    return;
                }

                
                if (events.Count == 0) return;

                var eventsCopy = new List<RandomEvent>(events);
                int ended = 0;
                var timerField = typeof(RandomEvent).GetField("eventTimer",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var evt in eventsCopy)
                {
                    if (evt == null) continue;
                    try
                    {
                        
                        
                        
                        try
                        {
                            if (timerField != null)
                            {
                                var timer = timerField.GetValue(evt) as System.Collections.IEnumerator;
                                if (timer != null) evt.StopCoroutine(timer);
                            }
                        }
                        catch (System.Exception) { }
                        
                        
                        
                        
                        
                        bool isGravity = evt is GravityEvent;
                        evt.End();
                        
                        
                        ForceCleanseEvent(evt, ec);
                        ended++;
                        if (isGravity)
                        {
                            
                        }
                        else
                        {
                            
                        }
                    }
                    catch (System.Exception )
                    {
                        
                    }
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void ShortenAllRandomEvents(EnvironmentController ec, float remaining)
        {
            try
            {
                
                FieldInfo currentEventsField = null;
                Type t = typeof(EnvironmentController);
                while (t != null && currentEventsField == null)
                {
                    currentEventsField = t.GetField("currentEvents",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    t = t.BaseType;
                }
                if (currentEventsField == null)
                {
                    
                    return;
                }

                var events = currentEventsField.GetValue(ec) as List<RandomEvent>;
                if (events == null || events.Count == 0)
                {
                    
                    return;
                }

                int shortened = 0;
                foreach (var evt in events)
                {
                    if (evt == null) continue;
                    try
                    {
                        
                        var remainingField = evt.GetType().GetField("remainingTime",
                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                        if (remainingField != null)
                        {
                            float old = (float)remainingField.GetValue(evt);
                            remainingField.SetValue(evt, Mathf.Min(old, remaining));
                            shortened++;
                            
                        }
                    }
                    catch (System.Exception )
                    {
                        
                    }
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void ForceCleanseEvent(RandomEvent evt, EnvironmentController ec)
        {
            try
            {
                
                if (evt is LockdownEvent)
                {
                    foreach (var fieldName in new[] { "doors", "trappedDoors" })
                    {
                        var field = evt.GetType().GetField(fieldName,
                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                        var list = field?.GetValue(evt) as System.Collections.IEnumerable;
                        if (list == null) continue;
                        foreach (var obj in list)
                        {
                            if (obj is Door door)
                            {
                                try { door.Unlock(); } catch (System.Exception) { }
                                try { door.Open(true, false); } catch (System.Exception) { }
                            }
                        }
                    }
                    
                }
                
                
                else if (evt is PartyEvent)
                {
                    var audioField = evt.GetType().GetField("partyAudio",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    if (audioField?.GetValue(evt) is UnityEngine.Object partyAudioObj && partyAudioObj != null)
                    {
                        UnityEngine.Object.Destroy(partyAudioObj);
                    }
                    
                    foreach (var balloon in UnityEngine.Object.FindObjectsOfType<Balloon>())
                    {
                        UnityEngine.Object.Destroy(balloon.gameObject);
                    }


                }
                
                
                else if (evt is BalderEvent)
                {
                    var baldersField = evt.GetType().GetField("balders",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    var balders = baldersField?.GetValue(evt) as System.Collections.IEnumerable;
                    if (balders != null)
                    {
                        foreach (var obj in new System.Collections.Generic.List<object>(
                            balders.Cast<object>()))
                        {
                            if (obj is Balder_Entity balder && balder != null && !balder.Crumbled)
                            {
                                try { balder.Crumble(false); } catch (System.Exception) { }
                            }
                        }
                    }

                }
                
                
                else if (evt is FogEvent && ec != null)
                {
                    try { ec.MaxRaycast = float.PositiveInfinity; } catch (System.Exception) { }

                }
            }
            catch (System.Exception )
            {
                
            }
        }

        private void ResetAllNPCs(EnvironmentController ec)
        {
            try
            {
                var npcs = ec.Npcs;
                if (npcs == null || npcs.Count == 0) return;

                var blindedField = typeof(NPC).GetField("blinded", BindingFlags.NonPublic | BindingFlags.Instance);
                var guiltField = typeof(NPC).GetField("guilt", BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var npc in npcs)
                {
                    if (npc == null) continue;

                    try
                    {
                        if (blindedField != null) blindedField.SetValue(npc, false);
                        if (guiltField != null) guiltField.SetValue(npc, 0f);
                        
                        
                        if (npc.Entity != null && npc.Entity.Squished)
                        {
                            npc.Entity.Unsquish();
                        }
                    }
                    catch (System.Exception )
                    {

                    }
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        private void ResetDetention(PlayerManager player)
        {
            try
            {
                EnvironmentController ec = player.ec;

                
                var detFunctions = UnityEngine.Object.FindObjectsOfType<DetentionRoomFunction>();
                foreach (var det in detFunctions)
                {
                    try
                    {
                        
                        var timeField = typeof(DetentionRoomFunction).GetField("time",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        timeField?.SetValue(det, 0f);

                        
                        var roomField = typeof(RoomFunction).GetField("room",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        var room = roomField?.GetValue(det) as RoomController;
                        if (room != null)
                        {
                                foreach (var door in room.doors)
                                {
                                    door.Unlock();
                                    door.Open(true, false);
                                }
                        }
                        
                    }
                    catch (System.Exception )
                    {
                        
                    }
                }

                
                foreach (var princ in ec.Npcs)
                {
                    if (princ is Principal principal)
                    {
                        var lvlField = typeof(Principal).GetField("detentionLevel",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        lvlField?.SetValue(principal, 0);
                    }
                }

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        
        
        
        
        
        
        private void SpawnMilkSodaSpray(PlayerManager player, float duration = 20f, Sprite spraySprite = null)
        {
            try
            {
                
                ItemObject bsodaIo = Resources.FindObjectsOfTypeAll<ItemObject>()
                    .FirstOrDefault(x => x.itemType == Items.Bsoda);
                if (bsodaIo == null || bsodaIo.item == null)
                {
                    
                    return;
                }

                
                Item spray = UnityEngine.Object.Instantiate(bsodaIo.item);
                spray.gameObject.SetActive(true);

                
                
                Sprite milkSpraySprite = spraySprite;
                if (milkSpraySprite == null)
                    milkSpraySprite = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, "MilkSodaSpray.png");
                if (milkSpraySprite != null)
                {
                    var spriteRenderer = spray.GetComponentInChildren<SpriteRenderer>(true);
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sprite = milkSpraySprite;
                    }
                    else
                    {
                        
                    }
                }

                
                var timeField = spray.GetType().GetField("time",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (timeField != null)
                {
                    timeField.SetValue(spray, duration);
                }

                
                var speedField = spray.GetType().GetField("speed",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (speedField != null)
                {
                    float curSpeed = (float)speedField.GetValue(spray);
                    float newSpeed = curSpeed * 0.7f; 
                    speedField.SetValue(spray, newSpeed);
                    
                }

                
                
                try
                {
                    Collider original = spray.GetComponentInChildren<Collider>(true);
                    if (original != null)
                    {
                        var solidCol = spray.gameObject.AddComponent<BoxCollider>();
                        solidCol.isTrigger = false;
                        solidCol.gameObject.layer = original.gameObject.layer;
                        solidCol.size = new Vector3(0.8f, 2f, 0.8f); 
                        
                    }
                }
                catch (System.Exception )
                {
                    
                }

                
                var tracker = spray.gameObject.AddComponent<MilkSodaSprayTracker>();
                MilkSodaSprayTracker.player_ref = player;

                
                spray.transform.position = player.transform.position;
                spray.transform.forward = Singleton<CoreGameManager>.Instance
                    .GetCamera(player.playerNumber).transform.forward;
                spray.Use(player);

                
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void DropMilkSodaCanToGround(PlayerManager player)
        {
            if (player == null) return;

            
            string iconName = (Variant == MilkVariant.DietMilkSoda) ? "DietMilkSodaIcon_Large.png" : "MilkSodaIcon_Large.png";
            Sprite canSprite = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 50f, iconName);
            if (canSprite == null)
            {
                
                return;
            }

            
            var go = new GameObject("MilkSodaCan_Ground");
            go.transform.position = player.transform.position + Vector3.up * 2f;
            go.transform.rotation = UnityEngine.Random.rotation;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = canSprite;
            sr.sortingOrder = 10;

            
            player.StartCoroutine(ThrowCanCoroutine(go, player.transform.position + player.transform.forward * 3f));
        }

        private System.Collections.IEnumerator ThrowCanCoroutine(GameObject go, Vector3 target)
        {
            Vector3 start = go.transform.position;
            float duration = 0.6f;
            float elapsed = 0f;
            var sr = go.GetComponent<SpriteRenderer>();

            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(start, target, t);
                
                pos.y += Mathf.Sin(t * Mathf.PI) * 1.5f;
                go.transform.position = pos;
                
                go.transform.Rotate(0f, 0f, 120f * Time.deltaTime);
                yield return null;
            }

            
            go.transform.position = new Vector3(target.x, 0.1f, target.z);
            if (sr != null)
            {
                sr.color = new Color(0.5f, 0.5f, 0.5f, 1f); 
            }
            
        }

        
        
        private void PlayCompressedMilkShatter(PlayerManager player)
        {
            if (player == null) return;

            
            Vector3 origin = player.transform.position + player.transform.forward * 1.2f + Vector3.up * 0.3f;
            int shards = 10;

            for (int i = 0; i < shards; i++)
            {
                
                GameObject shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shard.transform.position = origin;
                shard.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                shard.transform.rotation = UnityEngine.Random.rotation;

                var mr = shard.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    
                    var mat = new Material(Shader.Find("Sprites/Default"));
                    mat.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                    mr.sharedMaterial = mat;
                }

                
                Vector3 dir = (player.transform.forward + Vector3.up * 0.6f +
                               UnityEngine.Random.insideUnitSphere * 0.8f).normalized;
                player.StartCoroutine(ShatterShardCoroutine(shard, dir));
            }

            
        }

        private System.Collections.IEnumerator ShatterShardCoroutine(GameObject shard, Vector3 dir)
        {
            if (shard == null) yield break;

            float life = 0.9f;
            float elapsed = 0f;
            Vector3 vel = dir * 6f; 
            Vector3 gravity = new Vector3(0f, -10f, 0f);
            var mr = shard.GetComponent<MeshRenderer>();

            while (elapsed < life)
            {
                float dt = Time.deltaTime;
                elapsed += dt;

                
                vel += gravity * dt;
                shard.transform.position += vel * dt;
                shard.transform.Rotate(UnityEngine.Random.insideUnitSphere * 720f * dt, Space.World);

                
                if (mr != null)
                {
                    var c = mr.material.color;
                    c.a = Mathf.Lerp(1f, 0f, elapsed / life);
                    mr.material.color = c;
                }

                yield return null;
            }

            UnityEngine.Object.Destroy(shard);
        }

        
        
        
        
        
        
        
        private System.Collections.IEnumerator CompressedMilkEffectCoroutine(PlayerManager player, float duration)
        {
            if (player == null || player.plm == null) yield break;

            
            player.plm.stamina = 400f;
            

            
            
            
            
            var entity = player.plm.Entity;
            if (entity != null)
            {
                entity.Squish(duration);
                
                if (player.gameObject.GetComponent<CompressedSquishMarker>() == null)
                {
                    player.gameObject.AddComponent<CompressedSquishMarker>();
                }
                
            }

            
            MovementModifier slowMod = new MovementModifier(Vector3.zero, 0.35f, 0);
            player.plm.am.moveMods.Add(slowMod);
            

            
            bool stuckFrozen = false;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                
                bool nearDoor = IsPlayerNearDoor(player, 3f);
                bool running = false;
                try
                {
                    running = Singleton<InputManager>.Instance.GetDigitalInput("Run", onDown: false);
                }
                catch (System.Exception) { }

                bool wantFrozen = nearDoor && !running;
                if (wantFrozen && !stuckFrozen)
                {
                    entity.SetFrozen(true);
                    stuckFrozen = true;
                    
                }
                else if (!wantFrozen && stuckFrozen)
                {
                    entity.SetFrozen(false);
                    stuckFrozen = false;
                    
                }

                yield return null;
            }

            
            if (entity != null)
            {
                if (stuckFrozen) entity.SetFrozen(false);
                if (entity.Squished) entity.Unsquish();
                
            }
            var marker = player.gameObject.GetComponent<CompressedSquishMarker>();
            if (marker != null)
            {
                UnityEngine.Object.Destroy(marker);
            }
            player.plm.am.moveMods.Remove(slowMod);
            
        }

        
        private bool IsPlayerNearDoor(PlayerManager player, float threshold)
        {
            if (player == null || player.ec == null) return false;
            var doors = player.ec.standardDoors;
            if (doors == null || doors.Count == 0) return false;
            Vector3 pPos = player.transform.position;
            foreach (var door in doors)
            {
                if (door == null) continue;
                if (Vector3.Distance(pPos, door.transform.position) <= threshold) return true;
            }
            return false;
        }
    }

    
    
    public class EmptyBucketComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            
            return false; 
        }
    }

    
    
    
    
    public class LostBilkComponent : ITM_Acceptable
    {
        public override bool Use(PlayerManager player)
        {
            try
            {
                if (player == null || player.ec == null) return false;
                Plugin.StopMilkRandomEvents(); 
                
                try
                {
                    AudioClip ohhClip = AssetLoader.AudioClipFromMod(Plugin.Instance, "BAL_Ohh.wav");
                    if (ohhClip != null)
                    {
                        SoundObject ohhSo = ObjectCreators.CreateSoundObject(ohhClip, "BAL_Ohh", SoundType.Voice, Color.green, ohhClip.length);
                        var cgmOhh = Singleton<CoreGameManager>.Instance;
                        if (cgmOhh != null && cgmOhh.audMan != null) cgmOhh.audMan.PlaySingle(ohhSo);
                    }
                }
                catch (System.Exception) { }
                var mc = gameObject.GetComponent<MilkComponent>();
                if (mc == null) mc = gameObject.AddComponent<MilkComponent>();
                mc.Variant = MilkVariant.Normal;
                try { AchievementHelper.UnlockAchievement("milk_lostbilk"); } catch (System.Exception) { }
                mc.consumeWithoutBucket = true;
                bool r = mc.Use(player);
                mc.consumeWithoutBucket = false;
                return r;
            }
            catch (System.Exception )
            {
                
                return false;
            }
        }
    }

    
    
    
    
    public class MilkSodaSprayTracker : MonoBehaviour, IEntityTrigger
    {
        
        public static readonly System.Collections.Generic.HashSet<NPC> Deafened = new System.Collections.Generic.HashSet<NPC>();
        
        private static readonly System.Collections.Generic.Dictionary<NPC, int> pushCount =
            new System.Collections.Generic.Dictionary<NPC, int>();

        
        public static PlayerManager player_ref;

        public void EntityTriggerEnter(Entity otherEntity, Collider other, bool validCollision)
        {
            if (otherEntity == null) return;
            
            if (otherEntity.CompareTag("Player")) return;
            try
            {
                var ec = FindObjectOfType<EnvironmentController>();
                if (ec == null || ec.Npcs == null) return;
                foreach (var npc in ec.Npcs)
                {
                    if (npc != null && npc.Entity == otherEntity)
                    {
                        pushCount[npc] = pushCount.ContainsKey(npc) ? pushCount[npc] + 1 : 1;
                        Deafened.Add(npc);
                        
                        
                        try { AchievementHelper.UnlockAchievement("milk_sodapush"); } catch (System.Exception) { }
                        
                        try
                        {
                            if (player_ref != null)
                                player_ref.StartCoroutine(Plugin.PoisonMilkNPCEffectCoroutine(npc, 3f));
                        }
                        catch (System.Exception) { }
                        break;
                    }
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        public void EntityTriggerStay(Entity otherEntity, Collider other, bool validCollision) { }

        public void EntityTriggerExit(Entity otherEntity, Collider other, bool validCollision)
        {
            if (otherEntity == null) return;
            try
            {
                var ec = FindObjectOfType<EnvironmentController>();
                if (ec == null || ec.Npcs == null) return;
                foreach (var npc in ec.Npcs)
                {
                    if (npc != null && npc.Entity == otherEntity)
                    {
                        if (pushCount.ContainsKey(npc))
                        {
                            pushCount[npc]--;
                            if (pushCount[npc] <= 0)
                            {
                                pushCount.Remove(npc);
                                Deafened.Remove(npc);
                                
                            }
                        }
                        else
                        {
                            Deafened.Remove(npc);
                        }
                        break;
                    }
                }
            }
            catch (System.Exception )
            {
                
            }
        }

        
        
        private void OnDestroy()
        {
            try
            {
                if (Deafened.Count > 0 || pushCount.Count > 0)
                {
                    
                    Deafened.Clear();
                    pushCount.Clear();
                }
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    [HarmonyPatch(typeof(NPC), "Hear")]
    public class PatchMilkSodaDeafNpc
    {
        static bool Prefix(NPC __instance)
        {
            if (MilkSodaSprayTracker.Deafened.Contains(__instance))
            {
                return false; 
            }
            return true;
        }
    }

    
    
    
    [HarmonyPatch(typeof(ITM_BSODA), "EntityTriggerEnter")]
    public class PatchMilkSodaDontPushPlayer
    {
        static bool Prefix(ITM_BSODA __instance, Entity otherEntity, Collider other, bool validCollision)
        {
            if (__instance != null && __instance.GetComponent<MilkSodaSprayTracker>() != null
                && other != null && other.CompareTag("Player"))
            {
                return false; 
            }
            return true;
        }
    }

    
    public static class PoisonMilkNoMapBlock
    {
        public static bool active = false;
    }

    [HarmonyPatch(typeof(CoreGameManager), "OpenMap")]
    public class PatchPoisonMilkNoMap
    {
        static bool Prefix()
        {
            if (PoisonMilkNoMapBlock.active)
            {
                return false; 
            }
            return true;
        }
    }

    
    [HarmonyPatch(typeof(Map), "Find")]
    public class PatchMapFindErodedRed
    {
        static void Postfix(int posX, int posZ)
        {
            try
            {
                if (!Plugin.MooRedWhiteActive) return;
                var bgm = Singleton<BaseGameManager>.Instance;
                if (bgm == null || bgm.Ec == null || bgm.Ec.map == null) return;
                var eroded = Plugin.RedWhiteModeErodedCells();
                if (eroded == null) return;
                var key = new IntVector2(posX, posZ);
                if (!eroded.Contains(key)) return;
                var m = bgm.Ec.map;
                var tilesField = typeof(Map).GetField("tiles",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (tilesField == null) return;
                MapTile[,] tileArr = tilesField.GetValue(m) as MapTile[,];
                if (tileArr == null || tileArr[posX, posZ] == null) return;
                tileArr[posX, posZ].SpriteRenderer.color = new Color(1f, 0.2f, 0.15f, 1f);
            }
            catch (System.Exception) { }
        }
    }

    
    public static class PoisonMilkDownTracker
    {
        public static readonly HashSet<NPC> downed = new HashSet<NPC>();
    }

    
    [HarmonyPatch(typeof(NpcStateMachine), "Update")]
    public class PatchPoisonMilkNpcAiSkip
    {
        static bool Prefix(NpcStateMachine __instance)
        {
            try
            {
                if (__instance != null && __instance.CurrentState != null &&
                    PoisonMilkDownTracker.downed.Contains(__instance.CurrentState.Npc))
                {
                    return false; 
                }
            }
            catch (System.Exception) { }
            return true;
        }
    }

    
    
    
    
    [HarmonyPatch(typeof(NavigationStateMachine), "DestinationEmpty")]
    public class PatchNavStateMachineDestEmptyNullGuard
    {
        static bool Prefix(NavigationStateMachine __instance)
        {
            try
            {
                if (__instance == null) return false;
                var cur = AccessTools.Field(typeof(NavigationStateMachine), "currentState").GetValue(__instance);
                if (cur == null) return false; 
            }
            catch (System.Exception) { }
            return true;
        }
    }

    
    
    
    [HarmonyPatch(typeof(BaseGameManager), "Initialize")]
    public class PatchFactoryFloorReplace
    {
        static System.Reflection.FieldInfo GetSObjectField(System.Type t, string name)
        {
            while (t != null && t != typeof(object))
            {
                System.Reflection.FieldInfo f = t.GetField(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                if (f != null) return f;
                t = t.BaseType;
            }
            return null;
        }
        static void SetSObjectField(object obj, string name, object val)
        {
            System.Reflection.FieldInfo f = GetSObjectField(obj.GetType(), name);
            if (f != null) f.SetValue(obj, val);
        }
        static void Prefix(BaseGameManager __instance)
        {
            try
            {
                CoreGameManager cgm = Singleton<CoreGameManager>.Instance;
                if (cgm == null) return;
                SceneObject so = cgm.sceneObject;
                if (so == null) return;

                
                
                
                if (Plugin.FactorySceneObject != null && (object)so == (object)Plugin.FactorySceneObject)
                {
                    SceneObject loadCopy = UnityEngine.Object.Instantiate<SceneObject>(Plugin.FactorySceneObject);
                    System.Reflection.FieldInfo frf = GetSObjectField(loadCopy.GetType(), "randomizedLevelObject");
                    if (frf != null) SetSObjectField(loadCopy, "randomizedLevelObject", System.Array.CreateInstance(frf.FieldType.GetElementType(), 0));
                    SetSObjectField(loadCopy, "levelContainer", null);
                    cgm.sceneObject = loadCopy;
                    
                    return;
                }

                
                
            }
                catch (System.Exception ) {  }
        }
    }

    
    
    
    
    
    

    [HarmonyPatch(typeof(LevelBuilder), "StartGenerate")]
    public class PatchFactoryForcePerFloor
    {
        static void Prefix(LevelBuilder __instance)
        {
            try
            {
                if (__instance == null || __instance.scene == null) return;
                
                
                
                bool isEndlessMilk = __instance.scene.levelObject != null && (int)(object)__instance.scene.levelObject.type == (int)(object)Plugin.MilkFactory;
                
                
                
                
                var actualTheme = Plugin.GetActualTheme(__instance.scene);
                bool isVanillaFactory = actualTheme != null && (int)(object)actualTheme.type == (int)(object)LevelType.Factory;
                
                
                bool isVanillaLab = actualTheme != null && (int)(object)actualTheme.type == (int)(object)LevelType.Laboratory;
                Plugin.SilentLog($"[FactoryGate] scene={__instance.scene.name} levelNo={__instance.scene.levelNo} theme={(actualTheme != null ? actualTheme.name + ":" + actualTheme.type : "null")} plan={Plugin.factoryReplaceLevelNo} endlessMilk={isEndlessMilk} vanillaFactory={isVanillaFactory} vanillaLab={isVanillaLab}");
                if (__instance.scene.levelNo < 1 && !isEndlessMilk && !isVanillaFactory) return;

                
                
                
                
                try
                {
                    if (__instance.scene.levelNo == 4 && __instance.ld != null && !Plugin.MooRedWhiteActive)
                    {
                        Plugin.Inject99RoomIntoPool(__instance.ld);
                    }
                }
                catch (System.Exception ) {  }

                
                
                
                
                
                int levelNoForFactory = __instance.scene.levelNo;
                bool isMilkFactory = (Plugin.factoryReplaceLevelNo == levelNoForFactory) || isEndlessMilk || isVanillaFactory || isVanillaLab;
                
                
                
                if (!isMilkFactory) return;
                
                {
                    var _lo = __instance.scene.levelObject as LevelObject;
                    int _tv = __instance.scene.levelObject != null ? (int)(object)__instance.scene.levelObject.type : -1;
                    
                }
                if (__instance.ld == null) return;
                
                
                
                
                try
                {
                    
                    
                    
                    
                    bool savedFinalLevel = __instance.ld.finalLevel;
                    LevelObject schoolLO = Plugin.GetSchoolLevelObject();
                    if (schoolLO != null)
                    {
                        LevelObject schoolClone = UnityEngine.Object.Instantiate<LevelObject>(schoolLO);
                        LevelGenerationModifier schMod = new LevelGenerationModifier();
                        __instance.ld.AssignData(schoolClone, schMod);
                        __instance.ld.finalLevel = savedFinalLevel; 
                        Plugin.SilentLog("[Factory] forced school template base: " + schoolClone.name + " (finalLevel preserved=" + savedFinalLevel + ")");
                    }
                }
                catch (System.Exception ) {  }
                
                
                try
                {
                    if (__instance.scene.levelNo == 4 && __instance.ld != null && !Plugin.MooRedWhiteActive)
                    {
                        Plugin.Inject99RoomIntoPool(__instance.ld);
                    }
                }
                catch (System.Exception ) {  }
                
                Plugin.LoadMilkRoomsFromFiles();
                if (Plugin.LoadedMilkRooms.Count == 0)
                {
                    
                    return;
                }
                
                
                WeightedRoomAsset[] milkHalls = Plugin.LoadedMilkRooms.ToArray();
                foreach (WeightedRoomAsset w in milkHalls)
                {
                    if (w != null && w.selection != null)
                    {
                        w.selection.category = RoomCategory.Hall;
                        w.selection.type = RoomType.Hall;
                    }
                }

                var lgp = __instance.ld;
                
                
                bool bilkActive = Plugin.StickersReady
                    && Singleton<StickerManager>.Instance != null
                    && Singleton<StickerManager>.Instance.StickerValue(Plugin.BilkSticker) > 0;
                
                
                
                try
                {
                    lgp.minSize = new IntVector2(26, 26);
                    lgp.maxSize = new IntVector2(32, 32);
                    lgp.outerEdgeBuffer = 5;
                    lgp.minPlots = 2;
                    lgp.maxPlots = 3;
                    lgp.minPlotSize = 5;
                    
                }
                catch (System.Exception ) {  }
                
                
                try
                {
                    RoomGroup[] schoolGroups = Plugin.GetSchoolRoomGroups();
                    if (schoolGroups != null && schoolGroups.Length > 0)
                    {
                        lgp.roomGroup = new System.Collections.Generic.List<RoomGroup>(schoolGroups);
                        
                    }
                    else
                    {
                        
                    }
                }
                catch (System.Exception ) {  }
                
                
                lgp.potentialPrePlotSpecialHalls = milkHalls;
                lgp.potentialPostPlotSpecialHalls = milkHalls;
                

                
                
                
                
                
                
                
                if (bilkActive)
                {
                    
                    
                    
                    
                    var bilkAssets = Plugin.GetBilkClassroomAssets();
                    if (bilkAssets != null && bilkAssets.Length > 0)
                    {
                        int bilkGroups = 0;
                        foreach (var rg in lgp.roomGroup)
                        {
                            if (rg == null) continue;
                            rg.potentialRooms = bilkAssets;
                            rg.minRooms = 1;
                            rg.maxRooms = 2;
                            bilkGroups++;
                        }
                        
                    }
                }
                else
                {
                try
                {
                    Plugin.LoadMathRoomsFromFiles();
                    if (Plugin.LoadedMathRooms.Count > 0 && lgp.roomGroup != null)
                    {
                        
                        
                        var rnd = new System.Random(System.Environment.TickCount ^ (__instance.scene.levelNo + 1));
                        
                        var newRG = new System.Collections.Generic.List<RoomGroup>();
                        int replacedCount = 0;
                        int floorNo = __instance.scene.levelNo;
                        
                        
                        
                        
                        
                        
                        int classGroupCount = 0;
                        foreach (var _rg in lgp.roomGroup)
                        {
                            if (_rg == null || _rg.potentialRooms == null) continue;
                            string _gn = _rg.name != null ? _rg.name.ToLowerInvariant() : "";
                            if (_gn.Contains("class") || _gn.Contains("lesson") || _gn.Contains("教室")) classGroupCount++;
                        }
                        if (classGroupCount == 0) classGroupCount = 1;
                        
                        
                        int lvl = (__instance != null && __instance.scene != null) ? __instance.scene.levelNo : 1;
                        int targetNotebooks = UnityEngine.Random.Range(7, 10); 
                        int baseShare = targetNotebooks / classGroupCount;
                        int remShare = targetNotebooks % classGroupCount; 
                        Plugin.SilentLog($"[Factory] Notebook target={targetNotebooks} over {classGroupCount} class group(s) (milk math classrooms).");
                        int shareIdx = 0;
                        foreach (var rg in lgp.roomGroup)
                        {
                            if (rg == null) { newRG.Add(null); continue; }
                            RoomGroup c = new RoomGroup();
                            var rgt = rg.GetType();
                            foreach (var fld in rgt.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                            {
                                try { fld.SetValue(c, fld.GetValue(rg)); } catch (System.Exception) { }
                            }
                            string gname = rg.name != null ? rg.name.ToLowerInvariant() : "";
                            bool isClassGroup = gname.Contains("class") || gname.Contains("lesson") || gname.Contains("教室");
                            if (isClassGroup && c.potentialRooms != null)
                            {
                                
                                int share = baseShare + (shareIdx < remShare ? 1 : 0);
                                c.minRooms = share;
                                c.maxRooms = share;
                                
                                
                                var list = new System.Collections.Generic.List<WeightedRoomAsset>();
                                foreach (var w in c.potentialRooms)
                                {
                                    if (w == null) { list.Add(null); continue; }
                                    WeightedRoomAsset nw = new WeightedRoomAsset();
                                    nw.selection = Plugin.LoadedMathRooms[rnd.Next(0, Plugin.LoadedMathRooms.Count)].selection;
                                    nw.weight = (w.weight > 0) ? w.weight : 100;
                                    list.Add(nw);
                                    replacedCount++;
                                }
                                int guard = 0;
                                while (list.Count < share && guard < 40)
                                {
                                    WeightedRoomAsset pad = new WeightedRoomAsset();
                                    pad.selection = Plugin.LoadedMathRooms[rnd.Next(0, Plugin.LoadedMathRooms.Count)].selection;
                                    pad.weight = 70;
                                    list.Add(pad);
                                    guard++;
                                }
                                c.potentialRooms = list.ToArray();
                                shareIdx++;
                            }
                            else
                            {
                                
                                
                                
                                if (c.minRooms < 2) c.minRooms = 2;
                                if (c.maxRooms > 3) c.maxRooms = 3;
                            }
                            newRG.Add(c);
                        }
                        lgp.roomGroup = newRG;
                        {
                            int gmin = 0, gmax = 0;
                            foreach (var rg in lgp.roomGroup) { if (rg != null) { gmin += rg.minRooms; gmax += rg.maxRooms; } }
                            
                        }
                    }
                }
                catch (System.Exception ) {  }
                } 

                
                
                

                
                try
                {
                    var structs = Plugin.GetBeltAndSteamStructures();
                    if (structs != null && structs.Count > 0)
                    {
                        var forced = new System.Collections.Generic.List<StructureWithParameters>(lgp.forcedStructures ?? new StructureWithParameters[0]);
                        int beltCount = 0, steamCount = 0;
                        foreach (var s in structs)
                        {
                            if (s == null || s.prefab == null) continue;
                            if (s.prefab is Structure_ConveyorBelt)
                            {
                                
                                forced.Add(s);
                                forced.Add(Plugin.CloneStructure(s));
                                beltCount += 2;
                            }
                            else if (s.prefab is Structure_SteamValves)
                            {
                                
                                StructureWithParameters clone = Plugin.CloneStructure(s);
                                if (clone.parameters != null && clone.parameters.minMax != null && clone.parameters.minMax.Length >= 1)
                                {
                                    clone.parameters.minMax[0] = new IntVector2(4, 6);
                                }
                                forced.Add(clone);
                                steamCount++;
                            }
                        }
                        
                        
                        int ventCount = 0, doorCount = 0, rotoCount = 0;
                        var extras = Plugin.GetVentDoorRotoStructures();
                        if (extras != null && extras.Count > 0)
                        {
                            foreach (var e in extras)
                            {
                                if (e == null || e.prefab == null) continue;
                                if (e.prefab is Structure_Vent) { forced.Add(Plugin.CloneStructure(e)); ventCount++; }
                                else if (e.prefab is Structure_HallDoor) { forced.Add(Plugin.CloneStructure(e)); doorCount++; }
                                else if (e.prefab is Structure_Rotohalls) { forced.Add(Plugin.CloneStructure(e)); rotoCount++; }
                            }
                        }

                        
                        
                        
                        int powerLeverCount = 0;
                        if (!Plugin.LevelHasPowerLever(lgp))
                        {
                            
                            
                            Plugin.RemovePowerLeverFromPotential(lgp);
                            var pl = Plugin.GetPowerLeverStructure();
                            if (pl != null)
                            {
                                forced.Add(pl);
                                powerLeverCount++;
                            }
                        }

                        if (beltCount > 0 || steamCount > 0 || powerLeverCount > 0 || ventCount > 0 || doorCount > 0 || rotoCount > 0)
                        {
                            lgp.forcedStructures = forced.ToArray();
                            Plugin.SilentLog($"[Factory] injected forcedStructures: belt={beltCount} steam={steamCount} powerLever={powerLeverCount} vent={ventCount} hallDoor={doorCount} roto={rotoCount} total={forced.Count}");
                        }
                        else
                        {
                            Plugin.Log?.LogWarning("[Factory] no structures injected (empty source scan); belt/steam/vent/door/roto/power all 0.");
                        }
                    }
                }
                catch (System.Exception ) {  }

                
                
                try
                {
                    
                    
                    
                    if (lgp.potentialSpecialRooms != null && isVanillaLab)
                    {
                        var filtered = new System.Collections.Generic.List<WeightedRoomAsset>();
                        foreach (var _ps in lgp.potentialSpecialRooms)
                        {
                            if (_ps == null || _ps.selection == null) { filtered.Add(_ps); continue; }
                            string _sn = (_ps.selection.name ?? "").ToLowerInvariant();
                            if (_sn.Contains("wormhole") || _sn.Contains("teleport")) continue; 
                            filtered.Add(_ps);
                        }
                        lgp.potentialSpecialRooms = filtered.ToArray();
                        
                        
                        if (__instance.scene.levelNo != 4)
                        {
                            lgp.minSpecialRooms = 0;
                            lgp.maxSpecialRooms = 0;
                        }
                        
                    }
                    RoomAsset beltRoom = Plugin.GetBeltRoomAsset();
                    Plugin.SilentLog("[Factory] beltRoom asset=" + (beltRoom != null ? beltRoom.name : "null"));
                    if (beltRoom != null)
                    {
                        var specials = new System.Collections.Generic.List<WeightedRoomAsset>(lgp.potentialSpecialRooms ?? new WeightedRoomAsset[0]);
                        WeightedRoomAsset bw = new WeightedRoomAsset();
                        bw.selection = beltRoom;
                        bw.weight = 100;
                        specials.Add(bw);
                        lgp.potentialSpecialRooms = specials.ToArray();
                        if (lgp.minSpecialRooms < 1) lgp.minSpecialRooms = 1;
                        if (lgp.maxSpecialRooms < 1) lgp.maxSpecialRooms = 1;
                        
                    }
                    else
                    {
                        
                    }
                }
                catch (System.Exception ) {  }

                
                try
                {
                    var aliases = LevelLoaderPlugin.Instance.roomTextureAliases;
                    Texture2D mtW = null, mtF = null, mtC = null;
                    aliases.TryGetValue("MilkRoom_Wall", out mtW);
                    aliases.TryGetValue("MilkRoom_Floor", out mtF);
                    aliases.TryGetValue("MilkRoom_Ceiling", out mtC);
                    if (mtW != null) { WeightedTexture2D wt = new WeightedTexture2D(); wt.selection = mtW; wt.weight = 100; lgp.hallWallTexs = new WeightedTexture2D[] { wt }; }
                    if (mtF != null) { WeightedTexture2D ft = new WeightedTexture2D(); ft.selection = mtF; ft.weight = 100; lgp.hallFloorTexs = new WeightedTexture2D[] { ft }; }
                    if (mtC != null) { WeightedTexture2D ct = new WeightedTexture2D(); ct.selection = mtC; ct.weight = 100; lgp.hallCeilingTexs = new WeightedTexture2D[] { ct }; }
                }
                catch (System.Exception ) {  }

                
            }
            catch (System.Exception ) {  }
        }
    }

    
    
    
    
    
    
    [HarmonyPatch(typeof(LevelBuilder), "StartGenerate")]
    public class PatchRanchReskin
    {
        static void Prefix(LevelBuilder __instance)
        {
            try
            {
                if (__instance == null || __instance.scene == null || __instance.ld == null) { Plugin.activeRanchReskin = false; return; }
                int levelNo = __instance.scene.levelNo;
                Plugin.activeRanchReskin = (levelNo >= 0 && levelNo <= 4) && Plugin.IsRanchFloor(levelNo) && Plugin.RanchReady();
                if (!Plugin.activeRanchReskin)
                {
                    if ((levelNo >= 0 && levelNo <= 4) && Plugin.IsRanchFloor(levelNo) && !Plugin.RanchReady())
                        Plugin.Log?.LogWarning("[Ranch] Ranch textures not loaded, skip reskin.");
                    return;
                }

                var lgp = __instance.ld;

                
                
                try
                {
                    if (lgp.roomGroup == null || lgp.roomGroup.Count == 0)
                    {
                        RoomGroup[] schoolGroups = Plugin.GetSchoolRoomGroups();
                        if (schoolGroups != null && schoolGroups.Length > 0)
                        {
                            lgp.roomGroup = new System.Collections.Generic.List<RoomGroup>(schoolGroups);
                            Plugin.SilentLog("[Ranch] roomGroup was empty, filled with school structure.");
                        }
                    }
                    
                    
                    if (lgp.roomGroup != null && lgp.roomGroup.Count > 0)
                    {
                        
                        var lightGroups = Plugin.ShallowReduceRoomGroupPressure(lgp.roomGroup, 7);
                        if (lightGroups != null) lgp.roomGroup = lightGroups;
                        Plugin.SilentLog("[Ranch] Capped room count to ~7 to avoid plot-expansion timeout.");
                    }
                }
                catch (System.Exception rmex) { Plugin.Log?.LogWarning("[Ranch] roomGroup fallback failed: " + rmex.Message); }

                
                if (Plugin.RanchGrassTex != null)
                {
                    WeightedTexture2D ft = new WeightedTexture2D();
                    ft.selection = Plugin.RanchGrassTex;
                    ft.weight = 100;
                    lgp.hallFloorTexs = new WeightedTexture2D[] { ft };
                }
                if (Plugin.RanchFenceTex != null)
                {
                    WeightedTexture2D wt = new WeightedTexture2D();
                    wt.selection = Plugin.RanchFenceTex;
                    wt.weight = 100;
                    lgp.hallWallTexs = new WeightedTexture2D[] { wt };
                }

                
                try
                {
                    var _lo = __instance.scene.levelObject as LevelObject;
                    if (_lo != null)
                    {
                        try { lgp.hallBuffer = 4; } catch (System.Exception) { }
                        try { _lo.hallBuffer = 4; } catch (System.Exception) { }
                        try
                        {
                            var dayCubemap = System.Array.Find(
                                UnityEngine.Resources.FindObjectsOfTypeAll<UnityEngine.Cubemap>(),
                                cb => cb != null && cb.name == "Cubemap_DayStandard");
                            if (dayCubemap != null) _lo.skybox = dayCubemap;
                        }
                        catch (System.Exception cex) { Plugin.Log?.LogWarning("[Ranch] day skybox set failed: " + cex.Message); }
                        _lo.maxLightDistance = 5000;
                        _lo.standardDarkLevel = UnityEngine.Color.white;
                        Plugin.SilentLog("[Ranch] Open-ranch pass: hallBuffer=4, day skybox, light dist 5000 (map/plots kept native).");
                    }
                }
                catch (System.Exception opx) { Plugin.Log?.LogWarning("[Ranch] open-ranch pass failed: " + opx.Message); }

                
                if (lgp.roomGroup != null)
                {
                    foreach (RoomGroup rg in lgp.roomGroup)
                    {
                        if (rg == null) continue;
                        if (Plugin.RanchGrassTex != null)
                        {
                            WeightedTexture2D f = new WeightedTexture2D();
                            f.selection = Plugin.RanchGrassTex;
                            f.weight = 100;
                            rg.floorTexture = new WeightedTexture2D[] { f };
                        }
                        if (Plugin.RanchFenceTex != null)
                        {
                            WeightedTexture2D w = new WeightedTexture2D();
                            w.selection = Plugin.RanchFenceTex;
                            w.weight = 100;
                            rg.wallTexture = new WeightedTexture2D[] { w };
                        }
                    }
                }

                Plugin.SilentLog($"[Ranch] Forced floor {levelNo} to Ranch skeleton (school rooms + grass floor + fence walls; ceiling/school rooms kept; map size untouched).");

                
                
                var rtr = __instance.gameObject.GetComponent<RanchTerrainRework>();
                if (rtr == null)
                {
                    rtr = __instance.gameObject.AddComponent<RanchTerrainRework>();
                    rtr.levelNo = levelNo;
                    rtr.StartCoroutine(rtr.ReworkWhenReady(__instance));
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.LogWarning("[Ranch] reskin failed: " + ex.Message);
            }
        }
    }

    
    
    [HarmonyPatch(typeof(Skybox), "InitializeMaterials")]
    public class PatchRanchSkybox
    {
        static void Prefix(ref Texture tex)
        {
            try
            {
                if (Plugin.activeRanchReskin && Plugin.RanchEdgeTex != null)
                {
                    tex = Plugin.RanchEdgeTex;
                }
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    
    
    
    
    
    public class RanchTerrainRework : UnityEngine.MonoBehaviour
    {
        public int levelNo = -1;

        public System.Collections.IEnumerator ReworkWhenReady(LevelBuilder lb)
        {
            EnvironmentController ec = null;
            float wait = 0f;
            while (wait < 60f)
            {
                if (lb != null && lb.Ec != null && !lb.levelInProgress)
                {
                    ec = lb.Ec;
                    if (ec.cells != null && ec.mainHall != null && ec.mainHall.cells != null)
                    {
                        for (int k = 0; k < 10; k++) yield return null;
                        break;
                    }
                }
                wait += UnityEngine.Time.deltaTime;
                yield return null;
            }
            if (ec == null)
            {
                Plugin.Log?.LogWarning("[Ranch] terrain rework: no ready ec; abort. cells=" + (lb != null && lb.Ec != null && lb.Ec.cells != null));
                yield break;
            }
            
            if (Plugin.nineNineTriggeredThisRun) yield break;
            System.Random rng = new System.Random(lb.seed ^ levelNo);
            Plugin.SilentLog($"[Ranch] Terrain: ec ready. cellsX={ec.cells.GetLength(0)} cellsZ={ec.cells.GetLength(1)} mainHallCells={ec.mainHall.cells.Count}");
            
                
                Cell[,] cellGrid = ec.cells;
                int filled = 0;
                int skipped = 0;
                int fillCount = 0;
                if (cellGrid != null)
                {
                    for (int x = 0; x < cellGrid.GetLength(0); x++)
                    {
                        for (int z = 0; z < cellGrid.GetLength(1); z++)
                        {
                            Cell c = cellGrid[x, z];
                            if (c == null) { skipped++; continue; }
                            if (!c.Null && c.room != null) continue; 
                            if (c.Null == false && c.room == null) { skipped++; continue; }
                            try
                            {
                                ec.CreateCell(15, c.position, ec.mainHall);
                                c.offLimits = false;
                                filled++;
                            }
                            catch (System.Exception) { }
                            
                            if (++fillCount % 300 == 0) yield return null;
                        }
                    }
                    Plugin.SilentLog($"[Ranch] Terrain: filled {filled} Null cells into mainHall grass field (skipped {skipped}).");
                }
                else { Plugin.Log?.LogWarning("[Ranch] Terrain: ec.cells is null, skip null-fill."); }

            
                
                
                
                
                
                Cell[,] owGrid = ec.cells;
                int opened = 0;
                int owCount = 0;
                for (int x = 0; x < owGrid.GetLength(0); x++)
                {
                    for (int z = 0; z < owGrid.GetLength(1); z++)
                    {
                        Cell cA = owGrid[x, z];
                        if (cA == null || cA.Null || cA.room == null) continue;
                        RoomController rA = cA.room;
                        if (rA.category == RoomCategory.Special || rA.category == RoomCategory.Store
                            || rA.category == RoomCategory.Mystery || rA.type == RoomType.Hall) continue;
                        
                        for (int di = 0; di < 4; di++)
                        {
                            Direction dir = (Direction)di;
                            IntVector2 adj = cA.position + dir.ToIntVector2();
                            if (!ec.ContainsCoordinates(adj)) continue;
                            Cell cB = ec.cells[adj.x, adj.z];
                            if (cB == null || cB.Null || cB.room == null) continue;
                            RoomController rB = cB.room;
                            
                            
                            if (rB != ec.mainHall) continue; 
                            if (rB == rA) continue; 
                            if (rB.category == RoomCategory.Special || rB.category == RoomCategory.Store
                                || rB.category == RoomCategory.Mystery) continue;
                            
                            try
                            {
                                if (cA.WallHardCovered(dir) || cB.WallHardCovered(dir.GetOpposite())) continue;
                            }
                            catch (System.Exception) { }
                            try
                            {
                                ec.ConnectCells(cA.position, dir);
                                opened++;
                            }
                            catch (System.Exception) { }
                        }
                        
                        if (++owCount % 64 == 0) yield return null;
                    }
                }
                Plugin.SilentLog($"[Ranch] Terrain: opened {opened} wall passage(s) between rooms/halls (open ranch).");

            try
            {
                
                bool movedDoor = false;
                foreach (RoomController room in ec.rooms)
                {
                    if (room == null || room.category == RoomCategory.Special
                        || room.category == RoomCategory.Store
                        || room.category == RoomCategory.Mystery
                        || room.type == RoomType.Hall) continue;
                    if (room.doors == null) continue;
                    foreach (Door door in room.doors)
                    {
                        if (door == null) continue;
                        try
                        {
                            door.transform.position += UnityEngine.Vector3.up * -10f;
                            movedDoor = true;
                        }
                        catch (System.Exception) { }
                    }
                }
                if (movedDoor) Plugin.SilentLog("[Ranch] Terrain: sunk room doors -10 (open ranch, no door gates).");
            }
            catch (System.Exception dex) { Plugin.Log?.LogWarning("[Ranch] terrain door-sink failed: " + dex.Message); }

            try
            {
                
                Window[] wins = UnityEngine.Object.FindObjectsOfType<Window>(false);
                foreach (Window w in wins) if (w != null) w.gameObject.SetActive(false);
                WaterFountain[] waters = UnityEngine.Object.FindObjectsOfType<WaterFountain>(false);
                foreach (WaterFountain wf in waters) if (wf != null) wf.gameObject.SetActive(false);
                LockdownDoor[] locks = UnityEngine.Object.FindObjectsOfType<LockdownDoor>(false);
                foreach (LockdownDoor ld in locks) if (ld != null) ld.gameObject.SetActive(false);
                SpriteRenderer[] sprites = UnityEngine.Object.FindObjectsOfType<SpriteRenderer>(false);
                foreach (SpriteRenderer sr in sprites)
                {
                    if (sr == null) continue;
                    try { if (sr.name != null && sr.name.Contains("Light")) sr.enabled = false; } catch (System.Exception) { }
                }
                int hiddenLights = 0;
                try
                {
                    Light[] allLights = UnityEngine.Object.FindObjectsOfType<Light>(false);
                    foreach (Light lg in allLights)
                    {
                        if (lg == null) continue;
                        try
                        {
                            foreach (MeshRenderer mr in lg.GetComponentsInChildren<MeshRenderer>(false))
                            {
                                if (mr != null) { mr.enabled = false; hiddenLights++; }
                            }
                            foreach (SpriteRenderer sl in lg.GetComponentsInChildren<SpriteRenderer>(false))
                            {
                                if (sl != null) { sl.enabled = false; hiddenLights++; }
                            }
                        }
                        catch (System.Exception) { }
                    }
                    foreach (MeshRenderer mr2 in UnityEngine.Object.FindObjectsOfType<MeshRenderer>(false))
                    {
                        if (mr2 == null || mr2.name == null) continue;
                        try
                        {
                            if (mr2.name.ToLowerInvariant().Contains("bulb") || mr2.name.ToLowerInvariant().Contains("lamp")
                                || mr2.name.ToLowerInvariant().Contains("fluorescent") || mr2.name.ToLowerInvariant().Contains("light"))
                            {
                                mr2.enabled = false;
                                hiddenLights++;
                            }
                        }
                        catch (System.Exception) { }
                    }
                }
                catch (System.Exception lht) { Plugin.Log?.LogWarning("[Ranch] light-hide: " + lht.Message); }
                Plugin.SilentLog($"[Ranch] Terrain: disabled windows/fountains/lockdown doors; hidden {hiddenLights} light device(s) (invisible lights).");
            }
            catch (System.Exception lex) { Plugin.Log?.LogWarning("[Ranch] terrain outdoor-clean failed: " + lex.Message); }

            try
            {
                
                if (!Plugin.activeRanchReskin) {  }
                else
                {
                    Texture2D transparent = new Texture2D(256, 256, UnityEngine.TextureFormat.RGBA32, false);
                    Color[] cols = new Color[256 * 256];
                    for (int i = 0; i < cols.Length; i++) cols[i] = UnityEngine.Color.clear;
                    transparent.SetPixels(cols);
                    transparent.Apply();
                    transparent.name = "MilkRanch_TransparentCeiling";
                    transparent.filterMode = UnityEngine.FilterMode.Bilinear;

                    int atlased = 0;
                    if (ec.mainHall != null)
                    {
                        try { ec.mainHall.ceilTex = transparent; ec.mainHall.GenerateTextureAtlas(); atlased++; }
                        catch (System.Exception mh) { Plugin.Log?.LogWarning("[Ranch] mainHall ceiling clear failed: " + mh.Message); }
                    }
                    foreach (RoomController room in ec.rooms)
                    {
                        if (room == null || room.category == RoomCategory.Special
                            || room.category == RoomCategory.Store
                            || room.category == RoomCategory.Mystery
                            || room.category == RoomCategory.FieldTrip
                            || room.type == RoomType.Hall) continue;
                        try { room.ceilTex = transparent; room.GenerateTextureAtlas(); atlased++; }
                        catch (System.Exception rx) { Plugin.Log?.LogWarning("[Ranch] room ceiling clear failed: " + rx.Message); }
                    }
                    Plugin.SilentLog($"[Ranch] Terrain: cleared ceilings (transparent) on {atlased} room(s) — outdoor no-roof.");
                }
            }
            catch (System.Exception tl) { Plugin.Log?.LogWarning("[Ranch] terrain ceiling-clear failed: " + tl.Message); }

            try
            {
                
                
                if (Plugin.nineNineTriggeredThisRun) { Plugin.SilentLog("[Ranch] Terrain: 99 room entered, skip cow herd."); }
                else
                {
                NPC cowPrefab6 = null;
                try { if (Plugin.Instance != null && Plugin.Instance.assetMan != null) cowPrefab6 = Plugin.Instance.assetMan.Get<NPC>("PolishCow"); }
                catch (System.Exception) { cowPrefab6 = null; }
                if (cowPrefab6 != null && ec.cells != null)
                {
                    var openCells = new System.Collections.Generic.List<Cell>();
                    int xMax6 = ec.cells.GetLength(0), zMax6 = ec.cells.GetLength(1);
                    for (int x = 0; x < xMax6; x++)
                    {
                        for (int z = 0; z < zMax6; z++)
                        {
                            Cell cc = ec.cells[x, z];
                            if (cc == null || cc.Null || cc.offLimits) continue;
                            openCells.Add(cc);
                        }
                    }
                    if (openCells.Count > 0)
                    {
                        int cowCount = 16 + rng.Next(0, 8); 
                        int spawned6 = 0;
                        for (int i = 0; i < cowCount; i++)
                        {
                            Cell cc = openCells[rng.Next(0, openCells.Count)];
                            if (cc == null) continue;
                            try { ec.SpawnNPC(cowPrefab6, cc.position); spawned6++; }
                            catch (System.Exception) { }
                        }
                        Plugin.SilentLog($"[Ranch] Terrain: spawned {spawned6} PolishCow(s) across the ranch field (herd).");
                    }
                }
                }
            }
            catch (System.Exception hx) { Plugin.Log?.LogWarning("[Ranch] terrain cow-herd failed: " + hx.Message); }

            try
            {
                
                int removedDoors = 0;
                if (ec.rooms != null)
                {
                    foreach (RoomController room in ec.rooms)
                    {
                        if (room == null || room.doors == null) continue;
                        for (int di = 0; di < room.doors.Count; di++)
                        {
                            Door d = room.doors[di];
                            if (d == null) continue;
                            try
                            {
                                if (d is SwingDoor)
                                {
                                    UnityEngine.Object.Destroy(d.gameObject);
                                    removedDoors++;
                                }
                            }
                            catch (System.Exception) { }
                        }
                    }
                }
                if (removedDoors > 0) Plugin.SilentLog($"[Ranch] Terrain: removed {removedDoors} swinging door(s) (open ranch, no doors).");
            }
            catch (System.Exception dd) { Plugin.Log?.LogWarning("[Ranch] terrain swing-door removal failed: " + dd.Message); }
        }
    }

    
    
    [HarmonyPatch(typeof(PlayerMovement), "StaminaUpdate")]
    public class PatchRanchStamina
    {
        private static float origDrop = -1f;
        private static float origRise = -1f;
        private static bool cached = false;

        static void Prefix(PlayerMovement __instance)
        {
            try
            {
                if (!cached)
                {
                    origDrop = __instance.staminaDrop;
                    origRise = __instance.staminaRise;
                    cached = true;
                }
                if (!Plugin.activeRanchReskin) return;
                __instance.staminaDrop = origDrop * 0.08f;
                __instance.staminaRise = Mathf.Max(origRise, 4f) * 18f;
            }
            catch (System.Exception) { }
        }

        static void Postfix(PlayerMovement __instance)
        {
            try
            {
                if (cached && origDrop > 0f) __instance.staminaDrop = origDrop;
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    [HarmonyPatch(typeof(LevelBuilder), "StartGenerate")]
    public class PatchSalesmanInject
    {
        
        
        internal static bool IsFinalCelebrationScene(SceneObject so)
        {
            try
            {
                if (so == null) return false;
                string n = so.name ?? "";
                string t = so.levelTitle ?? "";
                return n.IndexOf("Finished", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("YAY", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || t.IndexOf("YAY", System.StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch (System.Exception) { return false; }
        }

        static void Prefix(LevelBuilder __instance)
        {
            try
            {
                if (__instance == null || __instance.scene == null || __instance.ld == null) return;
                if (Plugin.MilkSalesmanPrefab == null) return; 
                if (__instance.scene.levelNo < 0 || __instance.scene.levelNo > 4) return; 
                
                
                if (IsFinalCelebrationScene(__instance.scene)) return;
                
                if (Plugin.MooRedWhiteActive) return;

                
                
                

                
                var runner = __instance.gameObject.GetComponent<SalesmanSpawnRunner>();
                if (runner == null)
                {
                    runner = __instance.gameObject.AddComponent<SalesmanSpawnRunner>();
                    runner.levelNo = __instance.scene.levelNo;
                    runner.StartCoroutine(runner.SpawnWhenReady(__instance.scene.levelNo));
                }
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    public class SalesmanSpawnRunner : UnityEngine.MonoBehaviour
    {
        public int levelNo = -1;

        public System.Collections.IEnumerator SpawnWhenReady(int lvl)
        {
            float wait = 0f;
            LevelBuilder lb = null;
            EnvironmentController ec = null;
            
            
            
            
            while (wait < 60f)
            {
                lb = UnityEngine.Object.FindObjectOfType<LevelBuilder>();
                if (lb != null && lb.Ec != null && !lb.levelInProgress
                    && lb.Ec.rooms != null && lb.Ec.rooms.Count > 0)
                {
                    ec = lb.Ec;
                    for (int k = 0; k < 10; k++) yield return null; 
                    break;
                }
                wait += Time.deltaTime;
                yield return null;
            }
            if (lb == null || ec == null || Plugin.MilkSalesmanPrefab == null)
            {
                
                yield break;
            }
            
            if (Plugin.nineNineTriggeredThisRun) yield break;
            
            try
            {
                var _cgm = Singleton<CoreGameManager>.Instance;
                if (_cgm != null && _cgm.sceneObject != null && PatchSalesmanInject.IsFinalCelebrationScene(_cgm.sceneObject)) yield break;
            }
            catch (System.Exception) { }
            try
            {
                
                
                Cell c = null;
                var hallCells = new System.Collections.Generic.List<Cell>();
                if (ec.mainHall != null && ec.mainHall.cells != null) hallCells.AddRange(ec.mainHall.cells);
                if (ec.rooms != null)
                {
                    foreach (RoomController room in ec.rooms)
                    {
                        if (room != null && room.type == RoomType.Hall && room.cells != null)
                            hallCells.AddRange(room.cells);
                    }
                }
                
                try
                {
                    Vector3 spPt = ec.spawnPoint;
                    hallCells.RemoveAll(cellT =>
                    {
                        if (cellT == null) return true;
                        return Vector3.Distance(cellT.FloorWorldPosition, spPt) < 12f;
                    });
                }
                catch (System.Exception) { }
                
                for (int tries = 0; tries < 50 && hallCells.Count > 0; tries++)
                {
                    var cell = hallCells[UnityEngine.Random.Range(0, hallCells.Count)];
                    if (cell != null) { c = cell; break; }
                }
                
                if (c == null)
                {
                    for (int tries = 0; tries < 50; tries++)
                    {
                        var room = ec.rooms[UnityEngine.Random.Range(0, ec.rooms.Count)];
                        if (room == null) continue;
                        RoomCategory rc = room.category;
                        if (rc == RoomCategory.Special || rc == RoomCategory.Store
                            || rc == RoomCategory.Mystery || rc == RoomCategory.FieldTrip) continue;
                        var sc = room.RandomEntitySafeCellNoGarbage();
                        if (sc != null) { c = sc; break; }
                    }
                }
                if (c == null)
                {
                    
                    yield break;
                }
                ec.SpawnNPC(Plugin.MilkSalesmanPrefab, c.position);
                
            }
            catch (System.Exception )
            {
                
            }
            
            if (this != null) { Destroy(this); }
        }
    }

    
    
    
    
    
    
    [HarmonyPatch(typeof(LevelBuilder), "StartGenerate")]
    public class PatchInjectStampedeEvent
    {
        static void Prefix(LevelBuilder __instance)
        {
            try
            {
                if (__instance == null || __instance.ld == null) return;
                
                
                
                if (Plugin.MooRedWhiteActive)
                {
                    if (__instance.ld.randomEvents != null) __instance.ld.randomEvents.Clear();
                    __instance.ld.minEvents = 0;
                    __instance.ld.maxEvents = 0;
                    return;
                }
                
                if (__instance.scene.levelNo == 0 && Plugin.KeyItemObject != null)
                {
                    var ecF = UnityEngine.Object.FindObjectOfType<EnvironmentController>();
                    if (ecF != null) ecF.StartCoroutine(PatchSpawnKeyInFaculty.EnsureKeyOnFirstFloor(__instance));
                }
                
                __instance.ld.initialEventGap = 10f;
                __instance.ld.minEventGap = 25f;
                __instance.ld.maxEventGap = 50f;

                
                
                
                
                
                
                if (Plugin.StampedeEventTemplate != null || Plugin.MilkFloodEventTemplate != null)
                    PatchInjectStampedeEvent.InjectMilkEventsDirectly(__instance.ld.randomEvents);
                
                if (__instance.ld.minEvents < 1) __instance.ld.minEvents = 1;
                if (__instance.ld.maxEvents < __instance.ld.minEvents) __instance.ld.maxEvents = __instance.ld.minEvents + 1;

                
                
                if (Plugin.KeyItemObject != null && __instance.ld.shopItems != null)
                {
                    bool hasKey = false;
                    foreach (var si in __instance.ld.shopItems)
                    {
                        if (si != null && si.selection == Plugin.KeyItemObject) { hasKey = true; break; }
                    }
                    if (!hasKey)
                    {
                        var shops = new System.Collections.Generic.List<WeightedItemObject>(__instance.ld.shopItems);
                        WeightedItemObject kw = new WeightedItemObject();
                        kw.selection = Plugin.KeyItemObject;
                        kw.weight = 50;
                        shops.Add(kw);
                        __instance.ld.shopItems = shops.ToArray();
                        
                    }
                }
            }
            catch (System.Exception ) {  }
        }

        
        
        internal static void InjectMilkEventsDirectly(System.Collections.Generic.List<WeightedRandomEvent> pool)
        {
            try
            {
                if (pool == null) return;
                if (Plugin.StampedeEventTemplate != null)
                {
                    bool has = false;
                    foreach (var we in pool)
                        if (we != null && we.selection == Plugin.StampedeEventTemplate) { has = true; break; }
                    if (!has)
                    {
                        WeightedRandomEvent ev = new WeightedRandomEvent();
                        ev.selection = Plugin.StampedeEventTemplate;
                        ev.weight = 110;
                        pool.Add(ev);
                        
                    }
                }
                if (Plugin.MilkFloodEventTemplate != null)
                {
                    bool has = false;
                    foreach (var we in pool)
                        if (we != null && we.selection == Plugin.MilkFloodEventTemplate) { has = true; break; }
                    if (!has)
                    {
                        WeightedRandomEvent ev = new WeightedRandomEvent();
                        ev.selection = Plugin.MilkFloodEventTemplate;
                        ev.weight = 110;
                        pool.Add(ev);
                        
                    }
                }
            }
            catch (System.Exception ) {  }
        }
    }
    
    
    
    
    
    [HarmonyPatch(typeof(LevelBuilder), "LoadRoom", new System.Type[] { typeof(RoomAsset), typeof(IntVector2), typeof(IntVector2), typeof(Direction), typeof(bool), typeof(Texture2D), typeof(Texture2D), typeof(Texture2D) })]
    public class PatchSpawnKeyInFaculty
    {
        static void Postfix(LevelBuilder __instance, RoomController __result)
        {
            try
            {
                if (__result == null || __result.ec == null) return;
                RoomController room = __result;

                
                if (Plugin.Room99CategoryReady && (int)(object)room.category == (int)(object)Plugin.Room99Category)
                {
                    int doorCount = (room.doors != null) ? room.doors.Count : 0;
                    
                    
                    if (!Plugin.key99SpawnedThisRun)
                    {
                        Plugin.Last99Room = room;
                        Plugin.key99SpawnedThisRun = true;
                        
                        try { __result.ec.StartCoroutine(Monitor99RoomEntry(__result.ec)); }
                        catch (System.Exception ) {  }
                    }
                    else
                    {
                        Plugin.Last99Room = room; 
                    }
                    try
                    {
                        var cells = room.cells;
                        if (cells != null && cells.Count > 0)
                        {
                            var center = cells[cells.Count / 2];
                            
                        }
                    }
                    catch { }
                    
                    try { __result.ec.StartCoroutine(BuildDoorsFor99(__result, __instance)); }
                    catch (System.Exception ) {  }
                }

                
                
                Plugin.SilentLog("[MilkKey] LoadRoom levelNo=" + __instance.scene.levelNo + " category=" + room.category + " KeyObj=" + (Plugin.KeyItemObject != null) + " spawned=" + Plugin.keySpawnedThisRun);
                if (Plugin.KeyItemObject != null && !Plugin.keySpawnedThisRun
                    && __instance.scene.levelNo == 0
                    && (int)(object)room.category == (int)(object)RoomCategory.Faculty)
                {
                    Plugin.keySpawnedThisRun = true; 
                    room.ec.StartCoroutine(SpawnKeyAfterGeneration(room));
                    Plugin.SilentLog("[MilkKey] Faculty room hit; spawn scheduled.");
                }
            }
            catch (System.Exception ) {  }
        }

        
        private static System.Collections.IEnumerator SpawnKeyAfterGeneration(RoomController room)
        {
            float wait = 0f;
            LevelBuilder lb = null;
            while (wait < 60f)
            {
                lb = UnityEngine.Object.FindObjectOfType<LevelBuilder>();
                if (lb != null && !lb.levelInProgress) break;
                wait += Time.deltaTime;
                yield return null;
            }
            Plugin.SilentLog("[MilkKey] after-wait lb=" + (lb != null) + " stillGenerating=" + (lb != null && lb.levelInProgress) + " wait=" + wait);
            if (lb == null || Plugin.KeyItemObject == null)
            {
                Plugin.SilentLog("[MilkKey] abort: lb=" + (lb != null) + " KeyObj=" + (Plugin.KeyItemObject != null));
                Plugin.keySpawnedThisRun = false;
                Plugin.keySpawnAttemptDone = true;
                yield break;
            }
            if (room == null || room.ec == null) { Plugin.keySpawnAttemptDone = true; yield break; }
            try
            {
                var cell = room.RandomEntitySafeCellNoGarbage();
                if (cell == null)
                {
                    Plugin.SilentLog("[MilkKey] faculty room had no safe cell; retrying later.");
                    Plugin.keySpawnedThisRun = false;
                    Plugin.keySpawnAttemptDone = true;
                    yield break;
                }
                Vector3 pos3 = cell.CenterWorldPosition;
                lb.CreateItem(room, Plugin.KeyItemObject, new Vector2(pos3.x, pos3.z), false);
                Plugin.keySpawnedThisRun = true;
                Plugin.keySpawnAttemptDone = true;
                Plugin.SilentLog("[MilkKey] KEY placed at " + pos3);
            }
            catch (System.Exception e)
            {
                Plugin.SilentLog("[MilkKey] CreateItem threw: " + e);
                Plugin.keySpawnedThisRun = false; 
                Plugin.keySpawnAttemptDone = true;
            }
        }

        
        
        
        internal static System.Collections.IEnumerator EnsureKeyOnFirstFloor(LevelBuilder lb)
        {
            float wait = 0f;
            while (wait < 30f)
            {
                if (lb != null && !lb.levelInProgress) break;
                wait += Time.deltaTime;
                yield return null;
            }
            
            wait = 0f;
            while (!Plugin.keySpawnAttemptDone && wait < 8f)
            {
                wait += Time.deltaTime;
                yield return null;
            }
            yield return null;
            if (Plugin.KeyItemObject == null || Plugin.keySpawnedThisRun) yield break;
            if (lb == null) yield break;
            try
            {
                var ec = UnityEngine.Object.FindObjectOfType<EnvironmentController>();
                if (ec == null) yield break;
                RoomController room = null;
                Cell c = null;
                
                foreach (var r in ec.rooms)
                {
                    if (r == null || r.category != RoomCategory.Faculty) continue;
                    var sc = r.RandomEntitySafeCellNoGarbage();
                    if (sc != null) { room = r; c = sc; break; }
                }
                
                if (c == null)
                {
                    for (int t = 0; t < 80; t++)
                    {
                        if (ec.rooms == null || ec.rooms.Count == 0) break;
                        var r = ec.rooms[UnityEngine.Random.Range(0, ec.rooms.Count)];
                        if (r == null) continue;
                        RoomCategory rc = r.category;
                        if (rc == RoomCategory.Special || rc == RoomCategory.Store
                            || rc == RoomCategory.Mystery || rc == RoomCategory.FieldTrip)
                            continue;
                        var sc = r.RandomEntitySafeCellNoGarbage();
                        if (sc != null) { room = r; c = sc; break; }
                    }
                }
                if (c == null || room == null)
                {
                    Plugin.SilentLog("[MilkKey] EnsureKey: no safe cell anywhere on F1.");
                    yield break;
                }
                Vector3 pos3 = c.CenterWorldPosition;
                lb.CreateItem(room, Plugin.KeyItemObject, new Vector2(pos3.x, pos3.z), false);
                Plugin.keySpawnedThisRun = true;
                Plugin.SilentLog("[MilkKey] EnsureKey (fallback) placed at " + pos3 + " roomCat=" + room.category);
            }
            catch (System.Exception e)
            {
                Plugin.SilentLog("[MilkKey] EnsureKey error: " + e);
            }
        }

        
        
        
        private static System.Collections.IEnumerator BuildDoorsFor99(RoomController room, LevelBuilder lb)
        {
            float wait = 0f;
            while (wait < 60f)
            {
                if (lb != null && !lb.levelInProgress) break;
                wait += UnityEngine.Time.deltaTime;
                yield return null;
            }
            if (lb == null || room == null || room.ec == null)
            {
                
                yield break;
            }
            int built = 0;
            var visited = new System.Collections.Generic.HashSet<IntVector2>(); 
            
            
            if (room.potentialDoorPositions != null && room.potentialDoorPositions.Count > 0)
            {
                built = TryBuild99Doors(room, lb, room.potentialDoorPositions, visited, 4);
            }
            
            
            if (built == 0 && room.cells != null && room.cells.Count > 0)
            {
                
                built = TryBuild99Doors(room, lb, room.cells.ConvertAll(c => c.position), visited, 4);
            }
            if (built == 0) 
            
            
            yield return null;
            
            int locked = 0;
            if (room.doors != null)
            {
                foreach (var d in room.doors)
                {
                    if (d == null) continue;
                    try
                    {
                        d.lockBlocks = true;   
                        d.Lock(true);
                        locked++;
                    }
                    catch (System.Exception ) {  }
                }
            }
            

            
            
            try { room.ec.StartCoroutine(Keep99DoorsLocked(room.ec, room)); }
            catch (System.Exception ) {  }
        }

        
        
        
        private static int TryBuild99Doors(RoomController room, LevelBuilder lb,
            System.Collections.Generic.IEnumerable<IntVector2> candidates,
            System.Collections.Generic.HashSet<IntVector2> visited, int maxDoors)
        {
            int built = 0;
            foreach (var pos in candidates)
            {
                if (visited.Contains(pos) || built >= maxDoors) continue;
                for (int i = 0; i < 4; i++)
                {
                    Direction dir = (Direction)i;
                    IntVector2 adj = pos + dir.ToIntVector2();
                    if (!room.ec.ContainsCoordinates(adj)) continue;
                    Cell adjCell = room.ec.CellFromPosition(adj);
                    if (adjCell.Null || adjCell.room == null || adjCell.room == room) continue;
                    if (adjCell.WallHardCovered(dir.GetOpposite())) continue; 
                    try
                    {
                        room.ec.ConnectCells(pos, dir); 
                        lb.ManuallyBuildDoor(room, null, pos, dir);
                        visited.Add(pos);
                        built++;
                        
                    }
                    catch (System.Exception ) {  }
                    break; 
                }
            }
            return built;
        }

        
        
        
        private static System.Collections.IEnumerator Keep99DoorsLocked(EnvironmentController ec, RoomController room)
        {
            while (true)
            {
                if (Plugin.nineNineDoorUnlockedByPlayer) yield break;
                if (room == null || room.ec == null) yield break;
                try
                {
                    if (room.doors != null)
                    {
                        foreach (var d in room.doors)
                        {
                            if (d == null) continue;
                            try
                            {
                                d.Shut();
                                d.lockBlocks = true;
                                d.Lock(true);
                            }
                            catch (System.Exception) { }
                        }
                    }
                    
                    
                    if (Plugin.Room99CategoryReady)
                    {
                        foreach (var d in UnityEngine.Object.FindObjectsOfType<Door>())
                        {
                            if (d == null) continue;
                            try
                            {
                                bool touches99 = false;
                                var aTile = d.aTile; var bTile = d.bTile;
                                if (aTile != null && aTile.room != null && (int)(object)aTile.room.category == (int)(object)Plugin.Room99Category) touches99 = true;
                                if (!touches99 && bTile != null && bTile.room != null && (int)(object)bTile.room.category == (int)(object)Plugin.Room99Category) touches99 = true;
                                if (touches99)
                                {
                                    d.Shut();
                                    d.lockBlocks = true;
                                    d.Lock(true);
                                    try { d.Block(true); } catch (System.Exception) { }
                                }
                            }
                            catch (System.Exception) { }
                        }
                    }
                }
                catch (System.Exception ) {  }
                yield return null;
            }
        }

        
        
        
        private static System.Collections.IEnumerator Monitor99RoomEntry(EnvironmentController ec)
        {
            float wait = 0f;
            while (wait < 60f)
            {
                LevelBuilder lb = UnityEngine.Object.FindObjectOfType<LevelBuilder>();
                if (lb != null && !lb.levelInProgress) break;
                wait += UnityEngine.Time.deltaTime;
                yield return null;
            }
            while (true)
            {
                if (Plugin.nineNineTriggeredThisRun) yield break;
                RoomController hit = null;
                bool shouldBreak = false;
                try
                {
                    CoreGameManager cgm = Singleton<CoreGameManager>.Instance;
                    if (cgm == null) { shouldBreak = true; }
                    else if (ec == null) { shouldBreak = true; }
                    else
                    {
                        PlayerManager player = cgm.GetPlayer(0);
                        if (player != null)
                        {
                            Cell cell = ec.CellFromPosition(player.transform.position);
                            if (cell != null && !cell.Null && cell.room != null
                                && Plugin.Room99CategoryReady
                                && (int)(object)cell.room.category == (int)(object)Plugin.Room99Category)
                            {
                                hit = cell.room;
                            }
                        }
                    }
                }
                catch (System.Exception ) {  }
                if (shouldBreak) yield break;
                if (hit != null) { Activate99Room(hit); yield break; }
                yield return null;
            }
        }

        
        private static void Activate99Room(RoomController room)
        {
            if (Plugin.nineNineTriggeredThisRun) return;
            Plugin.nineNineTriggeredThisRun = true;

            
            try
            {
                int cleared = 0;
                
                if (room.ec != null && room.ec.Npcs != null)
                {
                    var npcs = room.ec.Npcs;
                    for (int i = npcs.Count - 1; i >= 0; i--)
                    {
                        var npc = npcs[i];
                        if (npc == null) continue;
                        try { npc.Despawn(); cleared++; } catch (System.Exception) { }
                        try { UnityEngine.Object.Destroy(npc.gameObject); } catch (System.Exception) { }
                    }
                    try { npcs.Clear(); } catch (System.Exception) { }
                }
                
                foreach (var npc in UnityEngine.Object.FindObjectsOfType<NPC>())
                {
                    if (npc == null || !npc.gameObject.activeInHierarchy) continue;
                    try { npc.Despawn(); cleared++; } catch (System.Exception) { }
                    try { UnityEngine.Object.Destroy(npc.gameObject); } catch (System.Exception) { }
                }
                
                try
                {
                    var ec = room.ec;
                    if (ec != null)
                    {
                        var fNpc = AccessTools.Field(ec.GetType(), "npcs") ?? AccessTools.Field(ec.GetType(), "Npcs");
                        if (fNpc != null)
                        {
                            var npcList = fNpc.GetValue(ec) as System.Collections.IList;
                            if (npcList != null) try { npcList.Clear(); } catch (System.Exception) { }
                        }
                        
                        try
                        {
                            var fToSpawn = AccessTools.Field(ec.GetType(), "npcsToSpawn");
                            if (fToSpawn != null) { var l = fToSpawn.GetValue(ec) as System.Collections.IList; if (l != null) try { l.Clear(); } catch (System.Exception) { } }
                            var fLeft = AccessTools.Field(ec.GetType(), "npcsLeftToSpawn");
                            if (fLeft != null) { var l = fLeft.GetValue(ec) as System.Collections.IList; if (l != null) try { l.Clear(); } catch (System.Exception) { } }
                        }
                        catch (System.Exception) { }
                    }
                }
                catch (System.Exception) { }
            }
            catch (System.Exception ) {  }

            
            
            try
            {
                foreach (var runner in UnityEngine.Object.FindObjectsOfType<SalesmanSpawnRunner>())
                {
                    if (runner == null) continue;
                    try { runner.StopAllCoroutines(); } catch (System.Exception) { }
                    try { runner.enabled = false; } catch (System.Exception) { }
                }
                foreach (var rtr in UnityEngine.Object.FindObjectsOfType<RanchTerrainRework>())
                {
                    if (rtr == null) continue;
                    try { rtr.StopAllCoroutines(); } catch (System.Exception) { }
                    try { rtr.enabled = false; } catch (System.Exception) { }
                }
            }
            catch (System.Exception ) {  }

            
            try
            {
                if (room.doors != null)
                {
                    int locked = 0;
                    foreach (var d in room.doors)
                    {
                        if (d == null) continue;
                        try
                        {
                            d.Shut();
                            d.lockBlocks = true;
                            d.Lock(true);
                            locked++;
                        }
                        catch (System.Exception) { }
                    }
                    
                }
            }
            catch (System.Exception ) {  }

            
            try
            {
                CoreGameManager cgm = Singleton<CoreGameManager>.Instance;
                if (cgm != null)
                {
                    if (cgm.audMan != null) cgm.audMan.FlushQueue(true);
                    if (cgm.musicMan != null) cgm.musicMan.FlushQueue(true);
                    
                }
            }
            catch (System.Exception ) {  }

            
            try
            {
                var ec = room.ec;
                if (ec != null)
                {
                    var fEvents = AccessTools.Field(ec.GetType(), "events");
                    var fEventTimes = AccessTools.Field(ec.GetType(), "eventTimes");
                    var fCurrent = AccessTools.Field(ec.GetType(), "currentEvents");
                    if (fEvents != null) fEvents.SetValue(ec, new System.Collections.Generic.List<RandomEvent>());
                    if (fEventTimes != null) fEventTimes.SetValue(ec, new System.Collections.Generic.List<float>());
                    if (fCurrent != null)
                    {
                        var cur = fCurrent.GetValue(ec) as System.Collections.Generic.List<RandomEvent>;
                        if (cur != null)
                        {
                            var copy = new System.Collections.Generic.List<RandomEvent>(cur);
                            foreach (var r in copy)
                            {
                                if (r == null) continue;
                                try
                                {
                                    var endM = r.GetType().GetMethod("End", System.Type.EmptyTypes);
                                    if (endM != null) endM.Invoke(r, null);
                                }
                                catch (System.Exception) { }
                                try { UnityEngine.Object.Destroy(r); } catch (System.Exception) { }
                            }
                            cur.Clear();
                        }
                    }
                    
                }
            }
            catch (System.Exception ) {  }

            
        }
    }

    
    
    
    [HarmonyPatch(typeof(ElevatorScreen), "UpdateFloorDisplay")]
    public class PatchElevatorFloorDisplayMilk
    {
        static void Postfix(ElevatorScreen __instance)
        {
            try
            {
                CoreGameManager cgm = Singleton<CoreGameManager>.Instance;
                if (cgm == null || cgm.sceneObject == null) return;
                bool isMilk = Plugin.IsFactoryFloor(cgm.sceneObject.levelNo);
                if (!isMilk) return;
                var ftField = AccessTools.Field(typeof(ElevatorScreen), "floorText");
                if (ftField == null) return;
                TMP_Text ft = ftField.GetValue(__instance) as TMP_Text;
                if (ft != null) ft.text = "Mi" + (cgm.sceneObject.levelNo + 1);
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    
    [HarmonyPatch(typeof(ElevatorScreen), "UpdateFloorDisplay")]
    public class PatchMooElevatorGarble
    {
        static void Postfix(ElevatorScreen __instance)
        {
            try
            {
                
                
                
                if (Plugin.MooRedWhiteActive)
                {
                    float c = Time.unscaledTime % 1.2f;
                    var ff = AccessTools.Field(typeof(ElevatorScreen), "floorText");
                    if (ff != null)
                    {
                        TMP_Text ft = ff.GetValue(__instance) as TMP_Text;
                        if (ft != null)
                        {
                            if (c < 0.58f)
                                ft.text = Plugin.MakeGarble(UnityEngine.Random.Range(4, 7)) + "F";
                            else if (c < 0.88f)
                                ft.text = "!! DANGER !!";
                            else
                                ft.text = "99%" + Plugin.MakeGarble(3, true) + "F";
                        }
                    }
                    var sf = AccessTools.Field(typeof(ElevatorScreen), "seedText");
                    if (sf != null)
                    {
                        TMP_Text st = sf.GetValue(__instance) as TMP_Text;
                        if (st != null)
                        {
                            if (c < 0.40f)
                                st.text = Plugin.MakeGarble(9, true);
                            else if (c < 0.70f)
                                st.text = "SEED UNKNOWN";
                            else if (c < 0.90f)
                                st.text = "NO SIGNAL";
                            else
                                st.text = "ERROR " + Plugin.MakeGarble(4, true);
                        }
                    }
                    return;
                }

                if (!Plugin.MooF1Active) return;
                
                if (Plugin.MooElevFloorGarble == null || Plugin.MooElevSeedGarble == null) Plugin.RerollMooGarble();
                var ff2 = AccessTools.Field(typeof(ElevatorScreen), "floorText");
                if (ff2 != null)
                {
                    TMP_Text ft = ff2.GetValue(__instance) as TMP_Text;
                    if (ft != null) ft.text = Plugin.MooElevFloorGarble;
                }
                var sf2 = AccessTools.Field(typeof(ElevatorScreen), "seedText");
                if (sf2 != null)
                {
                    TMP_Text st = sf2.GetValue(__instance) as TMP_Text;
                    if (st != null) st.text = Plugin.MooElevSeedGarble;
                }
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    
    [HarmonyPatch(typeof(ElevatorManager), "Update")]
    public class PatchElevatorManagerUpdateGuard
    {
        static bool _warned = false;
        static bool Prefix(ElevatorManager __instance)
        {
            var f = AccessTools.Field(typeof(ElevatorManager), "environmentController");
            if (f != null && f.GetValue(__instance) == null)
            {
                if (!_warned)
                {
                    _warned = true;
                    
                }
                return false; 
            }
            return true;
        }
        static System.Exception Finalizer(ElevatorManager __instance, System.Exception __exception)
        {
            if (__exception != null)
            {
                try {  } catch { }
            }
            return null; 
        }
    }

    
    [HarmonyPatch(typeof(LevelBuilder), "StartGenerate")]
    public class PatchMilkFactoryReset
    {
        static void Prefix(LevelBuilder __instance)
        {
            try
            {
                if (__instance == null || __instance.scene == null) return;
                if (__instance.scene.levelNo == 0) { Plugin.Last99Room = null; Plugin.nineNineTriggeredThisRun = false; Plugin.nineNineDoorUnlockedByPlayer = false; if (!Plugin.factoryPlanRolled) { Plugin.RollReplacementPlan(); Plugin.factoryPlanRolled = true; } }
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    
    
    
    
    
    
    
    
    [HarmonyPatch(typeof(RoomFunctionContainer), "OnGenerationFinished")]
    public class PatchMilkClassroomLight
    {
        static void Postfix(RoomFunctionContainer __instance)
        {
            try
            {
                var roomField = AccessTools.Field(typeof(RoomFunctionContainer), "room");
                RoomController room = (roomField != null) ? (roomField.GetValue(__instance) as RoomController) : null;
                if (room == null) return;
                
                if (!Plugin.MilkMachineClassroomCategoryReady) return;
                if ((int)(object)room.category != (int)(object)Plugin.MilkMachineClassroomCategory) return;
                EnvironmentController ec = room.ec;
                if (ec == null) return;
                
                if (room.lightPre != null && room.lights != null && room.lights.Count > 0) return;
                
                Transform lt = null;
                if (LevelLoaderPlugin.Instance.lightTransforms.TryGetValue("standardhanging", out lt) && lt != null)
                {
                    room.lightPre = lt;
                }

                
                var candidates = new System.Collections.Generic.List<Cell>();
                if (room.entitySafeCells != null && room.entitySafeCells.Count > 0)
                {
                    foreach (IntVector2 ip in room.entitySafeCells)
                    {
                        Cell c = ec.CellFromPosition(ip);
                        if (c != null) candidates.Add(c);
                    }
                }
                if (candidates.Count == 0)
                {
                    foreach (Cell c in room.cells) candidates.Add(c);
                }
                if (candidates.Count == 0) return;

                
                int added = 0;
                foreach (Cell c in candidates)
                {
                    try { ec.GenerateLight(c, Color.white, 9, false); added++; }
                    catch (System.Exception) { }
                }
                
            }
            catch (System.Exception ) {  }
        }
    }

    [HarmonyPatch(typeof(RoomFunctionContainer), "OnGenerationFinished")]
    public class PatchMilkHallDecor
    {
        static void Postfix(RoomFunctionContainer __instance)
        {
            try
            {
                var roomField = AccessTools.Field(typeof(RoomFunctionContainer), "room");
                RoomController room = (roomField != null) ? (roomField.GetValue(__instance) as RoomController) : null;
                if (room == null) return;
                if (room.type != RoomType.Hall) return; 

                CoreGameManager cgm = Singleton<CoreGameManager>.Instance;
                bool isFactory = Plugin.IsFactoryFloor(cgm != null && cgm.sceneObject != null ? cgm.sceneObject.levelNo : -1);
                if (!isFactory) return;

                Plugin.SpawnMilkHallDecor(room);
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    
    [HarmonyPatch(typeof(Structure_StudentSpawner), "OnGenerationFinished")]
    public class PatchFactorySkipStudentSpawn
    {
        static bool Prefix(Structure_StudentSpawner __instance)
        {
            try
            {
                CoreGameManager cgm = Singleton<CoreGameManager>.Instance;
                
                if (Plugin.IsFactoryFloor(cgm != null && cgm.sceneObject != null ? cgm.sceneObject.levelNo : -1))
                {
                    
                    return false; 
                }
            }
            catch (System.Exception ) {  }
            return true;
        }
    }

    
    
    
    
    
    [HarmonyPatch(typeof(EnvironmentController), "RespawnItemInRandomRoom")]
    public class PatchFactoryRespawnGuard
    {
        static bool Prefix(EnvironmentController __instance, ItemObject item)
        {
            bool anyBad = false;
            foreach (RoomController room in __instance.rooms)
            {
                if ((object)room == null || room.gameObject == null) { anyBad = true; break; }
            }
            if (!anyBad) return true; 
            System.Collections.Generic.List<Pickup> list = new System.Collections.Generic.List<Pickup>();
            foreach (RoomController room in __instance.rooms)
            {
                if ((object)room == null || room.gameObject == null) continue;
                if (room.pickups == null) continue;
                foreach (Pickup p in room.pickups)
                {
                    if (p != null && p.gameObject != null && !p.gameObject.activeSelf) list.Add(p);
                }
            }
            if (list.Count > 0)
            {
                Pickup pickup = list[UnityEngine.Random.Range(0, list.Count)];
                pickup.AssignItem(item);
                pickup.Hide(hidden: false);
            }
            
            return false;
        }
    }

    
    
    [HarmonyPatch(typeof(EnvironmentController), "RespawnItemInRoom")]
    public class PatchFactoryRespawnInRoomGuard
    {
        static bool Prefix(EnvironmentController __instance, ItemObject item, RoomController room)
        {
            if ((object)room == null || room.gameObject == null) return false;
            return true;
        }
    }

    
    
    
    [HarmonyPatch(typeof(EnvironmentController), "Update")]
    internal class PatchEnvironmentControllerUpdateSafe
    {
        [HarmonyFinalizer]
        static void Finalizer(EnvironmentController __instance, ref System.Exception __exception)
        {
            if (__exception == null) return;
            try
            {
                
                System.Type targetCol = typeof(ICollection<RandomEvent>);
                System.Reflection.FieldInfo[] fields = typeof(EnvironmentController).GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                foreach (var f in fields)
                {
                    if (f.IsLiteral || f.IsInitOnly) continue;
                    object val = null;
                    try { val = f.GetValue(__instance); } catch { continue; }
                    if (val == null) continue;
                    if (!targetCol.IsAssignableFrom(val.GetType())) continue;
                    var list = (System.Collections.IEnumerable)val;
                    var toRemove = new System.Collections.Generic.List<RandomEvent>();
                    foreach (var o in list)
                    {
                        RandomEvent re = o as RandomEvent;
                        if (re == null) continue;
                        UnityEngine.Component c = re;
                        if (c == null || c.gameObject == null) toRemove.Add(re);
                    }
                    foreach (var re in toRemove)
                    {
                        try { targetCol.GetMethod("Remove").Invoke(val, new object[] { re }); } catch { }
                    }
                    
                    foreach (var re in toRemove)
                    {
                        foreach (var f2 in fields)
                        {
                            if (f2.FieldType != typeof(RandomEvent)) continue;
                            object cur = null;
                            try { cur = f2.GetValue(__instance); } catch { continue; }
                            if (object.ReferenceEquals(cur, re)) { try { f2.SetValue(__instance, null); } catch { } }
                        }
                    }
                }
            }
            catch { }
            __exception = null;
        }
    }

    
    
    
    
    
    [HarmonyPatch(typeof(NpcStateMachine), "Update")]
    internal class PatchNpcStateMachineUpdateSafe
    {
        [HarmonyFinalizer]
        static void Finalizer(ref System.Exception __exception)
        {
            if (__exception == null) return;
            if (__exception is System.NullReferenceException) __exception = null;
        }
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    [HarmonyPatch]
    internal class PatchChalkFaceMilkLabel
    {
        static System.Reflection.MethodBase TargetMethod()
        {
            var t = typeof(ChalkFace);
            foreach (var name in new[] { "UpdateText", "Render", "SetTexts" })
            {
                var m = t.GetMethod(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (m != null) return m;
            }
            return null;
        }
        static bool Prepare() => TargetMethod() != null;
        static System.Reflection.FieldInfo GetTextComponentField()
        {
            
            foreach (var f in typeof(ChalkFace).GetFields(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                if (f.FieldType == typeof(TMP_Text) || f.FieldType == typeof(TextMesh) || f.FieldType == typeof(UnityEngine.UI.Text))
                    return f;
            }
            return null;
        }
        
        static void Postfix(ChalkFace __instance)
        {
            try
            {
                CoreGameManager cgm = Singleton<CoreGameManager>.Instance;
                if (cgm == null || cgm.sceneObject == null) return;
                int nextNo = cgm.sceneObject.levelNo;
                if (!Plugin.IsFactoryFloor(nextNo)) return; 

                var f = GetTextComponentField();
                if (f == null) return;
                object val = f.GetValue(__instance);
                if (val == null) return;
                ReplaceFactoryKeywordOnText(val);
            }
            catch { }
        }
        internal static void ReplaceFactoryKeywordOnText(object textComponent)
        {
            try
            {
                if (textComponent == null) return;
                string text = null;
                System.Reflection.PropertyInfo prop = textComponent.GetType().GetProperty("text",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (prop == null || !prop.CanRead || !prop.CanWrite) return;
                text = prop.GetValue(textComponent, null) as string;
                if (string.IsNullOrEmpty(text)) return;
                string nu = text
                    .Replace("Factory", "Milk")
                    .Replace("FACTORY", "MILK")
                    .Replace("Laboratory", "Milk")
                    .Replace("LABORATORY", "MILK")
                    .Replace("工厂", "牛奶")
                    .Replace("實驗室", "牛奶")
                    .Replace("实验室", "牛奶");
                if (nu != text) prop.SetValue(textComponent, nu, null);
            }
            catch { }
        }
    }
    
    
    
    
    [HarmonyPatch]
    internal class PatchPitStopMilkLabelFallback
    {
        static System.Type TryGetPitStopType()
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type t = null;
                try { t = asm.GetType("Structure_PitStop", false); } catch { }
                if (t != null) return t;
            }
            return null;
        }
        static System.Reflection.MethodBase TargetMethod()
        {
            System.Type t = TryGetPitStopType();
            if (t == null) return null;
            var m = t.GetMethod("OnGenerationFinished",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (m == null) m = t.GetMethod("Initialize",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null, System.Type.EmptyTypes, null);
            return m;
        }
        static bool Prepare() => TryGetPitStopType() != null;
        static void Postfix(RoomFunction __instance)
        {
            try
            {
                CoreGameManager cgm = Singleton<CoreGameManager>.Instance;
                if (cgm == null || cgm.sceneObject == null) return;
                int nextNo = cgm.sceneObject.levelNo;
                if (!Plugin.IsFactoryFloor(nextNo)) return;
                foreach (var c in __instance.GetComponentsInChildren<TMP_Text>(true))
                    PatchChalkFaceMilkLabel.ReplaceFactoryKeywordOnText(c);
                foreach (var c in __instance.GetComponentsInChildren<TextMesh>(true))
                    PatchChalkFaceMilkLabel.ReplaceFactoryKeywordOnText(c);
                foreach (var c in __instance.GetComponentsInChildren<UnityEngine.UI.Text>(true))
                    PatchChalkFaceMilkLabel.ReplaceFactoryKeywordOnText(c);
            }
            catch { }
        }
    }

    
    
    
    
    
    
    [HarmonyPatch(typeof(CoreGameManager), "Pause", new System.Type[] { typeof(bool) })]
    internal class Patch99EffectDisablePause
    {
        static bool Prefix()
        {
            try
            {
                if (Plugin.MooRedWhiteActive) return false; 
                if (Plugin.MooF1Active) return false;       
            }
            catch { }
            return true;
        }
    }

    
    
    
    
    
    [HarmonyPatch(typeof(PlayerManager), "RuleBreak", new System.Type[] { typeof(string), typeof(float), typeof(float) })]
    internal class PatchMilkSodaNoDrinkingRule
    {
        static bool Prefix(PlayerManager __instance, string rule)
        {
            try
            {
                if (string.IsNullOrEmpty(rule) || rule != "Drinking") return true;
                
                foreach (var evt in UnityEngine.Object.FindObjectsOfType<MilkFloodEvent>())
                {
                    if (evt != null && evt.IsActive) return true;
                }
                
                
                if (__instance.ec != null && __instance.ec.timeOut) return false;
                
                if (UnityEngine.Time.realtimeSinceStartup > Plugin.sodaDrinkNoRuleBreakUntil) return true;
                return false; 
            }
            catch { return true; }
        }
    }

    
    
    
    
    
    [HarmonyPatch(typeof(NoLateTeacher), "AssignClassRoom", new System.Type[] { typeof(PlayerManager) })]
    internal class PatchNoLateTeacherTweaksCompat
    {
        [HarmonyFinalizer]
        static void Finalizer(ref System.Exception __exception)
        {
            __exception = null; 
        }
    }

    
    
    
    
    
    
    
    
    [HarmonyPatch(typeof(Ambience), "Initialize")]
    internal class PatchAmbienceTweaksCompat
    {
        static void Prefix(Ambience __instance)
        {
            try
            {
                if (__instance == null) return;
                var audMan = __instance.audMan;
                if (audMan == null) return;
                if (audMan.audioDevice != null) return; 
                var go = new GameObject("TweaksPlusCompatAudio");
                go.transform.SetParent(audMan.transform, false);
                audMan.audioDevice = go.AddComponent<AudioSource>();
            }
            catch (System.Exception) { }
        }
    }

    
    
    [HarmonyPatch(typeof(Baldi_Chase), "OnStateTriggerStay")]
    public class PatchBaldiAppleMilk
    {
        
        
        public static Baldi pendingAppleMilkBaldi;

        static bool Prefix(Baldi_Chase __instance, Entity otherEntity, Collider other, bool validCollision)
        {
            if (!validCollision || !other.CompareTag("Player")) return true;
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player == null || player.invincible) return true;

            Baldi baldi = __instance.Npc as Baldi;
            if (baldi == null || baldi.looker == null) return true;

            
            try
            {
                baldi.looker.Raycast(other.transform,
                    Vector3.Magnitude(baldi.transform.position - other.transform.position),
                    out bool targetSighted);
                if (!targetSighted) return true;
            }
            catch (System.Exception) { return true; }

            
            var itm = player.itm;
            if (itm == null) return true;
            int appleMilkSlot = -1;
            for (int i = 0; i <= itm.maxItem; i++)
            {
                if (itm.items[i] != null && itm.items[i] == Plugin.AppleMilkItemObject)
                {
                    appleMilkSlot = i;
                    break;
                }
            }
            if (appleMilkSlot < 0) return true; 

            
            
            itm.RemoveItem(appleMilkSlot);

            
            try
            {
                baldi.TakeApple();

                
                baldi.StartCoroutine(AppleMilkVisual.AppleMilkVisualCoroutine(baldi));
                pendingAppleMilkBaldi = null; 
            }
            catch (System.Exception )
            {
                
            }

            
            try
            {
                if (baldi.ec != null && baldi.ec.Npcs != null)
                {
                    foreach (var npc2 in baldi.ec.Npcs)
                    {
                        if (npc2 is FirstPrize firstPrize)
                        {
                            firstPrize.CutWires();
                            
                        }
                    }
                }
            }
            catch (System.Exception )
            {
                
            }

            return false; 
        }
    }

    
    
    
    public static class AppleMilkVisual
    {
        public static IEnumerator AppleMilkVisualCoroutine(Baldi baldi)
        {
            if (baldi == null) yield break;
            object anim = GetBaldiAnimator(baldi);
            
            
            SpriteRenderer sr = (baldi.spriteRenderer != null && baldi.spriteRenderer.Length > 0)
                ? baldi.spriteRenderer[0]
                : baldi.GetComponentsInChildren<SpriteRenderer>().FirstOrDefault();
            if (sr == null)
            {
                
                yield break;
            }
            Sprite originalSprite = sr.sprite;               
            Vector3 originalScale = sr.transform.localScale;  
            if (Plugin.AppleMilkBaldiSprite1 != null)
            {
                sr.sprite = Plugin.AppleMilkBaldiSprite1;
                FitSpriteScale(sr, originalSprite, originalScale);
            }

            
            
            
            try
            {
                if (Plugin.AppleMilkSound != null && baldi.AudMan != null)
                    baldi.AudMan.PlaySingle(Plugin.AppleMilkSound);
            }
            catch (System.Exception) { }

            
            object origCtrl = null;
            if (anim != null)
            {
                var ctrlProp = anim.GetType().GetProperty("runtimeAnimatorController");
                if (ctrlProp != null) { origCtrl = ctrlProp.GetValue(anim); ctrlProp.SetValue(anim, null); }
                else
                {
                    var ctrlField = anim.GetType().GetField("runtimeAnimatorController", BindingFlags.Public | BindingFlags.Instance);
                    if (ctrlField != null) { origCtrl = ctrlField.GetValue(anim); ctrlField.SetValue(anim, null); }
                }
            }

            ExtendAppleFreeze(baldi);

            float voice = Plugin.AppleMilkAudioLength > 0.1f ? Plugin.AppleMilkAudioLength : 2f;
            yield return new WaitForSeconds(voice);

            bool toggle = false;
            float flashTimeout = 15f; 
            while (flashTimeout > 0f)
            {
                Sprite nextSprite = toggle ? Plugin.AppleMilkBaldiSprite : Plugin.AppleMilkBaldiSprite1;
                sr.sprite = nextSprite;
                FitSpriteScale(sr, originalSprite, originalScale);
                toggle = !toggle;
                yield return new WaitForSeconds(0.12f);
                flashTimeout -= 0.12f;
                if (IsInAppleState(baldi)) continue;   
                if (flashTimeout < 3f) break;          
            }

            if (anim != null)
            {
                var ctrlProp = anim.GetType().GetProperty("runtimeAnimatorController");
                if (ctrlProp != null) ctrlProp.SetValue(anim, origCtrl);
                else { var ctrlField = anim.GetType().GetField("runtimeAnimatorController", BindingFlags.Public | BindingFlags.Instance); if (ctrlField != null) ctrlField.SetValue(anim, origCtrl); }
            }
            
            sr.transform.localScale = originalScale;
        }

        private static object GetBaldiAnimator(Baldi baldi)
        {
            try
            {
                
                
                var f = typeof(Baldi).GetField("animator",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (f == null) f = typeof(Baldi).GetField("Animator",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                return f?.GetValue(baldi);
            }
            catch (System.Exception) { return null; }
        }

        
        
        private static void FitSpriteScale(SpriteRenderer sr, Sprite originalSprite, Vector3 originalScale)
        {
            if (sr == null || originalSprite == null) return;
            Sprite target = sr.sprite;
            if (target == null) return;
            try
            {
                Vector3 o = originalSprite.bounds.size;
                Vector3 t = target.bounds.size;
                if (o.x > 1e-4f && o.y > 1e-4f && t.x > 1e-4f && t.y > 1e-4f)
                {
                    float sx = o.x / t.x;
                    float sy = o.y / t.y;
                    sr.transform.localScale = new Vector3(originalScale.x * sx, originalScale.y * sy, originalScale.z);
                }
            }
            catch (System.Exception) { }
        }

        private static object GetBaldiStateMachine(Baldi baldi)
        {
            try
            {
                var t = typeof(NPC);
                var f = t.GetField("behaviorStateMachine", BindingFlags.NonPublic | BindingFlags.Instance);
                if (f != null) return f.GetValue(baldi);
                var p = t.GetProperty("behaviorStateMachine", BindingFlags.NonPublic | BindingFlags.Instance);
                if (p != null) return p.GetValue(baldi);
            }
            catch (System.Exception) { }
            return null;
        }

        private static object GetBaldiCurrentState(Baldi baldi)
        {
            var sm = GetBaldiStateMachine(baldi);
            if (sm == null) return null;
            try
            {
                var t = sm.GetType();
                var p = t.GetProperty("CurrentState");
                if (p != null) return p.GetValue(sm);
                var f = t.GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance)
                     ?? t.GetField("CurrentState", BindingFlags.NonPublic | BindingFlags.Instance);
                if (f != null) return f.GetValue(sm);
            }
            catch (System.Exception) { }
            return null;
        }

        private static bool IsInAppleState(Baldi baldi)
        {
            var s = GetBaldiCurrentState(baldi);
            return s != null && s.GetType().Name == "Baldi_Apple";
        }

        private static void ExtendAppleFreeze(Baldi baldi)
        {
            var s = GetBaldiCurrentState(baldi);
            if (s == null || s.GetType().Name != "Baldi_Apple") return;
            try
            {
                var tField = s.GetType().GetField("time", BindingFlags.Public | BindingFlags.Instance);
                if (tField == null) return;
                float t = (float)tField.GetValue(s);
                
                tField.SetValue(s, t * Plugin.AppleMilkFreezeScale);
            }
            catch (System.Exception) { }
        }
    }

    

    
    
    
    public class PoisonMilkComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            
            try { AchievementHelper.UnlockAchievement("milk_poison"); } catch (System.Exception) { }
            player.StartCoroutine(Plugin.PoisonMilkPlayerEffectCoroutine(player, 20f));
            return true; 
        }
    }

    
    
    public class AppleMilkComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();
            Plugin.StopMilkRandomEvents(); 
            if (player.plm != null)
            {
                int amount = UnityEngine.Random.Range(50, 101);
                player.plm.AddStamina(amount, true);
                
            }
            try { AchievementHelper.UnlockAchievement("milk_apple"); } catch (System.Exception) { }
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.AppleMilkItemObject);
        }
    }

    
    public class ReverseMilkComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();
            Plugin.StopMilkRandomEvents(); 
            player.StartCoroutine(ReverseMilkCoroutine(player));
            
            try { AchievementHelper.UnlockAchievement("milk_reverse"); } catch (System.Exception) { }
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.ReverseMilkItemObject);
        }

        
        private static System.Collections.IEnumerator ReverseMilkCoroutine(PlayerManager player)
        {
            float duration = 35f; 

            Entity entity = player.plm != null ? player.plm.Entity : null;
            bool flipped = false;
            try
            {
                if (entity != null)
                {
                    entity.Flip();
                    flipped = true;
                }
            }
            catch (System.Exception )
            {
                
            }

            if (flipped)
            {
                float elapsed = 0f;
                while (elapsed < duration && player != null)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                try
                {
                    if (entity != null) entity.Flip();
                }
                catch (System.Exception) { }
                
            }
        }
    }

    
    
    public class WindowMilkComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();
            Plugin.StopMilkRandomEvents(); 
            player.StartCoroutine(WindowMilkCoroutine());
            
            try { AchievementHelper.UnlockAchievement("milk_window"); } catch (System.Exception) { }
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.WindowMilkItemObject);
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern System.IntPtr GetActiveWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetWindowRect(System.IntPtr hWnd, out RECT lpRect);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;

        private static System.Collections.IEnumerator WindowMilkCoroutine()
        {
            
            if (MilkSettings.WindowEffects != null && !MilkSettings.WindowEffects.Value) yield break;
            float duration = 12f;
            bool wasFull = Screen.fullScreen;

            
            try { Screen.fullScreen = false; }
            catch (System.Exception ) {  }
            yield return null; 

            System.IntPtr hWnd = GetActiveWindow();
            if (hWnd == System.IntPtr.Zero)
            {
                try { Screen.fullScreen = wasFull; } catch (System.Exception) { }
                yield break;
            }

            
            RECT orig;
            int origLeft = 0, origTop = 0, w = 0, h = 0;
            if (GetWindowRect(hWnd, out orig))
            {
                origLeft = orig.Left; origTop = orig.Top;
                w = orig.Right - orig.Left; h = orig.Bottom - orig.Top;
            }

            int screenW = Screen.currentResolution.width;
            int screenH = Screen.currentResolution.height;

            float moveSpeed = 220f;   
            float floatAmp = 70f;     
            float floatFreq = 2.2f;   
            float t = 0f;
            float x = origLeft;       
            float baseY = origTop;    
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                t += Time.deltaTime;
                if (hWnd != System.IntPtr.Zero)
                {
                    x -= moveSpeed * Time.deltaTime;                  
                    if (x < -w) x = screenW;                          
                    float y = baseY + Mathf.Sin(t * floatFreq) * floatAmp; 
                    SetWindowPos(hWnd, System.IntPtr.Zero, (int)x, (int)y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
                }
                yield return null;
            }

            
            if (hWnd != System.IntPtr.Zero)
                SetWindowPos(hWnd, System.IntPtr.Zero, origLeft, origTop, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
            try
            {
                
                if (wasFull) Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                else Screen.fullScreen = false;
            }
            catch (System.Exception ) {  }
            
        }
    }

    
    
    
    
    public class SilentMilkComponent : Item
    {
        public const float SilenceDuration = 60f;   
        public const float SlowDuration = 15f;      
        public const float SlowFactor = 0.6f;       
        const float SilenceRadius = 3f;             
        const float RescanInterval = 0.5f;          
        const float LeaveClearDelay = 1.5f;         

        
        private readonly Dictionary<Cell, float> silenced = new Dictionary<Cell, float>();

        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();
            Plugin.StopMilkRandomEvents();
            player.StartCoroutine(RunEffects(player));
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.SilentMilkItemObject);
        }

        private IEnumerator RunEffects(PlayerManager player)
        {
            
            float timer = 0f, scan = 0f;
            while (timer < SilenceDuration)
            {
                if (player == null || player.gameObject == null) { Cleanup(); yield break; }
                timer += Time.deltaTime;
                scan += Time.deltaTime;
                if (scan >= RescanInterval)
                {
                    scan = 0f;
                    ScanAndSilence(player);
                }
                yield return null;
            }
            Cleanup();

            
            if (player == null || player.gameObject == null) yield break;
            var slow = new TimeScaleModifier(1f, 1f, SlowFactor);
            try { player.AddTimeScale(slow); } catch (System.Exception) { }
            float s = 0f;
            while (s < SlowDuration)
            {
                if (player == null || player.gameObject == null) break;
                s += Time.deltaTime;
                yield return null;
            }
            try { player.RemoveTimeScale(slow); } catch (System.Exception) { }
        }

        
        
        private void ScanAndSilence(PlayerManager player)
        {
            try
            {
                EnvironmentController ec = player.ec;
                if (ec == null) return;
                IntVector2 origin = IntVector2.GetGridPosition(player.transform.position);
                int r = Mathf.CeilToInt(SilenceRadius);
                float now = Time.time;

                
                for (int dx = -r; dx <= r; dx++)
                {
                    for (int dz = -r; dz <= r; dz++)
                    {
                        if (Mathf.Abs(dx) > SilenceRadius || Mathf.Abs(dz) > SilenceRadius) continue;
                        Cell cell = ec.CellFromPosition(new IntVector2(origin.x + dx, origin.z + dz));
                        if (cell == null) continue;

                        if (silenced.ContainsKey(cell))
                        {
                            
                            silenced[cell] = now;
                            continue;
                        }
                        if (cell.Silent)
                        {
                            
                            continue;
                        }
                        cell.SetSilence(true);
                        silenced[cell] = now;
                    }
                }

                
                List<Cell> toClear = null;
                foreach (KeyValuePair<Cell, float> kv in silenced)
                {
                    if (now - kv.Value > LeaveClearDelay)
                    {
                        if (toClear == null) toClear = new List<Cell>();
                        toClear.Add(kv.Key);
                    }
                }
                if (toClear != null)
                {
                    foreach (Cell c in toClear)
                    {
                        try { c.SetSilence(false); } catch (System.Exception) { }
                        silenced.Remove(c);
                    }
                }
            }
            catch (System.Exception) { }
        }

        private void Cleanup()
        {
            foreach (Cell c in silenced.Keys)
            {
                if (c != null)
                {
                    try { c.SetSilence(false); } catch (System.Exception) { }
                }
            }
            silenced.Clear();
        }
    }

    
    
    public class MooMilkComponent : Item
    {
        private const float AngryCowChance = 0.3f; 

        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;

            
            

            
            try { if (player.plm != null) player.plm.stamina = player.plm.StaminaMax * 1.5f; } catch (System.Exception) { }

            
            SpawnCow(player);

            
            Plugin.PlayMilkDrinkSound();
            try { AchievementHelper.UnlockAchievement("milk_moo"); } catch (System.Exception) { }
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.MooMilkItemObject);
        }

        private static void SpawnCow(PlayerManager player)
        {
            try
            {
                EnvironmentController ec = player.ec;
                if (ec == null) return;

                bool angry = UnityEngine.Random.value < AngryCowChance;
                NPC cowPrefab = null;
                if (angry)
                {
                    cowPrefab = Plugin.StampedeCowPrefab;
                    if (cowPrefab == null && Plugin.Instance != null && Plugin.Instance.assetMan != null)
                        cowPrefab = Plugin.Instance.assetMan.Get<NPC>("StampedeCow");
                }
                else
                {
                    if (Plugin.Instance != null && Plugin.Instance.assetMan != null)
                        cowPrefab = Plugin.Instance.assetMan.Get<NPC>("PolishCow");
                }
                if (cowPrefab == null) return;

                Cell cell = ec.CellFromPosition(player.transform.position);
                if (cell == null) return;
                ec.SpawnNPC(cowPrefab, cell.position);
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    
    public class NineNineMilkComponent : Item
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern System.IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetWindowRect(System.IntPtr hWnd, out RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetWindowTextW(System.IntPtr hWnd, string lpString);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowTextW(System.IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct RECT { public int left; public int top; public int right; public int bottom; }

        private const float SequenceSeconds = 50f;

        
        private static Texture2D _wall99;
        public static Texture2D Wall99Texture
        {
            get
            {
                if (_wall99 == null)
                {
                    try { _wall99 = AssetLoader.TextureFromMod(Plugin.Instance, "99_Wall.png"); }
                    catch (System.Exception) { }
                }
                return _wall99;
            }
        }

        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();
            
            if (MilkSettings.Enable99EasterEgg != null && MilkSettings.Enable99EasterEgg.Value)
            {
                try { AchievementHelper.UnlockAchievement("milk_99"); } catch (System.Exception) { }
                
                try { StartCoroutine(NineNineSequence(player)); }
                catch (System.Exception ) {  }
            }
            else
            {
                if (player.plm != null) player.plm.AddStamina(UnityEngine.Random.Range(6, 16), true);
            }
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.NineNineMilkItemObject);
        }

        
        private static Texture2D MakeSnowTexture(int w, int h, int seed)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            var px = new Color[w * h];
            var rng = new System.Random(seed);
            for (int i = 0; i < px.Length; i++)
            {
                byte v = (byte)rng.Next(256);
                px[i] = new Color(v / 255f, v / 255f, v / 255f);
            }
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Point;
            tex.SetPixels(px);
            tex.Apply();
            return tex;
        }

        private System.Collections.IEnumerator NineNineSequence(PlayerManager player)
        {
            
            int rw = Screen.width, rh = Screen.height, rx = 0, ry = 0;
            bool gotWin = false;
            System.IntPtr hWnd = System.IntPtr.Zero;
            try
            {
                hWnd = GetForegroundWindow();
                RECT r; r.left = r.top = r.right = r.bottom = 0;
                if (hWnd != System.IntPtr.Zero && GetWindowRect(hWnd, out r))
                {
                    rx = r.left; ry = r.top; rw = r.right - r.left; rh = r.bottom - r.top;
                    gotWin = true;
                }
            }
            catch (System.Exception ) {  }

            
            Image blackPanel = null;
            Image snowImage = null;
            Texture2D snowTex = MakeSnowTexture(160, 90, UnityEngine.Random.Range(1, 999999));
            AudioSource staticSource = null;
            AudioClip staticClip = null;
            GameObject root = null;
            try
            {
                root = new GameObject("Milk99_Film");
                root.layer = LayerMask.NameToLayer("UI");
                Canvas c = root.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                c.sortingOrder = 32767;
                
                

                
                RectTransform br = new GameObject("Black").AddComponent<RectTransform>();
                br.SetParent(root.transform, false);
                br.anchorMin = Vector2.zero; br.anchorMax = Vector2.one;
                br.offsetMin = Vector2.zero; br.offsetMax = Vector2.zero;
                br.pivot = new Vector2(0.5f, 0.5f);
                br.anchoredPosition = Vector2.zero;
                blackPanel = br.gameObject.AddComponent<Image>();
                blackPanel.color = Color.black;

                
                RectTransform sr = new GameObject("Snow").AddComponent<RectTransform>();
                sr.SetParent(root.transform, false);
                sr.anchorMin = Vector2.zero; sr.anchorMax = Vector2.one;
                sr.offsetMin = Vector2.zero; sr.offsetMax = Vector2.zero;
                sr.pivot = new Vector2(0.5f, 0.5f);
                sr.anchoredPosition = Vector2.zero;
                snowImage = sr.gameObject.AddComponent<Image>();
                snowImage.sprite = Sprite.Create(snowTex, new Rect(0, 0, snowTex.width, snowTex.height), Vector2.one / 2f, 1f);
                snowImage.color = new Color(1f, 1f, 1f, 0.35f);
                snowImage.type = Image.Type.Simple;
                snowImage.preserveAspect = false;
            }
            catch (System.Exception ) {  }

            
            try
            {
                Screen.fullScreen = false;
                Screen.SetResolution(800, 600, false);
            }
            catch (System.Exception ) {  }
            yield return null;

            
            try
            {
                const int len = 44100; 
                var data = new float[len];
                var rng = new System.Random(998877);
                for (int i = 0; i < len; i++) data[i] = (float)rng.NextDouble() * 2f - 1f;
                staticClip = AudioClip.Create("Milk99_Static", len, 1, 44100, false);
                staticClip.SetData(data, 0);
                GameObject srcObj = new GameObject("Milk99_StaticSrc");
                staticSource = srcObj.AddComponent<AudioSource>();
                staticSource.clip = staticClip;
                staticSource.loop = true;
                staticSource.spatialBlend = 0f;
                Plugin.RouteToMixer(staticSource, Plugin.MilkMixerRoute.Effect);
                staticSource.Play();
            }
            catch (System.Exception ) {  }

            
            
            System.Text.StringBuilder titleBuf = new System.Text.StringBuilder(256);
            string origTitle = "";
            try
            {
                if (hWnd != System.IntPtr.Zero)
                {
                    if (GetWindowTextW(hWnd, titleBuf, titleBuf.Capacity) > 0) origTitle = titleBuf.ToString();
                    SetWindowTextW(hWnd, "");
                    
                }
            }
            catch (System.Exception ) {  }

            
            try { var cgmN = Singleton<CoreGameManager>.Instance; if (cgmN != null) cgmN.disablePause = true; } catch (System.Exception) { }

            float elapsed = 0f;
            int frame = 0;
            
            float snowDuration = SequenceSeconds;
            if (MilkSettings.Skip99SnowScreen != null && MilkSettings.Skip99SnowScreen.Value)
            {
                snowDuration = 0.03f;
            }
            while (elapsed < snowDuration)
            {
                float progress = elapsed / SequenceSeconds;
                
                if (staticSource != null)
                    try { staticSource.volume = Mathf.Lerp(0f, 1f, progress); } catch (System.Exception) { }
                
                if (snowImage != null)
                    try { snowImage.rectTransform.anchoredPosition = new Vector2(UnityEngine.Random.Range(-18f, 18f), UnityEngine.Random.Range(-18f, 18f)); } catch (System.Exception) { }
                
                bool winFxOn = MilkSettings.WindowEffects == null || MilkSettings.WindowEffects.Value;
                if (winFxOn && gotWin && hWnd != System.IntPtr.Zero)
                {
                    try
                    {
                        float intensity = Mathf.Lerp(4f, 60f, progress);
                        int dx = Mathf.RoundToInt(UnityEngine.Random.Range(-1f, 1f) * intensity);
                        int dy = Mathf.RoundToInt(UnityEngine.Random.Range(-1f, 1f) * intensity);
                        SetWindowPos(hWnd, System.IntPtr.Zero, rx + dx, ry + dy, rw, rh, 0x0001);
                    }
                    catch (System.Exception) { }
                }
                
                if (++frame % 8 == 0 && snowImage != null)
                {
                    try
                    {
                        Texture2D t = MakeSnowTexture(160, 90, UnityEngine.Random.Range(1, 999999));
                        snowImage.sprite = Sprite.Create(t, new Rect(0, 0, 160, 90), Vector2.one / 2f, 1f);
                    }
                    catch (System.Exception) { }
                }
                yield return null;
                elapsed += Time.deltaTime;
            }

            
            
            
            
            try { Screen.fullScreen = false; Screen.SetResolution(1366, 768, false); }
            catch (System.Exception) { }
            yield return null;   
            try
            {
                if (hWnd == System.IntPtr.Zero) hWnd = GetForegroundWindow();
                RECT nr; nr.left = nr.top = nr.right = nr.bottom = 0;
                if (hWnd != System.IntPtr.Zero && GetWindowRect(hWnd, out nr))
                {
                    int ww = nr.right - nr.left, wh = nr.bottom - nr.top;
                    int cx = Mathf.Max(0, (Screen.currentResolution.width - ww) / 2);
                    int cy = Mathf.Max(0, (Screen.currentResolution.height - wh) / 2);
                    SetWindowPos(hWnd, System.IntPtr.Zero, cx, cy, 0, 0, 0x0001);   
                }
            }
            catch (System.Exception) { }
            
            try { if (hWnd != System.IntPtr.Zero) SetWindowTextW(hWnd, string.IsNullOrEmpty(origTitle) ? "Baldi's Basics Milk!" : origTitle); } catch (System.Exception) { }

            
            try { var cgmN2 = Singleton<CoreGameManager>.Instance; if (cgmN2 != null) cgmN2.disablePause = false; } catch (System.Exception) { }

            
            try
            {
                Light[] lights = UnityEngine.Object.FindObjectsOfType<Light>();
                int n = 0;
                foreach (Light l in lights)
                {
                    if (l == null) continue;
                    try { l.color = Color.gray; n++; } catch (System.Exception) { }
                }
                
            }
            catch (System.Exception ) {  }

            
            try
            {
                var bgm = Singleton<BaseGameManager>.Instance;
                if (bgm != null)
                {
                    bgm.CollectNotebooks(99); 
                    
                }
            }
            catch (System.Exception ) {  }
            try
            {
                var cgm = Singleton<CoreGameManager>.Instance;
                if (cgm != null) cgm.GetHud(0).UpdateNotebookText(0, "99/99", false);
                
            }
            catch (System.Exception ) {  }

            
            Texture2D wall99 = Wall99Texture;
            if (wall99 != null)
            {
                int replaced = 0;
                try
                {
                    Tile[] tiles = UnityEngine.Object.FindObjectsOfType<Tile>();
                    foreach (Tile tile in tiles)
                    {
                        if (tile == null || tile.MeshRenderer == null) continue;
                        Material shared = tile.MeshRenderer.sharedMaterial;
                        if (shared == null) continue;
                        try
                        {
                            Material newMat = new Material(shared);
                            newMat.mainTexture = wall99;
                            tile.MeshRenderer.material = newMat;
                            replaced++;
                        }
                        catch (System.Exception) { }
                    }
                    
                }
                catch (System.Exception ) {  }
            }
            else 

            
            {
                int quizCleared = 0;
                try
                {
                    var quizes = UnityEngine.Object.FindObjectsOfType<QuizMachine>();
                    foreach (var q in quizes)
                    {
                        if (q == null || q.gameObject == null) continue;
                        try { UnityEngine.Object.Destroy(q.gameObject); quizCleared++; } catch (System.Exception) { }
                    }
                }
                catch (System.Exception )
                {
                    
                }
                
            }

            
            
            
            Plugin.nineNineDoorUnlockedByPlayer = true;
            {
                int unlocked = 0;
                try
                {
                    Door[] doors = UnityEngine.Object.FindObjectsOfType<Door>();
                    foreach (Door d in doors)
                    {
                        if (d == null) continue;
                        try { d.Unlock(); d.Open(true, false); unlocked++; }
                        catch (System.Exception) { }
                    }
                }
                catch (System.Exception) { }
                
            }

            
            
            
            try
            {
                var elevMgrs = UnityEngine.Object.FindObjectsOfType<ElevatorManager>();
                foreach (var em in elevMgrs)
                {
                    if (em == null) continue;
                    try
                    {
                        em.SetAllElevators(ElevatorState.OpenForExit);
                        em.SetTotalOutOfOrderElevators(UnityEngine.Object.FindObjectsOfType<Elevator>().Length - 1);
                        
                    }
                    catch (System.Exception) { }
                }
            }
            catch (System.Exception ) {  }

            
            if (staticSource != null) try { staticSource.Stop(); } catch (System.Exception) { }
            if (staticClip != null) try { UnityEngine.Object.Destroy(staticClip); } catch (System.Exception) { }
            if (root != null) try { UnityEngine.Object.Destroy(root); } catch (System.Exception) { }
            if (snowTex != null) try { UnityEngine.Object.Destroy(snowTex); } catch (System.Exception) { }
            
            DestroyAllSteamValves();
            
            
            EruptEpicBlackFog();
            
            
            Plugin.MooArmed = true;
            Plugin.MooEntryTriggered = false;
            Plugin.MooRedWhiteActive = true;       
            Plugin.MooRedWhiteFloor = 0;           
            
            try { Plugin.ensureRedWhiteComponent(); Plugin.StartRedWhiteMusic(); } catch (System.Exception) { }
            
            
            try { var mmi = Singleton<MusicManager>.Instance; if (mmi != null) mmi.StopMidi(); } catch (System.Exception) { }
            Plugin.MooRedWhiteFloorReady = false;
            Plugin.MooRedWhiteFailed = false;
            Plugin.MooRedWhiteCountdown = Plugin.RedWhiteTotalSeconds;   
            Plugin.RedWhiteMode.ResetRun();   
            
            try { var cgmP = Singleton<CoreGameManager>.Instance; if (cgmP != null) cgmP.disablePause = true; } catch (System.Exception) { }
            try
            {
                var cgm2 = Singleton<CoreGameManager>.Instance;
                if (cgm2 != null) { try { cgm2.SetLives(2, true); } catch (System.Exception) { } }
            }
            catch (System.Exception) { }
            
            yield break;
        }

        
        private static void DestroyAllSteamValves()
        {
            int n = 0;
            try
            {
                var vals = UnityEngine.Object.FindObjectsOfType<Structure_SteamValves>();
                foreach (var v in vals)
                {
                    if (v == null || v.gameObject == null) continue;
                    try { UnityEngine.Object.Destroy(v.gameObject); n++; } catch (System.Exception) { }
                }
            }
            catch (System.Exception ) {  }
            
        }

        
        
        
        
        
        private static Fog activeFog = null;   

        private static void EruptEpicBlackFog()
        {
            try
            {
                EnvironmentController ec = UnityEngine.Object.FindObjectOfType<EnvironmentController>();
                if (ec == null)
                {
                    
                    return;
                }

                
                
                RenderSettings.ambientLight = new Color(0.12f, 0.12f, 0.12f);
                RenderSettings.fogColor = Color.black;

                
                
                if (activeFog == null)
                {
                    activeFog = new Fog();
                }
                activeFog.color = Color.black;
                activeFog.startDist = 4f;    
                activeFog.maxDist = 60f;     
                activeFog.strength = 0.35f;  
                activeFog.priority = 999;    
                ec.AddFog(activeFog);
                ec.UpdateFog();

                
                ec.StartCoroutine(EpicBlackFogDriver(ec));
                
            }
            catch (System.Exception ) {  }
        }

        
        private static System.Collections.IEnumerator EpicBlackFogDriver(EnvironmentController ec)
        {
            const float duration = 3f;
            const float maxStrength = 1f;     
            float progress = 0f;
            float nextFlash = 0.6f;

            while (progress < duration)
            {
                progress += Time.deltaTime;
                float thick = Mathf.Clamp01(progress / duration);
                thick *= thick;               
                float strength = Mathf.Lerp(0.35f, maxStrength, thick);

                
                if (progress >= nextFlash)
                {
                    nextFlash += UnityEngine.Random.Range(0.35f, 0.7f);
                    float strike = 0f;
                    while (strike < 0.1f)
                    {
                        strike += Time.deltaTime;
                        if (activeFog != null)
                        {
                            activeFog.strength = strength * 0.2f;
                            activeFog.color = new Color(0.88f, 0.88f, 0.97f); 
                        }
                        PushFog(ec);
                        yield return null;
                    }
                    if (activeFog != null)
                    {
                        activeFog.strength = strength;
                        activeFog.color = Color.black;
                    }
                }
                else if (activeFog != null)
                {
                    activeFog.strength = strength;
                    activeFog.color = Color.black;
                }

                PushFog(ec);
                yield return null;
            }

            if (activeFog != null)
            {
                activeFog.strength = maxStrength;
                activeFog.color = Color.black;   
            }
            PushFog(ec);
            
        }

        
        private static void PushFog(EnvironmentController ec)
        {
            try
            {
                if (ec != null) ec.UpdateFog();
            }
            catch (System.Exception) { }
        }
    }

    
    
    public class TimeMilkComponent : Item
    {
        private const float TickInterval = 5f;      
        private const int TickCount = 6;            
        private const float HalfPoint = 50f;        
        private const float SpeedMult = 1.1f;       
        private const float FinalLag = 5f;          

        public override bool Use(PlayerManager player)
        {
            if (player == null || player.ec == null) return false;
            Plugin.PlayMilkDrinkSound();

            
            RestorePercent(player, 0.5f);

            
            MovementModifier speedMod = null;
            if (player.plm != null && player.plm.am != null)
            {
                speedMod = new MovementModifier(Vector3.zero, 1f, 0);
                speedMod.movementMultiplier = SpeedMult;
                player.plm.am.moveMods.Add(speedMod);
            }

            player.StartCoroutine(TimeMilkLoop(player, speedMod));
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.TimeMilkItemObject);
        }

        
        private static System.Collections.IEnumerator TimeMilkLoop(PlayerManager player, MovementModifier speedMod)
        {
            for (int i = 0; i < TickCount; i++)
            {
                yield return new WaitForSeconds(TickInterval);
                if (player == null) yield break;
                RestoreHalf(player); 
            }
            
            yield return new WaitForSeconds(FinalLag);
            try
            {
                if (speedMod != null && player != null && player.plm != null && player.plm.am != null)
                    player.plm.am.moveMods.Remove(speedMod);
            }
            catch (System.Exception) { }
        }

        
        private static void RestorePercent(PlayerManager player, float percent)
        {
            try
            {
                if (player.plm == null) return;
                float maxStamina = 100f; 
                try
                {
                    var val = player.plm.GetType().GetField("staminaMax",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.GetValue(player.plm);
                    if (val != null) maxStamina = (float)val;
                }
                catch (System.Exception) { }
                float restoreAmount = maxStamina * percent;
                float newStamina = player.plm.stamina + restoreAmount;
                if (newStamina > 200f) restoreAmount = 200f - player.plm.stamina;
                if (restoreAmount < 0f) restoreAmount = 0f;
                player.plm.AddStamina((int)System.Math.Ceiling(restoreAmount), false);
            }
            catch (System.Exception) { }
        }

        
        private static void RestoreHalf(PlayerManager player)
        {
            try
            {
                if (player.plm != null) player.plm.AddStamina((int)HalfPoint, false);
            }
            catch (System.Exception) { }
        }
    }

    public class BusPassMilkComponent : MilkComponent
    {
        public override bool Use(PlayerManager player)
        {
            
            bool result = base.Use(player);
            
            SpawnBusPass(player);
            return result;
        }

        private static void SpawnBusPass(PlayerManager player)
        {
            try
            {
                if (player == null) return;

                ItemMetaStorage metaStorage = ItemMetaStorage.Instance;
                if (metaStorage == null) {  return; }

                
                ItemObject busObj = metaStorage.FindByEnum(Items.BusPass)?.value;
                if (busObj == null || busObj.item == null)
                {
                    
                    return;
                }

                
                
                Pickup pre = null;
                try
                {
                    LevelBuilder lb = UnityEngine.Object.FindObjectOfType<LevelBuilder>();
                    if (lb != null && lb.pickupPre != null) pre = lb.pickupPre;
                }
                catch (System.Exception ) {  }
                if (pre == null) pre = Resources.FindObjectsOfTypeAll<Pickup>().FirstOrDefault(p => p != null && p.item != null);
                if (pre == null)
                {
                    
                    return;
                }

                Vector3 pos = player.transform.position;
                Pickup pickup = UnityEngine.Object.Instantiate(pre, pos, Quaternion.identity);
                pickup.item = busObj;   
                pickup.free = true;
                pickup.transform.position = pos;
                
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    
    public class QuarterMilkComponent : Item
    {
        private const float SlowDuration = 30f;   
        private const float SlowMult = 0.5f;      

        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();
            Plugin.StopMilkRandomEvents(); 
            player.StartCoroutine(QuarterMilkCoroutine(player));
            
            try { AchievementHelper.UnlockAchievement("milk_quarter"); } catch (System.Exception) { }
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.QuarterMilkItemObject);
        }

        private static System.Collections.IEnumerator QuarterMilkCoroutine(PlayerManager player)
        {
            
            MovementModifier slow = null;
            try
            {
                if (player.plm != null && player.plm.am != null)
                {
                    slow = new MovementModifier(Vector3.zero, 1f, 0);
                    slow.movementMultiplier = SlowMult;
                    player.plm.am.moveMods.Add(slow);
                }
            }
            catch (System.Exception )
            {
                
            }

            
            SpawnQuarters(player);

            
            HudGauge gauge = ActivateQuarterGauge();

            
            float elapsed = 0f;
            while (elapsed < SlowDuration && player != null)
            {
                elapsed += Time.deltaTime;
                if (gauge != null)
                {
                    try { gauge.SetValue(Mathf.Max(0f, SlowDuration - elapsed), SlowDuration); }
                    catch (System.Exception )
                    {
                        
                    }
                }
                yield return null;
            }
            if (gauge != null)
            {
                try { gauge.Deactivate(); } catch (System.Exception ) { }
            }

            try
            {
                if (slow != null && player != null && player.plm != null && player.plm.am != null)
                    player.plm.am.moveMods.Remove(slow);
            }
            catch (System.Exception )
            {
                
            }
            
        }

        
        
        private static HudGauge ActivateQuarterGauge()
        {
            try
            {
                if (Singleton<CoreGameManager>.Instance == null) return null;
                HudManager hud = Singleton<CoreGameManager>.Instance.GetHud(0);
                if (hud == null || hud.gaugeManager == null) return null;

                Sprite ico = null;
                var io = Plugin.QuarterMilkItemObject;
                if (io != null)
                {
                    System.Type t = io.GetType();
                    
                    object candidate = null;
                    try
                    {
                        var p = t.GetProperty("sprite", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (p != null) candidate = p.GetValue(io, null);
                    }
                    catch (System.Exception) { }
                    if (!(candidate is Sprite))
                    {
                        try
                        {
                            var p = t.GetProperty("icon", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            if (p != null) candidate = p.GetValue(io, null);
                        }
                        catch (System.Exception) { }
                    }
                    if (!(candidate is Sprite))
                    {
                        var f = t.GetField("sprite", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                            ?? t.GetField("icon", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (f != null) { try { candidate = f.GetValue(io); } catch (System.Exception) { } }
                    }
                    ico = candidate as Sprite;
                }
                if (ico == null) return null;

                return hud.gaugeManager.ActivateNewGauge(ico, SlowDuration);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private static void SpawnQuarters(PlayerManager player)
        {
            try
            {
                if (player == null) return;
                ItemMetaStorage metaStorage = ItemMetaStorage.Instance;
                if (metaStorage == null) {  return; }

                
                ItemObject quarterObj = metaStorage.FindByEnum((Items)6)?.value;
                if (quarterObj == null || quarterObj.item == null)
                {
                    
                    return;
                }

                
                
                
                Pickup pre = null;
                try
                {
                    LevelBuilder lb = UnityEngine.Object.FindObjectOfType<LevelBuilder>();
                    if (lb != null && lb.pickupPre != null) pre = lb.pickupPre;
                }
                catch (System.Exception ) {  }
                
                if (pre == null) pre = Resources.FindObjectsOfTypeAll<Pickup>().FirstOrDefault(p => p != null && p.item != null);
                if (pre == null)
                {
                    
                    return;
                }

                Vector3 center = player.transform.position;
                
                Vector3[] offsets = new Vector3[]
                {
                    new Vector3(2f, 0f, 2f),
                    new Vector3(-2f, 0f, 2f),
                    new Vector3(2f, 0f, -2f),
                    new Vector3(-2f, 0f, -2f),
                };
                foreach (Vector3 off in offsets)
                {
                    Pickup pickup = UnityEngine.Object.Instantiate(pre, center + off, Quaternion.identity);
                    pickup.item = quarterObj;   
                    pickup.free = true;         
                    pickup.transform.position = center + off;
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    
    
    public class RottenMilkComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;

            
            Vector3 dropTarget = player.transform.position + player.transform.forward * 3f;
            GameObject bucket = SpawnRottenBucket(player, dropTarget);
            
            player.StartCoroutine(RottenMilkZoneCoroutine(player, bucket, dropTarget));
            
            
            try { AchievementHelper.UnlockAchievement("milk_rotten"); } catch (System.Exception) { }
            return true; 
        }

        
        private static GameObject SpawnRottenBucket(PlayerManager player, Vector3 target)
        {
            GameObject bucket = new GameObject("RottenMilkBucket");
            bucket.transform.position = player.transform.position + Vector3.up * 2f;
            bucket.transform.rotation = UnityEngine.Random.rotation;

            Sprite spr = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, "nooRottenMilk_Large.png");
            if (spr != null)
            {
                SpriteRenderer ren = bucket.AddComponent<SpriteRenderer>();
                ren.sprite = spr;
                ren.sortingOrder = 5;
            }

            
            player.StartCoroutine(ThrowBucketCoroutine(bucket, target));
            return bucket;
        }

        
        private static System.Collections.IEnumerator ThrowBucketCoroutine(GameObject bucket, Vector3 target)
        {
            if (bucket == null) yield break;
            Vector3 start = bucket.transform.position;
            float duration = 0.6f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(start, target, t);
                
                pos.y += Mathf.Sin(t * Mathf.PI) * 1.5f;
                bucket.transform.position = pos;
                
                bucket.transform.Rotate(0f, 0f, 120f * Time.deltaTime);
                yield return null;
            }

            
            bucket.transform.position = new Vector3(target.x, 0.1f, target.z);
            bucket.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }

        
        
        
        public static System.Collections.IEnumerator RottenMilkZoneCoroutine(PlayerManager player, GameObject bucket, Vector3 target)
        {
            if (player == null) yield break;
            EnvironmentController ec = player.ec;
            float duration = 125f;
            int zoneRadiusCells = 3;   

            
            Vector3 center = target;

            
            var blocked = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<Cell, Direction>>();

            try
            {
                if (ec != null)
                {
                    IntVector2 centerCellPos = IntVector2.GetGridPosition(center);
                    
                    var zoneCells = new System.Collections.Generic.HashSet<Cell>();
                    for (int ox = -zoneRadiusCells; ox <= zoneRadiusCells; ox++)
                    {
                        for (int oz = -zoneRadiusCells; oz <= zoneRadiusCells; oz++)
                        {
                            int cx = centerCellPos.x + ox;
                            int cz = centerCellPos.z + oz;
                            
                            if (ox * ox + oz * oz > zoneRadiusCells * zoneRadiusCells) continue;
                            Cell cell = ec.CellFromPosition(cx, cz);
                            if (cell != null) zoneCells.Add(cell);
                        }
                    }

                    
                    
                    foreach (Cell cell in zoneCells)
                    {
                        if (cell.Null || cell.room == null) continue;
                        for (int d = 0; d < 4; d++)
                        {
                            Direction dir = (Direction)d;
                            try
                            {
                                cell.Block(dir, true);
                                blocked.Add(new System.Collections.Generic.KeyValuePair<Cell, Direction>(cell, dir));
                            }
                            catch (System.Exception) { }
                        }
                    }
                    
                }
            }
            catch (System.Exception )
            {
                
            }

            
            float elapsed = 0f;
            float pushRadius = zoneRadiusCells * 10f;
            var pushMods = new System.Collections.Generic.Dictionary<NPC, MovementModifier>();

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (ec != null && ec.Npcs != null)
                {
                    foreach (var npc in ec.Npcs)
                    {
                        if (npc == null || npc.Entity == null) continue;
                        Vector3 npcPos = npc.transform.position;
                        float dx = npcPos.x - center.x;
                        float dz = npcPos.z - center.z;
                        float dist = Mathf.Sqrt(dx * dx + dz * dz);
                        if (dist < pushRadius && dist > 0.1f)
                        {
                            Vector3 away = new Vector3(dx, 0f, dz).normalized;
                            if (!pushMods.ContainsKey(npc))
                            {
                                MovementModifier mod = new MovementModifier(away * 80f, 0f);
                                npc.Entity.ExternalActivity.moveMods.Add(mod);
                                pushMods[npc] = mod;
                            }
                            else
                            {
                                pushMods[npc].movementAddend = away * 80f;
                            }
                        }
                    }
                    
                    var toRemove = new System.Collections.Generic.List<NPC>();
                    foreach (var kv in pushMods)
                    {
                        if (kv.Key == null || kv.Key.Entity == null) { toRemove.Add(kv.Key); continue; }
                        Vector3 npcPos = kv.Key.transform.position;
                        float dist = Vector3.Distance(new Vector3(npcPos.x, 0f, npcPos.z),
                                                       new Vector3(center.x, 0f, center.z));
                        if (dist >= pushRadius)
                        {
                            kv.Key.Entity.ExternalActivity.moveMods.Remove(kv.Value);
                            toRemove.Add(kv.Key);
                        }
                    }
                    foreach (var npc in toRemove) pushMods.Remove(npc);
                }
                yield return null;
            }

            
            foreach (var kv in pushMods)
            {
                try { kv.Key.Entity.ExternalActivity.moveMods.Remove(kv.Value); } catch { }
            }

            
            foreach (var kv in blocked)
            {
                try
                {
                    if (kv.Key != null) kv.Key.Block(kv.Value, false);
                }
                catch (System.Exception) { }
            }
            

            if (bucket != null) UnityEngine.Object.Destroy(bucket);
        }
    }

    
    
    
    
    
    
    
    
    
    public class FakeMilkComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();
            Plugin.StopMilkRandomEvents(); 
            FakeMilkNauseaManager.ApplyNausea(player);
            try { AchievementHelper.UnlockAchievement("milk_fake"); } catch (System.Exception) { }
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.FakeMilkItemObject);
        }
    }

    
    public static class FakeMilkNauseaManager
    {
        
        private const float NauseaDuration = 45f;
        
        private const float SlowMult = 0.55f;
        
        private const float VomitIntervalMin = 8f;
        private const float VomitIntervalMax = 18f;
        
        private const float VomitStaminaCost = 15f;
        
        private const int VomitZoneRadius = 2;
        private const float VomitZoneDuration = 45f;
        
        private const float StaminaDrainFraction = 0.6f;

        private static readonly System.Collections.Generic.List<NauseaInstance> _active =
            new System.Collections.Generic.List<NauseaInstance>();

        private sealed class NauseaInstance
        {
            public PlayerManager player;
            public MovementModifier slow;
            public Coroutine coroutine;
            public float remaining;
            public float nextVomit;
            public float maxPenalty; 
            public bool stopped = false;
        }

        
        public static void ApplyNausea(PlayerManager player)
        {
            if (player == null) return;

            
            try
            {
                if (player.plm != null)
                {
                    float maxStamina = 100f;
                    var f = player.plm.GetType().GetField("staminaMax",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (f != null) { try { maxStamina = (float)f.GetValue(player.plm); } catch (System.Exception) { } }
                    player.plm.stamina = Mathf.Max(0f, player.plm.stamina - maxStamina * StaminaDrainFraction);
                }
            }
            catch (System.Exception) { }

            var inst = new NauseaInstance { player = player };
            _active.Add(inst);
            
            
            try
            {
                if (player.plm != null)
                {
                    var f = player.plm.GetType().GetField("staminaMax",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (f != null)
                    {
                        float curMax = (float)f.GetValue(player.plm);
                        inst.maxPenalty = curMax * 0.5f;
                        float newMax = Mathf.Max(5f, curMax - inst.maxPenalty);
                        f.SetValue(player.plm, newMax);
                        player.plm.stamina = Mathf.Min(player.plm.stamina, newMax);
                    }
                }
            }
            catch (System.Exception) { }
            
            try
            {
                if (player.plm != null && player.plm.am != null)
                {
                    inst.slow = new MovementModifier(Vector3.zero, 1f, 0);
                    inst.slow.movementMultiplier = SlowMult;
                    player.plm.am.moveMods.Add(inst.slow);
                }
            }
            catch (System.Exception) { }
            
            inst.coroutine = player.StartCoroutine(NauseaCoroutine(inst));
        }

        
        public static void Cure(PlayerManager player)
        {
            if (player == null) return;
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (_active[i].player == player) StopInstance(_active[i], true);
            }
        }

        private static void StopInstance(NauseaInstance inst, bool removeFromList)
        {
            if (inst == null || inst.stopped) return;
            inst.stopped = true;
            try
            {
                if (inst.slow != null && inst.player != null && inst.player.plm != null && inst.player.plm.am != null)
                    inst.player.plm.am.moveMods.Remove(inst.slow);
            }
            catch (System.Exception) { }
            try
            {
                
                if (inst.player != null && inst.player.plm != null)
                {
                    var f = inst.player.plm.GetType().GetField("staminaMax",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (f != null)
                    {
                        float curMax = (float)f.GetValue(inst.player.plm);
                        float restored = Mathf.Max(curMax, curMax + inst.maxPenalty);
                        f.SetValue(inst.player.plm, restored);
                        inst.player.plm.stamina = Mathf.Min(inst.player.plm.stamina, restored);
                    }
                }
            }
            catch (System.Exception) { }
            try { if (inst.coroutine != null && inst.player != null) inst.player.StopCoroutine(inst.coroutine); } catch (System.Exception) { }
            if (removeFromList) _active.Remove(inst);
        }

        private static System.Collections.IEnumerator NauseaCoroutine(NauseaInstance inst)
        {
            inst.remaining = NauseaDuration;
            inst.nextVomit = UnityEngine.Random.Range(VomitIntervalMin, VomitIntervalMax);
            while (inst != null && inst.player != null && !inst.stopped && inst.remaining > 0f)
            {
                float dt = Time.deltaTime;
                inst.remaining -= dt;
                inst.nextVomit -= dt;
                
                if (inst.nextVomit <= 0f)
                {
                    try
                    {
                        if (inst.player.plm != null)
                            inst.player.plm.stamina = Mathf.Max(0f, inst.player.plm.stamina - VomitStaminaCost);
                    }
                    catch (System.Exception) { }
                    VomitWeakRottenZone(inst.player);
                    inst.nextVomit = UnityEngine.Random.Range(VomitIntervalMin, VomitIntervalMax);
                }
                yield return null;
            }
            
            if (!inst.stopped)
            {
                inst.stopped = true;
                try
                {
                    if (inst.slow != null && inst.player != null && inst.player.plm != null && inst.player.plm.am != null)
                        inst.player.plm.am.moveMods.Remove(inst.slow);
                }
                catch (System.Exception) { }
                
                try
                {
                    if (inst.player != null && inst.player.plm != null)
                    {
                        var f = inst.player.plm.GetType().GetField("staminaMax",
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (f != null)
                        {
                            float curMax = (float)f.GetValue(inst.player.plm);
                            float restored = Mathf.Max(curMax, curMax + inst.maxPenalty);
                            f.SetValue(inst.player.plm, restored);
                            inst.player.plm.stamina = Mathf.Min(inst.player.plm.stamina, restored);
                        }
                    }
                }
                catch (System.Exception) { }
                _active.Remove(inst);
            }
        }

        
        private static void VomitWeakRottenZone(PlayerManager player)
        {
            try
            {
                if (player == null || player.ec == null) return;
                Vector3 dropTarget = player.transform.position + player.transform.forward * 2f;
                
                GameObject bucket = new GameObject("VomitRottenBucket");
                bucket.transform.position = player.transform.position + Vector3.up * 2f;
                bucket.transform.rotation = UnityEngine.Random.rotation;
                Sprite spr = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, "nooRottenMilk_Large.png");
                if (spr != null)
                {
                    SpriteRenderer ren = bucket.AddComponent<SpriteRenderer>();
                    ren.sprite = spr;
                    ren.sortingOrder = 5;
                }
                
                player.StartCoroutine(ThrowVomitBucketCoroutine(bucket, dropTarget));
                
                player.StartCoroutine(WeakRottenZoneCoroutine(player, bucket, dropTarget, VomitZoneRadius, VomitZoneDuration));
            }
            catch (System.Exception) { }
        }

        
        private static System.Collections.IEnumerator ThrowVomitBucketCoroutine(GameObject bucket, Vector3 target)
        {
            if (bucket == null) yield break;
            Vector3 start = bucket.transform.position;
            float duration = 0.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(start, target, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * 1.2f;
                bucket.transform.position = pos;
                bucket.transform.Rotate(0f, 0f, 150f * Time.deltaTime);
                yield return null;
            }
            bucket.transform.position = new Vector3(target.x, 0.1f, target.z);
            bucket.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }

        
        
        private static System.Collections.IEnumerator WeakRottenZoneCoroutine(PlayerManager player, GameObject bucket, Vector3 target, int zoneRadiusCells, float duration)
        {
            if (player == null) yield break;
            EnvironmentController ec = player.ec;
            Vector3 center = target;
            var blocked = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<Cell, Direction>>();

            try
            {
                if (ec != null)
                {
                    IntVector2 centerCellPos = IntVector2.GetGridPosition(center);
                    var zoneCells = new System.Collections.Generic.HashSet<Cell>();
                    for (int ox = -zoneRadiusCells; ox <= zoneRadiusCells; ox++)
                    {
                        for (int oz = -zoneRadiusCells; oz <= zoneRadiusCells; oz++)
                        {
                            if (ox * ox + oz * oz > zoneRadiusCells * zoneRadiusCells) continue;
                            Cell cell = ec.CellFromPosition(centerCellPos.x + ox, centerCellPos.z + oz);
                            if (cell != null) zoneCells.Add(cell);
                        }
                    }
                    foreach (Cell cell in zoneCells)
                    {
                        if (cell.Null || cell.room == null) continue;
                        for (int d = 0; d < 4; d++)
                        {
                            Direction dir = (Direction)d;
                            try
                            {
                                cell.Block(dir, true);
                                blocked.Add(new System.Collections.Generic.KeyValuePair<Cell, Direction>(cell, dir));
                            }
                            catch (System.Exception) { }
                        }
                    }
                }
            }
            catch (System.Exception) { }

            float elapsed = 0f;
            float pushRadius = zoneRadiusCells * 10f;
            var pushMods = new System.Collections.Generic.Dictionary<NPC, MovementModifier>();
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (ec != null && ec.Npcs != null)
                {
                    foreach (var npc in ec.Npcs)
                    {
                        if (npc == null || npc.Entity == null) continue;
                        Vector3 npcPos = npc.transform.position;
                        float dx = npcPos.x - center.x;
                        float dz = npcPos.z - center.z;
                        float dist = Mathf.Sqrt(dx * dx + dz * dz);
                        if (dist < pushRadius && dist > 0.1f)
                        {
                            Vector3 away = new Vector3(dx, 0f, dz).normalized;
                            if (!pushMods.ContainsKey(npc))
                            {
                                MovementModifier mod = new MovementModifier(away * 80f, 0f);
                                npc.Entity.ExternalActivity.moveMods.Add(mod);
                                pushMods[npc] = mod;
                            }
                            else
                            {
                                pushMods[npc].movementAddend = away * 80f;
                            }
                        }
                    }
                    var toRemove = new System.Collections.Generic.List<NPC>();
                    foreach (var kv in pushMods)
                    {
                        if (kv.Key == null || kv.Key.Entity == null) { toRemove.Add(kv.Key); continue; }
                        Vector3 npcPos = kv.Key.transform.position;
                        float dist = Vector3.Distance(new Vector3(npcPos.x, 0f, npcPos.z), new Vector3(center.x, 0f, center.z));
                        if (dist >= pushRadius)
                        {
                            kv.Key.Entity.ExternalActivity.moveMods.Remove(kv.Value);
                            toRemove.Add(kv.Key);
                        }
                    }
                    foreach (var npc in toRemove) pushMods.Remove(npc);
                }
                yield return null;
            }

            foreach (var kv in pushMods)
            {
                try { kv.Key.Entity.ExternalActivity.moveMods.Remove(kv.Value); } catch (System.Exception) { }
            }
            foreach (var kv in blocked)
            {
                try { if (kv.Key != null) kv.Key.Block(kv.Value, false); } catch (System.Exception) { }
            }
            if (bucket != null) UnityEngine.Object.Destroy(bucket);
        }
    }

    
    
    
    
    
    [HarmonyPatch(typeof(HudManager), "SetItemSelect")]
    public class PatchFakeMilkNameFlicker
    {
        private const string FakeMilkKey = "ITM_FakeMilk";
        private static HudManager _flickerHud = null;
        private static Coroutine _flickerRoutine = null;
        private static FieldInfo _itemTitleField = null;

        static void Postfix(HudManager __instance, string key)
        {
            try
            {
                if (key == FakeMilkKey) StartFlicker(__instance);
                else StopFlicker();
            }
            catch (System.Exception) { }
        }

        private static void StartFlicker(HudManager hud)
        {
            if (hud == null) return;
            if (_flickerRoutine != null && _flickerHud == hud) return; 
            StopFlicker();
            _flickerHud = hud;
            _flickerRoutine = hud.StartCoroutine(FlickerRoutine(hud));
        }

        private static void StopFlicker()
        {
            try
            {
                if (_flickerRoutine != null && _flickerHud != null) _flickerHud.StopCoroutine(_flickerRoutine);
            }
            catch (System.Exception) { }
            _flickerRoutine = null;
            _flickerHud = null;
        }

        private static TMP_Text GetTitle(HudManager hud)
        {
            if (hud == null) return null;
            if (_itemTitleField == null)
                _itemTitleField = typeof(HudManager).GetField("itemTitle", BindingFlags.NonPublic | BindingFlags.Instance);
            return _itemTitleField?.GetValue(hud) as TMP_Text;
        }

        private static string Loc(string key)
        {
            try { return Singleton<LocalizationManager>.Instance.GetLocalizedText(key); }
            catch (System.Exception) { return null; }
        }

        private static IEnumerator FlickerRoutine(HudManager hud)
        {
            string real = Loc(FakeMilkKey);
            if (string.IsNullOrEmpty(real)) real = "Milk";
            string reveal = Loc("ITM_FakeMilk_Reveal");
            string[] subtle = new string[] { real + "?", real + "??", real + "...", real + "…?" };
            while (true)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(4f, 9f));
                if (hud == null || _flickerRoutine == null) yield break;
                TMP_Text title = GetTitle(hud);
                if (title == null) yield break;
                
                if (title.text != real) yield break;
                string shown = (reveal != null && UnityEngine.Random.value < 0.2f)
                    ? reveal
                    : subtle[UnityEngine.Random.Range(0, subtle.Length)];
                title.text = shown;
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.4f, 1.2f));
                title.text = real;
            }
        }
    }

    
    
    
    public struct RandomMilkOutcome
    {
        public ItemObject Item; 
        public bool IsAir;      
    }

    public class RandomMilkComponent : Item
    {
        
        
        protected virtual float AirChance => 0f;
        
        protected virtual string LogTag => "Random Milk";

        public override bool Use(PlayerManager player)
        {
            if (player == null || player.itm == null) return false;

            int slot = player.itm.selectedItem;
            RandomMilkOutcome outc = RollRandomMilkOutcome(AirChance);
            if (outc.IsAir)
            {
                
                
                return true;
            }
            if (outc.Item != null)
            {
                player.itm.SetItem(outc.Item, slot);
                
            }
            else
            {
                
            }
            
            return false;
        }

        
        public static bool IsAnyRandomMilk(ItemObject item)
        {
            if (item == null) return false;
            if (Plugin.RandomMilkItemObject != null && item == Plugin.RandomMilkItemObject) return true;
            if (Plugin.RandomMilkNoItemItemObject != null && item == Plugin.RandomMilkNoItemItemObject) return true;
            if (Plugin.RandomMilk75ItemObject != null && item == Plugin.RandomMilk75ItemObject) return true;
            return false;
        }

        
        public static float AirChanceFor(ItemObject item)
        {
            if (item != null && Plugin.RandomMilkNoItemItemObject != null && item == Plugin.RandomMilkNoItemItemObject)
                return Plugin.RandomMilkNoItemAirChance;
            if (item != null && Plugin.RandomMilk75ItemObject != null && item == Plugin.RandomMilk75ItemObject)
                return Plugin.RandomMilk75AirChance;
            return 0f;
        }

        
        public static string NameFor(ItemObject item)
        {
            if (item != null && Plugin.RandomMilkNoItemItemObject != null && item == Plugin.RandomMilkNoItemItemObject)
                return "Random Milk(Chance of no item)";
            if (item != null && Plugin.RandomMilk75ItemObject != null && item == Plugin.RandomMilk75ItemObject)
                return "Random Milk(75% no item)";
            return "Random Milk";
        }

        
        
        public static RandomMilkOutcome RollRandomMilkOutcome(float airChance)
        {
            if (airChance > 0f && UnityEngine.Random.value < airChance)
                return new RandomMilkOutcome { Item = null, IsAir = true };

            
            
            
            
            var items = new System.Collections.Generic.List<ItemObject>();
            var weights = new System.Collections.Generic.List<int>();
            
            bool baldishhBoost = Plugin.StickersReady
                && Singleton<StickerManager>.Instance != null
                && Singleton<StickerManager>.Instance.StickerValue(Plugin.BaldishhSticker) > 0;
            void AddWeighted(ItemObject it, int w, bool good = false)
            {
                if (it == null) return;
                if (baldishhBoost && good) w *= 2;
                items.Add(it); weights.Add(w);
            }
            AddWeighted(Plugin.MilkItemObject, 100, good: false);       
            AddWeighted(Plugin.MiItemObject, 80);              
            AddWeighted(Plugin.LkItemObject, 80);              
            AddWeighted(Plugin.MilkSodaItemObject, 60, good: true);    
            AddWeighted(Plugin.CompressedMilkItemObject, 55, good: true);
            AddWeighted(Plugin.AppleMilkItemObject, 30, good: true);    
            AddWeighted(Plugin.ChocolateMilkItemObject, 45, good: true);
            AddWeighted(Plugin.MilkYtpsItemObject, 35, good: true);     
            AddWeighted(Plugin.RottenMilkItemObject, 30);     
            AddWeighted(Plugin.ReverseMilkItemObject, 45);   
            AddWeighted(Plugin.WindowMilkItemObject, 35);     
            AddWeighted(Plugin.QuarterMilkItemObject, 35);    
            AddWeighted(Plugin.MooMilkItemObject, 30);    
            AddWeighted(Plugin.IceMilkItemObject, 30);    
            AddWeighted(Plugin.LostBilkItemObject, 30);       
            AddWeighted(Plugin.DietMilkSodaItemObject, 35, good: true);  
            AddWeighted(Plugin.SilentMilkItemObject, 25);     

            
            if (Plugin.IsF2MilkFactoryFloor())
                AddWeighted(Plugin.BusPassMilkItemObject, 50); 

            

            if (items.Count == 0) return new RandomMilkOutcome { Item = null, IsAir = false };

            int total = 0;
            for (int i = 0; i < weights.Count; i++) total += weights[i];
            int roll = UnityEngine.Random.Range(0, total); 
            for (int i = 0; i < items.Count; i++)
            {
                roll -= weights[i];
                if (roll < 0) return new RandomMilkOutcome { Item = items[i], IsAir = false };
            }
            
            return new RandomMilkOutcome { Item = items[items.Count - 1], IsAir = false };
        }
    }

    
    public class RandomMilkNoItemComponent : RandomMilkComponent
    {
        protected override float AirChance => Plugin.RandomMilkNoItemAirChance;
        protected override string LogTag => "Random Milk(Chance of no item)";
    }

    
    public class RandomMilk75Component : RandomMilkComponent
    {
        protected override float AirChance => Plugin.RandomMilk75AirChance;
        protected override string LogTag => "Random Milk(75% no item)";
    }

    
    public class MilkYtpsComponent : Item
    {
        
        
        
        
        public static void ApplyEffect(PlayerManager player)
        {
            if (player == null) return;

            
            int ytpGain = UnityEngine.Random.Range(75, 126); 
            try
            {
                var cgm = Singleton<CoreGameManager>.Instance;
                if (cgm != null)
                {
                    
                    int pNum = 0;
                    try { var val = player.GetType().GetField("playerNumber", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.GetValue(player); if (val != null) pNum = (int)val; } catch { }
                    cgm.AddPoints(ytpGain, pNum, playAnimation: true);
                }
            }
            catch (System.Exception )
            {
                
            }
            
            try { AchievementHelper.UnlockAchievement("milk_ytps"); } catch (System.Exception) { }

            
            if (player.plm != null)
            {
                float maxStamina = 100f; 
                try { var val = player.plm.GetType().GetField("staminaMax", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.GetValue(player.plm); if (val != null) maxStamina = (float)val; } catch { }

                float percent = UnityEngine.Random.Range(0.5f, 0.76f); 
                float restoreAmount = maxStamina * percent;
                
                float newStamina = player.plm.stamina + restoreAmount;
                if (newStamina > 200f) restoreAmount = 200f - player.plm.stamina;
                if (restoreAmount < 0f) restoreAmount = 0f;

                player.plm.AddStamina((int)System.Math.Ceiling(restoreAmount), false); 
                
            }
        }

        public override bool Use(PlayerManager player)
        {
            Plugin.PlayMilkDrinkSound();
            ApplyEffect(player);
            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.MilkYtpsItemObject);
        }
    }

    
    

    
    
    
    
    [HarmonyPatch(typeof(Pickup), "Start")]
    public class PatchRandomMilkPickupStart
    {
        static bool Prefix(Pickup __instance)
        {
            if (__instance == null) return true;
            if (RandomMilkComponent.IsAnyRandomMilk(__instance.item))
            {
                string tag = RandomMilkComponent.NameFor(__instance.item);
                RandomMilkOutcome outc = RandomMilkComponent.RollRandomMilkOutcome(
                    RandomMilkComponent.AirChanceFor(__instance.item));

                
                
                
                RemoveFromRoomRespawn(__instance);

                if (outc.IsAir)
                {
                    
                    
                    
                    if (__instance.icon != null && __instance.icon.spriteRenderer != null)
                        __instance.icon.spriteRenderer.enabled = false;
                    UnityEngine.Object.Destroy(__instance.gameObject);
                    return false; 
                }
                if (outc.Item != null)
                {
                    __instance.item = outc.Item;
                    Plugin._randomMilkPickups.Add(__instance); 
                    
                }
            }
            return true;
        }

        
        
        
        
        
        private static void RemoveFromRoomRespawn(Pickup p)
        {
            try
            {
                var rf = p.GetComponentInParent<RespawnItemsRoomFunction>();
                if (rf == null) return;
                var t = rf.GetType();
                var fPickups = t.GetField("pickups", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                var fItems = t.GetField("itemObjects", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                if (fPickups == null) return;
                var lPickups = fPickups.GetValue(rf) as System.Collections.IList;
                if (lPickups == null) return;
                int idx = lPickups.IndexOf(p);
                if (idx >= 0)
                {
                    
                    lPickups.RemoveAt(idx);
                    if (fItems != null)
                    {
                        var lItems = fItems.GetValue(rf) as System.Collections.IList;
                        if (lItems != null && idx < lItems.Count) lItems.RemoveAt(idx);
                    }
                }
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    [HarmonyPatch(typeof(RespawnItemsRoomFunction), "ItemCollected")]
    public class PatchRandomMilkSkipRespawn
    {
        static bool Prefix(Pickup pickup, int player)
        {
            if (pickup != null && Plugin._randomMilkPickups.Contains(pickup)) return false;
            return true;
        }
    }

    
    
    [HarmonyPatch(typeof(Pickup), "Collect")]
    public class PatchRandomMilkPickupCollect
    {
        static void Postfix(Pickup __instance)
        {
            if (__instance == null) return;
            if (Plugin._randomMilkPickups.Contains(__instance))
            {
                Plugin._randomMilkPickups.Remove(__instance);
                if (__instance.icon != null && __instance.icon.spriteRenderer != null)
                    __instance.icon.spriteRenderer.enabled = false;
            }
        }
    }

    
    [HarmonyPatch(typeof(ItemManager), "AddItem", new System.Type[] { typeof(ItemObject) })]
    public class PatchRandomMilkAddItem
    {
        static bool Prefix(ItemManager __instance, ref ItemObject item)
        {
            if (!RandomMilkComponent.IsAnyRandomMilk(item)) return true;
            {
                string tag = RandomMilkComponent.NameFor(item);
                RandomMilkOutcome outc = RandomMilkComponent.RollRandomMilkOutcome(
                    RandomMilkComponent.AirChanceFor(item));
                if (outc.IsAir)
                {
                    
                    item = null;
                    return false; 
                }
                if (outc.Item != null)
                {
                    item = outc.Item;
                    
                }
            }
            return true;
        }
    }

    
    [HarmonyPatch(typeof(ItemManager), "SetItem", new System.Type[] { typeof(ItemObject), typeof(int) })]
    public class PatchRandomMilkSetItem
    {
        static bool Prefix(ItemManager __instance, ref ItemObject item)
        {
            if (!RandomMilkComponent.IsAnyRandomMilk(item)) return true;
            {
                string tag = RandomMilkComponent.NameFor(item);
                RandomMilkOutcome outc = RandomMilkComponent.RollRandomMilkOutcome(
                    RandomMilkComponent.AirChanceFor(item));
                if (outc.IsAir)
                {
                    
                    item = null;
                    return false; 
                }
                if (outc.Item != null)
                {
                    item = outc.Item;
                    
                }
            }
            return true;
        }
    }

    
    
    
    [HarmonyPatch(typeof(LevelGenerator), "Generate")]
    public class PatchInjectMilkHallwayPosters
    {
        static void Prefix(LevelGenerator __instance)
        {
            try
            {
                if (__instance == null || __instance.ld == null) return;
                var aliases = LevelLoaderPlugin.Instance.posterAliases;
                if (aliases == null) return;

                var toAdd = new System.Collections.Generic.List<WeightedPosterObject>();
                AddPoster(aliases, toAdd, "MilkPoster2", 30);
                AddPoster(aliases, toAdd, "MilkPoster3", 30);
                AddPoster(aliases, toAdd, "MilkPoster4", 30);
                AddPoster(aliases, toAdd, "MilkPoster5", 30);
                AddPoster(aliases, toAdd, "MilkPoster6", 30);
                AddPoster(aliases, toAdd, "MilkPoster7", 30);
                AddPoster(aliases, toAdd, "MilkPoster10", 30);
                AddPoster(aliases, toAdd, "MilkPoster11", 30);
                AddPoster(aliases, toAdd, "MilkPoster12", 30);
                AddPoster(aliases, toAdd, "MilkPoster13", 30);
                
                AddPoster(aliases, toAdd, "MilkPoster17", 30);
                
                AddPoster(aliases, toAdd, "MilkPoster19", 30);
                if (toAdd.Count == 0) return;

                WeightedPosterObject[] old = __instance.ld.posters;
                int oldLen = (old != null) ? old.Length : 0;
                WeightedPosterObject[] merged = new WeightedPosterObject[oldLen + toAdd.Count];
                if (old != null) System.Array.Copy(old, merged, oldLen);
                for (int i = 0; i < toAdd.Count; i++) merged[oldLen + i] = toAdd[i];
                __instance.ld.posters = merged;

                
            }
            catch (System.Exception )
            {
                
            }
        }

        static void AddPoster(System.Collections.Generic.Dictionary<string, PosterObject> aliases,
                              System.Collections.Generic.List<WeightedPosterObject> list, string alias, int weight)
        {
            PosterObject poster;
            if (aliases.TryGetValue(alias, out poster) && poster != null)
            {
                WeightedPosterObject wpo = new WeightedPosterObject();
                wpo.selection = poster;
                wpo.weight = weight;
                list.Add(wpo);
            }
        }
    }

    
    
    
    
    
    
    
    
    [HarmonyPatch(typeof(BaseGameManager), "PrepareLevelGenerationModifier")]
    public class PatchPrepareLevelGenerationModifier
    {
        static void Prefix(LevelGenerationModifier ___levelGenerationModifier)
        {
            try
            {
                
                
                
                
                try
                {
                    var cgm0 = Singleton<CoreGameManager>.Instance;
                    if (cgm0 != null && cgm0.sceneObject != null && cgm0.sceneObject.levelObject != null)
                    {
                        int levelNo = cgm0.sceneObject.levelNo;
                        bool isFactoryLayer = Plugin.IsFactoryFloor(levelNo);
                        if (isFactoryLayer)
                        {
                            return;
                        }
                    }
                }
                catch (System.Exception) { }

                if (___levelGenerationModifier == null)
                {
                    
                    return;
                }
                if (___levelGenerationModifier.additionalRoomGroup == null)
                {
                    
                    return;
                }

                
                Plugin.LoadMilkRoomsFromFiles();
                
                if (Plugin.LoadedMilkRooms.Count == 0)
                {
                    
                    return;
                }

                
                foreach (RoomGroup existing in ___levelGenerationModifier.additionalRoomGroup)
                {
                    if (existing != null && existing.name == "MilkRooms") return;
                }

                
                RoomGroup group = new RoomGroup();
                group.name = "MilkRooms";
                group.minRooms = 1;
                group.maxRooms = 2;

                
                
                
                
                RoomAsset sample = Plugin.LoadedMilkRooms[0].selection;
                group.wallTexture = new WeightedTexture2D[]
                {
                    new WeightedTexture2D { selection = sample.wallTex, weight = 100 }
                };
                group.floorTexture = new WeightedTexture2D[]
                {
                    new WeightedTexture2D { selection = sample.florTex, weight = 100 }
                };
                group.ceilingTexture = new WeightedTexture2D[]
                {
                    new WeightedTexture2D { selection = sample.ceilTex, weight = 100 }
                };
                group.light = new WeightedTransform[]
                {
                    new WeightedTransform { selection = LevelLoaderPlugin.Instance.lightTransforms["fluorescent"], weight = 100 }
                };

                group.potentialRooms = Plugin.LoadedMilkRooms.ToArray();

                ___levelGenerationModifier.additionalRoomGroup.Add(group);
                
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    
    
    
    [HarmonyPatch(typeof(BaseGameManager), "GiveRandomSticker")]
    public class PatchStickerPackInjection
    {
        static void Prefix(StickerPackType packType, int total)
        {
            try
            {
                if (!Plugin.StickersReady) return;
                var cgm = Singleton<CoreGameManager>.Instance;
                if (cgm == null || cgm.sceneObject == null) return;
                SceneObject so = cgm.sceneObject;
                
                if (so.potentialStickers != null)
                {
                    foreach (WeightedSticker ws in so.potentialStickers)
                    {
                        if (ws == null) continue;
                        if (ws.selection == Plugin.BilkSticker
                            || ws.selection == Plugin.BaldishhSticker
                            || ws.selection == Plugin.PolishCowSticker
                            || ws.selection == Plugin.AngryPolishCowSticker) return;
                    }
                }
                CharissaHelpfulMod.AddSticker(so, Plugin.BilkSticker, 120);
                CharissaHelpfulMod.AddSticker(so, Plugin.BaldishhSticker, 140);
                CharissaHelpfulMod.AddSticker(so, Plugin.PolishCowSticker, 130);
                CharissaHelpfulMod.AddSticker(so, Plugin.AngryPolishCowSticker, 120);
                
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    
    
    [HarmonyPatch(typeof(BaseGameManager), "PrepareLevelGenerationData")]
    public class PatchBilkStickerGenerator
    {
        static void Prefix()
        {
            try
            {
                if (!Plugin.StickersReady) return;
                if (Singleton<StickerManager>.Instance == null
                    || Singleton<StickerManager>.Instance.StickerValue(Plugin.BilkSticker) <= 0) return;
                var lgp = Singleton<BaseGameManager>.Instance.levelObject;
                if (lgp == null) return;
                WeightedRoomAsset[] assets = Plugin.GetBilkClassroomAssets();
                if (assets == null || assets.Length == 0)
                {
                    
                    return;
                }
                if (lgp.roomGroup != null)
                {
                    int groups = 0;
                    foreach (RoomGroup rg in lgp.roomGroup)
                    {
                        if (rg == null) continue;
                        rg.potentialRooms = assets;
                        
                        
                        rg.minRooms = 2;
                        rg.maxRooms = 4;
                        groups++;
                    }
                    
                }
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    [HarmonyPatch(typeof(CoreGameManager), "get_YtpMultiplier")]
    public class PatchBilkYtpMultiplier
    {
        static void Postfix(ref float __result)
        {
            try
            {
                if (!Plugin.StickersReady) return;
                if (Singleton<StickerManager>.Instance != null
                    && Singleton<StickerManager>.Instance.StickerValue(Plugin.BilkSticker) > 0)
                    __result *= 5f;
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    [HarmonyPatch(typeof(Baldi), "Hear")]
    public class PatchBaldishhBaldi
    {
        static void Prefix(Baldi __instance, ref int value)
        {
            try
            {
                if (!Plugin.StickersReady) return;
                if (Singleton<StickerManager>.Instance == null
                    || Singleton<StickerManager>.Instance.StickerValue(Plugin.BaldishhSticker) <= 0) return;
                value = Mathf.RoundToInt(value * 0.25f);
                try
                {
                    if (__instance.AudMan != null) __instance.AudMan.volumeModifier = 0f;
                }
                catch (System.Exception) { }
            }
            catch (System.Exception) { }
        }
    }

    
    [HarmonyPatch(typeof(Baldi), "VirtualUpdate")]
    public class PatchBaldishhMute
    {
        static void Postfix(Baldi __instance)
        {
            try
            {
                if (!Plugin.StickersReady) return;
                if (Singleton<StickerManager>.Instance != null
                    && Singleton<StickerManager>.Instance.StickerValue(Plugin.BaldishhSticker) > 0
                    && __instance.AudMan != null)
                {
                    __instance.AudMan.volumeModifier = 0f;
                    Plugin.SilencedBaldiAudMan = __instance.AudMan; 
                }
            }
            catch (System.Exception) { }
        }
    }

    
    
    [HarmonyPatch(typeof(BaseGameManager), "PrepareLevelGenerationData")]
    public class PatchBaldishhGoodItems
    {
        static void Prefix()
        {
            try
            {
                if (!Plugin.StickersReady) return;
                if (Singleton<StickerManager>.Instance == null
                    || Singleton<StickerManager>.Instance.StickerValue(Plugin.BaldishhSticker) <= 0) return;
                var lgp = Singleton<BaseGameManager>.Instance.levelObject;
                if (lgp == null) return;
                if (lgp.potentialItems != null)
                {
                    int doubled = 0;
                    foreach (WeightedItemObject w in lgp.potentialItems)
                    {
                        if (w != null && w.selection != null && w.selection.value >= lgp.highEndCutoff)
                        {
                            w.weight *= 2;
                            doubled++;
                        }
                    }
                }
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    [HarmonyPatch(typeof(BaseGameManager), "PrepareLevelGenerationData")]
    public class PatchPolishCowStickerGenerator
    {
        static void Prefix()
        {
            try
            {
                if (!Plugin.StickersReady || Plugin.PolishCowSpawnStructure == null) return;
                if (Singleton<StickerManager>.Instance == null
                    || Singleton<StickerManager>.Instance.StickerValue(Plugin.PolishCowSticker) <= 0) return;
                var lgp = Singleton<BaseGameManager>.Instance.levelObject;
                if (lgp == null) return;
                
                if (lgp.forcedStructures != null)
                {
                    foreach (StructureWithParameters s in lgp.forcedStructures)
                    {
                        if (s != null && s.prefab is Structure_SpawnPolishCows) return;
                    }
                }
                StructureWithParameters swp = new StructureWithParameters();
                swp.prefab = Plugin.PolishCowSpawnStructure;
                swp.parameters = new StructureParameters();
                swp.parameters.minMax = new IntVector2[] { new IntVector2(1, 1) };
                StructureWithParameters[] cur = lgp.forcedStructures ?? new StructureWithParameters[0];
                StructureWithParameters[] nw = new StructureWithParameters[cur.Length + 1];
                System.Array.Copy(cur, nw, cur.Length);
                nw[cur.Length] = swp;
                lgp.forcedStructures = nw;
                
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    public class Structure_SpawnPolishCows : StructureBuilder
    {
        public override void Generate(LevelGenerator levelBuilder, System.Random rng)
        {
            try
            {
                EnvironmentController ec = ((LevelBuilder)levelBuilder).Ec;
                if (ec == null) return;
                NPC cowPrefab = null;
                try
                {
                    if (Plugin.Instance != null && Plugin.Instance.assetMan != null)
                        cowPrefab = Plugin.Instance.assetMan.Get<NPC>("PolishCow");
                }
                catch (System.Exception) { cowPrefab = null; }
                if (cowPrefab == null)
                {
                    
                    return;
                }

                var hallCells = new System.Collections.Generic.List<Cell>();
                if (ec.mainHall != null && ec.mainHall.cells != null) hallCells.AddRange(ec.mainHall.cells);
                if (ec.rooms != null)
                {
                    foreach (RoomController room in ec.rooms)
                    {
                        if (room != null && room.type == RoomType.Hall && room.cells != null)
                            hallCells.AddRange(room.cells);
                    }
                }
                if (hallCells.Count == 0)
                {
                    
                    return;
                }

                int count = rng.Next(3, 6); 
                
                try
                {
                    Vector3 spPt = ec.spawnPoint;
                    hallCells.RemoveAll(cellT =>
                    {
                        if (cellT == null) return true;
                        return Vector3.Distance(cellT.FloorWorldPosition, spPt) < 12f;
                    });
                }
                catch (System.Exception) { }
                int spawned = 0;
                for (int i = 0; i < count; i++)
                {
                    Cell c = hallCells[rng.Next(0, hallCells.Count)];
                    if (c == null) continue;
                    try
                    {
                        ec.SpawnNPC(cowPrefab, c.position);
                        spawned++;
                    }
                    catch (System.Exception )
                    {
                        
                    }
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    [HarmonyPatch(typeof(BaseGameManager), "PrepareLevelGenerationData")]
    public class PatchAngryPolishCowStickerGenerator
    {
        static void Prefix()
        {
            try
            {
                if (!Plugin.StickersReady || Plugin.AngryPolishCowSpawnStructure == null) return;
                if (Singleton<StickerManager>.Instance == null
                    || Singleton<StickerManager>.Instance.StickerValue(Plugin.AngryPolishCowSticker) <= 0) return;
                var lgp = Singleton<BaseGameManager>.Instance.levelObject;
                if (lgp == null) return;
                
                if (lgp.forcedStructures != null)
                {
                    foreach (StructureWithParameters s in lgp.forcedStructures)
                    {
                        if (s != null && s.prefab is Structure_SpawnStampedeCows) return;
                    }
                }
                StructureWithParameters swp = new StructureWithParameters();
                swp.prefab = Plugin.AngryPolishCowSpawnStructure;
                swp.parameters = new StructureParameters();
                swp.parameters.minMax = new IntVector2[] { new IntVector2(1, 1) };
                StructureWithParameters[] cur = lgp.forcedStructures ?? new StructureWithParameters[0];
                StructureWithParameters[] nw = new StructureWithParameters[cur.Length + 1];
                System.Array.Copy(cur, nw, cur.Length);
                nw[cur.Length] = swp;
                lgp.forcedStructures = nw;
                
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    
    public class Structure_SpawnStampedeCows : StructureBuilder
    {
        public override void Generate(LevelGenerator levelBuilder, System.Random rng)
        {
            try
            {
                EnvironmentController ec = ((LevelBuilder)levelBuilder).Ec;
                if (ec == null) return;
                NPC cowPrefab = Plugin.StampedeCowPrefab;
                if (cowPrefab == null)
                {
                    try
                    {
                        if (Plugin.Instance != null && Plugin.Instance.assetMan != null)
                            cowPrefab = Plugin.Instance.assetMan.Get<NPC>("StampedeCow");
                    }
                    catch (System.Exception) { cowPrefab = null; }
                }
                if (cowPrefab == null)
                {
                    
                    return;
                }

                var hallCells = new System.Collections.Generic.List<Cell>();
                if (ec.mainHall != null && ec.mainHall.cells != null) hallCells.AddRange(ec.mainHall.cells);
                if (ec.rooms != null)
                {
                    foreach (RoomController room in ec.rooms)
                    {
                        if (room != null && room.type == RoomType.Hall && room.cells != null)
                            hallCells.AddRange(room.cells);
                    }
                }
                if (hallCells.Count == 0)
                {
                    
                    return;
                }

                int count = rng.Next(2, 4); 
                
                try
                {
                    Vector3 spPt = ec.spawnPoint;
                    hallCells.RemoveAll(cellT =>
                    {
                        if (cellT == null) return true;
                        return Vector3.Distance(cellT.FloorWorldPosition, spPt) < 12f;
                    });
                }
                catch (System.Exception) { }
                int spawned = 0;
                for (int i = 0; i < count; i++)
                {
                    Cell c = hallCells[rng.Next(0, hallCells.Count)];
                    if (c == null) continue;
                    try
                    {
                        ec.SpawnNPC(cowPrefab, c.position);
                        spawned++;
                    }
                    catch (System.Exception )
                    {
                        
                    }
                }
                
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    
    [HarmonyPatch(typeof(NPC), "Hear")]
    public class PatchPolishCowNpcHearing
    {
        static void Prefix(NPC __instance, ref int value)
        {
            try
            {
                if (!Plugin.StickersReady) return;
                if (Singleton<StickerManager>.Instance == null
                    || Singleton<StickerManager>.Instance.StickerValue(Plugin.PolishCowSticker) <= 0) return;
                if (__instance == null || __instance.ec == null) return;
                float dist = Plugin.DistanceToNearestCow(__instance.ec, __instance.transform.position, 40f);
                float scale = Mathf.Max(0.15f, Mathf.Clamp01(dist / 40f));
                value = Mathf.RoundToInt(value * scale);
            }
            catch (System.Exception) { }
        }
    }

    
    [HarmonyPatch(typeof(Baldi), "Hear")]
    public class PatchPolishCowBaldiHearing
    {
        static void Prefix(Baldi __instance, ref int value)
        {
            try
            {
                if (!Plugin.StickersReady) return;
                if (Singleton<StickerManager>.Instance == null
                    || Singleton<StickerManager>.Instance.StickerValue(Plugin.PolishCowSticker) <= 0) return;
                if (__instance == null || __instance.ec == null) return;
                float dist = Plugin.DistanceToNearestCow(__instance.ec, __instance.transform.position, 40f);
                float scale = Mathf.Max(0.15f, Mathf.Clamp01(dist / 40f));
                value = Mathf.RoundToInt(value * scale);
            }
            catch (System.Exception) { }
        }
    }

    
    
    public class PolishCowStickerAudio : MonoBehaviour
    {
        private void Update()
        {
            try
            {
                bool active = Plugin.StickersReady
                    && Singleton<StickerManager>.Instance != null
                    && Singleton<StickerManager>.Instance.StickerValue(Plugin.PolishCowSticker) > 0;
                if (!active)
                {
                    
                    
                    
                    if (_wasActive) { _wasActive = false; if (AudioListener.volume > 0f) AudioListener.volume = 1f; }
                    return;
                }
                if (!_wasActive) _wasActive = true;
                var cgm = Singleton<CoreGameManager>.Instance;
                PlayerManager p = (cgm != null) ? cgm.GetPlayer(0) : null;
                EnvironmentController ec = (cgm != null) ? cgm.GetComponent<EnvironmentController>() : null;
                if (p == null || ec == null) return;
                float dist = Plugin.DistanceToNearestCow(ec, p.transform.position, 40f);
                float vol = Mathf.Max(0.2f, Mathf.Clamp01(dist / 40f));
                AudioListener.volume = vol;
            }
            catch (System.Exception) { }
        }
        private bool _wasActive = false;
    }

    
    
    
    
    [HarmonyPatch(typeof(LevelBuilder), "GenerateActivity")]
    public class PatchEnsureMilkMachinePrefab
    {
        static void Prefix(LevelBuilder __instance, RoomController room, ActivityData data)
        {
            if (data == null) return;

            MilkMachine fallback = Plugin.MilkMachinePrefabInstance;
            if (fallback == null && Plugin.Instance?.assetMan != null)
            {
                try { fallback = Plugin.Instance.assetMan.Get<MilkMachine>("MilkMachine"); }
                catch { fallback = null; }
            }

            
            
            
            
            if (fallback != null && data.prefab is MilkMachine)
            {
                if (LevelLoaderPlugin.Instance?.activityAliases != null
                    && !LevelLoaderPlugin.Instance.activityAliases.ContainsKey("MilkMachine"))
                {
                    LevelLoaderPlugin.Instance.activityAliases["MilkMachine"] = fallback;
                }
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        static void Postfix(LevelBuilder __instance, RoomController room, ActivityData data)
        {
            try
            {
                if (data == null || data.prefab == null) return;
                if (!(data.prefab is MilkMachine)) return;

                
                
                var activityField = typeof(RoomController).GetField(
                    "activity",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (activityField == null) return;

                Activity activity = activityField.GetValue(room) as Activity;
                if (activity != null && activity is MilkMachine)
                {
                    room.ec.AddActivity(activity);
                }
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    
    [HarmonyPatch(typeof(FieldTripBaseRoomFunction), "Initialize")]
    public class PatchCampingRewardPool
    {
        static void Prefix(FieldTripBaseRoomFunction __instance)
        {
            try
            {
                if (Plugin.AppleMilkItemObject == null) return;

                var field = typeof(FieldTripBaseRoomFunction).GetField(
                    "potentialItems",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field == null) return;

                var arr = (WeightedItemObject[])field.GetValue(__instance);
                int len = (arr != null) ? arr.Length : 0;

                
                bool already = false;
                for (int i = 0; i < len; i++)
                {
                    if (arr[i] != null && arr[i].selection == Plugin.AppleMilkItemObject) { already = true; break; }
                }
                if (already) return;

                var entry = (WeightedItemObject)System.Activator.CreateInstance(typeof(WeightedItemObject));
                entry.selection = Plugin.AppleMilkItemObject;
                entry.weight = 100; 

                var newArr = new WeightedItemObject[len + 1];
                for (int i = 0; i < len; i++) newArr[i] = arr[i];
                newArr[len] = entry;
                field.SetValue(__instance, newArr);

                
            }
            catch (System.Exception )
            {
                
            }
        }
    }

    
    
    
    
    [HarmonyPatch(typeof(ITM_BSODA), "EntityTriggerEnter")]
    public class PatchSodaPushAchievement
    {
        static void Postfix(Entity otherEntity, Collider other, bool validCollision)
        {
            try
            {
                if (validCollision && other != null && !other.CompareTag("Player") && otherEntity != null)
                {
                    AchievementHelper.UnlockAchievement("milk_sodapush");
                }
            }
            catch (System.Exception) { }
        }
    }

    
    
    [HarmonyPatch(typeof(EnvironmentController), "BuildPoster", new System.Type[] { typeof(PosterObject), typeof(Cell), typeof(Direction) })]
    public class PatchRandomMilkPoster
    {
        
        static void Prefix(ref PosterObject poster, ref bool __runOriginal)
        {
            __runOriginal = RandomPosterHelper.ResolveRandomPoster(ref poster);
        }
    }

    [HarmonyPatch(typeof(EnvironmentController), "BuildPoster", new System.Type[] { typeof(PosterObject), typeof(Cell), typeof(Direction), typeof(bool) })]
    public class PatchRandomMilkPoster4Param
    {
        static void Prefix(ref PosterObject poster, ref bool __runOriginal)
        {
            __runOriginal = RandomPosterHelper.ResolveRandomPoster(ref poster);
        }
    }

    [HarmonyPatch(typeof(EnvironmentController), "BuildPoster", new System.Type[] { typeof(PosterObject), typeof(Cell), typeof(Direction), typeof(System.Random) })]
    public class PatchRandomMilkPosterRng
    {
        static void Prefix(ref PosterObject poster, ref bool __runOriginal)
        {
            __runOriginal = RandomPosterHelper.ResolveRandomPoster(ref poster);
        }
    }

    
    
    
    internal static class RandomPosterHelper
    {
        
        
        
        
        internal static bool ResolveRandomPoster(ref PosterObject poster)
        {
            if (poster == null) return true;
            var aliases = LevelLoaderPlugin.Instance.posterAliases;
            if (aliases == null) return true;

            
            PosterObject placeholder;
            if (aliases.TryGetValue("MilkPoster_Random", out placeholder) && poster == placeholder)
            {
                PosterObject chosen = PickRandomMilkPoster(aliases);
                if (chosen != null)
                {
                    poster = chosen;
                    
                }
                return true; 
            }

            
            if (aliases.TryGetValue("MilkPoster_RandomNoItem", out placeholder) && poster == placeholder)
            {
                if (UnityEngine.Random.value < Plugin.RandomPosterNoShowChance)
                {
                    
                    return false; 
                }
                PosterObject chosen = PickRandomMilkPoster(aliases);
                if (chosen != null)
                {
                    poster = chosen;
                    
                }
                return true; 
            }

            return true; 
        }

        
        internal static PosterObject PickRandomMilkPoster(System.Collections.Generic.Dictionary<string, PosterObject> aliases)
        {
            var pool = new System.Collections.Generic.List<PosterObject>();
            PosterObject p;
            string[] names = new string[] { "MilkPoster", "MilkPoster2", "MilkPoster3", "MilkPoster4", "MilkPoster5", "MilkPoster6", "MilkPoster7", "MilkPoster10", "MilkPoster11", "MilkPoster12", "MilkPoster13" };
            for (int i = 0; i < names.Length; i++)
            {
                if (aliases.TryGetValue(names[i], out p) && p != null) pool.Add(p);
            }
            if (pool.Count == 0) return null;
            return pool[UnityEngine.Random.Range(0, pool.Count)];
        }
    }

    
    
    
    public class PoisonMilkProjectile : MonoBehaviour
    {
        private float speed = 22f;
        private float life = 5f;
        private float elapsed = 0f;
        private bool triggered = false;
        private Vector3 forward;
        private EnvironmentController ec;
        private float hitRadius = 3f;
        private GameObject visual;

        public void Initialize(PlayerManager player, float duration = 40f)
        {
            ec = player.ec;
            forward = player.transform.forward;
            transform.position = player.transform.position + Vector3.up * 1.5f;
            transform.forward = forward;
            life = 5f;

            
            var box = gameObject.AddComponent<BoxCollider>();
            box.isTrigger = false;
            box.size = new Vector3(0.5f, 0.5f, 0.5f);
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; 
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            
            Sprite spr = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, "PoisonMilk_Large.png");
            if (spr != null)
            {
                visual = new GameObject("PoisonMilkProjectileVisual");
                visual.transform.SetParent(transform, false);
                visual.transform.localPosition = Vector3.zero;
                var sr = visual.AddComponent<SpriteRenderer>();
                sr.sprite = spr;
                sr.sortingOrder = 0; 
            }
        }

        private void Update()
        {
            if (triggered) return;
            elapsed += Time.deltaTime;

            
            var rb = GetComponent<Rigidbody>();
            Vector3 newPos = transform.position + forward * speed * Time.deltaTime;

            
            try
            {
                float step = speed * Time.deltaTime + 0.3f;
                RaycastHit hitInfo;
                if (Physics.Raycast(transform.position, forward, out hitInfo, step, ~0, QueryTriggerInteraction.Ignore))
                {
                    bool hitNpcOrPlayer = hitInfo.collider != null &&
                        (hitInfo.collider.CompareTag("NPC") || hitInfo.collider.CompareTag("Player"));
                    if (!hitNpcOrPlayer)
                    {
                        
                        UnityEngine.Object.Destroy(gameObject);
                        return;
                    }
                }
            }
            catch (System.Exception) { }

            if (rb != null) rb.MovePosition(newPos);
            else transform.position = newPos;

            
            if (ec != null && ec.Npcs != null)
            {
                Vector3 pos = transform.position;
                foreach (var npc in ec.Npcs)
                {
                    if (npc == null) continue;
                    Vector3 npcPos = npc.transform.position;
                    float dx = pos.x - npcPos.x;
                    float dz = pos.z - npcPos.z;
                    if (Mathf.Sqrt(dx * dx + dz * dz) <= hitRadius)
                    {
                        triggered = true;
                        
                        try
                        {
                            if (PoisonMilkProjectile.player_ref != null)
                                PoisonMilkProjectile.player_ref.StartCoroutine(Plugin.PoisonMilkNPCEffectCoroutine(npc, 20f));
                        }
                        catch (System.Exception )
                        {
                            
                        }
                        UnityEngine.Object.Destroy(gameObject);
                        return;
                    }
                }
            }

            
            if (elapsed >= life)
            {
                
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (visual != null) UnityEngine.Object.Destroy(visual);
        }

        
        public static PlayerManager player_ref;
    }

    
    
    
    
    
    
    
    [HarmonyPatch(typeof(PlayerManager), "Update")]
    public class PatchPlayerUpdate
    {
        private static readonly System.Collections.Generic.Dictionary<PlayerManager, MovementModifier> slowMods =
            new System.Collections.Generic.Dictionary<PlayerManager, MovementModifier>();
        private static readonly System.Reflection.FieldInfo curRoomField =
            HarmonyLib.AccessTools.Field(typeof(PlayerManager), "currentRoom");
        
        private static readonly System.Collections.Generic.Dictionary<PlayerManager, bool> activeState =
            new System.Collections.Generic.Dictionary<PlayerManager, bool>();
        private static readonly System.Collections.Generic.Dictionary<PlayerManager, int> streak =
            new System.Collections.Generic.Dictionary<PlayerManager, int>();
        private const int HYST = 4;
        private static int diagFrame = 0;

        static void Postfix(PlayerManager __instance)
        {
            if (__instance == null) return;

            RoomController room = null;
            try { room = curRoomField?.GetValue(__instance) as RoomController; } catch (System.Exception) { }

            
            
            
            bool coldNow = room != null && Plugin.ColdRoomCategoryReady
                          && room.category == Plugin.ColdRoomCategory;

            bool active = activeState.TryGetValue(__instance, out bool a) && a;
            int s = 0; streak.TryGetValue(__instance, out s);

            
            if (coldNow)
            {
                diagFrame++;
            }

            if (coldNow == active)
            {
                
                s = coldNow ? HYST : 0;
            }
            else
            {
                s += coldNow ? 1 : -1;
                if (s < 0) s = 0;
                if (s > HYST) s = HYST;
            }
            streak[__instance] = s;

            
            if (!active && coldNow && s >= HYST)
            {
                active = true; activeState[__instance] = true;
                ApplyCold(__instance, room);
            }
            else if (active && !coldNow && s <= 0)
            {
                active = false; activeState[__instance] = false;
                RemoveCold(__instance);
            }

            
            ProcessNpcCold(__instance);

            
            ProcessHotRoom(__instance, room);
        }

        private static bool overlayInstalled = false;
        static void EnsureOverlay()
        {
            if (overlayInstalled) return;
            if (Plugin.Instance != null)
            {
                if (Plugin.Instance.gameObject.GetComponent<ColdBlueOverlay>() == null)
                    Plugin.Instance.gameObject.AddComponent<ColdBlueOverlay>();
                overlayInstalled = true;
            }
        }

        static void ApplyCold(PlayerManager pm, RoomController room)
        {
            

            
            
            try
            {
                if (room != null && room.functions == null)
                {
                    var cont = room.gameObject.AddComponent<RoomFunctionContainer>();
                    cont.Initialize(room);
                    room.functions = cont;
                    room.functionObject = cont.gameObject;
                }
            }
            catch (System.Exception )
            {
                
            }

            

            
            MovementModifier slow = null;
            if (pm.plm != null && pm.plm.am != null)
            {
                slow = new MovementModifier(Vector3.zero, 1f, 0);
                slow.movementMultiplier = 0.4f;
                pm.plm.am.moveMods.Add(slow);
                
            }
            else
            {
                
            }
            slowMods[pm] = slow;

            
            
            EnsureOverlay();
            ColdBlueOverlay.ActivePlayers.Add(pm.playerNumber);
            
        }

        static void RemoveCold(PlayerManager pm)
        {
            
            if (slowMods.TryGetValue(pm, out MovementModifier s))
            {
                if (s != null && pm.plm != null && pm.plm.am != null)
                    pm.plm.am.moveMods.Remove(s);
                slowMods.Remove(pm);
            }
            ColdBlueOverlay.ActivePlayers.Remove(pm.playerNumber);
        }

        
        private static readonly System.Collections.Generic.Dictionary<NPC, MovementModifier> npcSlowMods =
            new System.Collections.Generic.Dictionary<NPC, MovementModifier>();
        private static readonly System.Collections.Generic.Dictionary<NPC, bool> npcActive =
            new System.Collections.Generic.Dictionary<NPC, bool>();
        private static readonly System.Collections.Generic.Dictionary<NPC, int> npcStreak =
            new System.Collections.Generic.Dictionary<NPC, int>();
        private const float NPC_COLD_MULT = 0.4f;

        static void ProcessNpcCold(PlayerManager pm)
        {
            if (pm == null || pm.ec == null || pm.ec.Npcs == null) return;
            var live = new System.Collections.Generic.HashSet<NPC>(pm.ec.Npcs);

            foreach (var npc in pm.ec.Npcs)
            {
                if (npc == null || npc.Entity == null || npc.Entity.ExternalActivity == null) continue;
                RoomController nroom = GetNpcRoom(pm.ec, npc);
                bool coldNow = nroom != null && Plugin.ColdRoomCategoryReady
                               && nroom.category == Plugin.ColdRoomCategory;

                bool active = npcActive.TryGetValue(npc, out bool a) && a;
                int s = 0; npcStreak.TryGetValue(npc, out s);

                if (coldNow == active) s = coldNow ? HYST : 0;
                else
                {
                    s += coldNow ? 1 : -1;
                    if (s < 0) s = 0;
                    if (s > HYST) s = HYST;
                }
                npcStreak[npc] = s;

                if (!active && coldNow && s >= HYST)
                {
                    active = true; npcActive[npc] = true;
                    ApplyNpcCold(npc);
                }
                else if (active && !coldNow && s <= 0)
                {
                    active = false; npcActive[npc] = false;
                    RemoveNpcCold(npc);
                }
            }

            
            var dead = new System.Collections.Generic.List<NPC>();
            foreach (var k in npcActive.Keys)
                if (k == null || !live.Contains(k)) dead.Add(k);
            foreach (var k in dead)
            {
                RemoveNpcCold(k);
                npcActive.Remove(k);
                npcStreak.Remove(k);
            }
        }

        static RoomController GetNpcRoom(EnvironmentController ec, NPC npc)
        {
            try
            {
                var cell = ec.CellFromPosition(npc.transform.position);
                if (cell != null && cell.room != null) return cell.room;
            }
            catch (System.Exception) { }
            return null;
        }

        static void ApplyNpcCold(NPC npc)
        {
            try
            {
                if (npc.Entity == null || npc.Entity.ExternalActivity == null) return;
                var mod = new MovementModifier(Vector3.zero, 1f, 0);
                mod.movementMultiplier = NPC_COLD_MULT;
                npc.Entity.ExternalActivity.moveMods.Add(mod);
                npcSlowMods[npc] = mod;
            }
            catch (System.Exception )
            {
                
            }
        }

        static void RemoveNpcCold(NPC npc)
        {
            if (npcSlowMods.TryGetValue(npc, out MovementModifier m))
            {
                try { if (npc.Entity != null && npc.Entity.ExternalActivity != null) npc.Entity.ExternalActivity.moveMods.Remove(m); }
                catch (System.Exception) { }
                npcSlowMods.Remove(npc);
            }
        }

        
        
        
        private static readonly System.Collections.Generic.Dictionary<PlayerManager, MovementModifier> hotMods =
            new System.Collections.Generic.Dictionary<PlayerManager, MovementModifier>();
        private static readonly System.Collections.Generic.Dictionary<PlayerManager, bool> hotActiveState =
            new System.Collections.Generic.Dictionary<PlayerManager, bool>();
        private static readonly System.Collections.Generic.Dictionary<PlayerManager, int> hotStreak =
            new System.Collections.Generic.Dictionary<PlayerManager, int>();
        private static readonly System.Collections.Generic.Dictionary<PlayerManager, float> hotHeat =
            new System.Collections.Generic.Dictionary<PlayerManager, float>();
        private const float HOT_PLAYER_MULT = 1.4f;   
        private const float NPC_HOT_MULT = 1.4f;       
        private const float HOT_ROT_INTERVAL = 12f;    

        
        private static readonly System.Collections.Generic.Dictionary<NPC, MovementModifier> npcHotMods =
            new System.Collections.Generic.Dictionary<NPC, MovementModifier>();
        private static readonly System.Collections.Generic.Dictionary<NPC, bool> npcHotActive =
            new System.Collections.Generic.Dictionary<NPC, bool>();
        private static readonly System.Collections.Generic.Dictionary<NPC, int> npcHotStreak =
            new System.Collections.Generic.Dictionary<NPC, int>();

        static void ProcessHotRoom(PlayerManager pm, RoomController room)
        {
            bool hotNow = room != null && Plugin.HotRoomCategoryReady
                          && room.category == Plugin.HotRoomCategory;

            bool active = hotActiveState.TryGetValue(pm, out bool a) && a;
            int s = 0; hotStreak.TryGetValue(pm, out s);

            if (hotNow == active) s = hotNow ? HYST : 0;
            else
            {
                s += hotNow ? 1 : -1;
                if (s < 0) s = 0;
                if (s > HYST) s = HYST;
            }
            hotStreak[pm] = s;

            if (!active && hotNow && s >= HYST)
            {
                active = true; hotActiveState[pm] = true;
                ApplyHot(pm, room);
            }
            else if (active && !hotNow && s <= 0)
            {
                active = false; hotActiveState[pm] = false;
                RemoveHot(pm);
            }

            
            if (active)
            {
                float h = 0f; hotHeat.TryGetValue(pm, out h);
                h += Time.deltaTime;
                if (h >= HOT_ROT_INTERVAL)
                {
                    h = 0f;
                    ConvertInventoryMilkToRotten(pm);
                }
                hotHeat[pm] = h;
            }

            
            ProcessNpcHot(pm);
        }

        private static bool hotOverlayInstalled = false;
        static void EnsureHotOverlay()
        {
            if (hotOverlayInstalled) return;
            if (Plugin.Instance != null)
            {
                if (Plugin.Instance.gameObject.GetComponent<HotRedOverlay>() == null)
                    Plugin.Instance.gameObject.AddComponent<HotRedOverlay>();
                hotOverlayInstalled = true;
            }
        }

        static void ApplyHot(PlayerManager pm, RoomController room)
        {
            

            
            try
            {
                if (room != null && room.functions == null)
                {
                    var cont = room.gameObject.AddComponent<RoomFunctionContainer>();
                    cont.Initialize(room);
                    room.functions = cont;
                    room.functionObject = cont.gameObject;
                }
            }
            catch (System.Exception )
            {
                
            }

            
            MovementModifier fast = null;
            if (pm.plm != null && pm.plm.am != null)
            {
                fast = new MovementModifier(Vector3.zero, 1f, 0);
                fast.movementMultiplier = HOT_PLAYER_MULT;
                pm.plm.am.moveMods.Add(fast);
            }
            hotMods[pm] = fast;

            
            EnsureHotOverlay();
            HotRedOverlay.ActivePlayers.Add(pm.playerNumber);

            
            hotHeat[pm] = 0f;

            
        }

        static void RemoveHot(PlayerManager pm)
        {
            
            if (hotMods.TryGetValue(pm, out MovementModifier m))
            {
                if (m != null && pm.plm != null && pm.plm.am != null)
                    pm.plm.am.moveMods.Remove(m);
                hotMods.Remove(pm);
            }
            HotRedOverlay.ActivePlayers.Remove(pm.playerNumber);
            hotHeat.Remove(pm);
        }

        
        static void ProcessNpcHot(PlayerManager pm)
        {
            if (pm == null || pm.ec == null || pm.ec.Npcs == null) return;
            var live = new System.Collections.Generic.HashSet<NPC>(pm.ec.Npcs);

            foreach (var npc in pm.ec.Npcs)
            {
                if (npc == null || npc.Entity == null || npc.Entity.ExternalActivity == null) continue;
                RoomController nroom = GetNpcRoom(pm.ec, npc);
                bool hotNow = nroom != null && Plugin.HotRoomCategoryReady
                               && nroom.category == Plugin.HotRoomCategory;

                bool active = npcHotActive.TryGetValue(npc, out bool a) && a;
                int st = 0; npcHotStreak.TryGetValue(npc, out st);

                if (hotNow == active) st = hotNow ? HYST : 0;
                else
                {
                    st += hotNow ? 1 : -1;
                    if (st < 0) st = 0;
                    if (st > HYST) st = HYST;
                }
                npcHotStreak[npc] = st;

                if (!active && hotNow && st >= HYST)
                {
                    active = true; npcHotActive[npc] = true;
                    ApplyNpcHot(npc);
                }
                else if (active && !hotNow && st <= 0)
                {
                    active = false; npcHotActive[npc] = false;
                    RemoveNpcHot(npc);
                }
            }

            
            var dead = new System.Collections.Generic.List<NPC>();
            foreach (var k in npcHotActive.Keys)
                if (k == null || !live.Contains(k)) dead.Add(k);
            foreach (var k in dead)
            {
                RemoveNpcHot(k);
                npcHotActive.Remove(k);
                npcHotStreak.Remove(k);
            }
        }

        static void ApplyNpcHot(NPC npc)
        {
            try
            {
                if (npc.Entity == null || npc.Entity.ExternalActivity == null) return;
                var mod = new MovementModifier(Vector3.zero, 1f, 0);
                mod.movementMultiplier = NPC_HOT_MULT;
                npc.Entity.ExternalActivity.moveMods.Add(mod);
                npcHotMods[npc] = mod;
            }
            catch (System.Exception )
            {
                
            }
        }

        static void RemoveNpcHot(NPC npc)
        {
            if (npcHotMods.TryGetValue(npc, out MovementModifier m))
            {
                try { if (npc.Entity != null && npc.Entity.ExternalActivity != null) npc.Entity.ExternalActivity.moveMods.Remove(m); }
                catch (System.Exception) { }
                npcHotMods.Remove(npc);
            }
        }

        
        
        private static System.Collections.Generic.HashSet<ItemObject> _milkSet;
        private static bool _milkSetReady = false;
        static void EnsureMilkSet()
        {
            if (_milkSetReady) return;
            _milkSet = new System.Collections.Generic.HashSet<ItemObject>();
            _milkSet.Add(Plugin.MilkItemObject);
            _milkSet.Add(Plugin.ChocolateMilkItemObject);
            _milkSet.Add(Plugin.MilkSodaItemObject);
            _milkSet.Add(Plugin.CompressedMilkItemObject);
            _milkSet.Add(Plugin.AppleMilkItemObject);
            _milkSet.Add(Plugin.ReverseMilkItemObject);
            _milkSet.Add(Plugin.MiItemObject);
            _milkSet.Add(Plugin.LkItemObject);
            _milkSet.Add(Plugin.LostBilkItemObject);
            _milkSet.Add(Plugin.MilkYtpsItemObject);
            _milkSet.Add(Plugin.WindowMilkItemObject);
            _milkSet.Add(Plugin.NineNineMilkItemObject);
            _milkSet.Add(Plugin.QuarterMilkItemObject);
            _milkSet.Add(Plugin.RandomMilkItemObject);
            _milkSet.Add(Plugin.RandomMilkNoItemItemObject);
            _milkSet.Add(Plugin.RandomMilk75ItemObject);
            _milkSetReady = true;
        }

        static bool IsConvertibleMilk(ItemObject it)
        {
            EnsureMilkSet();
            return it != null
                && it != Plugin.RottenMilkItemObject
                && it != Plugin.PoisonMilkItemObject
                && it != Plugin.EmptyBucketItemObject
                && _milkSet.Contains(it);
        }

        static void ConvertInventoryMilkToRotten(PlayerManager pm)
        {
            if (pm == null || pm.itm == null || pm.itm.items == null) return;
            int converted = 0;
            for (int i = 0; i < pm.itm.items.Length; i++)
            {
                var it = pm.itm.items[i];
                if (IsConvertibleMilk(it))
                {
                    try { pm.itm.SetItem(Plugin.RottenMilkItemObject, i); converted++; }
                    catch (System.Exception ) {  }
                }
            }
            if (converted > 0)
            {
                
                
                try
                {
                    var hud = Singleton<CoreGameManager>.Instance?.GetHud(pm.playerNumber);
                    if (hud != null)
                    {
                        var m = hud.GetType().GetMethod("ShowNotice",
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                            null, new System.Type[] { typeof(string) }, null);
                        m?.Invoke(hud, new object[] { "The heat spoiled the milk in your bag into Rotten Milk!" });
                    }
                }
                catch (System.Exception) { }
            }
        }
    }

    
    
    

    
    [HarmonyPatch(typeof(Elevator), "ButtonPressed")]
    public class PatchMooExitElevator
    {
        static bool Prefix(Elevator __instance)
        {
            try
            {
                if (!Plugin.MooArmed || Plugin.MooEntryTriggered) return true;
                if (__instance == null || __instance.CurrentState != ElevatorState.OpenForExit) return true;

                Plugin.MooEntryTriggered = true;
                Plugin.MooArmed = false;
                
                if (Plugin.MooRedWhiteActive) { FillPlayerWithSodas(); Plugin.MooRedWhiteAdvance(); return false; }
                Plugin.MooStartEntry();   
                return false; 
            }
            catch (System.Exception )
            {
                
                return true;
            }
        }

        
        
        private static void FillPlayerWithSodas()
        {
            try
            {
                var pgm = Singleton<CoreGameManager>.Instance;
                if (pgm == null) return;
                var pm = pgm.GetPlayer(0);
                if (pm == null || pm.itm == null) return;

                ItemObject[] sodas = new ItemObject[4];
                
                ItemObject bsodaIo = null, dietBsodaIo = null;
                ItemObject[] allIo = Resources.FindObjectsOfTypeAll<ItemObject>();
                foreach (var io in allIo)
                {
                    if (io == null) continue;
                    if (io.itemType == Items.Bsoda && bsodaIo == null) bsodaIo = io;
                    else if (io.itemType == Items.DietBsoda && dietBsodaIo == null) dietBsodaIo = io;
                }
                sodas[0] = Plugin.MilkSodaItemObject;
                sodas[1] = Plugin.DietMilkSodaItemObject;
                sodas[2] = bsodaIo;
                sodas[3] = dietBsodaIo;

                
                pm.itm.ClearItems();
                int slots = pm.itm.maxItem;   
                if (slots < 0) return;
                for (int i = 0; i <= slots; i++)
                {
                    ItemObject s = sodas[i % sodas.Length];
                    if (s != null) pm.itm.SetItem(s, i);
                }
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
    public class PatchRedWhiteNoExit
    {
        static bool Prefix()
        {
            
            return !(Plugin.MooRedWhiteActive && !Plugin.MooRedWhiteFailed);
        }
    }

    
    
    
    
    [HarmonyPatch(typeof(MusicManager), "PlayMidi", new System.Type[] { typeof(string), typeof(float), typeof(bool) })]
    public class PatchMooMusicSilence
    {
        static bool Prefix()
        {
            return !(Plugin.MooPhase == 1 || Plugin.MooF1Active || Plugin.MooRedWhiteActive);
        }
    }
    
    [HarmonyPatch(typeof(MusicManager), "PlayMidi", new System.Type[] { typeof(string), typeof(bool) })]
    public class PatchMooMusicSilence2
    {
        static bool Prefix()
        {
            return !(Plugin.MooPhase == 1 || Plugin.MooF1Active || Plugin.MooRedWhiteActive);
        }
    }

    
    
    
    [HarmonyPatch(typeof(HudManager), "ReInit")]
    public class PatchHideMooBaldiTv
    {
        static void Postfix(HudManager __instance)
        {
            try
            {
                if (__instance == null) return;
                var bt = __instance.BaldiTv;
                if (bt == null) return;
                bool show = !(Plugin.MooPhase == 1 || Plugin.MooF1Active);
                if (bt.gameObject.activeSelf != show) bt.gameObject.SetActive(show);
            }
            catch (System.Exception) { }
        }
    }

    
    
    
    
    [HarmonyPatch(typeof(EnvironmentController), "Update")]
    public class PatchNoEventsAfter99
    {
        static void Postfix(EnvironmentController __instance)
        {
            try
            {
                if (!Plugin.nineNineTriggeredThisRun) return;
                if (__instance == null) return;
                var fEvents = AccessTools.Field(__instance.GetType(), "events");
                var fEventTimes = AccessTools.Field(__instance.GetType(), "eventTimes");
                var fCurrent = AccessTools.Field(__instance.GetType(), "currentEvents");

                if (fEvents != null)
                {
                    var l = fEvents.GetValue(__instance) as System.Collections.Generic.List<RandomEvent>;
                    if (l != null && l.Count > 0) l.Clear();
                }
                if (fEventTimes != null)
                {
                    var l = fEventTimes.GetValue(__instance) as System.Collections.Generic.List<float>;
                    if (l != null && l.Count > 0) l.Clear();
                }
                if (fCurrent != null)
                {
                    var cur = fCurrent.GetValue(__instance) as System.Collections.Generic.List<RandomEvent>;
                    if (cur != null && cur.Count > 0)
                    {
                        foreach (var r in new System.Collections.Generic.List<RandomEvent>(cur))
                        {
                            if (r == null) continue;
                            try
                            {
                                var endM = r.GetType().GetMethod("End", System.Type.EmptyTypes);
                                if (endM != null) endM.Invoke(r, null);
                            }
                            catch (System.Exception) { }
                            try { UnityEngine.Object.Destroy(r); } catch (System.Exception) { }
                        }
                        cur.Clear();
                    }
                }
            }
            catch (System.Exception) { }
        }
    }

    
    [HarmonyPatch(typeof(LevelBuilder), "StartGenerate")]
    public class PatchMooPhaseDetect
    {
        static void Postfix(LevelBuilder __instance)
        {
            try
            {
                if (__instance == null) return;
                
                if (Plugin.MooRedWhiteActive && Plugin.MooRedWhiteFloor >= 0 && !Plugin.MooRedWhiteFloorReady)
                {
                    Plugin.MooRedWhiteStage(__instance);
                    return;
                }
                if (Plugin.MooPhase == 1 && !Plugin.MooPh1Started)
                {
                    Plugin.MooPh1Started = true;
                    Plugin.MooPh2Started = false;
                    __instance.StartCoroutine(Plugin.MooPh1WaitThenF1(__instance));
                }
                else if (Plugin.MooPhase == 2 && !Plugin.MooPh2Started)
                {
                    Plugin.MooPh2Started = true;
                    __instance.StartCoroutine(Plugin.MooF1PostGen(__instance));   
                }
            }
            catch (System.Exception ) {  }
        }
    }

    
    [HarmonyPatch(typeof(BaseGameManager), "PrepareLevelGenerationData")]
    public class PatchRedWhiteTerrain
    {
        static void Prefix()
        {
            try
            {
                if (!Plugin.MooRedWhiteActive) return;
                var bgm = Singleton<BaseGameManager>.Instance;
                if (bgm == null) return;
                var lgp = bgm.levelObject;
                if (lgp == null) return;
                RenderSettings.ambientLight = new Color(0.80f, 0.06f, 0.05f);
                lgp.standardLightStrength = 2;                      
                lgp.standardLightColor = new Color(1f, 0.18f, 0.12f);   
                lgp.standardDarkLevel = new Color(0.22f, 0.01f, 0.01f);
            }
            catch (System.Exception ) {  }
        }
    }

    
    [HarmonyPatch(typeof(BaseGameManager), "PrepareLevelGenerationData")]
    public class PatchMooF1Terrain
    {
        static void Prefix()
        {
            try
            {
                if (!Plugin.MooF1Active) return;
                var bgm = Singleton<BaseGameManager>.Instance;
                if (bgm == null) return;
                var lgp = bgm.levelObject;
                if (lgp == null) return;
                
                RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.10f);
                lgp.standardLightStrength = 0;   
                lgp.standardLightColor = new Color(0.32f, 0.32f, 0.38f);
                lgp.standardDarkLevel = new Color(0.05f, 0.05f, 0.10f);   

                
                var w = lgp.hallWallTexs; var f = lgp.hallFloorTexs; var c = lgp.hallCeilingTexs;
                lgp.hallWallTexs = Plugin.ShuffleArr(f);
                lgp.hallFloorTexs = Plugin.ShuffleArr(c);
                lgp.hallCeilingTexs = Plugin.ShuffleArr(w);
                if (lgp.hallLights != null && lgp.hallLights.Length > 0)
                    lgp.hallLights = Plugin.ShuffleArr(lgp.hallLights);

                
                lgp.minSize = new IntVector2(UnityEngine.Random.Range(16, 40), UnityEngine.Random.Range(16, 40));
                lgp.maxSize = new IntVector2(lgp.minSize.x + UnityEngine.Random.Range(30, 70), lgp.minSize.z + UnityEngine.Random.Range(30, 70));
                lgp.minPlots = UnityEngine.Random.Range(3, 7);
                lgp.maxPlots = Math.Max(lgp.minPlots + 1, UnityEngine.Random.Range(7, 12));
                lgp.minPlotSize = UnityEngine.Random.Range(3, 9);
                lgp.outerEdgeBuffer = UnityEngine.Random.Range(2, 7);
                lgp.minHallsToRemove = UnityEngine.Random.Range(1, 4);
                lgp.maxHallsToRemove = Math.Max(lgp.minHallsToRemove, UnityEngine.Random.Range(4, 7));
                lgp.minReplacementHalls = UnityEngine.Random.Range(1, 4);
                lgp.maxReplacementHalls = Math.Max(lgp.minReplacementHalls, UnityEngine.Random.Range(3, 6));
                lgp.bridgeTurnChance = UnityEngine.Random.Range(0, 8);
                lgp.additionTurnChance = UnityEngine.Random.Range(1, 9);
                lgp.maxLightDistance = UnityEngine.Random.Range(4, 10);
                lgp.minSpecialRooms = UnityEngine.Random.Range(0, 2);
                lgp.maxSpecialRooms = Math.Max(lgp.minSpecialRooms, UnityEngine.Random.Range(2, 4));
                lgp.specialRoomsStickToEdge = (UnityEngine.Random.Range(0, 2) == 0);
                lgp.extraDoorChance = (float)(UnityEngine.Random.Range(0, 40)) / 100f;

                
                if (lgp.potentialSpecialRooms != null && lgp.potentialSpecialRooms.Length > 0)
                    lgp.potentialSpecialRooms = Plugin.ShuffleArr(lgp.potentialSpecialRooms);
                if (lgp.potentialPrePlotSpecialHalls != null && lgp.potentialPrePlotSpecialHalls.Length > 0)
                    lgp.potentialPrePlotSpecialHalls = Plugin.ShuffleArr(lgp.potentialPrePlotSpecialHalls);
                if (lgp.potentialPostPlotSpecialHalls != null && lgp.potentialPostPlotSpecialHalls.Length > 0)
                    lgp.potentialPostPlotSpecialHalls = Plugin.ShuffleArr(lgp.potentialPostPlotSpecialHalls);
                if (lgp.specialHallBuilders != null && lgp.specialHallBuilders.Length > 0)
                    lgp.specialHallBuilders = Plugin.ShuffleArr(lgp.specialHallBuilders);
                if (lgp.potentialStructures != null && lgp.potentialStructures.Length > 0)
                    lgp.potentialStructures = Plugin.ShuffleArr(lgp.potentialStructures);
                if (lgp.roomGroup != null)
                    foreach (var rg in lgp.roomGroup)
                    {
                        if (rg == null) continue;
                        if (rg.potentialRooms != null && rg.potentialRooms.Length > 0)
                            rg.potentialRooms = Plugin.ShuffleArr(rg.potentialRooms);
                        if (rg.wallTexture != null && rg.wallTexture.Length > 0)
                            rg.wallTexture = Plugin.ShuffleArr(rg.wallTexture);
                        if (rg.floorTexture != null && rg.floorTexture.Length > 0)
                            rg.floorTexture = Plugin.ShuffleArr(rg.floorTexture);
                        if (rg.ceilingTexture != null && rg.ceilingTexture.Length > 0)
                            rg.ceilingTexture = Plugin.ShuffleArr(rg.ceilingTexture);
                        if (rg.light != null && rg.light.Length > 0)
                            rg.light = Plugin.ShuffleArr(rg.light);
                        rg.stickToHallChance = (float)(UnityEngine.Random.Range(0, 100)) / 100f;
                        rg.centerWeightMultiplier = (float)(UnityEngine.Random.Range(0, 40) + 5) / 10f;
                        rg.minRooms = UnityEngine.Random.Range(1, 5);
                        rg.maxRooms = Math.Max(rg.minRooms, UnityEngine.Random.Range(4, 9));
                        
                        
                        string mgn = (rg.name ?? "").ToLowerInvariant();
                        if (mgn.Contains("class") || mgn.Contains("lesson") || mgn.Contains("教室"))
                        {
                            rg.minRooms = Math.Max(4, rg.minRooms);
                            rg.maxRooms = Math.Max(rg.minRooms, rg.maxRooms);
                        }
                    }
            }
            catch (System.Exception ) {  }
        }
    }

    
    [HarmonyPatch(typeof(BaseGameManager), "CollectNotebooks")]
    public class PatchNotebookCollectRestart
    {
        static void Postfix(BaseGameManager __instance)
        {
            try
            {
                int fn = (__instance != null) ? __instance.FoundNotebooks : -1;
                int total = (__instance != null && __instance.Ec != null) ? __instance.Ec.notebookTotal : -1;
                string mgr = (__instance != null) ? __instance.GetType().Name : "null";
                string scene = (Singleton<CoreGameManager>.Instance != null && Singleton<CoreGameManager>.Instance.sceneObject != null)
                    ? Singleton<CoreGameManager>.Instance.sceneObject.name : "none";
                Plugin.SilentLog($"[MooExit] Entry: MooF1Active={Plugin.MooF1Active} F1Trig={Plugin.F1RestartTriggered} MooPhase={Plugin.MooPhase} fn={fn} total={total} mgr={mgr} scene={scene}");
                
                bool reached = fn >= 4 || (total > 0 && fn >= total);
                if (Plugin.MooF1Active && !Plugin.F1RestartTriggered && __instance != null && reached)
                {
                    Plugin.SilentLog($"[MooExit] >>> TRIGGER QUIT (FoundNotebooks={fn}, total={total})");
                    Plugin.F1RestartTriggered = true;
                    Plugin.MooSetFlag();      
                    Application.Quit();
                }
            }
            catch (System.Exception e)
            {
                Plugin.Log?.LogError($"[MooExit] ERROR: {e}");
            }
        }
    }

    
    [HarmonyPatch(typeof(HudManager), "UpdateNotebookText")]
    public class PatchMooHudNotebook
    {
        static void Postfix(HudManager __instance, int textVal)
        {
            try
            {
                
                if (textVal >= 0) Plugin.MooNotebookSlot = textVal;
                if (!Plugin.MooF1Active) return;
                if (__instance == null) return;
                var fld = typeof(HudManager).GetField("textBox", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (fld == null) return;
                var arr = fld.GetValue(__instance) as Array;
                if (arr == null || textVal < 0 || textVal >= arr.Length) return;
                var el = arr.GetValue(textVal);
                if (el == null) return;
                var prop = el.GetType().GetProperty("text");
                if (prop != null) prop.SetValue(el, "");
            }
            catch (System.Exception ) {  }
        }
    }

    
    
    [HarmonyPatch(typeof(MainGameManager), "CreateHappyBaldi")]
    public class PatchMooNoHappyBaldiMain
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            bool allow = !(Plugin.MooPhase == 1 || Plugin.MooPhase == 2 || Plugin.MooF1Active || Plugin.MooRedWhiteActive);
            if (!allow)
                Plugin.SilentLog($"[Moo] CreateHappyBaldi blocked: MooPhase={Plugin.MooPhase} MooF1Active={Plugin.MooF1Active} MooRedWhiteActive={Plugin.MooRedWhiteActive}");
            return allow;
        }
    }
    [HarmonyPatch(typeof(EndlessGameManager), "CreateHappyBaldi")]
    public class PatchMooNoHappyBaldiEndless
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            bool allow = !(Plugin.MooPhase == 1 || Plugin.MooPhase == 2 || Plugin.MooF1Active || Plugin.MooRedWhiteActive);
            if (!allow)
                Plugin.SilentLog($"[Moo] CreateHappyBaldi blocked: MooPhase={Plugin.MooPhase} MooF1Active={Plugin.MooF1Active} MooRedWhiteActive={Plugin.MooRedWhiteActive}");
            return allow;
        }
    }

	    
	
	
	[HarmonyPatch(typeof(Material), "SetFloat", new System.Type[] { typeof(string), typeof(float) })]
	public class PatchMaterialSetFloatCullMode
	{
		[HarmonyPrefix]
		static bool Prefix(Material __instance, string name)
		{
			if (name == "_CullMode" && !__instance.HasProperty("_CullMode"))
				return false;
			return true;
		}
	}

	
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public class PatchMooCredits
    {
        static void Postfix()
        {
            try
            {
                
                
                
                
                Plugin.factoryPlanRolled = false;
                Plugin.keySpawnedThisRun = false;
                Plugin.keySpawnAttemptDone = false;
                Plugin.key99SpawnedThisRun = false;
                if (!Plugin.MooCreditsPending) return;
                Plugin.MooCreditsPending = false; 
                try { AchievementHelper.UnlockAchievement("milk_moocredit"); } catch (System.Exception) { }   
                var go = new GameObject("MooCreditsGUI");
                UnityEngine.Object.DontDestroyOnLoad(go);
                go.AddComponent<MooCreditsGUI>().Begin();
                
            }
            catch (System.Exception ) {  }
        }
    }

    
    public class MooCreditsGUI : MonoBehaviour
    {
        private static readonly string[] CreditsLines = new string[]
        {
            "Baldi's Basics Milk In Milk And Milk Than Milk Pack",
            "(BBMIMAMTMP)",
            "",
            "Author: Ruin321",
            "",
            "Special Thanks to:",
            "Eilmetion",
            "G31-L",
            "NightmaresXD",
            "ShrimpXD",
            "Baldi's Basics Mod Maker",
            "IceMelon",
            "CrazyWorld",
            "99",
            "Mystman12",
            "BepInEx Development Team",
            "FishAudio",
            "Ganaisthere",
            "XTR",
            "",
            "Map Creation:",
            "ShrimpXD",
            "XTR",
            "CrazyWorld",
            "Baldi's Basics Mod Maker",
            "",
            "Poster Design:",
            "ShrimpXD",
            "Me (Ruin321 - and my brain and hands)",
            "Baldi's Basics Mod Maker",
            "IceMelon",
            "CrazyWorld",
            "",
            "Voice Acting:",
            "ShrimpXD",
            "FishAudio",
            "",
            "Special Thanks:",
            "Eilmetion",
            "G31-L",
            "NightmaresXD",
            "ShrimpXD",
            "",
            "And a huge thanks to the one who helped find a ton of bugs:",
            "Ganaisthere",
            "",
            "Oh, and also thanks to all the testers out there - appreciate you guys.",
            "",
            "Oh yeah, and one more thing...",
            "",
            "The biggest thanks of all goes to...",
            "the one who's playing this mod right now...",
            "",
            "You!",
            "",
            "Thanks for supporting this mod~"
        };
        private bool running = false;
        private int phase = 0;      
        private float elapsed = 0f;
        private float flash = 0f;
        
        private const string FinalText = "Milk has been cleared from all effects.";
        
        private const float CharSpeed = 0.05f;
        private const float HoldAfterReveal = 2f;
        private const float FadeDuration = 2f;
        
        private float scrollDur = 34f;
        private const float FallbackScrollDur = 34f;
        private const float LineH = 46f;
        private const float CreditsMusicPitch = 1f; 
        private Font cachedGuiFont = null;            
        private bool guiFontResolved = false;        

        
        
        private Font ResolveGameFont()
        {
            if (cachedGuiFont != null) return cachedGuiFont;
            if (guiFontResolved) return null; 
            try
            {
                
                try
                {
                    var tmp = BaldiFonts.ComicSans24.FontAsset();
                    if (tmp != null)
                    {
                        var fi = tmp.GetType().GetField("sourceFontFile",
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (fi == null)
                            fi = tmp.GetType().GetField("m_SourceFontFile",
                                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (fi != null)
                        {
                            var f = fi.GetValue(tmp) as Font;
                            if (f != null) { cachedGuiFont = f; return cachedGuiFont; }
                        }
                    }
                }
                catch (System.Exception) { }

                var all = UnityEngine.Object.FindObjectsOfType<TextMesh>();
                if (all != null)
                {
                    foreach (var tm in all)
                    {
                        if (tm != null && tm.font != null) { cachedGuiFont = tm.font; break; }
                    }
                }
                if (cachedGuiFont == null)
                {
                    
                    
                    guiFontResolved = true;
                }
            }
            catch (System.Exception) { }
            return cachedGuiFont;
        }

        public void Begin()
        {
            running = true;
            scrollDur = FallbackScrollDur;
            
            try
            {
                var mi = Singleton<MusicManager>.Instance;
                if (mi != null) mi.StopMidi();
            }
            catch (System.Exception) { }
            try
            {
                CoreGameManager cgm = Singleton<CoreGameManager>.Instance;
                if (cgm != null)
                {
                    if (cgm.musicMan != null) cgm.musicMan.FlushQueue(true);
                    if (cgm.audMan != null) cgm.audMan.FlushQueue(true);
                }
            }
            catch (System.Exception) { }

            
            
            try
            {
                AudioClip creditsClip = AssetLoader.AudioClipFromMod(Plugin.Instance, "waiting.wav");
                if (creditsClip != null && creditsClip.length > 1f)
                {
                    scrollDur = creditsClip.length;
                    var bgmGo = new GameObject("MooCreditsBGM");
                    UnityEngine.Object.DontDestroyOnLoad(bgmGo);
                    var src = bgmGo.AddComponent<AudioSource>();
                    src.clip = creditsClip;
                    src.spatialBlend = 0f; 
                    src.loop = false;
                    src.playOnAwake = false;
                    src.volume = 1f;
                    src.pitch = CreditsMusicPitch; 
                    Plugin.RouteToMixer(src, Plugin.MilkMixerRoute.Music);
                    src.Play();
                    
                    try { Plugin.SetWindowTitle("Now Playing: waiting --Eilmetion"); } catch (System.Exception) { }
                }
                else
                {
                    
                    try
                    {
                        AudioClip mp3Clip = AssetLoader.AudioClipFromMod(Plugin.Instance, "waiting.mp3");
                        if (mp3Clip != null && mp3Clip.length > 1f)
                        {
                            scrollDur = mp3Clip.length;
                            var bgmGo = new GameObject("MooCreditsBGM");
                            UnityEngine.Object.DontDestroyOnLoad(bgmGo);
                            var src = bgmGo.AddComponent<AudioSource>();
                            src.clip = mp3Clip;
                            src.spatialBlend = 0f;
                            src.loop = false;
                            src.playOnAwake = false;
                            src.volume = 1f;
                            src.pitch = CreditsMusicPitch;
                            Plugin.RouteToMixer(src, Plugin.MilkMixerRoute.Music);
                            src.Play();
                            try { Plugin.SetWindowTitle("Now Playing: waiting --Eilmetion"); } catch (System.Exception) { }
                        }
                    }
                    catch (System.Exception) { }
                }
            }
            catch (System.Exception ) {  }
            StartCoroutine(Driver());
        }

        private System.Collections.IEnumerator Driver()
        {
            
            elapsed = 0f; phase = 0;
            while (elapsed < scrollDur) { elapsed += Time.unscaledDeltaTime; yield return null; }

            
            phase = 1; elapsed = 0f;
            while (elapsed < FadeDuration) { elapsed += Time.unscaledDeltaTime; yield return null; }

            
            phase = 2; elapsed = 0f;
            float revealDur = FinalText.Length * CharSpeed;
            while (elapsed < revealDur + HoldAfterReveal) { elapsed += Time.unscaledDeltaTime; yield return null; }

            
            phase = 3; elapsed = 0f;
            while (elapsed < FadeDuration) { elapsed += Time.unscaledDeltaTime; yield return null; }

            
            phase = 4; flash = 0f;
            while (flash < 1.3f) { flash += Time.unscaledDeltaTime; yield return null; }

            
            try { Plugin.MooClearFlag(); } catch (System.Exception) { }
            Plugin.UnloadMilkDll();
            Application.Quit();
        }

        private void OnGUI()
        {
            if (!running) return;
            GUI.depth = int.MinValue;

            if (phase == 0)
            {
                
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture, ScaleMode.StretchToFill);
                GUI.color = Color.white;

                var style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = Mathf.Max(16, Screen.width / 42);
                style.normal.textColor = Color.white;
                Font gf = ResolveGameFont();
                if (gf != null) { style.font = gf; style.fontStyle = FontStyle.Bold; }
                float totalH = CreditsLines.Length * LineH + 200f;
                float y = Screen.height - (elapsed / scrollDur) * (Screen.height + totalH) + 80f;
                for (int i = 0; i < CreditsLines.Length; i++)
                {
                    if (string.IsNullOrEmpty(CreditsLines[i])) { y += LineH * 0.6f; continue; }
                    GUI.Label(new Rect(0, y, Screen.width, LineH), CreditsLines[i], style);
                    y += LineH;
                }
            }
            else if (phase == 1)
            {
                
                float t = Mathf.Clamp01(elapsed / FadeDuration);
                Color bg = Color.Lerp(Color.black, Color.white, t);
                GUI.color = bg;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture, ScaleMode.StretchToFill);
                GUI.color = Color.white;
            }
            else if (phase == 2)
            {
                
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture, ScaleMode.StretchToFill);

                float revealDur = FinalText.Length * CharSpeed;
                int chars = Mathf.Clamp((int)(elapsed / CharSpeed), 0, FinalText.Length);
                string shown = FinalText.Substring(0, chars);

                var style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = Mathf.Max(24, Screen.width / 28);
                style.normal.textColor = Color.black;
                Font gf = ResolveGameFont();
                if (gf != null) { style.font = gf; style.fontStyle = FontStyle.Bold; }
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), shown, style);
            }
            else if (phase == 3)
            {
                
                float t = Mathf.Clamp01(elapsed / FadeDuration);
                Color bg = Color.Lerp(Color.white, Color.black, t);
                GUI.color = bg;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture, ScaleMode.StretchToFill);

                
                float textAlpha = Mathf.Lerp(1f, 0f, t);
                var style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = Mathf.Max(24, Screen.width / 28);
                style.normal.textColor = new Color(0f, 0f, 0f, textAlpha);
                Font gf = ResolveGameFont();
                if (gf != null) { style.font = gf; style.fontStyle = FontStyle.Bold; }
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), FinalText, style);
            }
            else if (phase == 4)
            {
                
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture, ScaleMode.StretchToFill);
                var style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = Mathf.Max(64, Screen.width / 12);
                style.normal.textColor = (Mathf.FloorToInt(flash * 8f) % 2 == 0) ? Color.white : Color.red;
                Font gf2 = ResolveGameFont();
                if (gf2 != null) { style.font = gf2; style.fontStyle = FontStyle.Bold; }
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "02589", style);
            }
        }
    }
}
