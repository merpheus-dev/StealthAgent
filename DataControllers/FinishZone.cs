using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.StealthAgent.GameCore
{
    public class FinishZone : MonoBehaviour, IDataController, IInteractable
    {
        [SerializeField]
        private FinisZoneData _data;
        public IDataContainer GetContainer()
        {
            return (IDataContainer)_data;
        }

        public void Inject(IDataContainer container)
        {
            _data = (FinisZoneData)container;
        }

        public void Interact()
        {
            PlayerEventHandler.OnGameOver(true);
        }

        public bool IsCurrentlyInteractable(params object[] optionalObjects)
        {
            return true;
        }
    }

}