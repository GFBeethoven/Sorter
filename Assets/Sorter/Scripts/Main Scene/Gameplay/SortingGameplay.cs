using UnityEngine;

public class SortingGameplay
{


    public void Launch(LaunchParameters parameters)
    {

    }

    public void Stop()
    {

    }

    public class LaunchParameters
    {
        public GameplayLayoutConfig Layout { get; }

        public GameplayConfig.State GameplayConfig { get; }

        public int BeltLineCount { get; }

        public FigureConfig.Data[] AllFigures { get; }
    }
}
