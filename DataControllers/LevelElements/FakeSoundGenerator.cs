using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;

namespace Subtegral.StealthAgent.Interactions
{
    public class FakeSoundGenerator : MonoBehaviour
    {
        public float TimerSet = 5f;

        public float Radius = 3f;

        public LayerMask LayerMask;

        public TextMesh Counter;

        public ParticleSystem SoundVisualizer;

        public Color MyColor;

        private void Awake()
        {
            GetComponent<SpriteRenderer>().color = MyColor;
            StartCoroutine(CountTime());
        }

        private IEnumerator CountTime()
        {
            bool isInstantiated = false;
            while (TimerSet > 0f)
            {
                yield return null;
                TimerSet -= Time.deltaTime;
                Counter.text = ((int)TimerSet).ToString();
                if (TimerSet < 2f && !isInstantiated)
                {
                    isInstantiated = true;
                    ParticleSystem.MainModule ps = Instantiate(SoundVisualizer, transform.position, Quaternion.identity).main;
                    ps.startColor = MyColor;
                }
            }
            BroadCastSound();
        }

        private void BroadCastSound()
        {
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, Radius, LayerMask);
            foreach (var item in hitObjects)
            {
                Enemy enemy = item.gameObject.GetComponent<Enemy>();
                enemy.CurrentEnemyState = Enemy.EnemyState.Notified;
                enemy.Faking = true;
                enemy.TargetPosition = transform.position;
                enemy.Chase();
                enemy.Faking = false;
            }
            Destroy(gameObject);
        }
    }

}