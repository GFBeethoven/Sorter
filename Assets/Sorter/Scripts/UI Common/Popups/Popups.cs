using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Popups : MonoBehaviour, IInitializable
{
    [SerializeField] private PopupWindow[] _windowPrefabs;

    private Dictionary<PopupType, Stack<PopupWindow>> _windows = new();

    public void Initialize()
    {
        bool defaultRegistered = false;

        for (int i = 0; i < _windowPrefabs.Length; i++)
        {
            if (_windows.ContainsKey(_windowPrefabs[i].Type))
            {
                throw new System.Exception($"Window for type {_windowPrefabs[i].Type} already registered");
            }

            if (_windowPrefabs[i].Type == PopupType.Default)
            {
                defaultRegistered = true;
            }

            _windows[_windowPrefabs[i].Type] = new Stack<PopupWindow>();
        }

        if (!defaultRegistered)
        {
            throw new System.Exception($"Default window should be registered");
        }
    }

    public PopupWindow.PopupState Show(PopupType type, string messageKey)
    {
        PopupWindow window = GetWindow(type);

        window.Closed += WindowClosed;

        window.gameObject.SetActive(true);

        return window.Show(messageKey);
    }

    private void WindowClosed(PopupWindow window, PopupWindowCloseEventArgs eventArgs)
    {
        window.Closed -= WindowClosed;

        window.gameObject.SetActive(false);

        ReleaseWindow(window);
    }

    private PopupWindow GetWindow(PopupType type)
    {
        if (_windows.TryGetValue(type, out Stack<PopupWindow> stack))
        {
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            else
            {
                for (int i = 0; i < _windowPrefabs.Length; i++)
                {
                    if (_windowPrefabs[i].Type == type)
                    {
                        PopupWindow window = Instantiate(_windowPrefabs[i], transform);

                        return window;
                    }
                }

                return GetWindow(PopupType.Default);
            }
        }
        else
        {
            return GetWindow(PopupType.Default);
        }
    }

    private void ReleaseWindow(PopupWindow window)
    {
        _windows[window.Type].Push(window);
    }

    public enum PopupType
    {
        Default,
        Error
    }
}
