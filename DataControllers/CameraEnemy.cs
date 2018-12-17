using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;
namespace Subtegral.StealthAgent.GameCore
{
    public class CameraEnemy : PhysicalMovement, IWatcher,IDataController
    {
        [SerializeField]
        private CameraData _data;

        private Light2DSource source;

        private int _angleIndex;

        private int AngleIndex
        {

            get
            {
                return _angleIndex;
            }
            set
            {
                if (_angleIndex + value < _data.Angles.Length)
                    _angleIndex = value;
                else
                    _angleIndex = 0;
            }
        }


        public void Inject(IDataContainer container)
        {
            _data = (CameraData)container;
            LookAtRatio = _data.LookAtRatio;
        }

        public IDataContainer GetContainer()
        {
            return _data;
        }


        void Start()
        {
            source = GetComponentInChildren<Light2DSource>();
            TargetPosition = transform.position;
            AfterMoving += LookAngle;
        }

        public void DrawFOV()
        {
            foreach (var item in source.m_EventManager.GetCollisionObjects())
            {
                if (item != null && item.CompareTag("Player"))
                {
                    NotifyOthers();
                    return;
                }
            }
        }



        public void NotifyOthers()
        {
            //TO-DO:Move this to awake for saving some performance
            Enemy[] enemies = FindObjectsOfType<Enemy>();

            foreach (var item in enemies)
            {
                if (item != this)
                {
                    if (item.CurrentEnemyState == Enemy.EnemyState.Chase)
                        continue;
                    item.CurrentEnemyState = Enemy.EnemyState.Notified;
                    item.Chase();
                }
            }
        }

        private void LookAngle()
        {
            AfterMoving -= LookAngle;
            StartCoroutine(LookToAngles());
        }

        private IEnumerator LookToAngles()
        {
            LookAtRatio =_data.Angles[AngleIndex].LookAtRatio;
            Angle = _data.Angles[AngleIndex].Angle;
            while (Quaternion.Angle(transform.rotation, Quaternion.AngleAxis((float)Angle, Vector3.forward)) > EstimationThreshold)
            {
                yield return null;
                LookAtTarget();
            }
            yield return new WaitForSeconds(_data.Angles[AngleIndex].TimeToWait);
            AngleIndex++;
            AfterMoving += LookAngle;
        }

        public override void UpdateByFrame()
        {
            DrawFOV();
        }
    }

}