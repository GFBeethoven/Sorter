using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class FSMStateMono<T> : MonoBehaviour where T : StateEnterData
{
    [Inject] private SignalBus _signalBus;

    protected SignalBus SignalBus => _signalBus;

    private FSMState<T> _associatedState;

    protected FSMState<T> AssociatedState => _associatedState;

    public virtual void Setup(FSMState<T> fsmState)
    {
        _associatedState = fsmState;
    }

    protected void TryLaunchCoroutineWithStateLifeSpan(IEnumerator<float> coroutine)
    {
        if (_associatedState == null)
        {
            Debug.LogWarning("Cannot launch coroutine with state life span - not assigned state (null)");
        }

        _associatedState?.LaunchCoroutineWithStateLifeSpan(coroutine.CancelWith(gameObject));
    }
}
