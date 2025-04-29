using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public SceneName _SceneName;

    void Start()
    {
        SetScene(_SceneName);
    }
    public void SetScene(SceneName name)
    {
        _SceneName = name;
        StartCoroutine(LoadLevel(OnComplete));
    }

    public IEnumerator LoadLevel(Action OnComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_SceneName.ToString());
        yield return new WaitUntil(() => asyncLoad.isDone == true);
        OnComplete?.Invoke();
    }

    private void OnComplete()
    {
        Debug.Log($"{_SceneName} Loaded Successfully");
    }
}
