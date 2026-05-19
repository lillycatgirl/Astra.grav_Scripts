using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoryDraggableItem : Draggable
    {
        private InventoryHoldableType _inventoryItemType;
        protected override void Start()
        {
            base.Start();
        }
    }
}