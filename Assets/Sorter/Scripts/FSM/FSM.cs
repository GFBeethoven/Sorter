using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    public event Action OnStateChanged;

    private IState _currentState = null;

    private IState[] _states;

    public FSM(IState[] allStates, StateEnterData initEnterData)
    {
        _states = allStates;

        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i].Initialize(this);
        }

        ChangeState(initEnterData);
    }

    public bool IsCurrentState(IState state)
    {
        return _currentState == state;
    }

    public void Signal<T>(T signal) where T : Signal
    {
        if (_currentState == null) return;

        if (_currentState is ISignalHandler<T> handler)
        {
            handler.Handle(signal);           
        }
    }

    public void ChangeState<T>(T enterData) where T : StateEnterData
    {
        for (int i = 0; i < _states.Length; i++)
        {
            if (_states[i] is IState<T> state)
            {
                ChangeState(state, enterData);
            }
        }
    }

    public void Update()
    {
        _currentState?.Update();
    }

    public void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }

    private void ChangeState<T>(IState<T> state, T enterData) where T : StateEnterData
    {
        _currentState?.Exit();
        _currentState = state;
        state.Enter(enterData);

        OnStateChanged?.Invoke();
    }
}
