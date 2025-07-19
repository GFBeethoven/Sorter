using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Draggable : MonoBehaviour, IDraggable
{
    public const string LayerMask = "Draggable";

    private Action<IDraggableReadOnly> _onDragStart;
    event Action<IDraggableReadOnly> IDraggableReadOnly.OnDragStart
    {
        add
        {
            _onDragStart += value;
        }

        remove
        {
            _onDragStart -= value;
        }
    }

    private Action<IDraggableReadOnly> _onDrag;
    event Action<IDraggableReadOnly> IDraggableReadOnly.OnDrag
    {
        add
        {
            _onDrag += value;
        }

        remove
        {
            _onDrag -= value;
        }
    }

    private Action<IDraggableReadOnly> _onDragEnd;
    event Action<IDraggableReadOnly> IDraggableReadOnly.OnDragEnd
    {
        add
        {
            _onDragEnd += value;
        }

        remove
        {
            _onDragEnd -= value;
        }
    }

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

    public void Dispose()
    {
        (this as IDraggable).SetOwner(null);

        _onDragStart = null;
        _onDragEnd = null;
        _onDrag = null;
    }

    protected virtual void OnDragStartHandler() { }

    protected virtual void OnDragHandler() { }

    protected virtual void OnDragEndHandler() { }

    void IDraggable.DragStart(Vector3 pointerPosition)
    {
        _pointerDeltaPosition = CachedTransform.position - pointerPosition;

        OnDragStartHandler();

        _onDragStart?.Invoke(this);
    }

    void IDraggable.Drag(Vector3 pointerPosition)
    {
        CachedTransform.position = _pointerDeltaPosition + pointerPosition;

        OnDragHandler();

        _onDrag?.Invoke(this);
    }

    void IDraggable.DragEnd(Vector3 pointerPosition)
    {
        CachedTransform.position = _pointerDeltaPosition + pointerPosition;

        OnDragEndHandler();

        _onDragEnd?.Invoke(this);
    }

    void IDraggable.SetOwner(IDraggableDropZone owner)
    {
        if (_owner == owner) return;

        _owner?.RemoveItem(this);

        _owner = owner;

        _owner?.TryAddItem(this);
    }
}

public interface IDraggableReadOnly : IDisposable
{
    public event Action<IDraggableReadOnly> OnDragStart;

    public event Action<IDraggableReadOnly> OnDrag;

    public event Action<IDraggableReadOnly> OnDragEnd;
}

public interface IDraggable : IDraggableReadOnly
{
    public void DragStart(Vector3 pointerPosition);

    public void Drag(Vector3 pointerPosition);

    public void DragEnd(Vector3 pointerPosition);

    public void SetOwner(IDraggableDropZone owner);
}
