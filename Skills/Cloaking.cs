using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using System.Linq;

namespace Subtegral.StealthAgent.Skills
{
    public class Cloaking : Skill, ISkill
    {
        public LayerMask DefaultLayer;
        public LayerMask CloakLayer;
        public SkinData Cloak;
        public SkinData Default;

        public void Activate()
        {
            player.gameObject.layer = CloakLayer;
            (player.GetContainer() as PlayerData).Skin = Cloak;
        }

        public override void Deactivate()
        {
            player.gameObject.layer = DefaultLayer;
            (player.GetContainer() as PlayerData).Skin = Default;

        }

        public override void OnAwake()
        {
            
        }
    }
}