using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using Zenject;

public class SortingGameplayCanvas : MonoBehaviour, IInitializable
{
    [Inject] private SignalBus _signalBus;

    [SerializeField] private Image _healthIcon;
    [SerializeField] private Image _scoreIcon;

    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private TextMeshProUGUI _health;

    private int _prevHealth = int.MinValue;
    private int _prevScore = int.MinValue;

    private CoroutineHandle _healthDropCoroutine;
    private CoroutineHandle _scoreHighlightCoroutine;

    public void Initialize()
    {
        _signalBus.Subscribe<SortingGameplayScoreChanged>(ScoreChanged);
        _signalBus.Subscribe<SortingGameplayHealthChanged>(HealthChanged);
    }

    private void OnDisable()
    {
        StopDropHealthIcon();

        StopHighlightScoreIcon();
    }

    private void HealthChanged(SortingGameplayHealthChanged signal)
    {
        _health.text = signal.CurrentHealth.ToString();

        if (_prevHealth > signal.CurrentHealth)
        {
            DropHealthIcon();
        }

        _prevHealth = signal.CurrentHealth;
    }

    private void ScoreChanged(SortingGameplayScoreChanged signal)
    {
        _score.text = $"{signal.CurrentScore}";

        if (_prevScore < signal.CurrentScore)
        {
            HighlightScoreIcon();
        }

        _prevScore = signal.CurrentScore;
    }

    private void DropHealthIcon()
    {
        StopDropHealthIcon();

        _healthDropCoroutine = Timing.RunCoroutine(_DropHealthIcon().CancelWith(gameObject));
    }

    private void StopDropHealthIcon()
    {
        if (_healthDropCoroutine.IsValid)
        {
            Timing.KillCoroutines(_healthDropCoroutine);
        }

        _healthIcon.rectTransform.anchoredPosition = Vector3.zero;
        _healthIcon.color = Color.white;
    }

    private void HighlightScoreIcon()
    {
        StopHighlightScoreIcon();

        _scoreHighlightCoroutine = Timing.RunCoroutine(_HighlightScoreIcon().CancelWith(gameObject));
    }

    private void StopHighlightScoreIcon()
    {
        if (_scoreHighlightCoroutine.IsValid)
        {
            Timing.KillCoroutines(_scoreHighlightCoroutine);
        }

        _scoreIcon.rectTransform.localScale = Vector3.one;
        _scoreIcon.color = Color.white;
    }

    private IEnumerator<float> _DropHealthIcon()
    {
        const float Duration = 0.3f;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            _healthIcon.rectTransform.anchoredPosition = new Vector2(0.0f, -Easing.InBack(i) * _scoreIcon.rectTransform.sizeDelta.y * 2.0f);
            _healthIcon.color = Color.Lerp(Color.white, Color.clear, Easing.InOutQuad(i));

            yield return Timing.WaitForOneFrame;
        }

        StopDropHealthIcon();
    }

    private IEnumerator<float> _HighlightScoreIcon()
    {
        const float Duration = 0.3f;
        const float IconUpscale = 2.3f;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            _scoreIcon.rectTransform.localScale = Vector3.one * Mathf.Lerp(1.0f, IconUpscale, Easing.OutQuad(i));
            _scoreIcon.color = Color.Lerp(Color.white, Color.clear, Easing.InOutQuad(i));

            yield return Timing.WaitForOneFrame;
        }

        StopHighlightScoreIcon();
    }
}
