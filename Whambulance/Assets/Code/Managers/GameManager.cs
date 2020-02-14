using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;

    public static bool IsPlaying
    {
        get
        {
            if (!gameManager)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            return gameManager.isPlaying;
        }
        private set
        {
            if (!gameManager)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            gameManager.isPlaying = value;
        }
    }

    [SerializeField]
    private bool isPlaying;

    /// <summary>
    /// Starts the game, thats about it.
    /// </summary>
    public static void Play()
    {
        Debug.Log("GameManager.Play");
    }

    /// <summary>
    /// Quits the game, duh.
    /// </summary>
    public static void Quit()
    {
        Debug.Log("GameManager.Quit");
        Application.Quit();
    }
}