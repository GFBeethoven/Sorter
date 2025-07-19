using MEC;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadGameStateOperation : ILoadingOperation
{
    public LoadingStatus Status { get; private set; } = LoadingStatus.Running;

    public float Progress { get; private set; } = 0.0f;

    public string DescriptionKey => "LoadGameState";

    public string FailureMessageKey { get; private set; } = null;

    public LoadGameStateOperation()
    {

    }

    public void Launch()
    {
        Timing.RunCoroutine(_Operation());
    }

    private IEnumerator<float> _Operation()
    {
        Status = LoadingStatus.Running;

#if UNITY_EDITOR
        Progress = 0.5f;

        yield return Timing.WaitForSeconds(0.5f);
#else
        Progress = 0.5f;

        yield return Timing.WaitForOneFrame;
#endif

        Status = LoadingStatus.Success;
    }
}
