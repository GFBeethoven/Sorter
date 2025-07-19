using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FSMSignal
{
    public FSMSignalData Data { get; }

    public FSMSignal(FSMSignalData data)
    {
        Data = data;
    }
}

public abstract class FSMSignalData
{

}
