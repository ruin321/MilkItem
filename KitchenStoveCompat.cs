using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using PlusStudioLevelLoader;
using UnityEngine;

namespace MilkItem
{
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    public static class KitchenStoveCompat
    {
        private const string AdvAssemblyName = "BaldisBasicsPlusAdvanced";
        private const string FoodRecipeDataTypeName = "BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove.FoodRecipeData";
        private const string ApiManagerTypeName = "BaldisBasicsPlusAdvanced.API.ApiManager";
        private const string KitchenStoveTypeName = "BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove.KitchenStove";

        
        private static Type foodRecipeType;
        private static MethodInfo setRaw;
        private static MethodInfo setCooked;
        private static MethodInfo createStoveRecipe;      
        private static MethodInfo createRecipePoster;     
        private static PropertyInfo rawFoodProp;          
        private static ConstructorInfo recipeCtor;
        private static System.Collections.IList recipeList;  

        public static void Register()
        {
            try
            {
                Assembly advAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == AdvAssemblyName);
                if (advAssembly == null)
                {
                    Plugin.SilentLog("[KitchenStoveCompat] 未安装 BaldisBasicsPlusAdvanced，跳过灶台配方注册");
                    return; 
                }

                if (!ResolveReflectionHandles(advAssembly)) return;

                ItemObject milk = Plugin.MilkItemObject;
                ItemObject apple = FindItem(Items.Apple);
                ItemObject bsoda = FindItem(Items.Bsoda);
                ItemObject dietBsoda = FindItem(Items.DietBsoda);
                ItemObject zesty = FindItem(Items.ZestyBar);
                ItemObject nana = FindItem(Items.NanaPeel);
                ItemObject quarter = FindItem(Items.Quarter);
                ItemObject scissors = FindItem(Items.Scissors);
                ItemObject portal = FindItem(Items.PortalPoster);
                ItemObject wd40 = FindItem(Items.Wd40);

                int before = SafeRecipeCount();

                
                AddRecipe(new ItemObject[] { apple, milk }, new ItemObject[] { Plugin.AppleMilkItemObject });

                
                AddRecipe(new ItemObject[] { bsoda, milk }, new ItemObject[] { Plugin.MilkSodaItemObject });

                
                AddRecipe(new ItemObject[] { dietBsoda, milk }, new ItemObject[] { Plugin.DietMilkSodaItemObject });

                
                AddRecipe(new ItemObject[] { Plugin.DietMilkSodaItemObject, Plugin.DietMilkSodaItemObject },
                    new ItemObject[] { Plugin.MilkSodaItemObject });

                
                AddRecipe(new ItemObject[] { milk }, new ItemObject[] { Plugin.RottenMilkItemObject });

                
                AddRecipe(new ItemObject[] { zesty, milk }, new ItemObject[] { Plugin.ChocolateMilkItemObject });

                
                AddRecipe(new ItemObject[] { milk, milk }, new ItemObject[] { Plugin.CompressedMilkItemObject });

                
                AddRecipe(new ItemObject[] { portal, milk }, new ItemObject[] { Plugin.ReverseMilkItemObject });

                
                AddRecipe(new ItemObject[] { nana, milk }, new ItemObject[] { Plugin.RottenMilkItemObject });

                
                AddRecipe(new ItemObject[] { quarter, milk }, new ItemObject[] { Plugin.QuarterMilkItemObject });

                
                AddRecipe(new ItemObject[] { scissors, milk }, new ItemObject[] { Plugin.WindowMilkItemObject });

                
                AddRecipe(new ItemObject[] { portal, milk }, new ItemObject[] { Plugin.LostBilkItemObject });

                
                AddRecipe(new ItemObject[] { Plugin.MiItemObject, Plugin.LkItemObject },
                    new ItemObject[] { Plugin.MilkItemObject });

                
                AddRecipe(new ItemObject[] { Plugin.RottenMilkItemObject, Plugin.RottenMilkItemObject, Plugin.RottenMilkItemObject, Plugin.RottenMilkItemObject },
                    new ItemObject[] { Plugin.KeyItemObject });

                
                AddRecipe(new ItemObject[] { milk, FindItemByName("Adv_Item_Hammer", "Hammer", "ITM_Hammer") },
                    new ItemObject[] { Plugin.CompressedMilkItemObject });

                
                AddRecipe(new ItemObject[] { FindItem(Items.BusPass), milk },
                    new ItemObject[] { Plugin.BusPassMilkItemObject });

                
                AddRecipe(new ItemObject[] { Plugin.EmptyBucketItemObject, Plugin.EmptyBucketItemObject },
                    new ItemObject[] { FindItem(Items.Quarter) });

                
                AddRecipe(new ItemObject[] { wd40, Plugin.MilkItemObject },
                    new ItemObject[] { Plugin.SilentMilkItemObject });

                
                ItemObject alarmClock = FindItem(Items.AlarmClock);
                AddRecipe(new ItemObject[] { alarmClock, Plugin.MilkItemObject },
                    new ItemObject[] { Plugin.TimeMilkItemObject });

                int after = SafeRecipeCount();
                Plugin.SilentLog($"[KitchenStoveCompat] 灶台配方注册完成，全服配方数 {before} → {after}（本次注册 {after - before} 条）");

                
                RegisterTips(advAssembly);
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.LogWarning($"[KitchenStoveCompat] 注册牛奶配方失败：{ex}");
            }
        }

        
        private static bool ResolveReflectionHandles(Assembly asm)
        {
            try
            {
                foodRecipeType = asm.GetType(FoodRecipeDataTypeName) ?? FuzzyType(asm, "FoodRecipeData");
                Type apiManagerType = asm.GetType(ApiManagerTypeName) ?? FuzzyType(asm, "ApiManager");
                Type kitchenStoveType = asm.GetType(KitchenStoveTypeName) ?? FuzzyType(asm, "KitchenStove");

                if (foodRecipeType == null)
                {
                    Plugin.Log?.LogWarning($"[KitchenStoveCompat] 找不到 FoodRecipeData 类型（BB+Advanced {asm.GetName().Version}，接口可能已变动），配方无法注册");
                    return false;
                }

                setRaw = FindMethod(foodRecipeType, "SetRawFood", new Type[] { typeof(ItemObject[]) },
                    m => m.GetParameters().Length == 1 && typeof(ItemObject[]).IsAssignableFrom(m.GetParameters()[0].ParameterType));
                setCooked = FindMethod(foodRecipeType, "SetCookedFood", new Type[] { typeof(ItemObject[]) },
                    m => m.GetParameters().Length == 1 && typeof(ItemObject[]).IsAssignableFrom(m.GetParameters()[0].ParameterType));
                createRecipePoster = FindMethod(foodRecipeType, "CreateRecipePoster", Type.EmptyTypes, m => m.GetParameters().Length == 0);
                rawFoodProp = foodRecipeType.GetProperty("RawFood", BindingFlags.Public | BindingFlags.Instance)
                    ?? foodRecipeType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .FirstOrDefault(p => p.Name == "RawFood");
                recipeCtor = foodRecipeType.GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(PluginInfo))
                    ?? foodRecipeType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0);

