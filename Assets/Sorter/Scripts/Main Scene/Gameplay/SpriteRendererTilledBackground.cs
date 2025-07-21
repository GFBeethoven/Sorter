using UnityEngine;
using Zenject;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererTilledBackground : MonoBehaviour
{
    [Inject] private GameViewport _viewport;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _viewport.Size.Subscribe(ViewportSizeChanged);
    }

    private void OnDisable()
    {
        _viewport.Size.Unsubscribe(ViewportSizeChanged);
    }

    private void ViewportSizeChanged(Vector2 size)
    {
        _spriteRenderer.size = size;
    }
}
