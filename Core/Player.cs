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
    public class Player : PhysicalMovement, IDataController
    {
        public LayerMask LayerMask;

        //Accessed over UIManager
        public Enemy CurrentEnemyTarget = null;

        #region Private Fields
        private LineRenderer lineRenderer;

        private Vector3 pos;

        private RaycastHit2D hit;

        private bool isHit = false;

        private bool updateLineBeginPoint;

        private IAstarAI ai;

        private AILerp lerp;

        //  private Seeker seeker;

        private SpriteRenderer _renderer;

        [SerializeField]
        private PlayerData _data;
        #endregion

        private bool destinationChanged = false;

        public void Inject(IDataContainer container)
        {
            _data = (PlayerData)container;
            MovementSpeed = _data.MovementSpeed;
            LookAtRatio = _data.LookAtRatio;
            EstimationThreshold = _data.EstimationThreshold;
            //Level design precaution
            if (_data.Skin != null)
                _renderer.sprite = _data.Skin.Icon;
        }

        public IDataContainer GetContainer()
        {
            return _data;
        }

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            ai = GetComponent<IAstarAI>();
            lerp = GetComponent<AILerp>();
            _renderer = GetComponent<SpriteRenderer>();
            lerp.speed = MovementSpeed;
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
                    else
                        d.HackInteraction();
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
                case Door d:
                    d.InterruptInteraction();
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
                //TO-DO:Try removing this, looks like always true.
                if (!isHit)
                {
                    // TargetPosition = lineRenderer.GetPosition(1);
                    ai.destination = lineRenderer.GetPosition(1);
                    ai.SearchPath();
                    // TargetPosition = ai.steeringTarget;
                    destinationChanged = true;
                }
            }
            if (!Input.GetMouseButton(0))
                ClearDashedLine();

            if (lerp.hasPath)
                if (TargetPosition != (Vector2)GetHeading() && destinationChanged)
                {
                    LookAtRatio = 10;
                    TargetPosition = ai.steeringTarget;
                    destinationChanged = false;
                    StartCoroutine(NormalizeLookRatio());
                }
                else
                {
                    TargetPosition = GetHeading();
                }

        }

        private Vector3 GetHeading()
        {
            Vector3 a = transform.position;
            if (lerp.interpolator.GetMyPath().Count > lerp.interpolator.segmentIndex + 10)
                a = lerp.interpolator.GetMyPath()[lerp.interpolator.segmentIndex + 10];
            else
            {
                a = lerp.interpolator.GetMyPath()[lerp.interpolator.GetMyPath().Count - 1];
            }
            return a;
        }
        private void OnDrawGizmos()
        {
            if (lerp.hasPath)
                Gizmos.DrawIcon(GetHeading(), "Target");
            //  Gizmos.DrawIcon(TargetPosition, "Target");
        }
        public void Teleport(Vector2 tPos)
        {
            lerp.Teleport(tPos, true);
        }

        IEnumerator NormalizeLookRatio()
        {
            float frameCount = 0;
            while (frameCount < 25)
            {
                yield return new WaitForEndOfFrame();
                frameCount += 1;
            }
            LookAtRatio = 5;
        }

        private void ClearDashedLine()
        {
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, transform.position);
            }
        }


    }

}