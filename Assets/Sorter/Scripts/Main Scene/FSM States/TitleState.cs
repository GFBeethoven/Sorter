using UnityEngine;
using Zenject;

public class TitleState : FSMState<TitleState.EnterData>, ISignalHandler
{
    public TitleState() : base(null) { }

    public override void Enter(EnterData enterData) { }

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

    }
}
