using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoriesManager : MonoBehaviour
    {
        public static InventoriesManager Instance;

        private readonly List<InventoryCollection> _inventories = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddInventory(InventoryCollection inventory)
        {
            if (!_inventories.Contains(inventory))
            {
                _inventories.Add(inventory);
            }
        }

        public void RemoveInventory(InventoryCollection inventory)
        {
            _inventories.Remove(inventory);
        }

        public List<InventoryCollection> GetInventories()
        {
            return _inventories;
        }

        public void TryMoveInventoryItem(
            InventoryDraggableItem item,
            Vector2 finalPosition,
            Vector2 initialPosition)
        {
            InventoryCollectionSlot sourceSlot =
                GetSlotAtScreenPosition(initialPosition);

            InventoryCollectionSlot targetSlot =
                GetSlotAtScreenPosition(finalPosition);

            if (sourceSlot == null || targetSlot == null)
            {
                print("not a slot :3");
                return;
            }

            if (sourceSlot == targetSlot)
            {
                print("... same thing");
                return;
            }

            if (!CanMoveToSlot(item, targetSlot))
            {
                print("not working! wrong type 3:");
                return;
            }

            MoveOrSwap(sourceSlot, targetSlot);
        }

        private bool CanMoveToSlot(
            InventoryDraggableItem item,
            InventoryCollectionSlot targetSlot)
        {
            InventoryCollection inventory =
                targetSlot.GetComponentInParent<InventoryCollection>();

            return inventory.inventoryCollectionHoldableType == item.draggableType;
        }

        private void MoveOrSwap(
            InventoryCollectionSlot sourceSlot,
            InventoryCollectionSlot targetSlot)
        {
            InventoryDraggableItem sourceItem = sourceSlot.currentItem;
            InventoryDraggableItem targetItem = targetSlot.currentItem;

            // Swap!!!
            if (targetItem != null)
            {
                sourceSlot.SetItem(targetItem);
            }
            else
            {
                sourceSlot.ClearSlot();
            }

            targetSlot.SetItem(sourceItem);
        }

        private InventoryCollectionSlot GetSlotAtScreenPosition(
            Vector2 screenPosition)
        {
            foreach (InventoryCollection inventory in _inventories)
            {
                foreach (InventoryCollectionSlot slot in inventory.GetSlots())
                {
                    RectTransform rect =
                        slot.GetComponent<RectTransform>();

                    if (rect == null)
                        continue;
                    // TODO swap prefabs from old values to new RectTransforms, in DebugDrawer too :3
                    if (RectTransformUtility.RectangleContainsScreenPoint(
                            rect,
                            screenPosition))
                    {
                        return slot;
                    }
                }
            }

            return null;
        }
    }
}