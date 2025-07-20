using UnityEngine;
using Zenject;

[RequireComponent(typeof(WorldRectTransform))]
public class SortingGameplayBelt : DraggableDropZone<SortingGameplayFigure>
{
    [SerializeField] private Belt _belt;

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

    public void Tick()
    {
        int count = DraggablesCount;

        for (int i = 0; i < count; i++)
        {
            SortingGameplayFigure figure = GetItem(i);

            figure.RelativePosition.Value += figure.RelativeVelocity * Time.deltaTime;

            if (figure.RelativePosition.Value >= 1.0f)
            {
                figure.CanDrag = false;

                SignalBus.Fire<SortingGameplayFigureDestroyed>(new SortingGameplayFigureDestroyed(figure));
            }
        }
    }

    protected override void OnAddItem(SortingGameplayFigure newDraggable)
    {
        newDraggable.RelativePositionChanged += DraggableRelativePositionChanged;
    }

    protected override void OnRemoveItem(SortingGameplayFigure oldDraggable)
    {
        oldDraggable.RelativePositionChanged -= DraggableRelativePositionChanged;
    }

    private void DraggableRelativePositionChanged(SortingGameplayFigure figure, float relativePosition)
    {
        Rect rect = _belt.MiddleWorldRect;

        Vector3 position = new Vector3(rect.xMin + rect.width * relativePosition, rect.center.y, 0.0f);

        figure.transform.position = position;
    }

    public class Pool : MonoMemoryPool<SortingGameplayBelt> { }
}
