using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LoadGameSettingsOperation : ILoadingOperation
{
    public LoadingStatus Status { get; private set; } = LoadingStatus.Running;

    public float Progress { get; private set; } = 0.0f;

    public string DescriptionKey => "LoadGameSettings";

    public string FailureMessageKey { get; private set; } = null;

    public LoadGameSettingsOperation()
    {

    }

    public void Launch()
    {
        Status = LoadingStatus.Success;

        Progress = 1.0f;
    }
}
