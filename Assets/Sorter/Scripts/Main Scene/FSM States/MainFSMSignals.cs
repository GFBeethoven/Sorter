using UnityEngine;

public class MainFSMToGameplaySignalData : FSMSignalData
{
    public SortingGameplay.LaunchParameters LaunchParameters { get; }

    public MainFSMToGameplaySignalData(SortingGameplay.LaunchParameters launchParameters)
    {
        LaunchParameters = launchParameters;
    }
}

public class MainFSMWinSignalData : FSMSignalData
{
    public int Health { get; }

    public int Score { get; }

    public int MaxScore { get; }

    public MainFSMWinSignalData(int health, int score, int maxScore)
    {
        Health = health;
        Score = score;
        MaxScore = maxScore;
    }
}

public class MainFSMLoseSignalData : FSMSignalData
{
    public int Score { get; }

    public int MaxScore { get; }

    public MainFSMLoseSignalData(int score, int maxScore)
    {
        Score = score;
        MaxScore = maxScore;
    }
}

