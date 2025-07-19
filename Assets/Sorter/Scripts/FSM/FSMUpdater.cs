using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMUpdater : MonoBehaviour
{
    private FSM _fsm;

    public void Initialize(FSM fsm)
    {
        _fsm = fsm;
    }

    private void Update()
    {
        _fsm?.Update();
    }

    private void FixedUpdate()
    {
        _fsm?.FixedUpdate();
    }
}
