﻿using UnityEngine;
using RoR2.Skills;
using static AncientScepter.SkillUtil;
using R2API;
using RoR2;

namespace AncientScepter
{
    public class MercEvisProjectile2 : ScepterSkill
    {
        public override SkillDef myDef { get; protected set; }

        public override string oldDescToken { get; protected set; }
        public override string newDescToken { get; protected set; }
        public override string overrideStr => "\n<color=#d299ff>SCEPTER: Charges four times faster. Hold and fire up to four charges at once.</color>";

        public override string targetBody => "MercBody";
        public override SkillSlot targetSlot => SkillSlot.Special;
        public override int targetVariantIndex => 1;

        internal override void SetupAttributes()
        {
            var oldDef = Resources.Load<SkillDef>("skilldefs/mercbody/MercBodyEvisProjectile");
            myDef = CloneSkillDef(oldDef);

            var nametoken = "CLASSICITEMS_SCEPMERC_EVISPROJNAME";
            newDescToken = "CLASSICITEMS_SCEPMERC_EVISPROJDESC";
            oldDescToken = oldDef.skillDescriptionToken;
            var namestr = "Gale-Force";
            LanguageAPI.Add(nametoken, namestr);

            myDef.skillName = namestr;
            myDef.skillNameToken = nametoken;
            myDef.skillDescriptionToken = newDescToken;
            myDef.icon = Resources.Load<Sprite>("@ClassicItems:Assets/ClassicItems/icons/scepter/merc_evisprojectileicon.png");
            myDef.baseMaxStock *= 4;
            myDef.baseRechargeInterval /= 4f;

            LoadoutAPI.AddSkillDef(myDef);
        }

        internal override void LoadBehavior()
        {
            On.EntityStates.Commando.CommandoWeapon.FireFMJ.OnEnter += On_FireFMJEnter;
        }

        internal override void UnloadBehavior()
        {
            On.EntityStates.Commando.CommandoWeapon.FireFMJ.OnEnter -= On_FireFMJEnter;
        }

        private void On_FireFMJEnter(On.EntityStates.Commando.CommandoWeapon.FireFMJ.orig_OnEnter orig, EntityStates.Commando.CommandoWeapon.FireFMJ self)
        {
            orig(self);
            if (!(self is EntityStates.Commando.CommandoWeapon.ThrowEvisProjectile) || AncientScepterItem.GetCount(self.outer.commonComponents.characterBody) < 1) return;
            if (!self.outer.commonComponents.skillLocator?.special) return;
            var fireCount = self.outer.commonComponents.skillLocator.special.stock;
            self.outer.commonComponents.skillLocator.special.stock = 0;
            for (var i = 0; i < fireCount; i++)
            {
                self.Fire();
            }
        }
    }
}