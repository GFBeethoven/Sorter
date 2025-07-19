using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Draggable : MonoBehaviour, IDraggable
{
    public const string LayerMask = "Draggable";

    private Transform _transform;
    protected Transform CachedTransform
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

    private Collider _collider;
    protected Collider Collider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }

            return _collider;
        }
    }

    private Vector3 _pointerDeltaPosition;

    private IDraggableDropZone _owner;

    private void OnDestroy()
    {
        Dispose();
    }

    public virtual void Dispose()
    {
        (this as IDraggable).SetOwner(null);
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

    protected virtual void SetOwner(IDraggableDropZone owner)
    {
        if (_owner == owner) return;

        _owner?.RemoveItem(this);

        _owner = owner;

        _owner?.TryAddItem(this);
    }

    void IDraggable.DragStart(Vector3 pointerPosition)
    {
        OnDragStart(pointerPosition);
    }

    void IDraggable.Drag(Vector3 pointerPosition)
    {
        OnDrag(pointerPosition);
    }

    void IDraggable.DragEnd(Vector3 pointerPosition)
    {
        OnDragEnd(pointerPosition);
    }

    void IDraggable.SetOwner(IDraggableDropZone owner)
    {
        SetOwner(owner);
    }
}

public interface IDraggableReadOnly : IDisposable { }

public interface IDraggable : IDraggableReadOnly
{
    public void DragStart(Vector3 pointerPosition);

    public void Drag(Vector3 pointerPosition);

    public void DragEnd(Vector3 pointerPosition);

    public void SetOwner(IDraggableDropZone owner);
}
