using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public static class Loader
{ 
private class LoadingMonoBehaviour : MonoBehaviour { }


    public enum Scene
{
    GameScene,
    MainMenuScene,
    CombatScene,
    Loading,
    
}

private static UnityAction OnLoaderCallback;
private static AsyncOperation loadingAsyncOperation;

public static void Load(Scene scene)
{
    OnLoaderCallback = () =>
    {
        GameObject loadingGameObject = new GameObject("Loading Game Object");
        loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));

    };

    SceneManager.LoadScene(Scene.Loading.ToString());
}

private static IEnumerator LoadSceneAsync(Scene scene)
{
    yield return null;

    AsyncOperation asyncOperation =  SceneManager.LoadSceneAsync(scene.ToString());

    while(!asyncOperation.isDone)
    {
        yield return null;
    }
}

public static float GetLoadingProgress()
{
    if(loadingAsyncOperation != null)
    {
        return loadingAsyncOperation.progress;
    }
    else
    {
        return 1f;
    }
}

public static void LoaderCallback()
{
    if(OnLoaderCallback != null)
    {
            OnLoaderCallback();
            OnLoaderCallback = null;
    }
}

}
