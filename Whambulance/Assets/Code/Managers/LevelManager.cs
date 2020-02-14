using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager levelManager;

    [SerializeField]
    private Level[] levels = { };

    private void Awake()
    {
        Clear();
    }

    /// <summary>
    /// Loads a level by name and returns a new instance if successfull.
    /// </summary>
    public static Level Load(string name)
    {
        if (!levelManager)
        {
            levelManager = FindObjectOfType<LevelManager>();
        }

        Clear();

        //check against names
        name = name.ToLower();
        foreach (Level level in levelManager.levels)
        {
            if (level.name.ToLower() == name)
            {
                Level newLevel = Instantiate(level);
                newLevel.name = level.name;
                return newLevel;
            }
        }

        Debug.LogError($"Level named {name} is not in the list of levels on the LevelManager");
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
    }
}