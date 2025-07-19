using UnityEngine;

public class WinState : FSMState<WinState.EnterData>, ISignalHandler
{
    public WinState() : base(null) { }

    public override void Enter(EnterData enterData) { }

    public override void Exit() { }

    public override void FixedUpdate() { }

    public override void Update() { }

    void ISignalHandler.Handle(FSMSignalData data)
    {
        switch (data)
        {
            case MainFSMToGameplaySignalData toGameplayData:
                Fsm.ChangeState(new GameplayState.EnterData(toGameplayData.LaunchParameters));
                break;
        }
    }

    public class EnterData : StateEnterData
    {
        public int Health { get; }

        public int Score { get; }

        public int TargetScore { get; }

        public EnterData(int health, int score, int targetScore)
        {
            Health = health;
            Score = score;
            TargetScore = targetScore;
        }
    }
}
