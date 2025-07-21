using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FigureHole : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer SpriteRenderer
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

    public void Setup(FigureConfig.Data data)
    {
        SpriteRenderer.sprite = data.OnLineSprite;
    }

    public void SetSize(float width, float height)
    {
        float size = Mathf.Min(width, height);

        transform.localScale = new Vector3(size, size, 1.0f);
    }
}
