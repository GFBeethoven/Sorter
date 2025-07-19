using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactivePropertyEnableBinding : MonoBehaviour
{
    private Action _subscribe;
    private Action _unsubscribe;

    public void Initialize(Action subscribe, Action unsubscribe)
    {
        _subscribe = subscribe;
        _unsubscribe = unsubscribe;
    }

    private void OnEnable()
    {
        _subscribe?.Invoke();
    }

    private void OnDisable()
    {
        _unsubscribe?.Invoke();
    }
}
