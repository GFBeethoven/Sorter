using UnityEngine;
using Zenject;

public class TitleState : FSMState<TitleState.EnterData>, ISignalHandler
{
    private TitleStateView _view;

    private SortingGameplay.LaunchParameters.IFactory _gameplayParamsFactory;

    public TitleState(TitleStateView view, SortingGameplay.LaunchParameters.IFactory gameplayParamsFactory) : base(view)
    {
        _view = view;

        _gameplayParamsFactory = gameplayParamsFactory;
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

    }
}
