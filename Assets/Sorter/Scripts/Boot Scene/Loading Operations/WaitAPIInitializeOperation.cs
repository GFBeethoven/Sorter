using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class WaitAPIInitializeOperation : ILoadingOperation
{
    public LoadingStatus Status { get; private set; } = LoadingStatus.Running;

    public float Progress { get; private set; } = 0.0f;

    public string DescriptionKey => "WaitAPIInitialize";

    public string FailureMessageKey { get; private set; } = null;

    public void Launch()
    {
        Timing.RunCoroutine(_Operation());
    }

    private IEnumerator<float> _Operation()
    {
        Status = LoadingStatus.Running;

        // ** SOME API INITIALIZATION **

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / 1.0f)
        {
            Progress = Easing.OutQuad(i);

            yield return Timing.WaitForOneFrame;
        }

        Status = LoadingStatus.Success;
    }
}
