using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using System;

namespace Subtegral.StealthAgent.Interactions
{
    public class Door : MonoBehaviour, IInterruptableInteractable, IDataController, IEnable
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
            if (_data.AnchorPoint == Vector3.zero)
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
            yield return null;
            transform.RotateAround(_data.AnchorPoint, Vector3.forward, _data.TargetAngle);

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

        public void Enable()
        {
            Interact();
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