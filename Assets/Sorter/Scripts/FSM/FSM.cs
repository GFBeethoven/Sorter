using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FSM
{
    public const string InitEnterDataId = "InitMainFSMState";

    private SignalBus _signalBus;

    public event Action OnStateChanged;

    private IState _currentState = null;

    private IState[] _states;

    public FSM(IState[] allStates, SignalBus signalBus, [Inject(Id = InitEnterDataId)] StateEnterData initEnterData)
    {
        _signalBus = signalBus;

        _signalBus.Subscribe<FSMSignal>(Signal);

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

    private void Signal(FSMSignal signal)
    {
        if (_currentState == null || signal.Data == null) return;

        if (_currentState is ISignalHandler handler)
        {
            handler.Handle(signal.Data);           
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
