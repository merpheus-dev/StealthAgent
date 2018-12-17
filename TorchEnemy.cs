using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;
using Pathfinding;
namespace Subtegral.StealthAgent.GameCore
{
    public class TorchEnemy : Enemy, IWatcher
    {
        private Light2DSource source;

        private HashSet<Enemy> enemiesThatAlreadySeen = new HashSet<Enemy>();

        public override void Awake()
        {
            base.Awake();
            source = GetComponentInChildren<Light2DSource>();
        }

        public void DrawFOV()
        {
            if(CurrentEnemyState == EnemyState.Dead)
            {
                source.gameObject.SetActive(false);
                return;
            }
            if (CurrentEnemyState != EnemyState.Notified)
            {
                foreach (var item in source.m_EventManager.GetCollisionObjects())
                {
                    if (item.CompareTag("Player"))
                    {
                        NotifyOthers();
                        return;
                    }
                    else if(item.CompareTag("Enemy"))
                    {
                        Enemy enemy = item.GetComponent<Enemy>();
                        if (enemiesThatAlreadySeen.Contains(enemy))
                            continue;
                        if (enemy.CurrentEnemyState == EnemyState.Dead)
                        {
                            StopPatrol();
                            CurrentEnemyState = Enemy.EnemyState.Patrol;
                            SearchAround();
                            enemiesThatAlreadySeen.Add(enemy);
                        }
                    }else if (item.CompareTag("Hostile"))
                    {
                        Debug.Log("Detected");
                        StopPatrol();
                        TargetPosition = item.transform.position;
                        Faking = true;
                        Chase();
                    }
                }
            }
            if (CurrentEnemyState != EnemyState.Patrol)
                LostFromFOV();

        }


        public void NotifyOthers()
        {
            //TO-DO:Move this to awake for saving some performance
            Enemy[] enemies = FindObjectsOfType<Enemy>();

            foreach (var item in enemies)
            {
                if (item != this)
                {
                    if (item.CurrentEnemyState == EnemyState.Chase || item.CurrentEnemyState==EnemyState.Dead)
                        continue;
                    item.CurrentEnemyState = EnemyState.Notified;
                    item.Chase();
                }
                else
                {
                    item.Chase();
                }
            }
        }

        public override void UpdateByFrame()
        {
            
            DrawFOV();
        }


    }
}