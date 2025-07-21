using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCanvas : MonoBehaviour
{
    [SerializeField] private AnimationCurve _inMovement;
    [SerializeField] private AnimationCurve _outMovement;

    [SerializeField] private RectTransform _panel;

    private CoroutineHandle _coroutineHandle;
    private Action _prevCallback;

    private void Awake()
    {
        _panel.gameObject.SetActive(false);
    }

    public void ShowTransition(Action onFullBlackoutCallback)
    {
        if (_coroutineHandle.IsValid)
        {
            Timing.KillCoroutines(_coroutineHandle);
        }

        _prevCallback?.Invoke();

        _prevCallback = onFullBlackoutCallback;

        _coroutineHandle = Timing.RunCoroutine(_Transition(onFullBlackoutCallback));
    }

    private IEnumerator<float> _Transition(Action middleCallback)
    {
        _panel.gameObject.SetActive(true);

        float maxInT = _inMovement.keys[^1].time;
        float maxOutT = _outMovement.keys[^1].time;

        float t = 0.0f;

        while (t < maxInT)
        {
            _panel.anchoredPosition = new Vector2(0.0f, _inMovement.Evaluate(t));

            yield return Timing.WaitForOneFrame;

            t += Timing.DeltaTime;
        }

        _panel.anchoredPosition = new Vector2(0.0f, _inMovement.Evaluate(maxInT));

        yield return Timing.WaitForOneFrame;

        if (_prevCallback == middleCallback)
        {
            _prevCallback = null;
        }

        middleCallback?.Invoke();

        t = 0.0f;

        while (t < maxOutT)
        {
            _panel.anchoredPosition = new Vector2(0.0f, _outMovement.Evaluate(t));

            yield return Timing.WaitForOneFrame;

            t += Timing.DeltaTime;
        }

        _panel.anchoredPosition = new Vector2(0.0f, _outMovement.Evaluate(1.0f));

        _panel.gameObject.SetActive(false);

        _coroutineHandle = new CoroutineHandle();
    }
}
