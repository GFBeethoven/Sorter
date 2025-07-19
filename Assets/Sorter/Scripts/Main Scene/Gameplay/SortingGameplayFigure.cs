using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingGameplayFigure : Draggable
{
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

    public int FigureId
    {
        get
        {
            if (Data == null) return -1;

            return Data.Id;
        }
    }

    private float _relativeVelocity;
    public float RelativeVelocity => _relativeVelocity;

    public void Setup(FigureConfig.Data data, SortingGameplayBelt belt, float relativeVelocity)
    {
        Data = data;
        _belt = belt;
        _relativeVelocity = relativeVelocity;
    }

    protected override void SetOwner(IDraggableDropZone owner)
    {
        base.SetOwner(owner);

        if (Owner == null && _belt != null)
        {
            base.SetOwner(_belt);
        }
    }
}
