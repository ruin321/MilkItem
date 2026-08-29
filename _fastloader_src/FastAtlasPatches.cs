using HarmonyLib;
using UnityEngine;

namespace FastLoader
{
    // 用批量 GetPixels 替换 MaterialModifier.GetColorsForTileTexture 的逐像素 GetPixel 采样。
    // 语义完全等价（同一套 floor 点采样 + 越界返回 (0,0,0,0)），但快一个数量级，
    // 牛奶工厂等重贴图楼层每层能省几百毫秒。
    public static class FastTextureSampler
    {
        public static Color[] Sample(Texture2D toCopy, int size)
        {
            Color[] src = toCopy.GetPixels(0);
            int srcW = toCopy.width;
            int srcH = toCopy.height;
            float scale = (float)srcW / (float)size;
            Color[] dst = new Color[size * size];
            Color clear = new Color(0f, 0f, 0f, 0f);
            for (int i = 0; i < size; i++)
            {
                int sy = Mathf.FloorToInt((float)i * scale);
                bool rowOk = sy < srcH;
                int rowBase = sy * srcW;
                for (int j = 0; j < size; j++)
                {
                    int sx = Mathf.FloorToInt((float)j * scale);
                    dst[i * size + j] = (sx < srcW && rowOk) ? src[rowBase + sx] : clear;
                }
            }
            return dst;
        }
    }

    [HarmonyPatch(typeof(MaterialModifier), "GetColorsForTileTexture")]
    public static class MaterialModifierPatches
    {
        [HarmonyPrefix]
        private static bool FastGetColors(Texture2D toCopy, int size, ref Color[] __result)
        {
            try
            {
                __result = FastTextureSampler.Sample(toCopy, size);
                return false;
            }
            catch
            {
                // 纹理不可读等异常时回退原版逻辑
                return true;
            }
        }
    }
}
