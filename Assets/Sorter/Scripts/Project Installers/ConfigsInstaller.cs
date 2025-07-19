using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ConfigsInstaller", menuName = "Installers/ConfigsInstaller")]
public class ConfigsInstaller : ScriptableObjectInstaller<ConfigsInstaller>
{
    [SerializeField] private GameplayLayoutConfig _layoutConfig;
    [SerializeField] private FigureConfig _figureConfig;
    [SerializeField] private GameplayConfig _gameplayConfig;

    public override void InstallBindings()
    {
        Container.Bind<GameplayLayoutConfig>().FromInstance(_layoutConfig).AsSingle();
        Container.Bind<FigureConfig>().FromInstance(_figureConfig).AsSingle();
        Container.Bind<GameplayConfig>().FromInstance(_gameplayConfig).AsSingle();
    }
}