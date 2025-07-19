using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactivePropertyUnsubscriber : MonoBehaviour
{
    private Action _unsubscribeAction;

    public void Initialize(Action unsubscribeAction)
    {
        _unsubscribeAction = unsubscribeAction;
    }

    private void OnDestroy()
    {
        _unsubscribeAction?.Invoke();
    }
}
