using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenarioManager: MonoBehaviour
{
    [SerializeField]private string previousScene;

    public void SelectScene()
    {
        previousScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneAsync(GameManager.Instance.CurrentScene.ToString(), OnComplete));
    }

    public IEnumerator LoadSceneAsync(string sceneName, Action OnComplete)
    {
        var SceneLoadingAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => SceneLoadingAsync.isDone);
        SceneLoadingAsync = SceneManager.UnloadSceneAsync(previousScene.ToString());
        yield return new WaitUntil(() => SceneLoadingAsync.isDone);
        OnComplete?.Invoke();
    }
    private void OnComplete()
    {
        Debug.Log($"{GameManager.Instance.CurrentScene} Switched from {previousScene} Completed");
    }
}
