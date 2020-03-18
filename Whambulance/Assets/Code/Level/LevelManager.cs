using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public delegate void OnLevelLoaded(Level level);
    public delegate void OnCleared();

    /// <summary>
    /// Happens when a level is loaded.
    /// </summary>
    public static OnLevelLoaded onLevelLoaded;

    /// <summary>
    /// Happens when all levels are cleared.
    /// </summary>
    public static OnCleared onCleared;

    /// <summary>
    /// Loads a level by name and returns a new instance if successfull.
    /// </summary>
    public static Level Load(string name)
    {
        Clear();

        //check against names
        foreach (Level level in GameManager.Settings.levels)
        {
            if (level.name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                Level newLevel = Instantiate(level);
                newLevel.name = level.name;
                onLevelLoaded?.Invoke(newLevel);
                return newLevel;
            }
        }

        Debug.LogError($"Level named {name} is not in the list of levels on the GameSettings asset");
        return null;
    }

    /// <summary>
    /// Deletes any existing level.
    /// </summary>
    public static void Clear()
    {
        Level[] levels = FindObjectsOfType<Level>();
        foreach (Level level in levels)
        {
            Destroy(level.gameObject);
        }

        onCleared?.Invoke();
    }
}