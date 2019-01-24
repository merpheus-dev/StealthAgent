using UnityEngine;
using System;
namespace Subtegral.StealthAgent.GameCore
{
    public class DataContainer : ScriptableObject, IDataContainer
    {
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private Quaternion rotation;

        public void AppendControllerData(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;

        }

        public (Vector3 position, Quaternion rotation) GetTransform()
        {
            return (position, rotation);
        }
    } 
}