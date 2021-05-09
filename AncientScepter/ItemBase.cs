using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AncientScepter
{
    // The directly below is entirely from TILER2 API (by ThinkInvis) specifically the Item module. Utilized to keep instance checking functionality as I migrate off TILER2.
    // TILER2 API can be found at the following places:
    // https://github.com/ThinkInvis/RoR2-TILER2
    // https://thunderstore.io/package/ThinkInvis/TILER2/

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        public static T instance { get; private set; }

        public ItemBase()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBoilerplate/Item was instantiated twice");
            instance = this as T;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public abstract class ItemBase
    {
        public abstract string ItemName { get; }
        public abstract string ItemLangTokenName { get; }
        public abstract string ItemPickupDesc { get; }
        public abstract string ItemFullDescription { get; }
        public abstract string ItemLore { get; }

        public abstract ItemTier Tier { get; }
        public virtual ItemTag[] ItemTags { get; set; } = new ItemTag[] { };

        public abstract GameObject ItemModel { get; }
        public abstract Sprite ItemIcon { get; }
        public abstract GameObject ItemDisplay { get; }

        public ItemDef ItemDef { get; set; }

        public virtual bool CanRemove { get; } = true;

        public virtual bool AIBlacklisted { get; set; } = false;

        public abstract void Init(ConfigFile config);

        public static GameObject displayPrefab;

        protected void CreateLang()
        {
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_NAME", ItemName);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_PICKUP", ItemPickupDesc);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_DESCRIPTION", ItemFullDescription);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_LORE", ItemLore);
        }

        public virtual ItemDisplayRuleDict CreateDisplayRules()
        {
            return new ItemDisplayRuleDict(new ItemDisplayRule[0]);
        }

        protected void CreateItem()
        {
            if (AIBlacklisted)
            {
                ItemTags = new List<ItemTag>(ItemTags) { ItemTag.AIBlacklist }.ToArray();
            }

            ItemDef = ScriptableObject.CreateInstance<ItemDef>();
            ItemDef.name = "ITEM_" + ItemLangTokenName;
            ItemDef.nameToken = "ITEM_" + ItemLangTokenName + "_NAME";
            ItemDef.pickupToken = "ITEM_" + ItemLangTokenName + "_PICKUP";
            ItemDef.descriptionToken = "ITEM_" + ItemLangTokenName + "_DESCRIPTION";
            ItemDef.loreToken = "ITEM_" + ItemLangTokenName + "_LORE";
            ItemDef.pickupModelPrefab = ItemModel;
            ItemDef.pickupIconSprite = ItemIcon;
            ItemDef.hidden = false;
            ItemDef.tags = ItemTags;
            ItemDef.canRemove = CanRemove;
            ItemDef.tier = Tier;
            if (ItemTags.Length > 0) { ItemDef.tags = ItemTags; }

            ItemAPI.Add(new CustomItem(ItemDef, CreateDisplayRules()));
        }

        public abstract void Hooks();

        protected virtual void SetupMaterials(GameObject modelPrefab)
        {
            Renderer[] children = modelPrefab.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < children.Length; i++)
            {
                children[i].material.shader = Assets.hotpoo;
            }
        }


        // The below is entirely from TILER2 API (by ThinkInvis) specifically the Item module. Utilized to keep easy count functionality as I migrate off TILER2.
        // TILER2 API can be found at the following places:
        // https://github.com/ThinkInvis/RoR2-TILER2
        // https://thunderstore.io/package/ThinkInvis/TILER2/

        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCount(ItemDef);
        }

        public int GetCount(CharacterMaster master)
        {
            if (!master || !master.inventory) { return 0; }

            return master.inventory.GetItemCount(ItemDef);
        }

        public static int GetCountSpecific(CharacterBody body, ItemIndex itemIndex)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCount(itemIndex);
        }

        ///<summary>A server-only rng instance based on the current run's seed.</summary>
        public Xoroshiro128Plus rng { get; internal set; }


        protected readonly List<LanguageAPI.LanguageOverlay> languageOverlays = new List<LanguageAPI.LanguageOverlay>();
        protected readonly Dictionary<string, string> genericLanguageTokens = new Dictionary<string, string>();
        protected readonly Dictionary<string, Dictionary<string, string>> specificLanguageTokens = new Dictionary<string, Dictionary<string, string>>();
        public bool languageInstalled { get; private set; } = false;
    }
}