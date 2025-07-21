using UnityEngine;
using Zenject;

[RequireComponent(typeof(WorldRectTransform))]
public class SortingGameplayBelt : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)] private float _heightFactor = 1.0f;

    [SerializeField] private SortingGameplayBeltDraggableZone _draggableZone;

    [SerializeField] private Belt _view;

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

    public void Tick()
    {
        _draggableZone.Tick();
    }

    public Vector3 GetBeltPosition(float relativePosition)
    {
        return _draggableZone.GetBeltPosition(relativePosition);
    }

    private void PositionChanged(Vector2 position)
    {
        transform.position = position;

        SetupEdgePoints();
    }

    private void SizeChanged(Vector2 size)
    {
        size.y *= _heightFactor;

        _draggableZone.SetSize(size);

        _view.SetSize(size.x, size.y);

        SetupEdgePoints();
    }

    private void SetupEdgePoints()
    {
        Vector3 startPoint = _view.MiddleWorldRect.min + new Vector2(0.0f, _view.MiddleWorldRect.height / 2.0f);
        Vector3 endPoint = startPoint + new Vector3(_view.MiddleWorldRect.width, 0.0f);

        _draggableZone.SetEdgePoints(startPoint, endPoint);
    }

    public class Pool : MonoMemoryPool<SortingGameplayBelt> { }
}
