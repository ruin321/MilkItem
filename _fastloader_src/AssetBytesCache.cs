using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using UnityEngine;

namespace FastLoader
{
    // TextureFromFile 磁盘字节缓存：按 (绝对路径, 最后修改时间) 命中。
    // 文件在磁盘上被改动 → mtime 变化 → 自动失效重新读取，热重载不受影响。
    // 只缓存字节，每次调用仍解码出全新的 Texture2D，不共享 Unity 对象实例。
    public static class AssetBytesCache
    {
        private struct CachedFile
        {
            public long mtime;
            public byte[] bytes;
        }

        private static readonly Dictionary<string, CachedFile> cache = new Dictionary<string, CachedFile>();
        private static readonly object cacheLock = new object();

        public static bool TryGet(string path, long mtime, out byte[] bytes)
        {
            lock (cacheLock)
            {
                if (cache.TryGetValue(path, out CachedFile c) && c.mtime == mtime)
                {
                    bytes = c.bytes;
                    return true;
                }
            }
            bytes = null;
            return false;
        }

        public static void Put(string path, long mtime, byte[] bytes)
        {
            lock (cacheLock)
            {
                cache[path] = new CachedFile { mtime = mtime, bytes = bytes };
            }
        }

        public static void Flush()
        {
            lock (cacheLock)
            {
                cache.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(AssetLoader), "TextureFromFile", new Type[] { typeof(string) })]
    public static class AssetLoaderPatches
    {
        [HarmonyPrefix]
        private static bool FastTextureFromFile(string path, ref Texture2D __result)
        {
            if (!FastLoaderPlugin.CurrentAssetCache)
            {
                return true;
            }
            try
            {
                FileInfo fi = new FileInfo(path);
                if (!fi.Exists)
                {
                    return true; // 交回原版抛出 FileNotFoundException
                }
                long mtime = fi.LastWriteTimeUtc.Ticks;
                if (!AssetBytesCache.TryGet(path, mtime, out byte[] bytes))
                {
                    bytes = File.ReadAllBytes(path);
                    AssetBytesCache.Put(path, mtime, bytes);
                }
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                ImageConversion.LoadImage(tex, bytes);
                tex.filterMode = FilterMode.Point;
                tex.name = Path.GetFileNameWithoutExtension(path);
                __result = tex;
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