                if (setRaw == null || setCooked == null)
                {
                    Plugin.Log?.LogWarning($"[KitchenStoveCompat] FoodRecipeData 缺少 SetRawFood/SetCookedFood（BB+Advanced {asm.GetName().Version}，接口可能已变动），配方无法注册");
                    return false;
                }

                if (apiManagerType != null)
                {
                    createStoveRecipe = FindMethod(apiManagerType, "CreateKitchenStoveRecipe", new Type[] { foodRecipeType },
                        m => m.IsStatic && m.GetParameters().Length == 1 && foodRecipeType.IsAssignableFrom(m.GetParameters()[0].ParameterType));
                }

                ResolveRecipeList(kitchenStoveType);

                string ver = asm.GetName().Version?.ToString() ?? "?";
                Plugin.SilentLog(
                    $"[KitchenStoveCompat] BB+Advanced {ver} 接口解析：ApiManager注册={(createStoveRecipe != null ? "OK" : "缺失(将直接注入)")}，" +
                    $"直接注入列表={(recipeList != null ? "OK" : "缺失")}，海报={(createRecipePoster != null ? "OK" : "缺失(不影响功能)")}，" +
                    $"构造器={(recipeCtor != null ? "OK" : "缺失")}");
                return true;
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.LogWarning($"[KitchenStoveCompat] 反射解析 BB+Advanced 接口异常：{ex.Message}");
                return false;
            }
        }

        private static Type FuzzyType(Assembly asm, string shortName)
        {
            try
            {
                return asm.GetTypes().FirstOrDefault(t => t != null && t.Name == shortName);
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.FirstOrDefault(t => t != null && t.Name == shortName);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private static MethodInfo FindMethod(Type t, string name, Type[] exactParams, Func<MethodInfo, bool> fuzzy)
        {
            try
            {
                MethodInfo m = t.GetMethod(name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance,
                    null, exactParams, null);
                if (m != null) return m;
            }
            catch (System.Exception) { }
            try
            {
                return t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                    .FirstOrDefault(x => x.Name == name && fuzzy(x));
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        
        private static void ResolveRecipeList(Type kitchenStoveType)
        {
            try
            {
                if (kitchenStoveType != null)
                {
                    PropertyInfo p = kitchenStoveType.GetProperty("RecipeData",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    recipeList = p?.GetValue(null, null) as System.Collections.IList;
                    if (recipeList == null)
                    {
                        foreach (FieldInfo f in kitchenStoveType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                        {
                            if (f.FieldType.IsGenericType
                                && f.FieldType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>)
                                && f.FieldType.GetGenericArguments()[0] == foodRecipeType)
                            {
                                recipeList = f.GetValue(null) as System.Collections.IList;
                                break;
                            }
                        }
                    }
                }
            }
            catch (System.Exception) { recipeList = null; }
        }

        private static int SafeRecipeCount()
        {
            try { return recipeList?.Count ?? -1; }
            catch (System.Exception) { return -1; }
        }

        
        private static void AddRecipe(ItemObject[] raw, ItemObject[] cooked)
        {
            string desc = Describe(raw) + " => " + Describe(cooked);
            try
            {
                if (raw.Any(io => io == null) || cooked.Any(io => io == null))
                {
                    
                    Plugin.Log?.LogWarning($"[KitchenStoveCompat] 配方跳过（原料或产物为空）：{desc}");
                    return;
                }

                
                
                EnsureSprites(raw);
                EnsureSprites(cooked);

                object recipe;
                if (recipeCtor != null && recipeCtor.GetParameters().Length == 1)
                    recipe = recipeCtor.Invoke(new object[] { Plugin.Instance.Info });
                else
                    recipe = Activator.CreateInstance(foodRecipeType);
                if (recipe == null)
                {
                    Plugin.Log?.LogWarning($"[KitchenStoveCompat] 配方跳过（FoodRecipeData 无法构造）：{desc}");
                    return;
                }
                recipe = setRaw.Invoke(recipe, new object[] { raw });
                recipe = setCooked.Invoke(recipe, new object[] { cooked });

                bool ok = false;
                bool apiRejected = false;
                if (createStoveRecipe != null)
                {
                    try
                    {
                        object r = createStoveRecipe.Invoke(null, new object[] { recipe });
                        ok = (r is bool b) && b;
                        apiRejected = !ok;
                    }
                    catch (System.Exception ex)
                    {
                        
                        Plugin.Log?.LogWarning($"[KitchenStoveCompat] ApiManager 注册配方异常（{ex.GetType().Name}: {ex.Message}），改用直接注入：{desc}");
                    }
                }
                if (!ok && !apiRejected)
                {
                    ok = AddDirect(recipe, raw, desc);
                }
                if (!ok)
                {
                    Plugin.Log?.LogWarning($"[KitchenStoveCompat] 配方未能注册：{desc}");
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.LogWarning($"[KitchenStoveCompat] 单条配方注册失败（{desc}）：{ex.Message}");
            }
        }

        
        
        private static bool AddDirect(object recipe, ItemObject[] raw, string desc)
        {
            try
            {
                if (recipeList == null)
                {
                    Plugin.Log?.LogWarning($"[KitchenStoveCompat] 拿不到 KitchenStove.RecipeData，无法直接注入：{desc}");
                    return false;
                }
                if (RecipeExists(raw))
                {
                    
                    Plugin.SilentLog($"[KitchenStoveCompat] 已存在同原料配方，跳过直接注入：{desc}");
                    return true;
                }
                
                try { if (createRecipePoster != null) createRecipePoster.Invoke(recipe, null); } catch (System.Exception) { }
                recipeList.Add(recipe);
                Plugin.SilentLog($"[KitchenStoveCompat] 直接注入配方成功：{desc}");
                return true;
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.LogWarning($"[KitchenStoveCompat] 直接注入配方失败（{desc}）：{ex.Message}");
                return false;
            }
        }

        
        private static bool RecipeExists(ItemObject[] raw)
        {
            try
            {
                if (recipeList == null || rawFoodProp == null) return false;
                foreach (object e in recipeList)
                {
                    if (e == null) continue;
                    ItemObject[] eraw = rawFoodProp.GetValue(e, null) as ItemObject[];
                    if (eraw == null || eraw.Length != raw.Length) continue;
                    var pool = eraw.ToList();
                    bool all = true;
                    foreach (ItemObject io in raw)
                    {
                        int idx = pool.FindIndex(x => ReferenceEquals(x, io));
                        if (idx < 0) { all = false; break; }
                        pool.RemoveAt(idx);
                    }
                    if (all) return true;
                }
            }
            catch (System.Exception) { }
            return false;
        }

        private static Sprite _whiteSprite;

        private static void EnsureSprites(ItemObject[] arr)
        {
            try
            {
                foreach (ItemObject io in arr)
                {
                    if (io == null) continue;
                    if (io.itemSpriteSmall == null)
                        io.itemSpriteSmall = io.itemSpriteLarge ?? WhiteSprite();
                    if (io.itemSpriteLarge == null)
                        io.itemSpriteLarge = io.itemSpriteSmall;
                }
            }
            catch (System.Exception) { }
        }

        private static Sprite WhiteSprite()
        {
            if (_whiteSprite != null) return _whiteSprite;
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one / 2f, 1f);
            return _whiteSprite;
        }

        private static string Describe(ItemObject[] arr)
        {
            return string.Join("+", Array.ConvertAll(arr, x => x == null ? "null" : x.name));
        }

        
        
        
        private static readonly string[] TipsLocalizationKeys =
        {
            "Mu_Tip_0",
            "Mu_Tip_1",
            "Mu_Tip_2",
            "Mu_Tip_3",
            "Mu_Tip_4",
            "Mu_Tip_5",
            "Mu_Tip_6",
            "Mu_Tip_7",
            "Mu_Tip_8",
            "Mu_Tip_9",
        };

        private static void RegisterTips(Assembly advAssembly)
        {
            try
            {
                Type apiManagerType = advAssembly.GetType(ApiManagerTypeName) ?? FuzzyType(advAssembly, "ApiManager");
                if (apiManagerType == null) return;
                MethodInfo addTips = FindMethod(apiManagerType, "AddNewTips",
                    new Type[] { typeof(PluginInfo), typeof(string[]) },
                    m => m.IsStatic && m.GetParameters().Length == 2 && m.GetParameters()[0].ParameterType == typeof(PluginInfo));
                if (addTips == null)
                {
                    Plugin.SilentLog("[KitchenStoveCompat] 未找到 AddNewTips 接口，跳过自定义提示注册（不影响配方）");
                    return;
                }
                addTips.Invoke(null, new object[] { Plugin.Instance.Info, TipsLocalizationKeys });
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.LogWarning($"[KitchenStoveCompat] 注册自定义提示失败：{ex.Message}");
            }
        }

        private static ItemObject FindItem(Items item)
        {
            try
            {
                return ItemMetaStorage.Instance?.FindByEnum(item)?.value;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        
        
        
        
        
        private static ItemObject FindItemByName(params string[] names)
        {
            if (names == null || names.Length == 0) return null;
            var candidates = new System.Collections.Generic.List<string>();
            foreach (string n in names) if (!string.IsNullOrEmpty(n)) candidates.Add(n.ToLowerInvariant());
            if (candidates.Count == 0) return null;

            
            try
            {
                var storage = ItemMetaStorage.Instance;
                if (storage != null)
                {
                    var metas = new System.Collections.Generic.List<object>();
                    try { metas.AddRange(storage.All()); } catch (System.Exception) { }
                    foreach (var meta in metas)
                    {
                        if (meta == null) continue;
                        ItemObject io = null;
                        try { io = meta.GetType().GetProperty("value")?.GetValue(meta, null) as ItemObject; } catch (System.Exception) { }
                        string mk = null;
                        try { mk = meta.GetType().GetField("nameKey")?.GetValue(meta) as string; } catch (System.Exception) { }
                        if (io != null && Matches(io, mk, candidates)) return io;
                    }
                }
            }
            catch (System.Exception) { }

            
            try
            {
                var loader = LevelLoaderPlugin.Instance;
                if (loader != null && loader.itemObjects != null)
                {
                    foreach (var kv in loader.itemObjects)
                    {
                        if (kv.Value == null) continue;
                        if (MatchesName(kv.Key, kv.Value, candidates)) return kv.Value;
                    }
                }
            }
            catch (System.Exception) { }

            return null;
        }

        
        private static bool Matches(ItemObject io, string enumKey, System.Collections.Generic.List<string> candidates)
        {
            string nk = null;
            try { nk = io.GetType().GetProperty("nameKey")?.GetValue(io, null) as string; } catch (System.Exception) { }
            return MatchesName(enumKey, io, candidates) || MatchesName(nk, io, candidates);
        }

        private static bool MatchesName(string key, ItemObject io, System.Collections.Generic.List<string> candidates)
        {
            if (!string.IsNullOrEmpty(key) &&
                candidates.Contains(key.ToLowerInvariant())) return true;
            try
            {
                if (io != null && !string.IsNullOrEmpty(io.name) &&
                    candidates.Contains(io.name.ToLowerInvariant())) return true;
            }
            catch (System.Exception) { }
            return false;
        }
    }
}
