using System;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public float radius;
    public float easing = 12;
    public ObjectDraggableManager.DraggableType[] draggableType;
    
    private ObjectDraggableManager _objectDraggableManager;

    protected virtual void Start()
    {
        _objectDraggableManager = ObjectDraggableManager.Instance;
        _objectDraggableManager.AddDraggableObject(this);
    }

    protected virtual void OnDestroy()
    {
        _objectDraggableManager.RemoveDraggableObject(this);
    }
}
