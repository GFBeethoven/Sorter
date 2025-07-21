using MEC;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameplayState : FSMState<GameplayState.EnterData>, ISignalHandler
{
    private TransitionCanvas _transitionCanvas;

    private SortingGameplay _sortingGameplay;

    private SignalBus _signalBus;

    private bool _gameplayIsLaunched = false;

    public GameplayState(SortingGameplay sortingGameplay, SortingGameplayView view, SignalBus signalBus, 
        TransitionCanvas transitionCanvas) : base(view)
    {
        _sortingGameplay = sortingGameplay;

        _signalBus = signalBus;

        _transitionCanvas = transitionCanvas;
    }

    public override void Enter(EnterData enterData)
    {
        _signalBus.Subscribe<SortingGameplayWin>(OnWin);  
        _signalBus.Subscribe<SortingGameplayLose>(OnLose);

        _gameplayIsLaunched = true;

        _sortingGameplay.Launch(enterData.LaunchParameters);
    }

    public override void Exit()
    {
        _signalBus.TryUnsubscribe<SortingGameplayWin>(OnWin);
        _signalBus.TryUnsubscribe<SortingGameplayLose>(OnLose);

        _gameplayIsLaunched = false;

        _sortingGameplay.Stop();
    }

    public override void FixedUpdate() { }

    public override void Update()
    {
        if (!_gameplayIsLaunched) return;

        _sortingGameplay.Tick();
    }

    private void OnWin(SortingGameplayWin signal)
    {
        _gameplayIsLaunched = false;

        Timing.RunCoroutine(_DelayedFire(new FSMSignal(new MainFSMWinSignalData(signal.Health,
            signal.Score, signal.MaxScore)), 2.0f));
    }

    private void OnLose(SortingGameplayLose signal)
    {
        _gameplayIsLaunched = false;

        Timing.RunCoroutine(_DelayedFire(new FSMSignal(new MainFSMLoseSignalData(signal.Score, 
            signal.MaxScore)), 2.0f));
    }

    void ISignalHandler.Handle(FSMSignalData data)
    {
        switch (data)
        {
            case MainFSMWinSignalData toWinData:
                _transitionCanvas.ShowTransition(() => Fsm.ChangeState(new WinState.EnterData(toWinData.Health, toWinData.Score, toWinData.MaxScore)));
                break;
            case MainFSMLoseSignalData toLoseData:
                _transitionCanvas.ShowTransition(() => Fsm.ChangeState(new LoseState.EnterData(toLoseData.Score, toLoseData.MaxScore)));
                break;
        }
    }

    private IEnumerator<float> _DelayedFire(FSMSignal signal, float delay)
    {
        yield return Timing.WaitForSeconds(delay);

        _signalBus.Fire(signal);
    }

    public class EnterData : StateEnterData
    {
        public SortingGameplay.LaunchParameters LaunchParameters { get; }

        public EnterData(SortingGameplay.LaunchParameters launchParameters)
        {
            LaunchParameters = launchParameters;
        }
    }
}
