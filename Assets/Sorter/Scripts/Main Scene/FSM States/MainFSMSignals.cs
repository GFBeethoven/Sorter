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

    public int TargetScore { get; }

    public MainFSMWinSignalData(int health, int score, int targetScore)
    {
        Health = health;
        Score = score;
        TargetScore = targetScore;
    }
}

public class MainFSMLoseSignalData : FSMSignalData
{
    public int Score { get; }

    public int TargetScore { get; }

    public MainFSMLoseSignalData(int score, int targetScore)
    {
        Score = score;
        TargetScore = targetScore;
    }
}

