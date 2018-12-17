using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
namespace Subtegral.StealthAgent.GameCore
{
    //TO-DO: Prevent enemy overlapping by creating random sphere destination point.
    //[!]You need to track down destination set references.
    public abstract class Enemy : PhysicalMovement, IInteractable, IDataController
    {
        #region Public Properties

        public EnemyState CurrentEnemyState = EnemyState.Patrol;

        public bool Faking = false;

        public EnemyState CachedState;

        #endregion

        #region Private Properties

        [SerializeField]
        private EnemyData _enemyData;

        private Waypoint activeWayPoint;

        private int _wayPointIndex;

        private int WayPointIndex
        {

            get
            {
                return _wayPointIndex;
            }
            set
            {
                if (value < _enemyData.Waypoints.Count)
                    _wayPointIndex = value;
                else
                    _wayPointIndex = 0;
            }
        }

        private IAstarAI ai;
        private AILerp lerper;

        private Player player;

        private float _lookAroundCache;
        #endregion

        public void Inject(IDataContainer container)
        {
            _enemyData = (EnemyData)container;
            MovementSpeed = _enemyData.MovementSpeed;
            EstimationThreshold = _enemyData.EstimationThreshold;
            LookAtRatio = _enemyData.DefaultLookAtRatio;
        }

        public IDataContainer GetContainer()
        {
            _enemyData.MovementSpeed = MovementSpeed;
            _enemyData.EstimationThreshold = EstimationThreshold;
            _enemyData.DefaultLookAtRatio = LookAtRatio;
            return _enemyData;
        }

        void Start()
        {
            if (_enemyData.Waypoints.Count > 0)
                activeWayPoint = _enemyData.Waypoints[0];
            else
                throw new Exception("No waypoint attached!");

            ai = GetComponent<IAstarAI>();
            player = FindObjectOfType<Player>();
            lerper = GetComponent<AILerp>();
            TargetPosition = activeWayPoint.Position;
            AfterMoving += AfterArrival;
        }


        #region Interactions

        public void Interact()
        {
            CachedState = CurrentEnemyState;
            CurrentEnemyState = EnemyState.KnockingInProgress;
        }

        public bool IsCurrentlyInteractable(params object[] objects)
        {
            return (CurrentEnemyState != EnemyState.Chase && CurrentEnemyState != EnemyState.ChaseComplete && CurrentEnemyState != EnemyState.Notified);
        }
        #endregion

