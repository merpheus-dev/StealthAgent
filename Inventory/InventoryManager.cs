using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Subtegral.StealthAgent.GameCore
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance;
        private HashSet<InventoryItem> Items = new HashSet<InventoryItem>();
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        public InventoryItem[] GetItems()
        {
            return Items.ToList().ToArray();
        }
        public void AddItem(InventoryItem item)
        {
            PlayerEventHandler.OnItemGrabbed.Invoke(item);
            Items.Add(item);
        }
    }

}