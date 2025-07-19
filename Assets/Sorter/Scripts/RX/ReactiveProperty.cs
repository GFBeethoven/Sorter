using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadOnlyReactiveProperty<T>
{
    private Action<T> _listeners;

    protected T _value;
    public T Value
    {
        get
        {
            return _value;
        }
    }

    protected IEqualityComparer<T> _comparer;

    public ReadOnlyReactiveProperty(T initValue, IEqualityComparer<T> comparer = null)
    {
        _value = initValue;

        if (comparer == null)
        {
            _comparer = EqualityComparer<T>.Default;
        }
        else
        {
            _comparer = comparer;
        }
    }

    public void Subscribe(Action<T> listener)
    {
        if (listener == null) return;

        _listeners += listener;

        listener.Invoke(Value);
    }

    public void Unsubscribe(Action<T> listener)
    {
        if (listener == null) return;

        _listeners -= listener;
    }

    public void SubscribeWithEnableBinding(Action<T> listener, MonoBehaviour component)
    {
        if (component == null || listener == null) return;

        //Subscribe(listener);

        var helper = component.gameObject.AddComponent<ReactivePropertyEnableBinding>();
        helper.Initialize(() => Subscribe(listener), () => Unsubscribe(listener));
    }

    protected void InvokeOnChanged()
    {
        _listeners?.Invoke(Value);
    }
}

public class ReactiveProperty<T> : ReadOnlyReactiveProperty<T>
{
    public new T Value
    {
        get { return _value; }

        set
        {
            bool changed = !_comparer.Equals(_value, value);

            _value = value;

            if (changed)
            {
                InvokeOnChanged();
            }
        }
    }

    public ReactiveProperty(T initValue) : base(initValue) { }
}
