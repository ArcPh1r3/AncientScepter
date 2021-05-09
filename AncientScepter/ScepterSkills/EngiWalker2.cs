using UnityEngine;
using RoR2.Skills;
using static AncientScepter.SkillUtil;
using R2API;
using RoR2;

namespace AncientScepter
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class EngiWalker2 : ScepterSkill
    {
        public override SkillDef myDef { get; protected set; }
        internal static SkillDef oldDef { get; private set; }

        public override string oldDescToken { get; protected set; }
        public override string newDescToken { get; protected set; }
        public override string overrideStr => "\n<color=#d299ff>SCEPTER: Hold and place two more turrets.</color>";

        public override string targetBody => "EngiBody";
        public override SkillSlot targetSlot => SkillSlot.Special;
        public override int targetVariantIndex => 1;

        internal override void SetupAttributes()
        {
            oldDef = Resources.Load<SkillDef>("skilldefs/engibody/EngiBodyPlaceWalkerTurret");
            myDef = CloneSkillDef(oldDef);

            var nametoken = "ANCIENTSCEPTER_SCEPENGI_WALKERNAME";
            newDescToken = "ANCIENTSCEPTER_SCEPENGI_WALKERDESC";
            oldDescToken = oldDef.skillDescriptionToken;
            var namestr = "TR58-C Carbonizer Mini";
            LanguageAPI.Add(nametoken, namestr);

            myDef.skillName = namestr;
            myDef.skillNameToken = nametoken;
            myDef.skillDescriptionToken = newDescToken;
            myDef.icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texEngiR2");
            myDef.baseMaxStock += 2;

            LoadoutAPI.AddSkillDef(myDef);
        }
    }
}