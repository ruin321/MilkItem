# -*- coding: utf-8 -*-
"""把牛奶汽水图标 + 喷雾贴图染色成淡蓝色，生成无糖牛奶汽水(Diet MilkSoda)版本。
源图与输出都直接放在游戏的 Modded/com.milk.item/ 下，C# 用 AssetLoader 按文件名加载。
淡蓝色：压制红色、略降/持平绿色、抬高蓝色，让原来的白色罐体/喷雾整体偏浅蓝。
"""
import io, os
from PIL import Image

MOD = r"D:/steam/steamapps/common/Baldi's Basics Plus/BALDI_Data/StreamingAssets/Modded/com.milk.item"

# (源文件, 输出文件)
MAP = [
    ("MilkSodaIcon_Small.png", "DietMilkSodaIcon_Small.png"),
    ("MilkSodaIcon_Large.png", "DietMilkSodaIcon_Large.png"),
    ("MilkSodaSpray.png", "DietMilkSodaSpray.png"),
]

def tint(r, g, b):
    # 淡蓝色配方：白→浅蓝(约 198,217,255)
    nr = int(min(255, r * 0.78))
    ng = int(min(255, g * 0.85))
    nb = int(min(255, b + (255 - b) * 0.0 + 55))  # 提高蓝色
    return nr, ng, nb

for src, dst in MAP:
    sp = os.path.join(MOD, src)
    dp = os.path.join(MOD, dst)
    if not os.path.exists(sp):
        print("missing source:", sp)
        continue
    img = Image.open(sp).convert("RGBA")
    px = img.load()
    for y in range(img.height):
        for x in range(img.width):
            r, g, b, a = px[x, y]
            if a > 0:
                px[x, y] = (*tint(r, g, b), a)
    img.save(dp)
    print("wrote:", dst, img.size)

print("done")