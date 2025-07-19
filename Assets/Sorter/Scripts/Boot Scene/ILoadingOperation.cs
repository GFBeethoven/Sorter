using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadingOperation
{
    public LoadingStatus Status { get; }

    public float Progress { get; }

    public string DescriptionKey { get; }

    public string FailureMessageKey { get; }

    public void Launch();
}

public enum LoadingStatus
{
    Running,
    Success,
    Failure
}
