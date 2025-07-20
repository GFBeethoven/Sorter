using UnityEngine;
using Zenject;

[RequireComponent(typeof(WorldRectTransform))]
public class SortingGameplayFigureHole : DraggableDropZone<SortingGameplayFigure>
{
    private WorldRectTransform _rectTransform;
    public WorldRectTransform WorldRectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<WorldRectTransform>();
            }

            return _rectTransform;
        }
    }

    private FigureConfig.Data _targetFigure;

    public void Setup(FigureConfig.Data targetFigure)
    {
        _targetFigure = targetFigure;
    }

    protected override void OnAddItem(SortingGameplayFigure newItem)
    {
        newItem.CanDrag = false;
    }

    protected override bool IsNewItemValid(SortingGameplayFigure draggable)
    {
        if (_targetFigure == null || draggable == null) return false;

        return _targetFigure.Id == draggable.FigureId;
    }

    public class Pool : MonoMemoryPool<FigureConfig.Data, SortingGameplayFigureHole>
    {
        protected override void Reinitialize(FigureConfig.Data data, SortingGameplayFigureHole item)
        {
            base.Reinitialize(data, item);

            item.Setup(data);
        }
    }
}
