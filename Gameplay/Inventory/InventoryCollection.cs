using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoryCollection : MonoBehaviour
    {
        public ObjectDraggableManager.DraggableType inventoryCollectionHoldableType;
        
        private List<InventoryCollectionSlot>  _slots = new();
        
        private Vector2 _dimensions;
        
        private void Start()
        {
            InventoriesManager.Instance.AddInventory(this);
        }

        private void OnDestroy()
        {
            InventoriesManager.Instance.RemoveInventory(this);
        }

        public Vector2 GetDimensions()
        {
            return _dimensions;
        }
    }
}