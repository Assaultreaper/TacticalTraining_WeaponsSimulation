using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(WaypointManager))]
public class WaypointPlacerEditor : Editor
{
    private WaypointManager manager;
    private int waypointCount => manager.waypoints.Count;
    private bool placementMode = false;

    private static HashSet<GameObject> previouslyEditableObjects = new();

    private void OnEnable()
    {
        manager = (WaypointManager)target;
        SceneView.duringSceneGui += HandleSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= HandleSceneGUI;
        SetAllSceneObjectsPickable(true); // Restore selection for everything
        SetWaypointPickable(true);
        HandleUtility.Repaint(); // Refresh the editor view to reflect changes
    }

    private void HandleSceneGUI(SceneView sceneView)
    {
        if (!placementMode) return;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CreateWaypointAt(hit.point);
                e.Use();
            }
        }

        if (placementMode)
        {
            // Prevent selection of non-waypoint objects
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
    }

    private void CreateWaypointAt(Vector3 position)
    {
        GameObject wp = new GameObject($"Waypoint_{waypointCount}");
        wp.transform.position = position;
        wp.transform.parent = manager.transform;

        Undo.RegisterCreatedObjectUndo(wp, "Create Waypoint");
        Undo.RecordObject(manager, "Add Waypoint");
        manager.waypoints.Add(wp.transform);

        EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
    }

    private void SetWaypointPickable(bool pickable)
    {
        foreach (Transform wp in manager.waypoints)
        {
            if (wp == null) continue;

            GameObject go = wp.gameObject;

            var flags = go.hideFlags;
            if (pickable)
                flags &= ~HideFlags.NotEditable;
            else
                flags |= HideFlags.NotEditable;

            if (go.hideFlags != flags)
            {
                go.hideFlags = flags;
                EditorUtility.SetDirty(go);
            }
        }

        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    private void SetAllSceneObjectsPickable(bool pickable)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go == manager.gameObject || manager.waypoints.Contains(go.transform))
                continue;

            var flags = go.hideFlags;

            if (pickable)
            {
                if (previouslyEditableObjects.Contains(go))
                {
                    flags &= ~HideFlags.NotEditable;
                    previouslyEditableObjects.Remove(go);
                }
            }
            else
            {
                if ((flags & HideFlags.NotEditable) == 0)
                {
                    flags |= HideFlags.NotEditable;
                    previouslyEditableObjects.Add(go);
                }
            }

            if (go.hideFlags != flags)
            {
                go.hideFlags = flags;
                EditorUtility.SetDirty(go);
            }
        }

        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        if (GUILayout.Toggle(placementMode, placementMode ? "Disable Placement Mode" : "Enable Placement Mode", "Button"))
        {
            if (!placementMode)
            {
                placementMode = true;
                SetWaypointPickable(false);
                SetAllSceneObjectsPickable(false);
            }
        }
        else
        {
            if (placementMode)
            {
                placementMode = false;
                SetWaypointPickable(true);
                SetAllSceneObjectsPickable(true);
            }
        }

        EditorGUILayout.HelpBox(
            "Click 'Enable Placement Mode' to place waypoints.\nWhile active, only the WaypointManager is selectable.",
            MessageType.Info
        );
    }
}
