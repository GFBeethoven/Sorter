using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour, IDraggable
{
    public const string LayerMask = "Draggable";

    [Inject] private SignalBus _signalBus;

    private Transform _transform;
    public Transform CachedTransform
    {
        get
        {
            if (_transform == null)
            {
                _transform = transform;
            }

            return _transform;
        }
    }

    private Collider2D _collider;
    protected Collider2D Collider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider2D>();
            }

            return _collider;
        }
    }

    private Vector3 _pointerDeltaPosition;

    private IDraggableDropZone _owner;
    protected IDraggableDropZone Owner => _owner;

    private bool _isDragging;
    public bool IsDragging => _isDragging;

    private bool _canDrag;
    public bool CanDrag
    {
        get
        {
            return _canDrag;
        }

        set
        {
            _canDrag = value;   

            if (!_canDrag)
            {
                if (IsDragging)
                {
                    _signalBus.Fire<OnCannotDragDraggable>(new OnCannotDragDraggable(this));
                }
            }
        }
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public virtual void Dispose()
    {
        SetOwner(null);
    }

    protected virtual void OnDragStart(Vector3 pointerPosition)
    {
        _pointerDeltaPosition = CachedTransform.position - pointerPosition;
    }

    protected virtual void OnDrag(Vector3 pointerPosition)
    {
        CachedTransform.position = _pointerDeltaPosition + pointerPosition;
    }

    protected virtual void OnDragEnd(Vector3 pointerPosition)
    {
        CachedTransform.position = _pointerDeltaPosition + pointerPosition;
    }

    protected virtual void DropOwnerOnDragStart()
    {
        SetOwner(null);
    }

    protected virtual void SetOwnerOnDrop(IDraggableDropZone owner)
    {
        SetOwner(owner);
    }

    protected virtual void SetOwner(IDraggableDropZone owner)
    {
        if (_owner == owner) return;

        _owner?.RemoveItem(this);

        _owner = owner;

        _owner?.TryAddItem(this);
    }

    void IDraggable.DragStart(Vector3 pointerPosition)
    {
        _isDragging = true;

        OnDragStart(pointerPosition);
    }

    void IDraggable.Drag(Vector3 pointerPosition)
    {
        OnDrag(pointerPosition);
    }

    void IDraggable.DragEnd(Vector3 pointerPosition)
    {
        _isDragging = false;

        OnDragEnd(pointerPosition);
    }

    void IDraggable.DropOwnerOnDragStart()
    {
        DropOwnerOnDragStart();
    }

    void IDraggable.SetOwnerOnDrop(IDraggableDropZone owner)
    {
        SetOwnerOnDrop(owner);
    }
}

public interface IDraggableReadOnly : IDisposable { }

public interface IDraggable : IDraggableReadOnly
{
    public bool CanDrag { get; }

    public void DragStart(Vector3 pointerPosition);

    public void Drag(Vector3 pointerPosition);

    public void DragEnd(Vector3 pointerPosition);

    public void DropOwnerOnDragStart();

    public void SetOwnerOnDrop(IDraggableDropZone owner);
}
