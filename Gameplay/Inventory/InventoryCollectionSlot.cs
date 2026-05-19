using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoryCollectionSlot : MonoBehaviour
    {
        private InventoryDraggableItem _currentItem  = null;

        public virtual bool CanCollectItem(InventoryDraggableItem item)
        {
            return _currentItem == null;
        }

        public virtual InventoryDraggableItem SetItemSlot(InventoryDraggableItem item)
        {
            if (!CanCollectItem(item)) return item;
            InventoryDraggableItem oldItem = _currentItem;
            _currentItem = item;
            return oldItem;
        }
    }
}