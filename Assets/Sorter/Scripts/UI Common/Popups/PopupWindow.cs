using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class PopupWindow : MonoBehaviour
{
    public event Action<PopupWindow, PopupWindowCloseEventArgs> Closed;

    public abstract Popups.PopupType Type { get; }

    protected abstract PopupState State { get; }

    [SerializeField] private TextMeshProUGUI _messageLabel;

    public virtual PopupState Show(string messageKey)
    {
        _messageLabel.text = messageKey;

        (State as IPopupStateSetupable).IsShownValue = true;

        return State;
    }

    protected virtual void Hide(PopupWindowCloseEventArgs eventArgs)
    {
        (State as IPopupStateSetupable).IsShownValue = false;

        Closed?.Invoke(this, eventArgs);
    }

    public class PopupState : IPopupStateSetupable
    {
        public bool IsShown { get; private set; }

        bool IPopupStateSetupable.IsShownValue
        {
            set
            {
                IsShown = value;
            }
        }
    }

    protected interface IPopupStateSetupable
    {
        public bool IsShownValue { set; }
    }
}
