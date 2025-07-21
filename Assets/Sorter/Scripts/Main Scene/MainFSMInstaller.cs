using UnityEngine;
using Zenject;

public class MainFSMInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<TitleStateView>().FromComponentInHierarchy(true).AsSingle();
        Container.BindInterfacesAndSelfTo<WinStateView>().FromComponentInHierarchy(true).AsSingle();
        Container.BindInterfacesAndSelfTo<LoseStateView>().FromComponentInHierarchy(true).AsSingle();
        Container.BindInterfacesAndSelfTo<SortingGameplayView>().FromComponentInHierarchy(true).AsSingle();

        Container.BindInterfacesAndSelfTo<TitleState>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameplayState>().AsSingle();
        Container.BindInterfacesAndSelfTo<WinState>().AsSingle();
        Container.BindInterfacesAndSelfTo<LoseState>().AsSingle();

        Container.Bind<FSM>().ToSelf().AsSingle();

        Container.BindInterfacesAndSelfTo<MainFSMEntryPoint>().AsSingle();

        Container.Bind<TransitionCanvas>().FromComponentInHierarchy(true).AsSingle();
    }
}