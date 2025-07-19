using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveList<T> : IList<T>
{
    public event Action<int, T> OnItemAdd;
    public event Action<int, T> OnItemRemove;
    public event Action OnBeforeClear;

    public int Count => _list.Count;

    public bool IsReadOnly => false;

    public T this[int index]
    {
        get
        {
            return _list[index];
        }

        set
        {
            _list[index] = value;
        }
    }

    private List<T> _list = new List<T>();

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        _list.Insert(index, item);

        OnItemAdd?.Invoke(index, item);
    }

    public void RemoveAt(int index)
    {
        T item = _list[index];  

        _list.RemoveAt(index);

        OnItemRemove?.Invoke(index, item);
    }

    public void Add(T item)
    {
        _list.Add(item);

        OnItemAdd?.Invoke(_list.Count - 1, item);
    }

    public void Clear()
    {
        OnBeforeClear?.Invoke();

        _list.Clear();
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        int index = _list.IndexOf(item);

        if (index >= 0 && index < _list.Count)
        {
            _list.RemoveAt(index);

            OnItemRemove(index, item);

            return true;
        }

        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}

public class ReadOnlyReactiveList<T>
{
    private readonly ReactiveList<T> _list;

    public event Action<int, T> OnItemAdd
    {
        add
        {
            _list.OnItemAdd += value;
        }

        remove
        {
            _list.OnItemAdd -= value;
        }
    }

    public event Action<int, T> OnItemRemove
    {
        add
        {
            _list.OnItemRemove += value;
        }

        remove
        {
            _list.OnItemRemove -= value;
        }
    }

    public event Action OnBeforeClear
    {
        add
        {
            _list.OnBeforeClear += value;
        }

        remove
        {
            _list.OnBeforeClear -= value;
        }
    }

    public int Count => _list.Count;

    public T this[int index]
    {
        get
        {
            return _list[index];
        }
    }

    public ReadOnlyReactiveList(ReactiveList<T> list)
    {
        _list = list;
    }
}