        #region FOV Trigger Cycle
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && CurrentEnemyState == EnemyState.Chase)
                CurrentEnemyState = EnemyState.ChaseComplete;
        }

        public void Chase()
        {
            if (CurrentEnemyState == EnemyState.ChaseComplete || CurrentEnemyState == EnemyState.KnockingInProgress || CurrentEnemyState == EnemyState.Dead)
                return;

            //TO-DO:Refactor getcomponent call
            GetComponent<AILerp>().speed = _enemyData.ChaseSpeed;

            if (!Faking)
            {
                Debug.Log("REAL TARGET FOUND");
                TargetPosition = player.transform.position;
            }
            Angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Debug.DrawRay(transform.position, direction, Color.cyan);
            //Cancel patrol state
            AfterMoving = null;
            ai.destination = TargetPosition;
            if (!Faking)
            {

                CurrentEnemyState = EnemyState.Notified;
            }
            ai.SearchPath();
            if (CurrentEnemyState == EnemyState.Notified)
            {
                Debug.Log("WAITING TO ARRIVE DESTINATION TO PROCEEED");
                if (ai.reachedDestination)
                {
                    Debug.Log("DESTINATION REACHED");
                    CurrentEnemyState = EnemyState.Chase;
                }
            }
            else
            {
                CurrentEnemyState = EnemyState.Chase;
            }


        }

        public void LostFromFOV()
        {

            if (CurrentEnemyState == EnemyState.KnockingInProgress)
                return;
            if (CurrentEnemyState == EnemyState.Notified)
            {
                if (ai.reachedDestination)
                    CurrentEnemyState = EnemyState.Chase;
            }
            if (CurrentEnemyState == EnemyState.Chase)
            {
                Debug.Log("CURRENTLY:SEARCH START");
                if (ai.reachedDestination)
                    SearchAround();
                return;
            }
            else if (CurrentEnemyState == EnemyState.Search)
            {
                Debug.Log("CURRENTLY:SEARCH");
                return;
            }

            //TO-DO:Refactor this part and move getcomponent call to Awake
            lerper.speed = _enemyData.SearchSpeed;

            Angle = Mathf.Atan2(lerper.interpolator.tangent.y, lerper.interpolator.tangent.x) * Mathf.Rad2Deg;
            LookAtRatio = 3f;
            //Dont pick a new location until ai finishes the current target look around path
            if (!ai.reachedEndOfPath)
                return;

            AstarPath path = FindObjectOfType<AstarPath>();

            Vector3 rnd = UnityEngine.Random.insideUnitSphere * _enemyData.RandomPointRadius + path.data.gridGraph.center;
            while (!path.GetNearest(rnd).node.Walkable)
            {
                Debug.Log("LOOKING FOR WALKABLE NODES");
                rnd = UnityEngine.Random.insideUnitSphere * _enemyData.RandomPointRadius + path.data.gridGraph.center;
            }

            ai.destination = rnd;
            ai.SearchPath();

        }

        public void SearchAround()
        {
            _lookAroundCache = activeWayPoint != null ? activeWayPoint.Angles[0].LookAtRatio : 1f;
            CurrentEnemyState = EnemyState.Search;
            ai.destination = transform.position;
            Debug.Log("SEARCH UNDERWAY");
            ai.SearchPath();
            StartCoroutine(TurnWhileSearch(new Waypoint()
            {
                Angles = new List<WaypointAngle>()
                    {
                        new WaypointAngle()
                        {
                            Angle=UnityEngine.Random.Range(0f,360f),
                            TimeToWait=2f
                        },
                        new WaypointAngle()
                        {
                            Angle=UnityEngine.Random.Range(0f,360f),
                            TimeToWait=2f
                        }
                    },
                Position = transform.position
            }));
        }

        IEnumerator TurnWhileSearch(Waypoint waypoint)
        {
            List<WaypointAngle> angles = waypoint.Angles;
            for (int i = 0; i < angles.Count; i++)
            {
                LookAtRatio = 1f;
                Angle = angles[i].Angle;
                while (Quaternion.Angle(transform.rotation, Quaternion.AngleAxis((float)Angle, Vector3.forward)) > EstimationThreshold)
                {
                    yield return null;
                    LookAtTarget();
                }
                Debug.Log("WAITING...");
                yield return new WaitForSeconds(angles[i].TimeToWait);
            }
            Debug.Log("WAITING FINISHED");

            LookAtRatio = _lookAroundCache;
            CurrentEnemyState = EnemyState.Seek;
        }
        public enum EnemyState
        {
            Patrol,
            Chase,
            Seek,
            Search,
            Notified,
            KnockingInProgress,
            Dead,
            ChaseComplete
        }
        #endregion

        #region Patrol Movement

        IEnumerator LookAround()
        {
            List<WaypointAngle> angles = activeWayPoint.Angles;
            for (int i = 0; i < angles.Count; i++)
            {
                LookAtRatio = angles[i].LookAtRatio;
                Angle = angles[i].Angle;
                while (Quaternion.Angle(transform.rotation, Quaternion.AngleAxis((float)Angle, Vector3.forward)) > EstimationThreshold)
                {
                    if (ImMovable)
                    {
                        //If I cant move stop patrol execution
                        StopCoroutine(LookAround());
                        AfterMoving += AfterArrival;
                        yield break;
                    }
                    yield return null;
                    LookAtTarget();
                }
                yield return new WaitForSeconds(angles[i].TimeToWait);
            }
            yield return null;
            WayPointIndex = WayPointIndex + 1;
            activeWayPoint = _enemyData.Waypoints[WayPointIndex];
            TargetPosition = activeWayPoint.Position;
            Angle = null;
            AfterMoving += AfterArrival;
        }

        public void StopPatrol()
        {
            AfterMoving -= AfterArrival;
            StopCoroutine(LookAround());
        }

        private void AfterArrival()
        {
            if (ImMovable)
                return;

            AfterMoving -= AfterArrival;
            StartCoroutine(LookAround());
        }


        #endregion
    }
}