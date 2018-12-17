using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;

namespace Subtegral.StealthAgent.Interactions
{
    public class Door : MonoBehaviour, IInteractable,IDataController
    {
        [SerializeField]
        private DoorData _data;

        public void Inject(IDataContainer container)
        {
            _data = (DoorData)container;
        }

        public IDataContainer GetContainer()
        {
            _data.AppendControllerData(transform,this);
            return _data;
        }

        public void Interact()
        {
            if (_data.AnchorPoint == null)
                throw new System.Exception("Anchor Not Assigned!");
            StartCoroutine(OpenTheDoor());
        }

        private IEnumerator OpenTheDoor()
        {
            while (Quaternion.Angle(transform.rotation, Quaternion.AngleAxis(_data.TargetAngle, Vector3.forward)) > .1f)
            {
                yield return null;
                transform.RotateAround(_data.AnchorPoint.position, Vector3.forward, _data.TargetAngle);
            }
        }

        public bool IsCurrentlyInteractable(params object[] optionalObjects)
        {
            if(optionalObjects!=null && optionalObjects.Length> 0)
            {
                foreach (var item in optionalObjects)
                {
                    if (item is KeyItem && ((KeyItem)item).KeyType== _data.DoorType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }

    public enum DoorType
    {
        Blue,
        Red,
        Yellow,
        Green
    }
}