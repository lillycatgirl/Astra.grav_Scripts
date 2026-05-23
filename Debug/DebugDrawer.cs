using System.Collections.Generic;
using Gameplay.Inventory;
using UnityEngine;

namespace Debug
{
    public class DebugDrawer : MonoBehaviour
    {
        [Header("Debug Gizmo Settings")]
        [SerializeField] private bool arenaWalls;
        [SerializeField] private bool massVelocity;
        [SerializeField] private bool massForce;
        [SerializeField] private bool massAcceleration;
        [SerializeField] private bool inventoryBoundaries;
        
        // References
        private ObjectGravityManager _objectGravityManager;
        private ObjectDraggableManager _objectDraggableManager;
        private GameManager _gameManager;
        private InventoriesManager _inventoriesManager;
        void Start()
        {
            if (!Application.isEditor) this.enabled = false;
            _objectGravityManager = ObjectGravityManager.Instance;
            _objectDraggableManager = ObjectDraggableManager.Instance;
            _gameManager = GameManager.Instance;
            _inventoriesManager = InventoriesManager.Instance;
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            if (arenaWalls) DrawWalls();
            if (inventoryBoundaries) DrawInventories();
        }

        private void DrawInventories()
        {
            foreach (var inventoryCollection in _inventoriesManager.GetInventories())
            {
                Gizmos.color = Color.green;
                var rectTransform = inventoryCollection.GetComponent<RectTransform>();
                DrawRectTransform(rectTransform);
                foreach (var slot in inventoryCollection.GetSlots())
                {
                    Gizmos.color = new Color(0.99f, 0.87f, 0.09f);
                    if (!slot.CanCollectItem())
                    {
                        Gizmos.color = Color.red;
                    }

                    var slotRectTransform = slot.GetComponent<RectTransform>();
                    DrawRectTransform(slotRectTransform);
                }
            }
        }
        
        private static void DrawRectTransform(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector3 center = (corners[0] + corners[2]) / 2f;
            // get diagonal dist
            Vector3 size = corners[2] - corners[0];
            Gizmos.DrawWireCube(center, size);
        }
        
        private void DrawWalls()
        {
            Gizmos.color = Color.yellow;
            Vector3[] corners = new Vector3[4];
            corners[0] = new Vector3(_gameManager.planetPositionLimit.x, _gameManager.planetPositionLimit.y, -9); // Top right
            corners[1] = new Vector3(-_gameManager.planetPositionLimit.x, _gameManager.planetPositionLimit.y, -9); // Top Left
            corners[2] = new Vector3(_gameManager.planetPositionLimit.x, -_gameManager.planetPositionLimit.y, -9); // bottom Right
            corners[3] = new Vector3(-_gameManager.planetPositionLimit.x, -_gameManager.planetPositionLimit.y, -9); // Bottom Left
            
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[0], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[1], corners[3]);
        }
    }

}