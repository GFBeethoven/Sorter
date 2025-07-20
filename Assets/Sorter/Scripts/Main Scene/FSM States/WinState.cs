using UnityEngine;

public class WinState : FSMState<WinState.EnterData>, ISignalHandler
{
    public int Health => _enterData?.Health ?? 0;
    public int Score => _enterData?.Score ?? 0;
    public int TargetScore => _enterData?.TargetScore ?? 0;

    private EnterData _enterData;

    private WinStateView _view;

    public WinState(WinStateView view) : base(view)
    {
        _view = view;
    }

    public override void Enter(EnterData enterData)
    {
        _enterData = enterData;

        _view.RefreshInfo();
    }

    public override void Exit() { }

    public override void FixedUpdate() { }

    public override void Update() { }

    void ISignalHandler.Handle(FSMSignalData data)
    {
        switch (data)
        {
            case MainFSMToGameplaySignalData toGameplayData:
                Fsm.ChangeState(new GameplayState.EnterData(toGameplayData.LaunchParameters));
                break;
        }
    }

    public class EnterData : StateEnterData
    {
        public int Health { get; }

        public int Score { get; }

        public int TargetScore { get; }

        public EnterData(int health, int score, int targetScore)
        {
            Health = health;
            Score = score;
            TargetScore = targetScore;
        }
    }
}
