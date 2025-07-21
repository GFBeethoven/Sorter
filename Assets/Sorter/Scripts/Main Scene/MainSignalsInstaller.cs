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
        Container.DeclareSignal<OnCannotDragDraggable>();

        Container.DeclareSignal<SortingGameplayHealthChanged>();
        Container.DeclareSignal<SortingGameplayScoreChanged>();
        Container.DeclareSignal<SortingGameplayFigureSorted>();
        Container.DeclareSignal<SortingGameplayFigureDestroyed>();
        Container.DeclareSignal<SortingGameplayWin>();
        Container.DeclareSignal<SortingGameplayLose>();
    }
}