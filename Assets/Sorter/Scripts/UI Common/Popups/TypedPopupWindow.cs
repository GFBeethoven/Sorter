using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypedPopupWindow : PopupWindow
{
    [SerializeField] private Popups.PopupType _type;
    [SerializeField, Range(0.1f, 60.0f)] private float _showDuration = 5.0f;
    [SerializeField] private bool _canHideByTap = true;

    [SerializeField] private Slider _timerBar;

    public override Popups.PopupType Type => _type;

    private PopupState _state = new PopupState();

    protected override PopupState State => _state;

    private CoroutineHandle _hideTimer;

    public override PopupState Show(string messageKey)
    {
        if (_hideTimer.IsValid)
        {
            Timing.KillCoroutines(_hideTimer);
        }

        _hideTimer = Timing.RunCoroutine(_HideTimer());

        return base.Show(messageKey);
    }

    private IEnumerator<float> _HideTimer()
    {
        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / _showDuration)
        {
            _timerBar.value = i;

            yield return Timing.WaitForOneFrame;
        }

        _hideTimer = new CoroutineHandle();

        Hide(PopupWindowCloseEventArgs.Empty);
    }
}
