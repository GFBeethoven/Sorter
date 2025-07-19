using UnityEngine;

public class Belt : MonoBehaviour
{
    private const string HorizontalOffsetVelocityProperty = "_OffsetVelocity";

    [SerializeField] private float _beltBorderThickness;
    [SerializeField] private float _beltVelocity;

    [SerializeField] private SpriteRenderer _beltTop;
    [SerializeField] private SpriteRenderer _beltMiddle;
    [SerializeField] private SpriteRenderer _beltBottom;

    public Rect MiddleWorldRect
    {
        get
        {
            Vector2 size = _beltMiddle.size;

            return new Rect((Vector2)transform.localPosition - size / 2.0f, size);
        }
    }

    private bool _isInitialized;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_isInitialized) return;

        MaterialPropertyBlock block = new MaterialPropertyBlock();

        _beltBottom.GetPropertyBlock(block);

        block.SetFloat(HorizontalOffsetVelocityProperty, _beltVelocity);

        _beltBottom.SetPropertyBlock(block);

        _isInitialized = true;

        SetSize(1.0f, 1.0f);
    }

    public void SetSize(float width, float height)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        width = Mathf.Clamp(width, 0.0f, float.MaxValue);
        height = Mathf.Clamp(height, 0.0f, float.MaxValue);

        float borderThickness = _beltBorderThickness;

        if (height < borderThickness * 2.0f)
        {
            borderThickness = height / 2.0f;
        }

        _beltTop.size = new Vector2(width, borderThickness);
        _beltBottom.size = new Vector2(width, borderThickness);

        float middleHeight = Mathf.Clamp(height - borderThickness * 2.0f, 0.0f, float.MaxValue);

        _beltMiddle.size = new Vector2(width, middleHeight);

        _beltTop.transform.localPosition = new Vector3(0.0f, middleHeight / 2.0f + borderThickness / 2.0f, 0.0f);
        _beltMiddle.transform.localPosition = Vector3.zero;
        _beltBottom.transform.localPosition = new Vector3(0.0f, -middleHeight / 2.0f - borderThickness / 2.0f);
    }
}
