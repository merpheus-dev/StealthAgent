using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.StealthAgent.Skills
{
    public class Teleport : Skill, ISkill
    {
        public void Activate()
        {
            StartCoroutine(WaitUntilTap());
        }

        public override void Deactivate()
        {
            StopAllCoroutines();
        }

        public override void OnAwake()
        {
        }

        IEnumerator WaitUntilTap()
        {
            yield return new WaitUntil(()=>Input.touchCount == 1);
            player.Teleport(Input.GetTouch(0).position);
            Deactivate();
        }
    }

}