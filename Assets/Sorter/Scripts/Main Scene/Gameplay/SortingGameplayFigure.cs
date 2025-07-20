using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingGameplayFigure : Draggable
{
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

    private void OnEnable()
    {
        RelativePosition.Subscribe(InvokeRelativePositionChanged);
    }

    private void OnDisable()
    {
        RelativePosition.Unsubscribe(InvokeRelativePositionChanged);
    }

    public void Setup(FigureConfig.Data data, SortingGameplayBelt belt, float relativeVelocity)
    {
        Data = data;
        _belt = belt;
        _relativeVelocity = relativeVelocity;
        RelativePosition.Value = 0.0f;
        CanDrag = true;
    }

    protected override void SetOwner(IDraggableDropZone owner)
    {
        base.SetOwner(owner);

        if (Owner == null && _belt != null)
        {
            base.SetOwner(_belt);
        }
    }

    private void InvokeRelativePositionChanged(float position)
    {
        _relativePositionChanged?.Invoke(this, position);
    }

    public class Pool : MonoMemoryPool<FigureConfig.Data, SortingGameplayBelt, float, SortingGameplayFigure>
    {
        protected override void Reinitialize(FigureConfig.Data data, SortingGameplayBelt belt, float relativeVelocity, 
            SortingGameplayFigure item)
        {
            base.Reinitialize(data, belt, relativeVelocity, item);

            item.Setup(data, belt, relativeVelocity);
        }
    }
}
