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

public struct SortingGameplayFigureDestroyed
{
    public SortingGameplayFigure DestroyedFigure { get; }

    public SortingGameplayFigureDestroyed(SortingGameplayFigure destroyedFigure)
    {
        DestroyedFigure = destroyedFigure;
    }
}

public struct SortingGameplayFigureSorted
{
    public SortingGameplayFigure SortedFigure { get; }

    public SortingGameplayFigureSorted(SortingGameplayFigure sortedFigure)
    {
        SortedFigure = sortedFigure;
    }
}