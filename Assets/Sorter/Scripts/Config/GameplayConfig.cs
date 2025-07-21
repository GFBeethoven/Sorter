using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayConfig", menuName = "Sorter/Gameplay Config")]
public class GameplayConfig : ScriptableObject
{
    [SerializeField] private State[] _states;

    public int StateCount => _states.Length;

    public State GetState(int index)
    {
        return _states[index];
    }

    private void OnValidate()
    {
        if (_states == null || _states.Length == 0) return;

        for (int i = 0; i < _states.Length; i++)
        {
            _states[i].Validate();
        }
    }

    [Serializable]
    public class State
    {
        private const int SortWinCountRangeMin = 1;
        private const int SortWinCountRangeMax = 150;

        private const float SpawnTimeoutRangeMin = 0.0f;
        private const float SpawnTimeoutRangeMax = 50.0f;

        private const float FigureVelocityRangeMin = 0.001f;
        private const float FigureVelocityRangeMax = 1.5f;

        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField, Min(1.0f)] public int PlayerHealth { get; private set; }

        [field: SerializeField, Range(SortWinCountRangeMin, SortWinCountRangeMax)] public int SortWinCountMin { get; private set; } 
        [field: SerializeField, Range(SortWinCountRangeMin, SortWinCountRangeMax)] public int SortWinCountMax { get; private set; } 

        [field: SerializeField, Range(SpawnTimeoutRangeMin, SpawnTimeoutRangeMax)] public float SpawnTimeoutMin { get; private set; } 
        [field: SerializeField, Range(SpawnTimeoutRangeMin, SpawnTimeoutRangeMax)] public float SpawnTimeoutMax { get; private set; }

        [field: SerializeField, Range(FigureVelocityRangeMin, FigureVelocityRangeMax)] public float FigureVelocityMin { get; private set; }
        [field: SerializeField, Range(FigureVelocityRangeMin, FigureVelocityRangeMax)] public float FigureVelocityMax { get; private set; }

        public int RandomSortWinCount => UnityEngine.Random.Range(SortWinCountMin, SortWinCountMax);

        public float RandomSpawnTimeout => UnityEngine.Random.Range(SpawnTimeoutMin, SpawnTimeoutMax);

        public float RandomFigureVelocity => UnityEngine.Random.Range(FigureVelocityMin, FigureVelocityMax);

        public void Validate()
        {
            SortWinCountMax = Mathf.Clamp(SortWinCountMax, SortWinCountMin, SortWinCountRangeMax);
            SortWinCountMin = Mathf.Clamp(SortWinCountMin, SortWinCountRangeMin, SortWinCountMax);

            SpawnTimeoutMax = Mathf.Clamp(SpawnTimeoutMax, SpawnTimeoutMin, SpawnTimeoutRangeMax);
            SpawnTimeoutMin = Mathf.Clamp(SpawnTimeoutMin, SpawnTimeoutRangeMin, SpawnTimeoutMax);

            FigureVelocityMax = Mathf.Clamp(FigureVelocityMax, FigureVelocityMin, FigureVelocityRangeMax);
            FigureVelocityMin = Mathf.Clamp(FigureVelocityMin, FigureVelocityRangeMin, FigureVelocityMax);
        }
    }
}
