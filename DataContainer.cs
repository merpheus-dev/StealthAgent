using UnityEngine;
using System;
namespace Subtegral.StealthAgent.GameCore
{
    [Serializable]
    public class DataContainer : IDataContainer
    {
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private Quaternion rotation;
        [SerializeField]
        private IDataController controller;

        public void AppendControllerData(Transform transform, IDataController controller)
        {
            position = transform.position;
            rotation = transform.rotation;
            this.controller = controller;
        }

        public IDataController GetController()
        {
            return controller;
        }

        public (Vector3 position, Quaternion rotation) GetTransform()
        {
            return (position, rotation);
        }
    } 
}