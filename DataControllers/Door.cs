using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using System;

namespace Subtegral.StealthAgent.Interactions
{
    public class Door : MonoBehaviour, IInterruptableInteractable, IDataController
    {
        [SerializeField]
        private DoorData _data;

        public Action<bool, DoorType> HookFunction;

        public void Inject(IDataContainer container)
        {
            _data = (DoorData)container;
        }

        public IDataContainer GetContainer()
        {
            _data.AppendControllerData(transform);
            return _data;
        }

        public void Interact()
        {
            if (_data.AnchorPoint == null)
                throw new System.Exception("Anchor Not Assigned!");
            StartCoroutine(OpenTheDoor());
        }

        public void HackInteraction()
        {
            HookFunction?.Invoke(true, _data.DoorType);
        }

        public void InterruptInteraction()
        {
            HookFunction?.Invoke(false, DoorType.Blue);
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
            if (optionalObjects != null && optionalObjects.Length > 0)
            {
                foreach (var item in optionalObjects)
                {
                    if (item is KeyItem && ((KeyItem)item).KeyType == _data.DoorType)
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