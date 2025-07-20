using UnityEngine;

public class TitleStateView : FSMStateMono<TitleState.EnterData>
{
    public void PlayButtonClicked()
    {
        SignalBus.Fire(new FSMSignal(new MainFSMToGameplaySignalData(null)));
    }    
}
