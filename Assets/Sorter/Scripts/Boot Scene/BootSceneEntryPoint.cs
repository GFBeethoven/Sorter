using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class BootSceneEntryPoint : IInitializable
{
    [Inject] private LoadingScreen _loadingScreen;

    public void Initialize()
    {
        ILoadingOperation[] operations = new ILoadingOperation[]
        {
            new LoadGameSettingsOperation(),
            new WaitAPIInitializeOperation(),
            new LoadGameStateOperation()
        };

        _loadingScreen.Show();
        _loadingScreen.PerformLoadingOperations(operations, LoadingSuccess, LoadingFailure);
    }

    private void LoadingSuccess()
    {
        SceneManager.LoadScene(Scenes.Main);
    }

    private void LoadingFailure(ILoadingOperation failedOperation)
    {
        Debug.LogError($"Failed to load: {failedOperation.GetType().FullName}");
    }
}
