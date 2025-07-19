using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class LoadMainSceneOperation : ILoadingOperation
{
    public LoadingStatus Status { get; private set; } = LoadingStatus.Running;

    public float Progress { get; private set; } = 0.0f;

    public string DescriptionKey => "LoadMainScene";

    public string FailureMessageKey { get; private set; } = null;

    public void Launch()
    {
        Timing.RunCoroutine(_Operation());
    }

    private IEnumerator<float> _Operation()
    {
        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / 10.0f)
        {
            Progress = Easing.InOutQuad(i * 0.9f);

            yield return Timing.WaitForOneFrame;
        }
    }
}
