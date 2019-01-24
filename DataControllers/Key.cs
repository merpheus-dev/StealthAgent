using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;

namespace Subtegral.StealthAgent.Interactions
{
    public class Key : MonoBehaviour, IInteractable, IDataController
    {
        [SerializeField]
        private KeyData _data;

        public IDataContainer GetContainer()
        {
            _data.AppendControllerData(transform);
            return _data;
        }

        public void Inject(IDataContainer container)
        {
            _data = (KeyData)container;
        }

        public void Interact()
        {
            InventoryManager.Instance.AddItem(_data.KeyItem);
            GameObject obj = Instantiate(_data.ParticleEffect, transform.position, Quaternion.identity).gameObject;
            Destroy(obj, _data.ParticleEffect.main.startLifetimeMultiplier);
            Destroy(gameObject);
        }

        public bool IsCurrentlyInteractable(params object[] optionalObjects)
        {
            return true;
        }
    }

}