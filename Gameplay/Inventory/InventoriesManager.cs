using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoriesManager : MonoBehaviour
    {
        public static InventoriesManager Instance;
        private List<InventoryCollection> _inventories = new();
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public void AddInventory(InventoryCollection inventory)
        {
            _inventories.Add(inventory);
        }

        public void RemoveInventory(InventoryCollection inventory)
        {
            _inventories.Remove(inventory);
        }

        public List<InventoryCollection> GetInventories()
        {
            return _inventories;
        }
    }
}