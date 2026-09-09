using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using PlusLevelStudio;
using PlusStudioLevelLoader;
using UnityEngine;

namespace MilkItem
{




    internal static class NewMilks
    {
        private const int BullyCost = 40;
        private const int MatryoshkaCost = 50;
        private const int CannonCost = 70;

        public static void RegisterAll()
        {
            RegisterBullyMilk();
            RegisterMatryoshkaMilk();
            RegisterMilkCannon();




            Plugin.MatryoshkaMilkItemObject1 = MakeVariant<MatryoshkaMilkComponent>(
                Plugin.MatryoshkaMilkEnumName1, Plugin.MatryoshkaMilkEnumName1, Plugin.MatryoshkaMilkEnumName + "_Desc",
                "MatryoshkaMilk_Small.png", "MatryoshkaMilk_Large.png", MatryoshkaCost);
            Plugin.MatryoshkaMilkItemObject2 = MakeVariant<MatryoshkaMilkComponent>(
                Plugin.MatryoshkaMilkEnumName2, Plugin.MatryoshkaMilkEnumName2, Plugin.MatryoshkaMilkEnumName + "_Desc",
                "MatryoshkaMilk_Small.png", "MatryoshkaMilk_Large.png", MatryoshkaCost);

            Plugin.MilkCannonItemObject1 = MakeVariant<MilkCannonComponent>(
                Plugin.MilkCannonEnumName1, Plugin.MilkCannonEnumName1, Plugin.MilkCannonEnumName + "_Desc",
                "MilkCannon_Small.png", "MilkCannon_Large.png", CannonCost);
            Plugin.MilkCannonItemObject2 = MakeVariant<MilkCannonComponent>(
                Plugin.MilkCannonEnumName2, Plugin.MilkCannonEnumName2, Plugin.MilkCannonEnumName + "_Desc",
                "MilkCannon_Small.png", "MilkCannon_Large.png", CannonCost);
            Plugin.MilkCannonItemObject3 = MakeVariant<MilkCannonComponent>(
                Plugin.MilkCannonEnumName3, Plugin.MilkCannonEnumName3, Plugin.MilkCannonEnumName + "_Desc",
                "MilkCannon_Small.png", "MilkCannon_Large.png", CannonCost);
        }

        private static void RegisterBullyMilk()
        {
            try
            {
                if (Plugin.BullyMilkItemObject != null) return;

                Sprite small = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, "BullyMilk_Small.png");
                Sprite large = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 50f, "BullyMilk_Large.png");

                var item = new ItemBuilder(Plugin.Instance.Info)
                    .SetNameAndDescription(Plugin.BullyMilkEnumName, Plugin.BullyMilkEnumName + "_Desc")
                    .SetEnum(Plugin.BullyMilkEnumName)
                    .SetShopPrice(600)
                    .SetGeneratorCost(BullyCost)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(small, large)
                    .SetItemComponent<BullyMilkComponent>()
                    .Build();

