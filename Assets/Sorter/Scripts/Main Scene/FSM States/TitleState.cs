using UnityEngine;
using Zenject;

public class TitleState : FSMState<TitleState.EnterData>, ISignalHandler
{
    private TransitionCanvas _transitionCanvas;

    private TitleStateView _view;

    private SortingGameplay.LaunchParameters.IFactory _gameplayParamsFactory;

    public TitleState(TitleStateView view, SortingGameplay.LaunchParameters.IFactory gameplayParamsFactory, 
        TransitionCanvas transitionCanvas) : base(view)
    {
        _view = view;

        _gameplayParamsFactory = gameplayParamsFactory;

        _transitionCanvas = transitionCanvas;
    }

    public override void Enter(EnterData enterData) 
    {
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

    }
}
