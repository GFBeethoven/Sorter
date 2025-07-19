using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(WorldRectTransform))]
public class WorldAspectFitter : MonoBehaviour
{
    [Inject] private GameViewport _viewport;

    public ReactiveProperty<Vector2> MinAnchor { get; private set; }
    public ReactiveProperty<Vector2> MaxAnchor { get; private set; }

    public ReadOnlyReactiveProperty<float> Factor => _factor;
    private ReactiveProperty<float> _factor;

    public WorldRectTransform WorldRectTransform => _worldRectTransform;

    private WorldRectTransform _worldRectTransform;

    private AnchorWorldPositions _worldPositions;

    private Transform _transform;

    public void Initialize(Vector2 minAnchor, Vector2 maxAnchor)
    {
        _transform = transform;

        _worldRectTransform = GetComponent<WorldRectTransform>();

        _factor = new ReactiveProperty<float>(1.0f);

        MinAnchor = new ReactiveProperty<Vector2>(minAnchor);
        MaxAnchor = new ReactiveProperty<Vector2>(maxAnchor);

        _worldPositions = new AnchorWorldPositions(_viewport, minAnchor, maxAnchor);

        MinAnchor.Subscribe(OnAnchorChanged);
        MaxAnchor.Subscribe(OnAnchorChanged);
        _viewport.Size.Subscribe(OnViewportChanged);
        _worldRectTransform.Size.Subscribe(OnSizeChanged);
        _worldRectTransform.Scale.Subscribe(OnScaleChanged);
    }

    private void OnScaleChanged(Vector3 scale)
    {
        _worldPositions.Recalculate();

        Vector3 size = _worldRectTransform.Size.Value;

        _transform.localScale = scale;

        _transform.position = (_worldPositions.LBWorldPosition + _worldPositions.RTWorldPosition) / 2.0f - scale.x * size / 2.0f;
    }

    private void OnAnchorChanged(Vector2 size)
    {
        _worldPositions.SetMinAnchor(MinAnchor.Value);
        _worldPositions.SetMaxAnchor(MaxAnchor.Value);

        Fit();
    }

    private void OnViewportChanged(Vector2 size)
    {
        Fit();
    }

    private void OnSizeChanged(Vector2 size)
    {
        Fit();
    }

    private void Fit()
    {
        _worldPositions.Recalculate();

        Vector2 size = _worldRectTransform.Size.Value;

        float scale = Mathf.Min(_worldPositions.AreaSize.x / size.x, _worldPositions.AreaSize.y / size.y);

        _worldRectTransform.Scale.Value = new Vector3(scale, scale, 1.0f);

        _factor.Value = scale;
    }
}
