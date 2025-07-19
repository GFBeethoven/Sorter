using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FigureConfig", menuName = "Sorter/Figure Config")]
public class FigureConfig : ScriptableObject
{
    [SerializeField] private Data[] _data;

    public int FigureCount => _data.Length;

    public Data GetData(int index)
    {
        return _data[index];
    }

    private void OnValidate()
    {
        if (_data == null || _data.Length == 0) return;

        for (int i = 0; i < _data.Length; i++)
        {
            for (int j = 0; j < _data.Length; j++)
            {
                if (i == j) continue;

                if (_data[i].Id == _data[j].Id)
                {
                    (_data[j] as IDataValidable).SetId(GetNextId());
                }
            }
        }

        int GetNextId()
        {
            if (_data == null || _data.Length == 0) return 0;

            int maxId = 0;

            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i].Id > maxId)
                {
                    maxId = _data[i].Id;
                }
            }

            return maxId + 1;
        }
    }

    [Serializable]
    public class Data : IDataValidable
    {
        [field: SerializeField] public int Id { get; private set; }

        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField] public Sprite OnLineSprite { get; private set; }

        [field: SerializeField] public Sprite OnDragSprite { get; private set; }

        void IDataValidable.SetId(int id)
        {
            Id = id;
        }
    }

    private interface IDataValidable
    {
        public void SetId(int id);
    }
    
}
