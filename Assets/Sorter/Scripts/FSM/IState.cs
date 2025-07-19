using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void Initialize(FSM fsm);

    public void Update();

    public void FixedUpdate();

    public void Exit();
}

public interface IState<T> : IState where T : StateEnterData
{
    public void Enter(T enterData);
}
