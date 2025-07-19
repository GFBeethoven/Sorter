using UnityEngine;
using Zenject;

public class MainSignalsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<FSMSignal>();
        Container.DeclareSignal<OnStartDragDraggable>();
        Container.DeclareSignal<OnDragDraggable>();
        Container.DeclareSignal<OnEndDragDraggable>();
    }
}