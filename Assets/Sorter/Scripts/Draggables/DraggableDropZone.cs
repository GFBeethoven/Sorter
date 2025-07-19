using UnityEngine;
using System.Collections.Generic;
using System;
using Zenject;

[RequireComponent(typeof(Collider))]
public abstract class DraggableDropZone : MonoBehaviour, IDraggableDropZone
{
    public const string LayerMask = "DraggableDropZone";

    private List<Draggable> _draggables = new();

    public int DraggablesCount => _draggables.Count;

    public void Dispose()
    {
        for (int i = 0; i < _draggables.Count; i++)
        {
            _draggables[i].Dispose();
        }
    }

    protected Draggable GetItem(int index)
    {
        return _draggables[index];
    }

    protected virtual void OnAddItem() { }

    protected virtual void OnRemoveItem() { }

    protected virtual void OnItemDragStart(Draggable draggable) { }

    protected virtual void OnItemDrag(Draggable draggable) { }

    protected virtual void OnItemDragEnd(Draggable draggable) { }

    protected virtual bool CanAcceptItem(IDraggable draggable)
    {
        return draggable is Draggable;
    }

    bool IDraggableDropZone.TryAddItem(IDraggableReadOnly draggable)
    {
        if (draggable is Draggable monoDraggable)
        {
            _draggables.Add(monoDraggable);

            OnAddItem();

            return true;
        }

        return false;
    }

    void IDraggableDropZone.RemoveItem(IDraggableReadOnly draggable)
    {
        if (draggable is Draggable monoDraggable)
        {
            _draggables.Remove(monoDraggable);

            OnRemoveItem();
        }
    }
}

public interface IDraggableDropZone : IDisposable
{
    public bool TryAddItem(IDraggableReadOnly draggable);

    public void RemoveItem(IDraggableReadOnly draggable);
}
