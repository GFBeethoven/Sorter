using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateEnterData
{
    public T As<T>() where T : StateEnterData
    {
        return this as T;
    }
}
