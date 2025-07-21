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

    private SortingGameplayView _view;

    private ReactiveProperty<int> _health;
    private ReactiveProperty<int> _score;

    private List<SortingGameplayBelt> _belts;
    private List<SortingGameplayFigure> _figures;
    private List<SortingGameplayFigureHole> _holes;

    private DraggablesDispatcher _draggablesDispatcher;

    private bool _isLaunched;

    private LaunchParameters _parameters;

    private CoroutineHandle _figureSpawnCoroutine;

    public SortingGameplay(SignalBus signalBus, SortingGameplayFigure.Pool figurePool, 
        SortingGameplayBelt.Pool beltPool, SortingGameplayFigureHole.Pool holePool, SortingGameplayView view,
        DraggablesDispatcher draggablesDispatcher)
    {
        _signalBus = signalBus;
        _figurePool = figurePool;
        _beltPool = beltPool;
        _holePool = holePool;
        _view = view;
        _draggablesDispatcher = draggablesDispatcher;

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

        _signalBus.Subscribe<SortingGameplayFigureSorted>(FigureSorted);
        _signalBus.Subscribe<SortingGameplayFigureDestroyed>(FigureDestroyed);

        _health.Subscribe(HealthChanged);
        _score.Subscribe(ScoreChanged);

        for (int i = 0; i < _parameters.BeltLineCount; i++)
        {
            _belts.Add(GetBelt());
        }

        for (int i = 0; i < _parameters.AllFigures.Length; i++)
        {
            _holes.Add(GetHole(_parameters.AllFigures[i]));
        }

        _view.Show();
        _view.UpdateView(_parameters.Layout, _belts, _holes);

        _draggablesDispatcher.Launch();

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

        _view.Hide();

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

        _draggablesDispatcher.Stop();

        Timing.KillCoroutines(_figureSpawnCoroutine);
    }

    public void Tick()
    {
        _draggablesDispatcher.Update();

        for (int i = 0; i < _belts.Count; i++)
        {
            _belts[i].Tick();
        }
    }

    private void HealthChanged(int health)
    {
        _signalBus.Fire(new SortingGameplayHealthChanged(health));  

        if (health <= 0)
        {
            _signalBus.Fire(new FSMSignal(new MainFSMLoseSignalData(_score.Value, _parameters.SortWinCount)));
        }
    }

    private void ScoreChanged(int score)
    {
        _signalBus.Fire(new SortingGameplayScoreChanged(score));

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

    private SortingGameplayFigure GetRandomFigure(FigureConfig.Data figureData, SortingGameplayBelt belt, 
        SortingGameplayFigureHole targetHole, float relativeVelocity)
    {
        return _figurePool.Spawn(figureData, belt, targetHole, relativeVelocity);
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
        figure.Dispose();

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

            var newFigure = GetRandomFigure(figureData, belt, GetHoleByFigureData(figureData), relativeVelocity);

            _figures.Add(newFigure);
        }

        SortingGameplayFigureHole GetHoleByFigureData(FigureConfig.Data data)
        {
            for (int i = 0; i < _holes.Count; i++)
            {
                if (_holes[i].TargetFigure == data)
                {
                    return _holes[i];
                }
            }

            return null;
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

        private LaunchParameters(GameplayLayoutConfig layout, GameplayConfig.State gameplayConfig, int health, int sortWinCount, int beltLineCount, FigureConfig.Data[] allFigures)
        {
            Layout = layout;
            GameplayConfig = gameplayConfig;
            Health = health;
            SortWinCount = sortWinCount;
            BeltLineCount = beltLineCount;
            AllFigures = allFigures;
        }

        public interface IFactory
        {
            public LaunchParameters Create();
        }

        public class RandomFactoryWithTaskConditions : IFactory
        {
            private GameplayLayoutConfig _layoutConfig;
            private GameplayConfig _gameplayConfig;
            private FigureConfig _figures;
            
            public RandomFactoryWithTaskConditions(GameplayLayoutConfig layoutConfig, GameplayConfig gameplayConfig, 
                FigureConfig figures)
            {
                _layoutConfig = layoutConfig;
                _gameplayConfig = gameplayConfig;
                _figures = figures;
            }

            public LaunchParameters Create()
            {
                var randomState = _gameplayConfig.GetState(Random.Range(0, _gameplayConfig.StateCount));

                FigureConfig.Data[] figures = new FigureConfig.Data[4];
                for (int i = 0; i < figures.Length; i++) 
                {
                    figures[i] = _figures.GetData(i);
                }

                return new LaunchParameters(_layoutConfig, randomState, randomState.PlayerHealth,
                    randomState.RandomSortWinCount, 3, figures);
            }
        }
    }
}
