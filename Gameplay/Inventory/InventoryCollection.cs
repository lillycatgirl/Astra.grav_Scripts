using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoryCollection : MonoBehaviour
    {
        public ObjectDraggableManager.DraggableType inventoryCollectionHoldableType;
        
        private List<InventoryCollectionSlot> _slots = new();
        
        [SerializeField] private Vector2 dimensions;
        
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
            return dimensions;
        }

        public void AddSlot(InventoryCollectionSlot slot)
        {
            _slots.Add(slot);
        }
        public void RemoveSlot(InventoryCollectionSlot slot)
        {
            _slots.Remove(slot);
        }
        public List<InventoryCollectionSlot> GetSlots()
        {
            return _slots;
        }
    }
}