using System;
using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoryCollectionSlot : MonoBehaviour
    {
        public InventoryDraggableItem currentItem  = null;
        private InventoryCollection _collection = null;
        [SerializeField] private Vector2 dimensions;

        // will be changed later, e.g. upgrading, stacking, using item on something.... idk
        public virtual bool CanCollectItem(InventoryDraggableItem item = null)
        {
            return currentItem == null;
        }

        public virtual InventoryDraggableItem SetItemSlot(InventoryDraggableItem item)
        {
            if (!CanCollectItem(item)) return item;
            InventoryDraggableItem oldItem = currentItem;
            currentItem = item;
            return oldItem;
        }

        private void Start()
        {
            _collection = GetComponentInParent<InventoryCollection>();
            _collection.AddSlot(this);
        }
        
        public Vector2 GetDimensions()
        {
            return dimensions;
        }
        
        public void SetItem(InventoryDraggableItem item)
        {
            currentItem = item;

            if (item != null)
            {
                item.transform.SetParent(transform);
                item.transform.localPosition = Vector3.zero;
            }
        }

        // most useful function ever
        public virtual void ClearSlot()
        {
            currentItem = null;
        }
    }
}