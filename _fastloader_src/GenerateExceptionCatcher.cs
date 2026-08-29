using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;

namespace FastLoader
{
    [HarmonyPatch]
    public static class GenerateExceptionCatcher
    {
        private static MethodBase FindMoveNext(Type owner, string coroutineName)
        {
            Type[] nestedTypes = owner.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
            foreach (Type type in nestedTypes)
            {
                if (type.Name.Contains("<" + coroutineName + ">"))
                {
                    MethodInfo method = type.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (method != null)
                    {
                        return method;
                    }
                }
            }
            return null;
        }

        private static IEnumerable<MethodBase> TargetMethods()
        {
            List<MethodBase> list = new List<MethodBase>();
            MethodBase methodBase = FindMoveNext(typeof(LevelGenerator), "Generate");
            if (methodBase != null)
            {
                list.Add(methodBase);
            }
            else
            {
                FastLoaderPlugin.Log.LogWarning("[FastLoader] Could not locate LevelGenerator.Generate state machine MoveNext");
            }
            MethodBase methodBase2 = FindMoveNext(typeof(LevelLoader), "Load");
            if (methodBase2 != null)
            {
                list.Add(methodBase2);
            }
            return list;
        }

        private static Exception Finalizer(Exception __exception, MethodBase __originalMethod)
        {
            if (__exception == null)
            {
                return null;
            }
            LevelBuilderPatches.MarkFaulted();
            FastLoaderPlugin.Log.LogError("[FastLoader] ================ GENERATION EXCEPTION ================\n  Method : " + __originalMethod.DeclaringType?.FullName + "." + __originalMethod.Name + "\n  Level  : " + LevelBuilderPatches.LevelName + "\n  Progress: " + LevelBuilderPatches.Percent.ToString("F2") + "%  (step " + LevelBuilderPatches.Steps + ")\n  Stage  : " + LevelBuilderPatches.Stage + "\n  Exception:\n" + __exception + "\n[FastLoader] ======================================================");
            return __exception;
        }
    }
}
