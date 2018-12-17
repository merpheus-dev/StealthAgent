using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using Pathfinding;
namespace Subtegral.StealthAgent.Interactions
{
    public class Hostile : PhysicalMovement, IInterruptableInteractable,IDataController
    {
        public bool Parented = false;

        private HostileData _data;

        private IAstarAI ai;

        private Transform target;

        public void Inject(IDataContainer container)
        {
            _data = (HostileData)container;
            LookAtRatio = _data.LookAtRatio;
            GetComponent<AILerp>().speed = _data.Speed;
        }

        public IDataContainer GetContainer()
        {
            return _data;
        }

        public override void Awake()
        {
            base.Awake();
            TargetPosition = transform.position;
            ai = GetComponent<IAstarAI>();
        }

        public void SetTarget(Transform _transform)
        {
            target = _transform;
            if (target == null)
                ai.canMove = false;
        }

        public void Interact()
        {
            if (Parented)
                return;
            PlayerEventHandler.OnHostileEnter(this);
        }

        public void InterruptInteraction()
        {
            PlayerEventHandler.OnHostileExit(this);
        }

        public bool IsCurrentlyInteractable(params object[] optionalObjects)
        {
            return !Parented;
        }

        public override void UpdateByFrame()
        {
            if (target != null)
            {
                ImMovable = true;
                direction = (target.position - transform.position).normalized;
                LookAtTarget();
                ai.destination = target.position;
                ai.SearchPath();
                ai.canMove = ai.remainingDistance > 1f;
            }
        }


    }
}
