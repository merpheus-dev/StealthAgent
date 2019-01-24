using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.Interactions;
using System.Linq;

namespace Subtegral.StealthAgent.Skills
{
    public class LockPick : Skill, ISkill
    {
        Door[] doorCache;
        public void Activate()
        {
            foreach (var door in doorCache)
            {
                door.HookFunction = (isActive, doorType) =>
                {
                    if (isActive)
                    {
                        PlayerEventHandler.OnDoorInteractionStart(door);
                        StartCoroutine(Pick(doorType));
                    }
                    else
                    {
                        PlayerEventHandler.OnDoorInteractionEnd(door);
                        StopAllCoroutines();
                    }
                };
            }
        }

        public override void Deactivate()
        {
            doorCache.ToList().ForEach((x) => x.HookFunction = null);
            IsActive = false;
        }

        public override void OnAwake()
        {
            doorCache = FindObjectsOfType<Door>();
            //Dont count down
            IsInfinite = true;
        }

        IEnumerator Pick(DoorType type)
        {
            Door target = doorCache.ToList().Find((x) => (x.GetContainer() as DoorData).DoorType == type);
            yield return new WaitForSeconds((target.GetContainer() as DoorData).Interval);
            //Deactivate after actually using this skill
            Deactivate();
        }
    }

}