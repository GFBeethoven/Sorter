using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public struct AnchorWorldPositions
{
    private GameViewport _viewport;

    public Vector3 LBWorldPosition;
    public Vector3 RTWorldPosition;
    public Vector2 AreaSize;

    private Vector2 _minAnchor;
    private Vector2 _maxAnchor;

    private bool _anchorChangeHandled;
    private float _cameraAspect;

    public AnchorWorldPositions(GameViewport viewport, Vector2 minAnchor, Vector2 maxAnchor)
    {
        _viewport = viewport;

        _minAnchor = minAnchor;
        _maxAnchor = maxAnchor;

        LBWorldPosition = Vector3.zero;
        RTWorldPosition = Vector3.zero;
        AreaSize = Vector3.zero;

        _anchorChangeHandled = false;

        _cameraAspect = 0.0f;

        RecalculateInner();
    }

    public void Recalculate()
    {
        if (Mathf.Approximately(_viewport.Aspect.Value, _cameraAspect) && _anchorChangeHandled) return;

        RecalculateInner();
    }

    public void SetMinAnchor(Vector2 anchor)
    {
        _minAnchor = anchor;

        _anchorChangeHandled = false;
    }

    public void SetMaxAnchor(Vector2 anchor)
    {
        _maxAnchor = anchor;

        _anchorChangeHandled = false;
    }

    private void RecalculateInner()
    {
        Vector2 viewportSize = _viewport.Size.Value;
        Vector2 viewportSizeHalf = viewportSize / 2.0f;

        LBWorldPosition = -viewportSizeHalf + _minAnchor * viewportSize;
        RTWorldPosition = -viewportSizeHalf + _maxAnchor * viewportSize;

        LBWorldPosition.z = RTWorldPosition.z = 0.0f;

        AreaSize = RTWorldPosition - LBWorldPosition;
        AreaSize[0] = Mathf.Abs(AreaSize[0]);
        AreaSize[1] = Mathf.Abs(AreaSize[1]);

        _cameraAspect = _viewport.Aspect.Value;

        _anchorChangeHandled = true;
    }
}
