using UnityEngine;
using Zenject;

public class SortingGameplay
{
    private SignalBus _signalBus;

    private ReactiveProperty<int> _health;
    private ReactiveProperty<int> _score;

    private bool _isLaunched;

    private LaunchParameters _parameters;

    public SortingGameplay(SignalBus signalBus)
    {
        _signalBus = signalBus;

        _health = new ReactiveProperty<int>(0);
        _score = new ReactiveProperty<int>(0);
    }

    public void Launch(LaunchParameters parameters)
    {
        if (_isLaunched) return;

        _isLaunched = true;

        _parameters = parameters;

        _health.Value = parameters.Health;
        _score.Value = 0;

        _health.Subscribe(HealthChanged);
        _score.Subscribe(ScoreChanged);

        _signalBus.Subscribe<SortingGameplayFigureSorted>(FigureSorted);
        _signalBus.Subscribe<SortingGameplayFigureDestroyed>(FigureDestroyed);
    }

    public void Stop()
    {
        if (!_isLaunched) return;

        _isLaunched = false;

        _health.Unsubscribe(HealthChanged);
        _score.Unsubscribe(ScoreChanged);

        _signalBus.TryUnsubscribe<SortingGameplayFigureSorted>(FigureSorted);
        _signalBus.TryUnsubscribe<SortingGameplayFigureDestroyed>(FigureDestroyed);
    }

    private void HealthChanged(int health)
    {
        if (health <= 0)
        {
            _signalBus.Fire(new FSMSignal(new MainFSMLoseSignalData(_score.Value, _parameters.SortWinCount)));
        }
    }

    private void ScoreChanged(int score)
    {
        if (score >= _parameters.SortWinCount)
        {
            _signalBus.Fire(new FSMSignal(new MainFSMWinSignalData(_health.Value, _score.Value, 
                _parameters.SortWinCount)));
        }
    }

    private void FigureSorted()
    {
        _score.Value++;
    }

    private void FigureDestroyed()
    {
        _health.Value--;
    }

    public class LaunchParameters
    {
        public GameplayLayoutConfig Layout { get; }

        public GameplayConfig.State GameplayConfig { get; }

        public int Health { get; }

        public int SortWinCount { get; }

        public int BeltLineCount { get; }

        public FigureConfig.Data[] AllFigures { get; }
    }
}
