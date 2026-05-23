using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Inventory;
using UI;
using UnityEngine;

public class ObjectDraggableManager : MonoBehaviour
{
    public enum DraggableType
    {
        Planet,
        PlanetVectorArrow,
        ShopItem,
        Interactable,
        InventoryItem
    }

    private readonly DraggableType[] _setupDraggables = {DraggableType.Planet, DraggableType.PlanetVectorArrow, DraggableType.InventoryItem };
    private readonly DraggableType[] _shopDraggables = {DraggableType.Interactable, DraggableType.ShopItem };
    
    
    private List<Draggable> _draggableObjects = new List<Draggable>();
    private Dictionary<Draggable, Vector3> _dragOffsets = new Dictionary<Draggable, Vector3>();
    public bool enableDragging;
    
    private Draggable _selectedObject;
    private bool _isObjectSelected = false;
    private Vector2 _initialDragPosition = new();
    
    public static ObjectDraggableManager Instance;
    
    private GameManager _gameManager;
    private ObjectGravityManager _objectGravityManager;
    private DialogueManager _dialogueManager;
    private Camera _cam;
    
    private void Awake()
    {
        _cam = Camera.main;
        if (Instance == null)
        {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _dialogueManager = DialogueManager.Instance;
        _gameManager = GameManager.Instance;
        _objectGravityManager = ObjectGravityManager.Instance;
    }

    public void AddDraggableObject(Draggable draggable)
    {
        _draggableObjects.Add(draggable);
    }
    
    public void RemoveDraggableObject(Draggable draggable)
    {
        _draggableObjects.Remove(draggable);
    }
    
    private void MouseUpDuringSetup()
    {
        if (_selectedObject.TryGetComponent<Tooltip>(out var tt))
        {
            tt.forceEnabled = false;
        }
        _dragOffsets[_selectedObject] = _cam.ScreenToWorldPoint(Input.mousePosition);
        _dragOffsets[_selectedObject] = new Vector3(_dragOffsets[_selectedObject].x, _dragOffsets[_selectedObject].y, 0);
        _selectedObject = null;
        _isObjectSelected = false;
        InventoriesManager.Instance.TryMoveInventoryItem((InventoryDraggableItem)_selectedObject, Input.mousePosition, _initialDragPosition);
    }

    private bool ValidDraggableObject(Draggable draggable) 
        // If the draggable is the right type
    {
        switch (_gameManager.gameState)
        {
            case GameManager.GameState.PostRound:
                return false;
            case GameManager.GameState.Shop:
                return draggable.draggableType == DraggableType.ShopItem;
            case GameManager.GameState.Setup:
                return draggable.draggableType is DraggableType.Planet or DraggableType.PlanetVectorArrow;
            case GameManager.GameState.Round:
            default:
                return false;
        }
    }
    
    
    private void MouseDownDuringSetup()
    {
        Vector3 pos = _cam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        foreach (var drag in _draggableObjects)
        {
            if(!ValidDraggableObject(drag)) continue;
            if ((pos - drag.transform.position).magnitude > drag.radius)
            {
                continue;
            }

            _selectedObject = drag;
            _isObjectSelected = true;
            if (!_dragOffsets.TryAdd(drag, pos))
            {
                _dragOffsets[drag] = pos;
            }

            _initialDragPosition = Input.mousePosition;
            break;
        }
    }

    private void Update()
    {
        if(enableDragging && !_dialogueManager.inDialogue) HandleDraggableMovement();
    }

    private void HandleDraggableMovement()
    {
        _objectGravityManager ??= ObjectGravityManager.Instance;
        foreach (var pair in _dragOffsets)
        {
            pair.Key.transform.position = Vector3.Lerp(
                pair.Key.transform.position,
                pair.Value,
                1f - Mathf.Exp(-pair.Key.easing * Time.deltaTime)
            );
        }
            
        if (Input.GetMouseButton(0))
        {
            if (_isObjectSelected)
            {
                _dragOffsets[_selectedObject] = _cam.ScreenToWorldPoint(Input.mousePosition);
                _dragOffsets[_selectedObject] = new Vector3(_dragOffsets[_selectedObject].x, _dragOffsets[_selectedObject].y, 0);
                if (_selectedObject.TryGetComponent<Tooltip>(out var tt))
                {
                    tt.forceEnabled = true;
                }
            }
            else
            {
                MouseDownDuringSetup();
            }
        }

        if (!Input.GetMouseButton(0) && _isObjectSelected)
        {
            MouseUpDuringSetup();
        }
            
        foreach (var mass in _objectGravityManager.MassSimulatedObjects)
        {
            if (mass is not PlanetObject p) continue;
            p.velocity = p.planetArrowController.arrowVector;
        } 
    }
}
