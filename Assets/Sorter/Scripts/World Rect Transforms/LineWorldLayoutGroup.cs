using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LineWorldLayoutGroup : MonoBehaviour
{
    [Inject] private GameViewport _viewport;

    public ReactiveProperty<Vector2> MinAnchor { get; private set; }
    public ReactiveProperty<Vector2> MaxAnchor { get; private set; }
    public ReactiveList<WorldRectTransform> Children { get; private set; }

    private Axis _axis;
    public Axis Direction
    {
        get
        {
            return _axis;
        }

        set
        {
            bool changed = _axis != value;

            _axis = value;

            if (changed)
            {
                RecalculatePositions();
            }
        }
    }

    private AnchorWorldPositions _anchorWorldPositions;

    private Transform _transform;

    public void Initialize(Vector2 anchorMin, Vector2 anchorMax)
    {
        _transform = transform;

        MinAnchor = new ReactiveProperty<Vector2>(anchorMin);
        MaxAnchor = new ReactiveProperty<Vector2>(anchorMax);

        Children = new();

        _anchorWorldPositions = new AnchorWorldPositions(_viewport, MinAnchor.Value, MaxAnchor.Value);

        MinAnchor.Subscribe(AnchorMinChanged);
        MaxAnchor.Subscribe(AnchorMaxChanged);

        Children.OnItemAdd += OnChildrenAdd;
        Children.OnItemRemove += OnChildrenRemove;
        Children.OnBeforeClear += OnBeforeClear;

        _viewport.Size.Subscribe(OnViewportSizeChanged);
    }

    private void OnChildrenAdd(int index, WorldRectTransform child)
    {
        child.transform.SetParent(_transform);
        child.transform.SetSiblingIndex(index);

        RecalculatePositions();
    }

    private void OnChildrenRemove(int index, WorldRectTransform child)
    {
        if (child.transform.IsChildOf(_transform))
        {
            child.transform.SetParent(null);
        }

        RecalculatePositions();
    }

    private void OnBeforeClear()
    {
        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i].transform.IsChildOf(_transform))
            {
                Children[i].transform.SetParent(null);
            }
        }
    }

    private void AnchorMinChanged(Vector2 anchorMin)
    {
        _anchorWorldPositions.SetMinAnchor(anchorMin);

        RecalculatePositions();
    }

    private void AnchorMaxChanged(Vector2 anchorMax)
    {
        _anchorWorldPositions.SetMaxAnchor(anchorMax);

        RecalculatePositions();
    }

    private void OnViewportSizeChanged(Vector2 size)
    {
        RecalculatePositions();
    }

    private void RecalculatePositions()
    {
        if (Children.Count == 0) return;

        _anchorWorldPositions.Recalculate();

        float cellSize = 0.0f;
        Vector3 position = Vector3.zero;
        Vector3 direction = Vector3.zero;

        if (_axis == Axis.Horizontal)
        {
            cellSize = Mathf.Min(_anchorWorldPositions.AreaSize.x / Children.Count, _anchorWorldPositions.AreaSize.y);
            position = _anchorWorldPositions.LBWorldPosition + new Vector3(cellSize, cellSize, 0.0f) / 2.0f;
            direction = new Vector3(cellSize, 0.0f);
        }
        else if (_axis == Axis.Vertical)
        {
            cellSize = Mathf.Min(_anchorWorldPositions.AreaSize.x, _anchorWorldPositions.AreaSize.y / Children.Count);
            position = _anchorWorldPositions.RTWorldPosition - new Vector3(cellSize, cellSize, 0.0f) / 2.0f;
            direction = new Vector3(0.0f, -cellSize);
        }

        float minScale = float.MaxValue;

        for (int i = 0; i < Children.Count; i++)
        {
            Vector2 size = Children[i].Size.Value;

            if (Mathf.Approximately(size.x, 0.0f) || Mathf.Approximately(size.y, 0.0f)) continue;

            float scale = cellSize / Mathf.Max(size.x, size.y);

            if (scale < minScale)
            {
                minScale = scale;
            }
        }

        Vector3 scale3 = new Vector3(minScale, minScale, 1.0f);

        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];

            child.Scale.Value = scale3;
            child.transform.localPosition = position;

            position += direction;
        }
    }

    public enum Axis
    {
        Horizontal,
        Vertical
    }
}
