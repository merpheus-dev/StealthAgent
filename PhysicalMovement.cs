using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Subtegral.StealthAgent.GameCore
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class PhysicalMovement : MonoBehaviour
    {
        public bool ImMovable = false;

        public Vector2 TargetPosition;

        public float MovementSpeed = 1f;

        public float EstimationThreshold = .1f;

        private Rigidbody2D _rigidbody;

        public Vector2 direction;

        private float? _angle = null;

        public float LookAtRatio = .1f;

        public float? Angle
        {
            get
            {
                if (_angle == null)
                    return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                else
                    return (float)_angle;

            }
            set
            {
                _angle = value;
            }
        }

        public abstract void UpdateByFrame();

        public Action AfterMoving;

        public virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (MeasureObjectDistance(_rigidbody.position, TargetPosition) > EstimationThreshold)
            {
                if (!ImMovable)
                {
                    direction = TargetPosition - (Vector2)transform.position;
                    if (Quaternion.Angle(transform.rotation, Quaternion.AngleAxis((float)Angle, Vector3.forward)) < EstimationThreshold)
                        MoveToTarget();
                    LookAtTarget();
                }
            }
            else
            {
                AfterMoving?.Invoke();
            }

            UpdateByFrame();
        }

        public void MoveToTarget()
        {
            _rigidbody.MovePosition(_rigidbody.position + direction.normalized * MovementSpeed * Time.deltaTime);
        }

        public void LookAtTarget()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis((float)Angle, Vector3.forward), LookAtRatio);
        }

        float MeasureObjectDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }
    }
}
