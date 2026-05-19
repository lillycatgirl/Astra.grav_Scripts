using System;
using UnityEngine;

namespace Gameplay.Inventory
{
    public enum InventoryHoldableType
    {
        Planet,
        Consumable
    }
    public class InventoriesManager : MonoBehaviour
    {
        
        public static InventoriesManager Instance;

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
    }
}