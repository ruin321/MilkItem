# -*- coding: utf-8 -*-
"""为狂暴牛（StampedeCow）生成独立的"红温"贴图。

读取 npc/cow/PolishCow_0~20.png，把每张标染成红温（愤怒）色调：
红通道保留（略增），绿/蓝通道大幅压暗 -> 白色变红、黑色保留，整体"红温"观感。
输出同名 StampedeCow_0~20.png，放在独立的 npc/cow/redhot/ 目录下，
供 StampedeCow.cs 用 AssetLoader.SpriteFromMod 独立加载（不影响普通波兰牛）。
"""
import os
from PIL import Image

SRC_DIR = r"D:\steam\steamapps\common\Baldi's Basics Plus\BALDI_Data\StreamingAssets\Modded\com.milk.item\npc\cow"
OUT_DIR = os.path.join(SRC_DIR, "redhot")

# 红温倍率：R 略增，G/B 适中压暗（轻一点的红调，别太辣眼）
R_K = 1.05
G_K = 0.62
B_K = 0.55


def red_hot_pixel(r, g, b):
    r = int(min(255, r * R_K))
    g = int(g * G_K)
    b = int(b * B_K)
    return r, g, b


def main():
    os.makedirs(OUT_DIR, exist_ok=True)
    out = []
    for i in range(21):
        src = os.path.join(SRC_DIR, "PolishCow_%d.png" % i)
        dst = os.path.join(OUT_DIR, "StampedeCow_%d.png" % i)
        if not os.path.exists(src):
            out.append("MISSING: %s" % src)
            continue
        img = Image.open(src).convert("RGBA")
        px = img.load()
        w, h = img.size
        for y in range(h):
            for x in range(w):
                r, g, b, a = px[x, y]
                if a == 0:
                    continue
                r, g, b = red_hot_pixel(r, g, b)
                px[x, y] = (r, g, b, a)
        img.save(dst)
        out.append("OK: %s (%dx%d)" % (os.path.basename(dst), w, h))
    print("\n".join(out))
    print("Done. Generated %d red-hot cow textures." % len([o for o in out if o.startswith("OK")]))


if __name__ == "__main__":
    main()