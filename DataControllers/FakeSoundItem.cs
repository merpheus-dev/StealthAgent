using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;

namespace Subtegral.StealthAgent.Interactions
{
    public class FakeSoundItem : MonoBehaviour, IInteractable, IDataController
    {
        [SerializeField]
        private NoiseGeneratorData _data;

        public IDataContainer GetContainer()
        {
            _data.AppendControllerData(transform);
            return _data;
        }

        public void Inject(IDataContainer container)
        {
            _data = (NoiseGeneratorData)container;
        }

        public void Interact()
        {
            InventoryManager.Instance.AddItem(_data.FakeSound);
            Destroy(gameObject);
        }

        public bool IsCurrentlyInteractable(params object[] optionalObjects)
        {
            return true;
        }
    }
}
