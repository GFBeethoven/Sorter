using UnityEngine;
using Zenject;

public class MainFSMEntryPoint : IInitializable
{
    private FSM _fsm;

    public MainFSMEntryPoint(FSM fsm)
    {
        _fsm = fsm;
    }

    void IInitializable.Initialize()
    {
        _fsm?.ChangeState<TitleState.EnterData>(new TitleState.EnterData());
    }
}
