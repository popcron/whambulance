using UnityEditor;
using UnityEngine;

public class LevelsPostProcessor : AssetPostprocessor
{
    private static bool IsLevel(string path)
    {
        if (path.EndsWith(".prefab"))
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!prefab)
            {
                return false;
            }

            return prefab.GetComponent<Level>();
        }

        return false;
    }

    private static void LoadAllLevels()
    {
        string[] assetGuids = AssetDatabase.FindAssets("t:GameSettingsAsset");
        if (assetGuids.Length == 0)
        {
            //no game settings asset found
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
        GameSettingsAsset asset = AssetDatabase.LoadAssetAtPath<GameSettingsAsset>(path);
        GameSettings settings = asset.settings;

        //load all by searching all prefabs
        settings.levels.Clear();
        string[] guids = AssetDatabase.FindAssets("t:GameObject");
        foreach (string guid in guids)
        {
            path = AssetDatabase.GUIDToAssetPath(guid);
            Level level = AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<Level>();
            if (level)
            {
                //then load every one of them in
                settings.levels.Add(level);
            }
        }

        //mark as dirty and save
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
        {
            if (IsLevel(path))
            {
                LoadAllLevels();
                return;
            }
        }

        foreach (string path in deletedAssets)
        {
            if (IsLevel(path))
            {
                LoadAllLevels();
                return;
            }
        }

        foreach (string path in movedAssets)
        {
            if (IsLevel(path))
            {
                LoadAllLevels();
                return;
            }
        }
    }
}
