using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.Interactions;
using System.Linq;
using Pathfinding;
using UnityEngine.EventSystems;

namespace Subtegral.StealthAgent.GameCore
{
    [RequireComponent(typeof(LineRenderer))]
    public class Player : PhysicalMovement
    {
        public LayerMask LayerMask;

        private LineRenderer lineRenderer;

        private Vector3 pos;

        private RaycastHit2D hit;

        private bool isHit = false;

        private bool updateLineBeginPoint;

        private IAstarAI ai;

        private AILerp lerp;

        public Enemy CurrentEnemyTarget = null;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            ai = GetComponent<IAstarAI>();
            lerp = GetComponent<AILerp>();
            TargetPosition = transform.position;
            ClearDashedLine();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            IInteractable interactable = collision?.gameObject.GetComponent<IInteractable>();
            if (interactable == null)
                return;
            switch (interactable)
            {
                case Door d:
                    if (interactable.IsCurrentlyInteractable(InventoryManager.Instance.GetItems()))
                        interactable.Interact();
                    break;
                case Enemy e:
                    if (CurrentEnemyTarget != null)
                        return;

                    CurrentEnemyTarget = e;

                    if (interactable.IsCurrentlyInteractable())
                    {
                        //If enemy is already dead,show dragging
                        if (e.CurrentEnemyState == Enemy.EnemyState.Dead)
                        {
                            PlayerEventHandler.OnDeadEnemyCollisionEnter(e.transform);
                        }
                        else
                        {
                            interactable.Interact();
                            PlayerEventHandler.OnKnockingEnemy(e);
                        }
                    }
                    break;
                default:
                    if (interactable.IsCurrentlyInteractable())
                        interactable.Interact();
                    break;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            IInteractable interactable = collision?.gameObject.GetComponent<IInteractable>();
            if (interactable == null)
                return;

            switch (interactable)
            {
                case Enemy takeDownAttemptedEnemy:
                    if (takeDownAttemptedEnemy.CurrentEnemyState != Enemy.EnemyState.Dead)
                    {
                        PlayerEventHandler.OnKnockingEnemyInterrupted(takeDownAttemptedEnemy);
                        takeDownAttemptedEnemy.CurrentEnemyState = takeDownAttemptedEnemy.CachedState;
                    }
                    else
                    {
                        PlayerEventHandler.OnDeadEnemyCollisionExit(takeDownAttemptedEnemy.transform);
                    }
                    break;
                case HackableObject h:
                    h.InterruptInteraction();
                    break;
                case Hostile h:
                    if (h.IsCurrentlyInteractable())
                        h.InterruptInteraction();
                    break;
            }

        }

        public override void UpdateByFrame()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;

            //TO-DO:Refactor this line drawing code after creating Line of Sight enemy and it's line creation with an external LineCreator script
            lineRenderer.SetPosition(0, transform.position);
            if (Input.GetMouseButton(0))
            {
                lineRenderer.SetPosition(1, pos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!isHit)
                {
                    TargetPosition = lineRenderer.GetPosition(1);
                    ai.destination = lineRenderer.GetPosition(1);
                    ai.SearchPath();
                }
            }
            if (!Input.GetMouseButton(0))
                ClearDashedLine();
        }

        private void ClearDashedLine()
        {
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, transform.position);
            }
        }

        public void SetSpeed(float speed)
        {
            lerp.speed = speed;
        }

        public float GetSpeed()
        {
            return lerp.speed;
        }
    }

}