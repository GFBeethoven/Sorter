using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LineWorldLayoutGroup : MonoBehaviour
{
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

    private GameViewport _viewport;

    public void Initialize(GameViewport viewport, Vector2 anchorMin, Vector2 anchorMax)
    {
        _transform = transform;

        _viewport = viewport;

        MinAnchor = new ReactiveProperty<Vector2>(anchorMin);
        MaxAnchor = new ReactiveProperty<Vector2>(anchorMax);

        Children = new();

        _anchorWorldPositions = new AnchorWorldPositions(viewport, MinAnchor.Value, MaxAnchor.Value);

        MinAnchor.Subscribe(AnchorMinChanged);
        MaxAnchor.Subscribe(AnchorMaxChanged);

        Children.OnItemAdd += OnChildrenAdd;
        Children.OnItemRemove += OnChildrenRemove;
        Children.OnBeforeClear += OnBeforeClear;

        viewport.Size.SubscribeWithEnableBinding(OnViewportSizeChanged, this);
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

        Vector2 position = Vector2.zero;
        Vector2 size = Vector2.zero;
        Vector2 direction = Vector2.zero;

        if (_axis == Axis.Horizontal)
        {
            size = _anchorWorldPositions.AreaSize;
            size.x /= Children.Count;
            position = (Vector2)_anchorWorldPositions.LBWorldPosition + size / 2.0f;
            direction = new Vector2(size.x, 0.0f);
        }
        else if (_axis == Axis.Vertical)
        {
            size = _anchorWorldPositions.AreaSize;
            size.y /= Children.Count;
            position = (Vector2)_anchorWorldPositions.RTWorldPosition - size / 2.0f;
            direction = new Vector2(0.0f, -size.y);
        }

        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];

            child.Position.Value = position;
            child.Size.Value = size;

            position += direction;
        }
    }

    public enum Axis
    {
        Horizontal,
        Vertical
    }
}
