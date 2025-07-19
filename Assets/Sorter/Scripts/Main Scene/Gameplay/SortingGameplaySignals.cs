using UnityEngine;

public struct SortingGameplayHealthChanged
{
    public int CurrentHealth { get; }

    public SortingGameplayHealthChanged(int currentHealth)
    {
        CurrentHealth = currentHealth;
    }
}

public struct SortingGameplayScoreChanged
{
    public int CurrentScore { get; }

    public SortingGameplayScoreChanged(int currentScore)
    {
        CurrentScore = currentScore;
    }
}

public struct SortingGameplayFigureDestroyed { }

public struct SortingGameplayFigureSorted { }