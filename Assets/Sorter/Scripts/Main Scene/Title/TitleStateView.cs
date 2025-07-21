using UnityEngine;

public class TitleStateView : FSMStateMono<TitleState.EnterData>
{
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void PlayButtonClicked()
    {
        SignalBus.Fire(new FSMSignal(new MainFSMToGameplaySignalData(null)));
    }    
}
