using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FSMUpdater : MonoBehaviour
{
    [Inject] private FSM _fsm;

    private void Update()
    {
        _fsm?.Update();
    }

    private void FixedUpdate()
    {
        _fsm?.FixedUpdate();
    }
}
