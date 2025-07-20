using TMPro;
using UnityEngine;
using Zenject;

public class SortingGameplayCanvas : MonoBehaviour, IInitializable
{
    [Inject] private SignalBus _signalBus;

    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private TextMeshProUGUI _health;

    public void Initialize()
    {
        _signalBus.Subscribe<SortingGameplayScoreChanged>(ScoreChanged);
        _signalBus.Subscribe<SortingGameplayHealthChanged>(HealthChanged);
    }

    private void HealthChanged(SortingGameplayHealthChanged signal)
    {
        _health.text = signal.CurrentHealth.ToString();
    }

    private void ScoreChanged(SortingGameplayScoreChanged signal)
    {
        _score.text = signal.CurrentScore.ToString();
    }
}
