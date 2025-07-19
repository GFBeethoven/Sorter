using UnityEngine;

public struct OnStartDragDraggable
{
    public Draggable Draggable;

    public OnStartDragDraggable(Draggable draggable)
    {
        Draggable = draggable;
    }
}

public struct OnDragDraggable
{
    public Draggable Draggable;

    public OnDragDraggable(Draggable draggable)
    {
        Draggable = draggable;
    }
}

public struct OnEndDragDraggable
{
    public Draggable Draggable;

    public OnEndDragDraggable(Draggable draggable)
    {
        Draggable = draggable;
    }
}

public struct OnCannotDragDraggable
{
    public Draggable Draggable;

    public OnCannotDragDraggable(Draggable draggable)
    {
        Draggable = draggable;
    }
}