                Plugin.Instance.assetMan.Add<ItemObject>(Plugin.BullyMilkEnumName, item);
                LevelLoaderPlugin.Instance.itemObjects.Add(Plugin.BullyMilkEnumName, item);
                LevelStudioPlugin.Instance.selectableShopItems.Add(Plugin.BullyMilkEnumName);
                Plugin.BullyMilkItemObject = item;
            }
            catch (System.Exception) { }
        }

        private static void RegisterMatryoshkaMilk()
        {
            try
            {
                if (Plugin.MatryoshkaMilkItemObject != null) return;

                Sprite small = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, "MatryoshkaMilk_Small.png");
                Sprite large = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 50f, "MatryoshkaMilk_Large.png");

                var item = new ItemBuilder(Plugin.Instance.Info)
                    .SetNameAndDescription(Plugin.MatryoshkaMilkEnumName, Plugin.MatryoshkaMilkEnumName + "_Desc")
                    .SetEnum(Plugin.MatryoshkaMilkEnumName)
                    .SetShopPrice(500)
                    .SetGeneratorCost(MatryoshkaCost)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(small, large)
                    .SetItemComponent<MatryoshkaMilkComponent>()
                    .Build();

                Plugin.Instance.assetMan.Add<ItemObject>(Plugin.MatryoshkaMilkEnumName, item);
                LevelLoaderPlugin.Instance.itemObjects.Add(Plugin.MatryoshkaMilkEnumName, item);
                LevelStudioPlugin.Instance.selectableShopItems.Add(Plugin.MatryoshkaMilkEnumName);
                Plugin.MatryoshkaMilkItemObject = item;
            }
            catch (System.Exception) { }
        }

        private static void RegisterMilkCannon()
        {
            try
            {
                if (Plugin.MilkCannonItemObject != null) return;

                Sprite small = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, "MilkCannon_Small.png");
                Sprite large = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 50f, "MilkCannon_Large.png");

                var item = new ItemBuilder(Plugin.Instance.Info)
                    .SetNameAndDescription(Plugin.MilkCannonEnumName, Plugin.MilkCannonEnumName + "_Desc")
                    .SetEnum(Plugin.MilkCannonEnumName)
                    .SetShopPrice(800)
                    .SetGeneratorCost(CannonCost)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(small, large)
                    .SetItemComponent<MilkCannonComponent>()
                    .Build();

                Plugin.Instance.assetMan.Add<ItemObject>(Plugin.MilkCannonEnumName, item);
                LevelLoaderPlugin.Instance.itemObjects.Add(Plugin.MilkCannonEnumName, item);
                LevelStudioPlugin.Instance.selectableShopItems.Add(Plugin.MilkCannonEnumName);
                Plugin.MilkCannonItemObject = item;
            }
            catch (System.Exception) { }
        }

        private static ItemObject MakeVariant<T>(string enumName, string titleKey, string descKey, string smallFile, string largeFile, int cost) where T : Item, new()
        {
            try
            {
                Sprite small = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 25f, smallFile);
                Sprite large = AssetLoader.SpriteFromMod(Plugin.Instance, Vector2.one / 2f, 50f, largeFile);

                var item = new ItemBuilder(Plugin.Instance.Info)
                    .SetNameAndDescription(titleKey, descKey)
                    .SetEnum(enumName)
                    .SetGeneratorCost(cost)
                    .SetMeta(ItemFlags.None, new string[0])
                    .SetSprites(small, large)
                    .SetItemComponent<T>()
                    .Build();

                Plugin.Instance.assetMan.Add<ItemObject>(enumName, item);
                LevelLoaderPlugin.Instance.itemObjects.Add(enumName, item);
                return item;
            }
            catch (System.Exception) { return null; }
        }

        internal static void AddBucket(PlayerManager player)
        {
            if (player == null || player.itm == null || Plugin.EmptyBucketItemObject == null) return;
            try { player.itm.AddItem(Plugin.EmptyBucketItemObject); } catch (System.Exception) { }
        }

        internal static bool Downgrade(PlayerManager player, ItemObject next)
        {
            if (player == null || player.itm == null || next == null) return false;
            try
            {
                int slot = player.itm.selectedItem;
                if (slot < 0 || slot > player.itm.maxItem) return false;
                player.itm.SetItem(next, slot);
                return true;
            }
            catch (System.Exception) { return false; }
        }
    }






    public class BullyMilkComponent : Item
    {
        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();
            if (player.gameObject.GetComponent<BullyBlessing>() == null)
            {
                player.gameObject.AddComponent<BullyBlessing>().Begin(player);
            }
            try { AchievementHelper.UnlockAchievement("milk_bully"); } catch (System.Exception) { }

            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.BullyMilkItemObject);
        }
    }

    public class BullyBlessing : MonoBehaviour
    {
        private const float Duration = 20f;
        private float timer;
        private PlayerManager player;

        public void Begin(PlayerManager pm)
        {
            player = pm;
            if (player != null) player.invincible = true;
            PushEveryoneAway();
        }

        private void PushEveryoneAway()
        {
            if (player == null) return;
            Vector3 origin = player.transform.position;
            Entity playerEntity = player.plm != null ? player.plm.Entity : null;
            Entity[] entities = UnityEngine.Object.FindObjectsOfType<Entity>();
            foreach (Entity e in entities)
            {
                if (e == null || e == playerEntity) continue;
                Vector3 dir = e.transform.position - origin;
                dir.y = 0f;
                if (dir.sqrMagnitude < 0.001f) dir = Vector3.forward;
                else dir.Normalize();
                e.AddForce(new Force(dir, 11f, 2.5f));
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= Duration) Finish();
        }

        private void Finish()
        {
            if (player != null)
            {
                player.invincible = false;
                if (player.itm != null)
                {
                    try { player.itm.RemoveRandomItem(); } catch (System.Exception) { }
                }
            }
            Destroy(gameObject);
        }
    }








    public class MatryoshkaMilkComponent : Item
    {


        private static int UsesFor(ItemObject cur)
        {
            if (cur == Plugin.MatryoshkaMilkItemObject) return 3;
            if (cur == Plugin.MatryoshkaMilkItemObject2) return 2;
            return 1;
        }


        private static ItemObject NextFor(int uses)
        {
            if (uses == 3) return Plugin.MatryoshkaMilkItemObject2;
            if (uses == 2) return Plugin.MatryoshkaMilkItemObject1;
            return null;
        }

        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();

            int slot = player.itm != null ? player.itm.selectedItem : -1;
            ItemObject cur = (slot >= 0 && slot <= player.itm.maxItem) ? player.itm.items[slot] : null;
            int uses = UsesFor(cur);

            if (player.plm != null)
            {
                if (uses == 3)
                {
                    Plugin.StopMilkRandomEvents();
                    player.plm.stamina = player.plm.StaminaMax * 1.5f;
                }
                else if (uses == 2)
                {
                    player.plm.stamina = player.plm.StaminaMax;
                }
                else
                {
                    player.plm.stamina = player.plm.StaminaMax * 0.5f;
                }
            }

            if (uses > 1)
            {
                NewMilks.AddBucket(player);
                NewMilks.Downgrade(player, NextFor(uses));
                return false;
            }



            return Plugin.ConsumeMilkToEmptyBucket(player, Plugin.MatryoshkaMilkItemObject1);
        }
    }






    public class MilkCannonComponent : Item
    {

        private static int UsesFor(ItemObject cur)
        {
            if (cur == Plugin.MilkCannonItemObject) return 4;
            if (cur == Plugin.MilkCannonItemObject3) return 3;
            if (cur == Plugin.MilkCannonItemObject2) return 2;
            return 1;
        }


        private static ItemObject NextFor(int uses)
        {
            if (uses == 4) return Plugin.MilkCannonItemObject3;
            if (uses == 3) return Plugin.MilkCannonItemObject2;
            if (uses == 2) return Plugin.MilkCannonItemObject1;
            return null;
        }

        public override bool Use(PlayerManager player)
        {
            if (player == null) return false;
            Plugin.PlayMilkDrinkSound();

            int slot = player.itm != null ? player.itm.selectedItem : -1;
            ItemObject cur = (slot >= 0 && slot <= player.itm.maxItem) ? player.itm.items[slot] : null;
            int uses = UsesFor(cur);

            Fire(player);

            if (uses > 1)
            {
                NewMilks.Downgrade(player, NextFor(uses));
                return false;
            }
            return true;
        }

        private static void Fire(PlayerManager player)
        {
            if (player.plm != null)
                player.plm.stamina = Mathf.Max(player.plm.stamina, 0.5f * player.plm.StaminaMax);

            Vector3 forward = player.transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;
            forward.Normalize();

            GameObject go = new GameObject("MilkCannonProjectile");
            go.transform.position = player.transform.position + forward * 1.5f + Vector3.up * 0.7f;

            Sprite ball;
            try { ball = AssetLoader.SpriteFromMod(Plugin.Instance, new Vector2(0.5f, 0.5f), 100f, "milkball_100.png"); }
            catch (System.Exception) { ball = null; }

            if (ball != null)
            {
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = ball;
                sr.sortingOrder = 300;
            }

            MilkCannonProjectile projectile = go.AddComponent<MilkCannonProjectile>();
            projectile.direction = forward;
            projectile.ignore = player.plm != null ? player.plm.Entity : null;
        }
    }

    public class MilkCannonProjectile : MonoBehaviour
    {
        public Vector3 direction = Vector3.forward;
        public float speed = 14f;
        public float life = 3f;
        public float range = 1.6f;
        public Entity ignore;

        private SpriteRenderer sr;
        private EnvironmentController ec;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            try { ec = Singleton<CoreGameManager>.Instance?.GetComponent<EnvironmentController>(); } catch (System.Exception) { }
        }

        private void Update()
        {

            if (Camera.main != null)
            {
                transform.rotation = Camera.main.transform.rotation;
            }



            if (sr != null)
            {
                if (ec != null)
                {
                    try
                    {
                        Cell cell = ec.CellFromPosition(transform.position);
                        sr.enabled = cell != null && cell.HasChunk && cell.Chunk.Rendering;
                    }
                    catch (System.Exception)
                    {
                        sr.enabled = true;
                    }
                }
                else
                {
                    sr.enabled = true;
                }
            }

            transform.position += direction * speed * Time.deltaTime;


            Collider[] hits = Physics.OverlapSphere(transform.position, range);
            foreach (Collider c in hits)
            {
                if (c == null) continue;
                Entity e = c.GetComponentInParent<Entity>();
                if (e == null || e == ignore) continue;

                Vector3 d = e.transform.position - transform.position;
                d.y = 0f;
                if (d.sqrMagnitude < 0.001f) d = -direction;
                else d.Normalize();
                e.AddForce(new Force(d, 12f, 2.5f));
                Destroy(gameObject);
                return;
            }

            life -= Time.deltaTime;
            if (life <= 0f) Destroy(gameObject);
        }
    }
}