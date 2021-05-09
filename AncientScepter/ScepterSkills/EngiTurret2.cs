using UnityEngine;
using RoR2.Skills;
using static AncientScepter.SkillUtil;
using R2API;
using RoR2;

namespace AncientScepter
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class EngiTurret2 : ScepterSkill
    {
        public override SkillDef myDef { get; protected set; }
        internal static SkillDef oldDef { get; private set; }

        public override string oldDescToken { get; protected set; }
        public override string newDescToken { get; protected set; }
        public override string overrideStr => "\n<color=#d299ff>SCEPTER: Hold and place one more turret.</color>";

        public override string targetBody => "EngiBody";
        public override SkillSlot targetSlot => SkillSlot.Special;
        public override int targetVariantIndex => 0;

        internal override void SetupAttributes()
        {
            oldDef = Resources.Load<SkillDef>("skilldefs/engibody/EngiBodyPlaceTurret");
            myDef = CloneSkillDef(oldDef);

            var nametoken = "ANCIENTSCEPTER_SCEPENGI_TURRETNAME";
            newDescToken = "ANCIENTSCEPTER_SCEPENGI_TURRETDESC";
            oldDescToken = oldDef.skillDescriptionToken;
            var namestr = "TR12-C Gauss Compact";
            LanguageAPI.Add(nametoken, namestr);

            myDef.skillName = namestr;
            myDef.skillNameToken = nametoken;
            myDef.skillDescriptionToken = newDescToken;
            myDef.icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texEngiR1");
            myDef.baseMaxStock += 1;

            LoadoutAPI.AddSkillDef(myDef);
        }
    }
}