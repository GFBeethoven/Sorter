using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISignalHandler
{
    public void Handle(FSMSignalData data);
}
