using UnityEngine;
using Zenject;

public class MainFSMInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<TitleStateView>().FromComponentInHierarchy(true).AsSingle();
        Container.Bind<WinStateView>().FromComponentInHierarchy(true).AsSingle();
        Container.Bind<LoseStateView>().FromComponentInHierarchy(true).AsSingle();

        Container.Bind<IState>().To<TitleState>().AsSingle();
        Container.Bind<IState>().To<GameplayState>().AsSingle();
        Container.Bind<IState>().To<WinState>().AsSingle();
        Container.Bind<IState>().To<LoseState>().AsSingle();

        Container.Bind<FSM>().FromMethod((context) =>
        {
            return new FSM(context.Container.ResolveAll<IState>().ToArray(), new TitleState.EnterData());
        });
    }
}