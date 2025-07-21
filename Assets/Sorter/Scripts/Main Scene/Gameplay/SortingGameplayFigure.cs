using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Zenject;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingGameplayFigure : Draggable
{
    public const int DefaultSortingOrder = 5;
    public const int OnDragSortingOrder = 100;

    public readonly static Color DefaultColor = Color.white;
    public readonly static Color DroppedToHoleColor = Color.clear;

    private Action<SortingGameplayFigure, float> _relativePositionChanged;
    public event Action<SortingGameplayFigure, float> RelativePositionChanged
    {
        add
        {
            _relativePositionChanged += value;

            value?.Invoke(this, RelativePosition.Value);
        }

        remove
        {
            _relativePositionChanged -= value;

            value?.Invoke(this, RelativePosition.Value);
        }
    }

    private SpriteRenderer _spriteRenderer;
    protected SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            return _spriteRenderer;
        }
    }

    private SortingGameplayBelt _belt;
    private SortingGameplayFigureHole _targetHole;

    private FigureConfig.Data _data;
    private FigureConfig.Data Data
    {
        get
        {
            return _data;
        }

        set
        {
            _data = value;

            SpriteRenderer.sprite = IsDragging ? _data?.OnDragSprite : _data?.OnLineSprite;
        }
    }

    public int FigureId => Data?.Id ?? -1;

    private float _relativeVelocity;
    public float RelativeVelocity => _relativeVelocity;

    public ReactiveProperty<float> RelativePosition { get; private set; } = new ReactiveProperty<float>(0.0f);

    private CoroutineHandle _currentCoroutine;

    private void OnEnable()
    {
        RelativePosition.Subscribe(InvokeRelativePositionChanged);
    }

    private void OnDisable()
    {
        RelativePosition.Unsubscribe(InvokeRelativePositionChanged);

        StopCurrentCoroutine();
    }

    public void Setup(FigureConfig.Data data, SortingGameplayBelt belt, SortingGameplayFigureHole targetHole,
        float relativeVelocity)
    {
        Data = data;
        _belt = belt;
        _targetHole = targetHole;
        _relativeVelocity = relativeVelocity;
        RelativePosition.Value = 0.0f;
        CanDrag = true;
        CachedTransform.localScale = Vector3.one;
        SpriteRenderer.color = DefaultColor;
        SpriteRenderer.sortingOrder = DefaultSortingOrder;
        CachedTransform.localEulerAngles = new Vector3(0.0f, 0.0f, UnityEngine.Random.Range(-20.0f, 20.0f));

        SetOwner(_belt.DraggableZone);
    }

    public bool IsOnThisBelt(SortingGameplayBeltDraggableZone belt)
    {
        if (belt == null || _belt == null)
        {
            return false;
        }

        return _belt.DraggableZone == belt;
    }

    protected override void OnDragStart(Vector3 pointerPosition)
    {
        base.OnDragStart(pointerPosition);

        SpriteRenderer.sortingOrder = OnDragSortingOrder;

        StopCurrentCoroutine();
    }

    protected override void OnDragEnd(Vector3 pointerPosition)
    {
        base.OnDragEnd(pointerPosition);

        SpriteRenderer.sortingOrder = DefaultSortingOrder;
    }

    protected override void SetOwnerOnDrop(IDraggableDropZone owner)
    {
        if (owner == null)
        {
            SetOwner(null);

            ReturnToBelt();
        }
        else if (owner == (IDraggableDropZone)_targetHole.DraggableZone)
        {
            SetOwner(null);

            PlaceToHole();
        }
        else
        {
            SetOwner(owner);
        }
    }

    protected override void SetOwner(IDraggableDropZone owner)
    {
        base.SetOwner(owner);

        if (owner is IDraggableItemSizeFitter sizeFitter)
        {
            Vector2 size = sizeFitter.GetPrefferedSize(this);

            CachedTransform.localScale = new Vector3(size.x, size.y, 1.0f);
        }
        else if (_targetHole?.DraggableZone is IDraggableItemSizeFitter holeSizeFitter)
        {
            Vector2 size = holeSizeFitter.GetPrefferedSize(this);

            CachedTransform.localScale = new Vector3(size.x, size.y, 1.0f);
        }
    }

    private void InvokeRelativePositionChanged(float position)
    {
        _relativePositionChanged?.Invoke(this, position);
    }

    private void ReturnToBelt()
    {
        StopCurrentCoroutine();

        if (_belt == null) return;

        Vector3 onBeltSize = CachedTransform.localScale;

        if (_belt.DraggableZone is IDraggableItemSizeFitter sizeFitter)
        {
            onBeltSize = sizeFitter.GetPrefferedSize(this);
        }

        _currentCoroutine = Timing.RunCoroutine(_ReturnToBelt(_belt.GetBeltPosition(RelativePosition.Value),
            onBeltSize));
    }

    private void PlaceToHole()
    {
        StopCurrentCoroutine();

        if (_belt == null) return;

        Vector3 targetScale = CachedTransform.localScale;

        if (_targetHole.DraggableZone is IDraggableItemSizeFitter sizeFitter)
        {
            targetScale = sizeFitter.GetPrefferedSize(this);
        }

        _currentCoroutine = Timing.RunCoroutine(_PlaceToHole(_targetHole.GetHolePosition(),
            targetScale));
    }

    private void StopCurrentCoroutine()
    {
        if (_currentCoroutine.IsValid)
        {
            Timing.KillCoroutines(_currentCoroutine);
        }
    }

    private IEnumerator<float> _ReturnToBelt(Vector3 onBeltPosition, Vector3 onBeltScale)
    {
        const float Duration = 0.2f;
        const float BezierCurveAmplitude = 2.0f;

        Vector3 startPosition = CachedTransform.position;
        Vector3 middlePosition = (startPosition + onBeltPosition) / 2.0f;
        Vector3 endPosition = onBeltPosition;

        if (Mathf.Approximately((endPosition - startPosition).magnitude, 0.0f) == false)
        {
            middlePosition += (Vector3)Vector2.Perpendicular(endPosition - startPosition).normalized * BezierCurveAmplitude;
        }

        Vector3 startScale = CachedTransform.localScale;
        Vector3 endScale = onBeltScale;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            CachedTransform.position = BezierCurve(Easing.OutBack(i));
            CachedTransform.localScale = Vector3.LerpUnclamped(startScale, endScale, Easing.OutBack(i));

            yield return Timing.WaitForOneFrame;
        }

        CachedTransform.position = endPosition;
        CachedTransform.localScale = onBeltScale;

        SetOwner(_belt?.DraggableZone);

        Vector3 BezierCurve(float t)
        {
            return Mathf.Pow(1 - t, 2) * startPosition
                     + 2 * (1 - t) * t * middlePosition
                     + Mathf.Pow(t, 2) * endPosition;
        }
    }

    private IEnumerator<float> _PlaceToHole(Vector3 onHolePosition, Vector3 scale)
    {
        const float MoveDuration = 0.2f;
        const float DropDuration = 0.2f;

        CanDrag = false;

        Vector3 startPosition = CachedTransform.position;
        Vector3 endPosition = onHolePosition;

        Vector3 startScale = CachedTransform.localScale;
        Vector3 endScale = scale;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / MoveDuration)
        {
            CachedTransform.position = Vector3.Lerp(startPosition, endPosition, Easing.OutQuad(i));
            CachedTransform.localScale = Vector3.LerpUnclamped(startScale, endScale, i);

            yield return Timing.WaitForOneFrame;
        }

        CachedTransform.position = endPosition;
        CachedTransform.localScale = endScale;

        Color startColor = SpriteRenderer.color;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / DropDuration)
        {
            SpriteRenderer.color = Color.Lerp(startColor, DroppedToHoleColor, Easing.InQuad(i));
            CachedTransform.localScale = Vector3.Lerp(endScale, Vector3.zero, Easing.InQuad(i));

            yield return Timing.WaitForOneFrame;
        }

        SpriteRenderer.color = DroppedToHoleColor;
        CachedTransform.localScale = Vector3.zero;

        SetOwner(_targetHole?.DraggableZone);
    }

    public class Pool : MonoMemoryPool<FigureConfig.Data, SortingGameplayBelt, SortingGameplayFigureHole, float, SortingGameplayFigure>
    {
        protected override void Reinitialize(FigureConfig.Data data, SortingGameplayBelt belt, 
            SortingGameplayFigureHole targetHole, float relativeVelocity, SortingGameplayFigure item)
        {
            base.Reinitialize(data, belt, targetHole, relativeVelocity, item);

            item.Setup(data, belt, targetHole, relativeVelocity);
        }
    }
}
