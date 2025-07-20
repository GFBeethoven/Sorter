using MEC;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SortingGameplay
{
    private SignalBus _signalBus;
    private SortingGameplayFigure.Pool _figurePool;
    private SortingGameplayBelt.Pool _beltPool;
    private SortingGameplayFigureHole.Pool _holePool;

    private ReactiveProperty<int> _health;
    private ReactiveProperty<int> _score;

    private List<SortingGameplayBelt> _belts;
    private List<SortingGameplayFigure> _figures;
    private List<SortingGameplayFigureHole> _holes;

    private bool _isLaunched;

    private LaunchParameters _parameters;

    private CoroutineHandle _figureSpawnCoroutine;

    public SortingGameplay(SignalBus signalBus, SortingGameplayFigure.Pool figurePool, SortingGameplayBelt.Pool beltPool, SortingGameplayFigureHole.Pool holePool)
    {
        _signalBus = signalBus;
        _figurePool = figurePool;
        _beltPool = beltPool;
        _holePool = holePool;

        _health = new ReactiveProperty<int>(0);
        _score = new ReactiveProperty<int>(0);

        _belts = new();
        _figures = new();
        _holes = new();
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

        if (_figureSpawnCoroutine.IsValid)
        {
            Timing.KillCoroutines(_figureSpawnCoroutine);
        }

        _figureSpawnCoroutine = Timing.RunCoroutine(_FigureSpawnProcess());
    }

    public void Stop()
    {
        if (!_isLaunched) return;

        _isLaunched = false;

        _health.Unsubscribe(HealthChanged);
        _score.Unsubscribe(ScoreChanged);

        _signalBus.TryUnsubscribe<SortingGameplayFigureSorted>(FigureSorted);
        _signalBus.TryUnsubscribe<SortingGameplayFigureDestroyed>(FigureDestroyed);

        for (int i = 0; i < _figures.Count; i++)
        {
            ReleaseFigure(_figures[i]);
        }

        _figures.Clear();

        for (int i = 0; i < _belts.Count; i++)
        {
            ReleaseBelt(_belts[i]);
        }

        _belts.Clear();

        for (int i = 0; i < _holes.Count; i++)
        {
            ReleaseHole(_holes[i]);
        }

        _holes.Clear();

        Timing.KillCoroutines(_figureSpawnCoroutine);
    }

    public void Tick()
    {
        if (!_isLaunched) return;

        for (int i = 0; i < _belts.Count; i++)
        {
            _belts[i].Tick();
        }
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

    private void FigureSorted(SortingGameplayFigureSorted signal)
    {
        _figures.Remove(signal.SortedFigure);
        ReleaseFigure(signal.SortedFigure);

        _score.Value++;
    }

    private void FigureDestroyed(SortingGameplayFigureDestroyed signal)
    {
        _figures.Remove(signal.DestroyedFigure);
        ReleaseFigure(signal.DestroyedFigure);

        _health.Value--;
    }

    private SortingGameplayFigure GetRandomFigure(FigureConfig.Data figureData, SortingGameplayBelt belt, float relativeVelocity)
    {
        return _figurePool.Spawn(figureData, belt, relativeVelocity);
    }

    private SortingGameplayBelt GetBelt()
    {
        return _beltPool.Spawn();
    }

    private SortingGameplayFigureHole GetHole(FigureConfig.Data data)
    {
        return _holePool.Spawn(data);
    }

    private void ReleaseFigure(SortingGameplayFigure figure)
    {
        _figurePool.Despawn(figure);
    }

    private void ReleaseBelt(SortingGameplayBelt belt)
    {
        _beltPool.Despawn(belt);
    }

    private void ReleaseHole(SortingGameplayFigureHole hole)
    {
        _holePool.Despawn(hole);
    }

    private IEnumerator<float> _FigureSpawnProcess()
    {
        while (true)
        {
            float timeout = _parameters.GameplayConfig.RandomSpawnTimeout;

            yield return Timing.WaitForSeconds(timeout);

            var belt = _belts[Random.Range(0, _belts.Count)];

            var figureData = _parameters.AllFigures[Random.Range(0, _parameters.AllFigures.Length)];

            var relativeVelocity = _parameters.GameplayConfig.RandomFigureVelocity;

            var newFigure = GetRandomFigure(figureData, belt, relativeVelocity);

            _figures.Add(newFigure);
        }
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
