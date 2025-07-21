using UnityEngine;
using Zenject;

[RequireComponent(typeof(WorldRectTransform))]
public class SortingGameplayFigureHole : MonoBehaviour
{
    [SerializeField] private SortingGameplayFigureHoleDraggableZone _draggableZone;

    [SerializeField] private FigureHole _view;

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

    public DraggableDropZone<SortingGameplayFigure> DraggableZone => _draggableZone;

    private FigureConfig.Data _targetFigure;
    public FigureConfig.Data TargetFigure => _targetFigure;

    private void OnEnable()
    {
        WorldRectTransform.Position.Subscribe(PositionChanged);
        WorldRectTransform.Size.Subscribe(SizeChanged);
    }

    private void OnDisable()
    {
        WorldRectTransform.Position.Unsubscribe(PositionChanged);
        WorldRectTransform.Size.Unsubscribe(SizeChanged);
    }

    public void Setup(FigureConfig.Data targetFigure)
    {
        _targetFigure = targetFigure;

        _draggableZone.Setup(targetFigure);

        _view.Setup(targetFigure);
    }

    public Vector3 GetHolePosition()
    {
        return _view.transform.position;
    }

    private void PositionChanged(Vector2 position)
    {
        transform.position = position;
    }

    private void SizeChanged(Vector2 size)
    {
        _draggableZone.SetSize(size);

        _view.SetSize(size.x, size.y);
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
