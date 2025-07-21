using System;
using UnityEngine;

public class SortingGameplayEffect : MonoBehaviour
{
    private Action<SortingGameplayEffect> _endCallback;

    private void OnDisable()
    {
        _endCallback?.Invoke(this);

        _endCallback = null;
    }

    public void Play(Vector3 position, Action<SortingGameplayEffect> endCallback)
    {
        if (gameObject.activeSelf) return;

        _endCallback = endCallback;
        transform.position = position;
        gameObject.SetActive(true);
    }
}
