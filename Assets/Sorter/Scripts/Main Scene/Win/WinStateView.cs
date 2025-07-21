using TMPro;
using UnityEngine;

public class WinStateView : FSMStateMono<WinState.EnterData>
{
    [SerializeField] private TextMeshProUGUI _health;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private TextMeshProUGUI _targetScore;

    private WinState _state;

    public override void Setup(FSMState<WinState.EnterData> fsmState)
    {
        base.Setup(fsmState);

        _state = (WinState)fsmState;
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
        _health.text = _state.Health.ToString();
        _score.text = _state.Score.ToString();
        _targetScore.text = _state.TargetScore.ToString();
    }

    public void ContinueButtonClicked()
    {
        SignalBus.Fire<FSMSignal>(new FSMSignal(new MainFSMToGameplaySignalData(null)));
    }
}
