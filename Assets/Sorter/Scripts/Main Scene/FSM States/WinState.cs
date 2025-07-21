using UnityEngine;

public class WinState : FSMState<WinState.EnterData>, ISignalHandler
{
    public int Health => _enterData?.Health ?? 0;
    public int Score => _enterData?.Score ?? 0;
    public int MaxScore => _enterData?.MaxScore ?? 0;

    private TransitionCanvas _transitionCanvas;

    private EnterData _enterData;

    private WinStateView _view;

    private SortingGameplay.LaunchParameters.IFactory _gameplayParamsFactory;

    public WinState(WinStateView view, SortingGameplay.LaunchParameters.IFactory gameplayParamsFactory,
        TransitionCanvas transitionCanvas) : base(view)
    {
        _view = view;

        _gameplayParamsFactory = gameplayParamsFactory;

        _transitionCanvas = transitionCanvas;
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
                GameplayState.EnterData enterData = null;
                if (toGameplayData.LaunchParameters != null)
                {
                    enterData = new GameplayState.EnterData(toGameplayData.LaunchParameters);
                }
                else
                {
                    enterData = new GameplayState.EnterData(_gameplayParamsFactory.Create());
                }

                _transitionCanvas.ShowTransition(() => Fsm.ChangeState<GameplayState.EnterData>(enterData));
                break;
        }
    }

    public class EnterData : StateEnterData
    {
        public int Health { get; }

        public int Score { get; }

        public int MaxScore { get; }

        public EnterData(int health, int score, int maxScore)
        {
            Health = health;
            Score = score;
            MaxScore = maxScore;
        }
    }
}
