using UnityEngine;
using Zenject;

public class SortingGameplayFigureHoleDraggableZone : DraggableDropZone<SortingGameplayFigure>, IDraggableItemSizeFitter
{
    private FigureConfig.Data _targetFigure;

    public void Setup(FigureConfig.Data targetFigure)
    {
        _targetFigure = targetFigure;
    }

    public void SetSize(Vector2 size)
    {
        transform.localScale = new Vector3(size.x, size.y, 1.0f);
    }

    protected override void OnAddItem(SortingGameplayFigure newItem)
    {
        if (_targetFigure != null || newItem != null)
        {
            newItem.CanDrag = false;

            if (_targetFigure.Id == newItem.FigureId)
            {
                SignalBus.Fire<SortingGameplayFigureSorted>(new SortingGameplayFigureSorted(newItem));
            }
            else
            {
                SignalBus.Fire <SortingGameplayFigureDestroyed>(new SortingGameplayFigureDestroyed(newItem));
            }
        }
    }

    Vector2 IDraggableItemSizeFitter.GetPrefferedSize(Draggable draggable)
    {
        Vector3 scale = transform.localScale;

        float size = Mathf.Min(scale.x, scale.y);

        return new Vector2(size, size);
    }
}
