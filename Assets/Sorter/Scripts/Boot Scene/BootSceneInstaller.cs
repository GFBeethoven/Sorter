using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class BootSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Popups>().FromComponentInHierarchy(true).AsSingle();
        Container.Bind<LoadingScreen>().FromComponentInHierarchy(true).AsSingle();

        Container.BindInterfacesAndSelfTo<BootSceneEntryPoint>().AsSingle();
    }
}