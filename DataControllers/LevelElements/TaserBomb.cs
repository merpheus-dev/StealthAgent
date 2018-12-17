using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using DigitalRuby.LightningBolt;

namespace Subtegral.StealthAgent.Interactions
{
    public class TaserBomb : MonoBehaviour
    {
        public float Radius = 5f;
        public float StallTime = 3f;
        public GameObject LightningPrefab;
        public GameObject ShockOnEnemyPrefab;

        public LayerMask LayerMask;

        public void TaseNearby(Vector2 pointOfOrigin)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(pointOfOrigin, Radius, LayerMask);
            List<LightningBoltScript> lightningBoltScripts = new List<LightningBoltScript>();

            foreach (var item in colliders)
            {
                LightningBoltScript lightning = Instantiate(LightningPrefab, pointOfOrigin, Quaternion.identity).GetComponent<LightningBoltScript>();
                lightning.StartObject = null;
                lightning.EndObject = null;
                lightning.StartPosition = pointOfOrigin;
                lightning.EndPosition = item.transform.position;
                lightningBoltScripts.Add(lightning);
                Destroy(lightning.gameObject, .5f);
            }
            GetComponent<SpriteRenderer>().enabled = false;
            foreach (var item in colliders)
            {
                GameObject tempObj = Instantiate(ShockOnEnemyPrefab, item.transform.position, Quaternion.identity);
                tempObj.transform.SetParent(item.transform, true);
                Destroy(tempObj, StallTime);
            }
            StartCoroutine(ShockFinish(colliders));

        }

        private IEnumerator ShockFinish(Collider2D[] colliders)
        {
            Enemy enemy = null;
            foreach (var collider in colliders)
            {
                enemy = collider.GetComponent<Enemy>();
                if (enemy == null)
                    continue;

                enemy.ImMovable = true;
                enemy.CurrentEnemyState = Enemy.EnemyState.KnockingInProgress;
            }
            yield return new WaitForSeconds(StallTime);
            foreach (var collider in colliders)
            {
                enemy = collider.GetComponent<Enemy>();
                if (enemy == null)
                    continue;

                enemy.StopPatrol();
                enemy.ImMovable = false;
                enemy.CurrentEnemyState = Enemy.EnemyState.Patrol;
                enemy.SearchAround();
                Destroy(gameObject);
            }
            yield return null;
        }
    }

}