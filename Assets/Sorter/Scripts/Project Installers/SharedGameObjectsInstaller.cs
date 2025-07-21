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

        Camera camera = shared.GetComponentInChildren<Camera>();
        GameViewport viewport = shared.GetComponentInChildren<GameViewport>();

        Container.Bind<Camera>().FromInstance(camera).AsSingle();
        Container.Bind<GameViewport>().FromInstance(viewport).AsSingle();

        Container.Bind<IInitializable>().FromInstance(viewport);

        Container.QueueForInject(viewport);
    }
}