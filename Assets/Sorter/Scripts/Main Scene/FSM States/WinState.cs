using UnityEngine;

public class WinState : FSMState<WinState.EnterData>, ISignalHandler
{
    public int Health => _enterData?.Health ?? 0;
    public int Score => _enterData?.Score ?? 0;
    public int TargetScore => _enterData?.TargetScore ?? 0;

    private EnterData _enterData;

    private WinStateView _view;

    private SortingGameplay.LaunchParameters.IFactory _gameplayParamsFactory;

    public WinState(WinStateView view, SortingGameplay.LaunchParameters.IFactory gameplayParamsFactory) : base(view)
    {
        _view = view;

        _gameplayParamsFactory = gameplayParamsFactory;
    }

    public override void Enter(EnterData enterData)
    {
        _enterData = enterData;

        _view.RefreshInfo();

        _view.Show();
    }

    public override void Exit()
    {
        _view.Hide();
    }

    public override void FixedUpdate() { }

    public override void Update() { }

    void ISignalHandler.Handle(FSMSignalData data)
    {
        switch (data)
        {
            case MainFSMToGameplaySignalData toGameplayData:
                if (toGameplayData.LaunchParameters != null)
                {
                    Fsm.ChangeState(new GameplayState.EnterData(toGameplayData.LaunchParameters));
                }
                else
                {
                    Fsm.ChangeState(new GameplayState.EnterData(_gameplayParamsFactory.Create()));
                }
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
