using UnityEngine;

public class LoseState : FSMState<LoseState.EnterData>, ISignalHandler
{
    public LoseState() : base(null) { }

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
        public int Score { get; }

        public int TargetScore { get; }

        public EnterData(int score, int targetScore)
        {
            Score = score;
            TargetScore = targetScore;
        }
    }
}
