using UnityEngine;
using Zenject;

public class SharedGameObjectsInstaller : MonoInstaller
{
    [SerializeField] private GameObject _sharedObjects;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        var shared = Instantiate(_sharedObjects, null);
        DontDestroyOnLoad(shared);

        Container.Bind<GameViewport>().FromInstance(_sharedObjects.GetComponentInChildren<GameViewport>())
            .AsSingle();
    }
}