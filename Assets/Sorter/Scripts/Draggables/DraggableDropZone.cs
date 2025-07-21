using UnityEngine;
using System.Collections.Generic;
using System;
using Zenject;

[RequireComponent(typeof(Collider2D))]
public abstract class DraggableDropZone<T> : MonoBehaviour, IDraggableDropZone where T : Draggable
{
    [Inject] private SignalBus _signalBus;

    protected SignalBus SignalBus => _signalBus;

    public const string LayerMask = "DraggableDropZone";

    private List<T> _draggables = new();
    private HashSet<T> _draggablesHashSet = new();

    public int DraggablesCount => _draggables.Count;

    private void OnEnable()
    {
        _signalBus.Subscribe<OnDragDraggable>(OnSomeDraggableDrag);
        _signalBus.Subscribe<OnStartDragDraggable>(OnSomeDraggableDragStart);
        _signalBus.Subscribe<OnEndDragDraggable>(OnSomeDraggableDragEnd);
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<OnDragDraggable>(OnSomeDraggableDrag);
        _signalBus.TryUnsubscribe<OnStartDragDraggable>(OnSomeDraggableDragStart);
        _signalBus.TryUnsubscribe<OnEndDragDraggable>(OnSomeDraggableDragEnd);
    }

    public void Dispose()
    {
        for (int i = 0; i < _draggables.Count; i++)
        {
            _draggables[i].Dispose();
        }

        _signalBus.TryUnsubscribe<OnDragDraggable>(OnSomeDraggableDrag);
        _signalBus.TryUnsubscribe<OnStartDragDraggable>(OnSomeDraggableDragStart);
        _signalBus.TryUnsubscribe<OnEndDragDraggable>(OnSomeDraggableDragEnd);

        _draggables.Clear();
        _draggablesHashSet.Clear();
    }

    protected T GetItem(int index)
    {
        return _draggables[index];
    }

    protected virtual void OnAddItem(T newDraggable) { }

    protected virtual void OnRemoveItem(T oldDraggable) { }

    protected virtual void OnItemDragStart(T draggable) { }

    protected virtual void OnItemDrag(T draggable) { }

    protected virtual void OnItemDragEnd(T draggable) { }

    protected virtual bool IsNewItemValid(T draggable)
    {
        return true;
    }

    private void OnSomeDraggableDragStart(OnStartDragDraggable signal)
    {
        if (signal.Draggable is T draggable && _draggablesHashSet.Contains(draggable))
        {
            OnItemDragStart(draggable);
        }
    }

    private void OnSomeDraggableDrag(OnDragDraggable signal)
    {
        if (signal.Draggable is T draggable && _draggablesHashSet.Contains(draggable))
        {
            OnItemDrag(draggable);
        }
    }

    private void OnSomeDraggableDragEnd(OnEndDragDraggable signal)
    {
        if (signal.Draggable is T draggable && _draggablesHashSet.Contains(draggable))
        {
            OnItemDragEnd(draggable);
        }
    }

    void IDraggableDropZone.TryAddItem(IDraggableReadOnly draggable)
    {
        if (draggable is T monoDraggable && !_draggablesHashSet.Contains(monoDraggable) &&
            IsNewItemValid(monoDraggable))
        {
            _draggables.Add(monoDraggable);
            _draggablesHashSet.Add(monoDraggable);

            OnAddItem(monoDraggable);
        }
    }

    void IDraggableDropZone.RemoveItem(IDraggableReadOnly draggable)
    {
        if (draggable is T monoDraggable)
        {
            _draggables.Remove(monoDraggable);
            _draggablesHashSet.Remove(monoDraggable);

            OnRemoveItem(monoDraggable);
        }
    }

    bool IDraggableDropZone.IsNewItemValid(IDraggableReadOnly draggable)
    {
        if (draggable is T monoDraggable)
        {
            return IsNewItemValid(monoDraggable);
        }

        return false;
    }
}

public interface IDraggableDropZone : IDisposable
{
    public void TryAddItem(IDraggableReadOnly draggable);

    public void RemoveItem(IDraggableReadOnly draggable);

    public bool IsNewItemValid(IDraggableReadOnly draggable);
}
