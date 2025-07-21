using CartoonFX;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

public class SortingGameplayFXs : MonoBehaviour, IInitializable
{
    [Inject] private GameViewport _viewport;

    [Inject] private SignalBus _signalBus;

    [SerializeField] private SortingGameplayEffect _figureExplosion;
    [SerializeField] private SortingGameplayEffect _winExplosion;
    [SerializeField] private SortingGameplayEffect _loseExplosion;

    private ObjectPool<SortingGameplayEffect> _figureExplosions;
    private ObjectPool<SortingGameplayEffect> _winExplosions;
    private ObjectPool<SortingGameplayEffect> _loseExplosions;

    private Action<SortingGameplayEffect> _releaseFigureExplosion;
    private Action<SortingGameplayEffect> _releaseWinExplosion;
    private Action<SortingGameplayEffect> _releaseLoseExplosion;

    private void OnEnable()
    {
        _signalBus.Subscribe<SortingGameplayFigureDestroyed>(FigureDestroyed);
        _signalBus.Subscribe<SortingGameplayWin>(OnWin);
        _signalBus.Subscribe<SortingGameplayLose>(OnLose);
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<SortingGameplayFigureDestroyed>(FigureDestroyed);
        _signalBus.TryUnsubscribe<SortingGameplayWin>(OnWin);
        _signalBus.TryUnsubscribe<SortingGameplayLose>(OnLose);
    }

    public void Initialize()
    {
        _figureExplosions = new ObjectPool<SortingGameplayEffect>
            (
            createFunc: () =>
            {
                var newItem = Instantiate(_figureExplosion, transform);
                newItem.gameObject.SetActive(false);
                return newItem;
            },
            actionOnRelease: (e) =>
            {
                e.gameObject.SetActive(false);
            }
            );

        _winExplosions = new ObjectPool<SortingGameplayEffect>
            (
            createFunc: () =>
            {
                var newItem = Instantiate(_winExplosion, transform);
                newItem.gameObject.SetActive(false);
                return newItem;
            },
            actionOnRelease: (e) =>
            {
                e.gameObject.SetActive(false);
            }
            );

        _loseExplosions = new ObjectPool<SortingGameplayEffect>
            (
            createFunc: () =>
            {
                var newItem = Instantiate(_loseExplosion, transform);
                newItem.gameObject.SetActive(false);
                return newItem;
            },
            actionOnRelease: (e) =>
            {
                e.gameObject.SetActive(false);
            }
            );

        _releaseFigureExplosion = (e) => _figureExplosions.Release(e);
        _releaseWinExplosion = (e) => _winExplosions.Release(e);
        _releaseLoseExplosion = (e) => _loseExplosions.Release(e);
    }

    public void SortingFigureExplosion(Vector3 position)
    {
        var effect = _figureExplosions.Get();
        
        effect.Play(position, _releaseFigureExplosion);
    }

    private void FigureDestroyed(SortingGameplayFigureDestroyed signal)
    {
        if (signal.DestroyedFigure == null) return;

        SortingFigureExplosion(signal.DestroyedFigure.transform.position);
    }

    private void OnWin(SortingGameplayWin signal)
    {
        Timing.RunCoroutine(_CoverByParticles(_winExplosions, 2.5f, _releaseWinExplosion).CancelWith(gameObject));
    }

    private void OnLose(SortingGameplayLose signal)
    {
        Timing.RunCoroutine(_CoverByParticles(_loseExplosions, 2.5f, _releaseLoseExplosion).CancelWith(gameObject));
    }

    private IEnumerator<float> _CoverByParticles(ObjectPool<SortingGameplayEffect> effectsPool, float duration,
        Action<SortingGameplayEffect> releaseAction)
    {
        const float MinDelay = 0.1f;
        const float MaxDelay = 0.5f;

        float t = 0.0f;

        float nextCoverT = 0.0f;

        while (t < duration)
        {
            if (t >= nextCoverT)
            {
                nextCoverT = t + UnityEngine.Random.Range(MinDelay, MaxDelay);

                float x = UnityEngine.Random.Range(0.0f, 1.0f);
                float y = UnityEngine.Random.Range(0.0f, 1.0f);

                Vector3 position = -_viewport.Size.Value / 2.0f + new Vector2(x, y) * _viewport.Size.Value;

                var effect = effectsPool.Get();
                effect.Play(position, releaseAction);
            }

            yield return Timing.WaitForOneFrame;

            t += Timing.DeltaTime;
        }
    }
}
