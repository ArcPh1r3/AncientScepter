﻿using System.Security;
using System.Security.Permissions;
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Networking;
using System.Linq;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace AncientScepter
{

    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI))]
    [R2APISubmoduleDependency(nameof(LoadoutAPI))]
    [R2APISubmoduleDependency(nameof(ProjectileAPI))]
    public class AncientScepterMain : BaseUnityPlugin
    {
        public const string ModVer = "1.0.3";
        public const string ModName = "StandaloneAncientScepter";
        public const string ModGuid = "com.DestroyedClone.AncientScepter";

        internal static BepInEx.Logging.ManualLogSource _logger;

        public List<ItemBase> Items = new List<ItemBase>();
        public static Dictionary<ItemBase, bool> ItemStatusDictionary = new Dictionary<ItemBase, bool>();
        protected readonly List<LanguageAPI.LanguageOverlay> languageOverlays = new List<LanguageAPI.LanguageOverlay>();

        public void Awake()
        {
            Assets.PopulateAssets();

            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));
            foreach (var itemType in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(itemType);
                if (ValidateItem(item, Items))
                {
                    item.Init(Config);
                }
            }

            On.RoR2.Run.Start += On_RunStart;
        }

        public void Start()
        {
            InstallLanguage();
        }

        public void InstallLanguage()
        {
            foreach (var skill in AncientScepterItem.instance.skills)
            {
                if (skill.oldDescToken == null)
                {
                    continue;
                }
                languageOverlays.Add(LanguageAPI.AddOverlay(skill.newDescToken, Language.GetString(skill.oldDescToken) + skill.overrideStr, Language.currentLanguageName));
            }
        }

        public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            var enabled = Config.Bind<bool>("Item: " + item.ItemName, "Enable Item?", true, "Should this item appear in runs?").Value;
            var aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?").Value;

            ItemStatusDictionary.Add(item, enabled);

            if (enabled)
            {
                itemList.Add(item);
                if (aiBlacklist)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }

        private static void On_RunStart(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            if (!NetworkServer.active) return;
            var itemRngGenerator = new Xoroshiro128Plus(self.seed);
            AncientScepterItem.instance.rng = new Xoroshiro128Plus(itemRngGenerator.nextUlong);
        }
    }
}