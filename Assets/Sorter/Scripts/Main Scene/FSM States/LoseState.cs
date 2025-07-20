using UnityEngine;

public class LoseState : FSMState<LoseState.EnterData>, ISignalHandler
{
    public int Score => _enterData?.Score ?? 0;
    public int TargetScore => _enterData?.TargetScore ?? 0;

    private EnterData _enterData;

    private LoseStateView _view;

    public LoseState(LoseStateView view) : base(view)
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
        public int Score { get; }

        public int TargetScore { get; }

        public EnterData(int score, int targetScore)
        {
            Score = score;
            TargetScore = targetScore;
        }
    }
}
