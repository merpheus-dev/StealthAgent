using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;

namespace Subtegral.StealthAgent.Skills
{
    public class EagleEye : Skill, ISkill
    {
        public LineProfile Profile;
        private Enemy[] enemies;
        private List<GameObject> lineCache = new List<GameObject>();
        public void Activate()
        {
            EnemyData data = null;
            foreach (var enemy in enemies)
            {
                if (enemy.CurrentEnemyState == Enemy.EnemyState.Patrol)
                {
                    data = (EnemyData)enemy.GetContainer();
                    lineCache.Add(LineFactory.DrawChunk(data.GetWayPoints(), Profile));
                }
            }
        }

        public override void Deactivate()
        {
            for (int i = 0; i < lineCache.Count; i++)
            {
                Destroy(lineCache[i]);
            }
            lineCache.Clear();
        }

        public override void OnAwake()
        {
            enemies = FindObjectsOfType<Enemy>();

        }
    }

}