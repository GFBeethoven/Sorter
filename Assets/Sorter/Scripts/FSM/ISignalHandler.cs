using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISignalHandler<T> where T : Signal
{
    public void Handle(T signal);
}
