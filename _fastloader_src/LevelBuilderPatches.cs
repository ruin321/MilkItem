using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace FastLoader
{
    [HarmonyPatch]
    public static class LevelBuilderPatches
    {
        private static int frameShouldEndCount;
        private static int lastLoggedWholePercent = -1;
        private static bool generationActive;
        private static bool cullingPhase; // Finalizing/occlusion-culling 阶段：放行原版 FrameShouldEnd 实现跨帧渲染
        private static string currentLevelName = "?";
        private static DateTime generationStartTime;
        private static int estimatedTotalSteps = 0;
        private static int lastCompletedSteps = 0;

        private const int HARD_TIMEOUT_SECONDS_BASE = 30;
        private const int HARD_TIMEOUT_SECONDS_FAST = 5;
        private const int STALL_DUMP_AT = 8000;
        private static bool roomInfoDumped;

        // 生效的硬超时秒数：开了「Skip 30s Wait」就只等 ~5 秒强制完成，不再让玩家干等 30 秒。
        private static int EffectiveHardTimeout =>
            (FastLoaderPlugin.SkipWait != null && FastLoaderPlugin.SkipWait.Value) ? HARD_TIMEOUT_SECONDS_FAST : HARD_TIMEOUT_SECONDS_BASE;

        public static bool GenerationFaulted;

        public static int Steps => frameShouldEndCount;
        public static string LevelName => currentLevelName;
        public static float Percent => CurrentPercent();
        public static string Stage => CurrentStage();
        public static bool Active => generationActive;

        public static void MarkFaulted()
        {
            GenerationFaulted = true;
        }

        [HarmonyPatch(typeof(LevelBuilder), "StartGenerate")]
        [HarmonyPrefix]
        private static void OnStartGenerate(LevelBuilder __instance)
        {
            frameShouldEndCount = 0;
            lastLoggedWholePercent = -1;
            generationActive = true;
            cullingPhase = false;
            GenerationFaulted = false;
            roomInfoDumped = false;
            generationStartTime = DateTime.Now;
            try
            {
                SceneObject val = Singleton<CoreGameManager>.Instance?.sceneObject;
                object obj = null;
                if (val != null && val.levelObject != null)
                {
                    obj = val.levelObject.name;
                }
                if (obj == null)
                {
                    obj = (val != null ? val.name : null) ?? ((__instance is LevelLoader) ? "Custom/FieldTrip" : "Procedural");
                }
                currentLevelName = (string)obj;
            }
            catch
            {
                currentLevelName = "?";
            }
            if (lastCompletedSteps > 0)
            {
                estimatedTotalSteps = lastCompletedSteps;
            }
            else
            {
                try
                {
                    LevelObject val2 = Singleton<CoreGameManager>.Instance?.sceneObject?.levelObject;
                    if (val2 != null)
                    {
                        int num = Mathf.Max(1, (val2.maxSize.x + val2.minSize.x) / 2);
                        int num2 = Mathf.Max(1, (val2.maxSize.z + val2.minSize.z) / 2);
                        estimatedTotalSteps = num * num2 * 3 + 400;
                    }
                    else
                    {
                        estimatedTotalSteps = 1200;
                    }
                }
                catch
                {
                    estimatedTotalSteps = 1200;
                }
            }
            string text = (__instance is LevelLoader) ? "Custom/FieldTrip" : "Procedural";
            if (FastLoaderPlugin.CurrentShowProgress)
            {
                FastLoaderPlugin.Log.LogInfo("[FastLoader] === Level generation STARTED: " + text + " (" + currentLevelName + ") ===");
            }
        }

        [HarmonyPatch(typeof(LevelBuilder), "FrameShouldEnd")]
        [HarmonyPrefix]
        private static bool FrameShouldEndPrefix()
        {
            // 生成主流程：直接跳过帧限流，一口气跑完
            // 遮挡剔除 finalize 阶段：放行原版逻辑 → 跨帧渲染（99 房等特殊房间依赖，防止单帧阻塞崩溃）
            if (generationActive && !cullingPhase)
            {
                frameShouldEndCount++;
                if (frameShouldEndCount % 25 == 0)
                {
                    ReportProgress();
                }
                if (frameShouldEndCount == STALL_DUMP_AT || (frameShouldEndCount > STALL_DUMP_AT && frameShouldEndCount % 5000 == 0))
                {
                    DumpStallStack();
                }
                return false;
            }
            return true;
        }

        // 进入遮挡剔除计算 → 开启跨帧阶段
        [HarmonyPatch(typeof(CullingManager), "PrepareOcclusionCalculations")]
        [HarmonyPrefix]
        private static void OnCullingPrepare()
        {
            cullingPhase = true;
        }

        // 遮挡剔除结束（SetActive(true) 收尾）→ 恢复快速模式
        [HarmonyPatch(typeof(CullingManager), "SetActive")]
        [HarmonyPrefix]
        private static void OnCullingSetActive()
        {
            cullingPhase = false;
        }

        private static void DumpStallStack()
        {
            try
            {
                StackTrace arg = new StackTrace(fNeedFileInfo: true);
                FastLoaderPlugin.Log.LogError("[FastLoader] ============ STALL DETECTED (step " + frameShouldEndCount + ") ============\n  Level: " + currentLevelName + "\n  This is the REAL call stack -- look for LevelGenerator/StructureBuilder frames:\n" + arg + "\n[FastLoader] ==================================================");
                if (!roomInfoDumped)
                {
                    roomInfoDumped = true;
                    DumpRoomPosterInfo();
                    DumpPlotExpansionInfo();
                }
            }
            catch (Exception ex)
            {
                FastLoaderPlugin.Log.LogWarning("[FastLoader] Stall stack dump failed: " + ex.Message);
            }
        }

        [HarmonyPatch(typeof(LevelBuilder), "Update")]
        [HarmonyPrefix]
        private static bool OnUpdatePrefix(LevelBuilder __instance)
        {
            if (!generationActive)
            {
                return true;
            }
            if (__instance.levelCreated)
            {
                generationActive = false;
                cullingPhase = false;
                lastCompletedSteps = frameShouldEndCount;
                if (FastLoaderPlugin.CurrentShowProgress)
                {
                    FastLoaderPlugin.Log.LogInfo("[FastLoader] === Level generation COMPLETED: " + currentLevelName + " (100.00%, steps=" + frameShouldEndCount + ", taken " + (DateTime.Now - generationStartTime).TotalSeconds.ToString("F1") + "s) ===");
                }
            }
            else
            {
                double totalSeconds = (DateTime.Now - generationStartTime).TotalSeconds;
                if (totalSeconds > EffectiveHardTimeout)
                {
                    generationActive = false;
                    cullingPhase = false;
                    try
                    {
                        FieldInfo field = typeof(LevelBuilder).GetField("levelCreated", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        FieldInfo field2 = typeof(LevelBuilder).GetField("levelInProgress", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        field?.SetValue(__instance, true);
                        field2?.SetValue(__instance, false);
                        CoreGameManager instance = Singleton<CoreGameManager>.Instance;
                        GameCamera obj = (instance != null) ? instance.GetCamera(0) : null;
                        if (obj != null)
                        {
                            obj.StopRendering(false);
                        }
                    }
                    catch (Exception arg)
                    {
                        FastLoaderPlugin.Log.LogError("[FastLoader] Force-finish failed: " + arg);
                    }
                    FastLoaderPlugin.Log.LogError("[FastLoader] GENERATION TIMEOUT after " + totalSeconds.ToString("F0") + "s on '" + currentLevelName + "' at ~" + CurrentPercent().ToString("F2") + "% / step " + frameShouldEndCount + " (stage: " + CurrentStage() + "). " + (GenerationFaulted ? "An exception was already reported above -- THAT is the root cause; the level will be incomplete." : "No exception was caught, so the coroutine is likely stalled waiting on something.") + " Forcing level finish to avoid hard-freeze.");
                    DumpLevelDiagnostics();
                }
            }
            return false;
        }

        private static void DumpRoomPosterInfo()
        {
            try
            {
                EnvironmentController val = UnityEngine.Object.FindObjectOfType<EnvironmentController>();
                if (val == null || val.rooms == null)
                {
                    FastLoaderPlugin.Log.LogWarning("[FastLoader] Room dump: EnvironmentController/rooms unavailable");
                    return;
                }
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("[FastLoader] ---- Room poster diagnostics (ec.rooms.Count = " + val.rooms.Count + ") ----");
                if (val.rooms.Count == 0)
                {
                    stringBuilder.AppendLine("  (no rooms yet -- generation stalled BEFORE rooms were created)");
                }
                int num = 0;
                foreach (RoomController room in val.rooms)
                {
                    if (room == null)
                    {
                        num++;
                        continue;
                    }
                    int num2 = -1;
                    try { num2 = room.potentialPosters?.Count ?? (-1); } catch { }
                    bool flag = false;
                    try { flag = room.HasFreeWall; } catch { }
                    bool flag2 = num2 > 0 && room.posterChance >= 1f;
                    stringBuilder.AppendLine("  [" + num + "] name=" + room.name + " type=" + room.type + " posterChance=" + room.posterChance.ToString("F3") + " potentialPosters=" + num2 + " HasFreeWall=" + flag + (flag2 ? "   <=== SUSPECT (infinite loop)" : ""));
                    num++;
                }
                stringBuilder.Append("[FastLoader] ---------------------------------------------------------------");
                FastLoaderPlugin.Log.LogError(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                FastLoaderPlugin.Log.LogWarning("[FastLoader] Room dump failed: " + ex.Message);
            }
        }

        private static void DumpPlotExpansionInfo()
        {
            try
            {
                LevelBuilder val = UnityEngine.Object.FindObjectOfType<LevelBuilder>();
                if (val == null)
                {
                    FastLoaderPlugin.Log.LogWarning("[FastLoader] Plot dump: LevelBuilder not found");
                    return;
                }
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("[FastLoader] ---- Plot expansion diagnostics (LevelGenerator.cs:326 loop) ----");
                try
                {
                    FieldInfo fieldInfo = AccessTools.Field(typeof(LevelBuilder), "levelSize");
                    if (fieldInfo != null)
                    {
                        IntVector2 val2 = (IntVector2)fieldInfo.GetValue(val);
                        stringBuilder.AppendLine("  levelSize = (" + val2.x + ", " + val2.z + ")");
                    }
                }
                catch { }
                try
                {
                    object obj2 = AccessTools.Field(typeof(LevelBuilder), "ld")?.GetValue(val);
                    if (obj2 != null)
                    {
                        Type type = obj2.GetType();
                        string[] array = new string[16]
                        {
                            "minSize", "maxSize", "minPlots", "maxPlots", "minPlotSize", "outerEdgeBuffer", "minRoomSize", "minHallsToRemove", "maxHallsToRemove", "minReplacementHalls",
                            "maxReplacementHalls", "maxHallAttempts", "deadEndBuffer", "includeBuffers", "fillEmptySpace", "exitCount"
                        };
                        foreach (string text in array)
                        {
                            FieldInfo fieldInfo2 = AccessTools.Field(type, text);
                            if (fieldInfo2 == null)
                            {
                                stringBuilder.AppendLine("  " + text + " = <field not found>");
                                continue;
                            }
                            object value = fieldInfo2.GetValue(obj2);
                            if (value is IntVector2 val3)
                            {
                                stringBuilder.AppendLine("  " + text + " = (" + val3.x + ", " + val3.z + ")");
                            }
                            else
                            {
                                stringBuilder.AppendLine("  " + text + " = " + value);
                            }
                        }
                        try
                        {
                            if (!(AccessTools.Field(type, "roomGroup")?.GetValue(obj2) is IList list))
                            {
                                stringBuilder.AppendLine("  roomGroup = <null or not a list>");
                            }
                            else
                            {
                                stringBuilder.AppendLine("  ---- roomGroup (count = " + list.Count + ") ----");
                                int num = 0;
                                for (int j = 0; j < list.Count; j++)
                                {
                                    object obj3 = list[j];
                                    if (obj3 == null)
                                    {
                                        stringBuilder.AppendLine("    [" + j + "] <null>");
                                        continue;
                                    }
                                    Type type2 = obj3.GetType();
                                    object obj4 = AccessTools.Field(type2, "minRooms")?.GetValue(obj3);
                                    object obj5 = AccessTools.Field(type2, "maxRooms")?.GetValue(obj3);
                                    Array array2 = AccessTools.Field(type2, "potentialRooms")?.GetValue(obj3) as Array;
                                    object obj6 = AccessTools.Field(type2, "stickToHallChance")?.GetValue(obj3);
                                    if (obj5 is int num2)
                                    {
                                        num += num2;
                                    }
                                    string text2 = ((obj5 is int num3 && num3 > 30) ? "   <=== SUSPECT (huge maxRooms)" : "");
                                    stringBuilder.AppendLine(string.Format("    [{0}] minRooms={1} maxRooms={2} potentialRooms={3} stickToHallChance={4}{5}", j, obj4, obj5, (array2 == null) ? "null" : array2.Length.ToString(), obj6, text2));
                                }
                                stringBuilder.AppendLine("  ---- total maxRooms across groups = " + num + " ----");
                            }
                        }
                        catch (Exception ex)
                        {
                            stringBuilder.AppendLine("  roomGroup read failed: " + ex.Message);
                        }
                    }
                    else
                    {
                        stringBuilder.AppendLine("  ld = null");
                    }
                }
                catch (Exception ex2)
                {
                    stringBuilder.AppendLine("  ld read failed: " + ex2.Message);
                }
                stringBuilder.Append("[FastLoader] ----------------------------------------------------------------");
                FastLoaderPlugin.Log.LogError(stringBuilder.ToString());
            }
            catch (Exception ex3)
            {
                FastLoaderPlugin.Log.LogWarning("[FastLoader] Plot dump failed: " + ex3.Message);
            }
        }

        private static void DumpLevelDiagnostics()
        {
            try
            {
                CoreGameManager instance = Singleton<CoreGameManager>.Instance;
                EnvironmentController val = (instance != null) ? instance.GetComponentInChildren<EnvironmentController>() : null;
                if (val == null)
                {
                    val = UnityEngine.Object.FindObjectOfType<EnvironmentController>();
                }
                LevelObject val2 = Singleton<CoreGameManager>.Instance?.sceneObject?.levelObject;
                string text = (val2 != null) ? ("minSize=(" + val2.minSize.x + "," + val2.minSize.z + ") maxSize=(" + val2.maxSize.x + "," + val2.maxSize.z + ") type=" + val2.type) : "levelObject=null";
                string text2 = "EnvironmentController=null";
                if (val != null)
                {
                    int num = -1;
                    try
                    {
                        num = (val.CullingManager != null) ? val.CullingManager.TotalChunks : (-1);
                    }
                    catch { }
                    text2 = "levelSize=(" + val.levelSize.x + "," + val.levelSize.z + ") rooms=" + (val.rooms?.Count ?? (-1)) + " chunks=" + num + " cullingManager=" + ((val.CullingManager == null) ? "NULL" : "ok");
                }
                FastLoaderPlugin.Log.LogError("[FastLoader] Diagnostics -> " + text + " | " + text2);
            }
            catch (Exception ex)
            {
                FastLoaderPlugin.Log.LogWarning("[FastLoader] Diagnostics dump failed: " + ex.Message);
            }
        }

        private static float CurrentPercent()
        {
            if (estimatedTotalSteps <= 0)
            {
                return 0f;
            }
            return Mathf.Clamp((float)frameShouldEndCount / (float)estimatedTotalSteps * 100f, 0f, 99.99f);
        }

        private static string CurrentStage()
        {
            float num = CurrentPercent();
            if (num < 12f) return "Initializing (cells/lighting/map)";
            if (num < 50f) return "Generating rooms";
            if (num < 75f) return "Building structures";
            if (num < 92f) return "Spawning NPCs/items/activities";
            return "Finalizing (occlusion/culling)";
        }

        private static void ReportProgress()
        {
            if (!FastLoaderPlugin.CurrentShowProgress)
            {
                return;
            }
            int num = (int)CurrentPercent();
            if (num != lastLoggedWholePercent)
            {
                lastLoggedWholePercent = num;
                FastLoaderPlugin.Log.LogInfo("[FastLoader] Progress: " + CurrentPercent().ToString("F2") + "% | Stage: " + CurrentStage() + " | Level: " + currentLevelName + " | steps=" + frameShouldEndCount);
            }
        }
    }
}
