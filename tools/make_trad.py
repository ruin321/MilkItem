# -*- coding: utf-8 -*-
"""把 Localization.Chinese.json（简体）转换为繁体中文版。
注意：zhconv 只支持 'zh-hant'（标准繁体）；'zh-hant-tw'/'zh-hant-hk' 会静默返回原文。
只转换 item.value 字段，key 原样保留，保持 2 空格缩进与 \n 转义格式。
"""
import json, io
from zhconv import convert

SRC = r"E:/bbplusbot/MilkItem/Localization.Chinese.json"
DST = r"E:/bbplusbot/MilkItem/Localization.ChineseTraditional.json"

with io.open(SRC, "r", encoding="utf-8-sig") as f:
    data = json.load(f)

items = data.get("items", [])
converted = 0
for it in items:
    if isinstance(it, dict) and "value" in it and it["value"]:
        v = convert(it["value"], "zh-hant")
        # zhconv 后处理：修正旧字形/台湾常用词（无争议项）
        v = v.replace("爲", "為").replace("着", "著").replace("幾率", "機率")
        it["value"] = v
        converted += 1

with io.open(DST, "w", encoding="utf-8", newline="\n") as f:
    json.dump(data, f, ensure_ascii=False, indent=2)
    f.write("\n")

# 校验
with io.open(DST, "r", encoding="utf-8-sig") as f:
    back = json.load(f)
print("items:", len(back.get("items", [])), "| converted:", converted)
assert len(back.get("items", [])) == len(items)
print("json valid, keys:", len(back.get("items", [])))
