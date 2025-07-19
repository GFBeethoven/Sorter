using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;
using TMPro;
using UnityEngine.UI;
using Zenject;

public class LoadingScreen : MonoBehaviour
{
    [Inject] private Popups _popups;

    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Image _progressBar;

    private Action _endCallback;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void PerformLoadingOperations(ILoadingOperation[] operations, Action endCallback, Action<ILoadingOperation> fallback)
    {
        _endCallback = endCallback;

        Timing.RunCoroutine(_PerformLoadingOperations(operations, endCallback, fallback).CancelWith(gameObject));
    }

    private IEnumerator<float> _PerformLoadingOperations(ILoadingOperation[] operations, Action endCallback, 
        Action<ILoadingOperation> fallback)
    {
        for (int i = 0; i < operations.Length; i++)
        {
            _progressBar.fillAmount = 0.0f;

            _description.text = operations[i].DescriptionKey;

            operations[i].Launch();

            while (operations[i].Status == LoadingStatus.Running)
            {
                yield return Timing.WaitForOneFrame;

                _progressBar.fillAmount = operations[i].Progress;
            }

            if (operations[i].Status == LoadingStatus.Failure)
            {
                _popups.Show(Popups.PopupType.Error, operations[i].FailureMessageKey);

                fallback?.Invoke(operations[i]);

                _endCallback = null;

                yield break;
            }
        }

        if (_endCallback != null)
        {
            endCallback?.Invoke();

            _endCallback = null;
        }
    }
}
