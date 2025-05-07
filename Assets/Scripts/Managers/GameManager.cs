using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public SceneName CurrentScene;
    public Scenarios CurrentScenario;

    public void SceneSelectionFromScenario()
    {
        switch (CurrentScenario)
        {
            case Scenarios.None:
                CurrentScene = SceneName.Main;
                break;
            case Scenarios.Prototype1:
                CurrentScene = SceneName.PrototypingEvironment;
                break;
            case Scenarios.ForestFlatGround:
            case Scenarios.ForestUpHill:
            case Scenarios.ForestDownHill:
                CurrentScene = SceneName.Forest;
                break;
        }
    }
}
