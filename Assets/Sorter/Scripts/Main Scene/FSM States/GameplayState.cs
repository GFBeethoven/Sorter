using UnityEngine;

public class GameplayState : FSMState<GameplayState.EnterData>, ISignalHandler
{
    private SortingGameplay _sortingGameplay;

    public GameplayState(SortingGameplay sortingGameplay, SortingGameplayView view) : base(view)
    {
        _sortingGameplay = sortingGameplay;
    }

    public override void Enter(EnterData enterData)
    {
        _sortingGameplay.Launch(enterData.LaunchParameters);
    }

    public override void Exit()
    {
        _sortingGameplay.Stop();
    }

    public override void FixedUpdate() { }

    public override void Update() { }

    void ISignalHandler.Handle(FSMSignalData data)
    {
        switch (data)
        {
            case MainFSMWinSignalData toWinData:
                Fsm.ChangeState(new WinState.EnterData(toWinData.Health, toWinData.Score, toWinData.TargetScore));
                break;
            case MainFSMLoseSignalData toLoseData:
                Fsm.ChangeState(new LoseState.EnterData(toLoseData.Score, toLoseData.TargetScore));
                break;
        }
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
