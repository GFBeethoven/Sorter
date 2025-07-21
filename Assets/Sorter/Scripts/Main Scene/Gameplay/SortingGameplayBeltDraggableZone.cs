using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SortingGameplayBeltDraggableZone : DraggableDropZone<SortingGameplayFigure>, IDraggableItemSizeFitter
{
    private Vector3 _start;
    private Vector3 _end;

    private List<SortingGameplayFigure> _figuresToDestroy = new();

    public void SetEdgePoints(Vector3 start, Vector3 end)
    {
        _start = start;
        _end = end;

        ForceRecalculate();
    }

    public void SetSize(Vector2 size)
    {
        transform.localScale = new Vector3(size.x, size.y, 1.0f);
    }

    public void Tick()
    {
        bool hasDestroyed = false;

        for (int i = 0; i < DraggablesCount; i++)
        {
            SortingGameplayFigure figure = GetItem(i);

            figure.RelativePosition.Value += figure.RelativeVelocity * Time.deltaTime;

            if (figure.RelativePosition.Value >= 1.0f)
            {
                figure.CanDrag = false;

                hasDestroyed = true;
            }
        }

        if (hasDestroyed)
        {
            _figuresToDestroy.Clear();

            for (int i = 0; i < DraggablesCount; i++)
            {
                SortingGameplayFigure figure = GetItem(i);

                figure.RelativePosition.Value += figure.RelativeVelocity * Time.deltaTime;

                if (figure.RelativePosition.Value >= 1.0f)
                {
                    _figuresToDestroy.Add(figure);  
                }
            }

            for (int i = 0; i < _figuresToDestroy.Count; i++)
            {
                SignalBus.Fire<SortingGameplayFigureDestroyed>(new SortingGameplayFigureDestroyed(_figuresToDestroy[i]));
            }

            _figuresToDestroy.Clear();
        }
    }

    public Vector3 GetBeltPosition(float relativePosition)
    {
        return Vector3.Lerp(_start, _end, relativePosition);
    }

    protected override void OnAddItem(SortingGameplayFigure newDraggable)
    {
        newDraggable.RelativePositionChanged += DraggableRelativePositionChanged;
    }

    protected override void OnRemoveItem(SortingGameplayFigure oldDraggable)
    {
        oldDraggable.RelativePositionChanged -= DraggableRelativePositionChanged;
    }

    protected override bool IsNewItemValid(SortingGameplayFigure draggable)
    {
        return base.IsNewItemValid(draggable) && draggable.IsOnThisBelt(this);
    }

    private void DraggableRelativePositionChanged(SortingGameplayFigure figure, float relativePosition)
    {
        figure.transform.position = Vector3.Lerp(_start, _end, relativePosition);
    }

    private void ForceRecalculate()
    {
        int count = DraggablesCount;

        for (int i = 0; i < count; i++)
        {
            SortingGameplayFigure figure = GetItem(i);

            DraggableRelativePositionChanged(figure, figure.RelativePosition.Value);
        }
    }

    Vector2 IDraggableItemSizeFitter.GetPrefferedSize(Draggable draggable)
    {
        float size = Mathf.Min(transform.localScale.x, transform.localScale.y);

        float factor = size /
            Mathf.Min(draggable.CachedTransform.localScale.x, draggable.CachedTransform.localScale.y);

        return draggable.CachedTransform.localScale * factor * 0.85f;
    }
}
