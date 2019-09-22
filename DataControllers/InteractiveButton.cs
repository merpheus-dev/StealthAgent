
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;

namespace Subtegral.StealthAgent.Interactions
{
    public class InteractiveButton : MonoBehaviour, IDataController,IInteractable
    {
        private ButtonData _data;
        public IDataContainer GetContainer()
        {
            return _data;
        }

        public void Inject(IDataContainer container)
        {
            _data = (ButtonData)container;
        }

        public void Interact()
        {
            foreach (var relationship in _data.Relationships)
            {
                relationship.Enable();
            }
            _data.IsPressed ^= true;
        }

        public bool IsCurrentlyInteractable(params object[] optionalObjects)
        {
            return !_data.IsPressed;
        }
    }
}