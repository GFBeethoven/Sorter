using TMPro;
using UnityEngine;

public class LoseStateView : FSMStateMono<LoseState.EnterData>
{
    [SerializeField] private TextMeshProUGUI _score;

    private LoseState _state;

    public override void Setup(FSMState<LoseState.EnterData> fsmState)
    {
        base.Setup(fsmState);

        _state = (LoseState)fsmState;
    }

    public void Show() 
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void RefreshInfo()
    {
        _score.text = $"{_state.Score}/{_state.TargetScore}";
    }

    public void RestartButtonClicked()
    {
        SignalBus.Fire<FSMSignal>(new FSMSignal(new MainFSMToGameplaySignalData(null)));
    }
}
