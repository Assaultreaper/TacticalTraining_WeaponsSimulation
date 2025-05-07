using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour 
{
    public Camera _camera => Camera.main;

    [Serializable]
    public struct CameraData
    {
        public Vector3 CameraPosition;
        public Vector3 CameraRotation;
        public Scenarios ScenarioType;
    }

    public Scenarios RequiredScenario => GameManager.Instance.CurrentScenario;
    public List<CameraData> Data;

    public CameraData GetCurrentScenarioCameraData()
    {
        return Data.Find(x => x.ScenarioType == RequiredScenario);
    }

    public IEnumerator Start()
    {
        yield return new WaitUntil(() => _camera != null);
        SetCamera(GetCurrentScenarioCameraData());
    }

    private void SetCamera(CameraData data)
    {
        _camera.transform.position = data.CameraPosition;
        _camera.transform.eulerAngles = data.CameraRotation;
    }

    void OnValidate()
    {
        SetCamera(GetCurrentScenarioCameraData());
    }
}
