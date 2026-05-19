using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoryDraggableItem : Draggable
    {
        private ObjectDraggableManager.DraggableType _inventoryItemType;
        protected override void Start()
        {
            base.Start();
        }

        public bool IsValidDragDestination(InventoryCollection collection, InventoryCollectionSlot collectionSlot)
        {
            return _inventoryItemType == collection.inventoryCollectionHoldableType && collectionSlot.CanCollectItem(this);
        }
    }
}