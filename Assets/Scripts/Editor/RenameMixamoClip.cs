using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class RenameMixamoClip : EditorWindow
{
    [MenuItem("Tools/Rename Mixamo Animation Clips")]
    public static void RenameSelectedFBXClips()
    {
        var selectedObjects = Selection.GetFiltered<Object>(SelectionMode.Assets);

        foreach (var obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            if (!path.ToLower().EndsWith(".fbx"))
                continue;

            string fileName = Path.GetFileNameWithoutExtension(path);
            var modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

            if (modelImporter == null)
                continue;

            var clips = modelImporter.defaultClipAnimations;

            if (clips.Length == 0)
            {
                Debug.LogWarning($"No default clip found in {path}");
                continue;
            }

            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name.ToLower().Contains("mixamo"))
                {
                    clips[i].name = fileName;
                }
            }

            modelImporter.clipAnimations = clips;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            Debug.Log($"Renamed animation clip in {fileName}.fbx to '{fileName}'");
        }
    }
}
