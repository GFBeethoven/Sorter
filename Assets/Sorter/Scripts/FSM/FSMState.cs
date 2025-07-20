using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMState<T> : IState<T> where T : StateEnterData
{
    private FSM _fsm;
    protected FSM Fsm => _fsm;

    private List<CoroutineHandle> _disposableOnExitCoroutines;

    public FSMState(FSMStateMono<T> mono)
    {
        if (mono != null)
        {
            mono.Setup(this);
        }

        _disposableOnExitCoroutines = new();
    }

    public virtual void Initialize(FSM fsm)
    {
        _fsm = fsm;
    }

    public void LaunchCoroutineWithStateLifeSpan(IEnumerator<float> coroutine)
    {
        if (coroutine == null) return;

        if (Fsm.IsCurrentState(this) == false) return;

        _disposableOnExitCoroutines.Add(Timing.RunCoroutine(coroutine));
    }

    public abstract void Enter(T enterData);
    public abstract void Exit();
    public abstract void Update();
    public abstract void FixedUpdate();

    void IState.Initialize(FSM fsm)
    {
        Initialize(fsm);
    }

    void IState<T>.Enter(T enterData)
    {
        Enter(enterData);
    }

    void IState.Exit()
    {
        for (int i = 0; i < _disposableOnExitCoroutines.Count; i++)
        {
            if (_disposableOnExitCoroutines[i].IsValid)
            {
                Timing.KillCoroutines(_disposableOnExitCoroutines[i]);
            }
        }

        _disposableOnExitCoroutines.Clear();

        Exit();
    }

    void IState.Update()
    {
        Update();
    }

    void IState.FixedUpdate()
    {
        FixedUpdate();
    }
}
