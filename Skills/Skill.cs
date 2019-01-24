using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;

namespace Subtegral.StealthAgent.Skills
{
    public abstract class Skill : MonoBehaviour
    {
        public int Interval;
        public bool IsInfinite = false;
        public bool IsActive = false;
        public abstract void Deactivate();
        public abstract void OnAwake();

        private float TimeCounter;
        public Player player;

        public void AppendPlayer(Player player)
        {
            this.player = player;
        }

        private void Awake()
        {
            TimeCounter = Interval;
            OnAwake();
        }

        private void Update()
        {
            if (IsInfinite)
                return;
            if (IsActive)
            {
                if (TimeCounter > 0f)
                    TimeCounter -= Time.deltaTime;
                else
                    Deactivate();
            }
        }
    }
}