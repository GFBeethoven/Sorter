using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMStateMono<T> : MonoBehaviour where T : StateEnterData 
{
    private FSMState<T> _associatedState;

    public void Setup(FSMState<T> hfsmState)
    {
        _associatedState = hfsmState;
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
