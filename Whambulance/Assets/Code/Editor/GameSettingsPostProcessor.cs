using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameSettingsPostProcessor : AssetPostprocessor
{
    private static bool Is<T>(string path) where T : Object
    {
        if (path.EndsWith(".prefab"))
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!prefab)
            {
                return false;
            }

            return prefab.GetComponent<T>();
        }

        return false;
    }

    private static void IfTypeThenLoad<T>(string path) where T : Object
    {
        float lastTime = EditorPrefs.GetFloat($"lastTimeLoaded{typeof(T)}", 0f);
        if (EditorApplication.timeSinceStartup < lastTime + 0.1f)
        {
            return;
        }

        if (Is<T>(path))
        {
            EditorPrefs.SetFloat($"lastTimeLoaded{typeof(T)}", (float)EditorApplication.timeSinceStartup);

            List<T> list = LoadAll<T>();
            string[] assetGuids = AssetDatabase.FindAssets("t:GameSettingsAsset");
            if (assetGuids.Length == 0)
            {
                //no game settings asset found
                return;
            }

            path = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
            GameSettingsAsset asset = AssetDatabase.LoadAssetAtPath<GameSettingsAsset>(path);
            GameSettings settings = asset.settings;
            if (typeof(T) == typeof(Level))
            {
                settings.levels = list.Cast<Level>().ToList();
            }
            else if (typeof(T) == typeof(Vehicle))
            {
                settings.vehicles = list.Cast<Vehicle>().ToList();
            }

            //mark as dirty and save
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }
    }

    private static List<T> LoadAll<T>() where T : Object
    {
        List<T> list = new List<T>();
        string[] guids = AssetDatabase.FindAssets("t:GameObject");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T level = AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<T>();
            if (level)
            {
                //then load every one of them in
                list.Add(level);
            }
        }

        return list;
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
        {
            IfTypeThenLoad<Level>(path);
            IfTypeThenLoad<Vehicle>(path);
        }

        foreach (string path in deletedAssets)
        {
            IfTypeThenLoad<Level>(path);
            IfTypeThenLoad<Vehicle>(path);
        }

        foreach (string path in movedAssets)
        {
            IfTypeThenLoad<Level>(path);
            IfTypeThenLoad<Vehicle>(path);
        }
    }
}
