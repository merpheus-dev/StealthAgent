using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using Pathfinding;
namespace Subtegral.StealthAgent.Skills
{
    public class BulletTime : Skill, ISkill
    {
        private GameObject TimeEffector;
        public void Activate()
        {
            if (Input.touchCount != 1)
                return;
            TimeEffector = Instantiate(TimeEffector, Input.GetTouch(0).position, Quaternion.identity);
        }

        public override void Deactivate()
        {
            Destroy(TimeEffector);
        }

        public override void OnAwake()
        {
            TimeEffector = Resources.Load<GameObject>("Prefabs/TimeEffector");
            if (TimeEffector is null)
                throw new System.Exception("Time Effector Prefab Not Created!");
        }
    }

}