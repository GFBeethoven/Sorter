using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(Image))]
public class PopupImage : MonoBehaviour
{
    [SerializeField] private Vector2 _startAnchoredPosition;
    [SerializeField] private Vector2 _endAnchoredPosition;
    [SerializeField, Min(0.1f)] private float _duration;

    private CoroutineHandle _coroutineHandle;

    private RectTransform _rectTransform;
    private RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }

    private Image _image;
    private Image Image
    {
        get
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }

            return _image;
        }
    }

    private void OnEnable()
    {
        if (_coroutineHandle.IsValid)
        {
            Timing.KillCoroutines(_coroutineHandle);
        }

        _coroutineHandle = Timing.RunCoroutine(_Popup().CancelWith(gameObject));
    }

    private void OnDisable()
    {
        if (_coroutineHandle.IsValid)
        {
            Timing.KillCoroutines(_coroutineHandle);
        }
    }

    private IEnumerator<float> _Popup()
    {
        Color trans = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / _duration)
        {
            RectTransform.anchoredPosition = Vector2.LerpUnclamped(_startAnchoredPosition, _endAnchoredPosition, Easing.OutBack(i));
            Image.color = Color.Lerp(trans, Color.white, Easing.InOutQuad(i));
            
            yield return Timing.WaitForOneFrame;
        }

        RectTransform.anchoredPosition = _endAnchoredPosition;
        Image.color = Color.white;

        _coroutineHandle = new CoroutineHandle();
    }
}
