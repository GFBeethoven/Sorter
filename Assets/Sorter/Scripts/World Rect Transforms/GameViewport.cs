using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameViewport : MonoBehaviour, IInitializable
{
    public ReadOnlyReactiveProperty<Vector2> Size => _size;
    private ReactiveProperty<Vector2> _size;

    public ReadOnlyReactiveProperty<float> Aspect => _aspect;
    private ReactiveProperty<float> _aspect;

    private Camera _camera;

    private bool _isInitialized = false;

    [Inject]
    private void Init(Camera camera)
    {
        if (_isInitialized) return;

        _camera = camera;

        _size = new ReactiveProperty<Vector2>(new Vector2(_camera.orthographicSize * 2.0f * _camera.aspect, 
            _camera.orthographicSize * 2.0f));

        _aspect = new ReactiveProperty<float>(_camera.aspect);

        _isInitialized = true;
    }

    public void Initialize()
    {
        Timing.RunCoroutine(_CheckAspectChange());
    }

    private IEnumerator<float> _CheckAspectChange()
    {
#if UNITY_EDITOR
        const float Period = 0.02f;
#else
        const float Period = 0.25f;
#endif

        while (true)
        {
            yield return Timing.WaitForSeconds(Period);

            if (_aspect.Value != _camera.aspect)
            {
                _size.Value = new Vector2(_camera.orthographicSize * 2.0f * _camera.aspect, _camera.orthographicSize * 2.0f);

                _aspect.Value = _camera.aspect;
            }
        }
    }
}
