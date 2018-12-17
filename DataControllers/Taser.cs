using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.Interactions;

namespace Subtegral.StealthAgent.GameCore
{
    public class Taser : MonoBehaviour,IInteractable,IDataController
    {

        [SerializeField]
        private TaserData _data;

        public IDataContainer GetContainer()
        {
            _data.AppendControllerData(transform,this);
            return _data;
        }

        public void Inject(IDataContainer container)
        {
            _data = (TaserData)container;
        }

        public void Interact()
        {
            InventoryManager.Instance.AddItem(_data.TaserItem);
            Destroy(gameObject);
        }

        public bool IsCurrentlyInteractable(params object[] optionalObjects)
        {
            return true;
        }
    } 
}
