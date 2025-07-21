using UnityEngine;
using Zenject;

public class MainSortingGameplayInstaller : MonoInstaller
{
    [SerializeField] private SortingGameplayFigure _sortingGameplayFigure;
    [SerializeField] private SortingGameplayBelt _sortingGameplayBelt;
    [SerializeField] private SortingGameplayFigureHole _sortingGameplayFigureHole;
    
    public override void InstallBindings()
    {
        Container.BindMemoryPool<SortingGameplayFigure, SortingGameplayFigure.Pool>().WithInitialSize(16)
            .FromComponentInNewPrefab(_sortingGameplayFigure).UnderTransformGroup("Figures");

        Container.BindMemoryPool<SortingGameplayBelt, SortingGameplayBelt.Pool>().WithInitialSize(5)
            .FromComponentInNewPrefab(_sortingGameplayBelt).UnderTransformGroup("Belts");

        Container.BindMemoryPool<SortingGameplayFigureHole, SortingGameplayFigureHole.Pool>().WithInitialSize(6)
            .FromComponentInNewPrefab(_sortingGameplayFigureHole).UnderTransformGroup("Holes");

        Container.Bind<SortingGameplay.LaunchParameters.IFactory>()
            .To<SortingGameplay.LaunchParameters.RandomFactory>()
            .AsSingle();

        Container.BindInterfacesAndSelfTo<SortingGameplayCanvas>().FromComponentInHierarchy(true).AsSingle();

        Container.Bind<SortingGameplay>().ToSelf().AsSingle();

        Container.Bind<DraggablesDispatcher>().ToSelf().AsSingle();

        Container.Bind<SpriteRendererTilledBackground>().FromComponentInHierarchy(true).AsSingle();

        Container.BindInterfacesAndSelfTo<SortingGameplayFXs>().FromComponentInHierarchy(true).AsSingle();
    }
}