using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;

    /// <summary>
    /// Is the game considered to be in the play state?
    /// </summary>
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

    /// <summary>
    /// Is the game currently considered to be paused?
    /// </summary>
    public static bool IsPaused
    {
        get
        {
            //time scale so small that time is practically stopped
            return Mathf.Abs(Time.timeScale) <= 0.001f;
        }
    }

    [SerializeField]
    private bool isPlaying;

    [SerializeField]
    private Player playerPrefab;

    /// <summary>
    /// Starts the game, thats about it.
    /// </summary>
    public static void Play()
    {
        Debug.Log("GameManager.Play");

        //load the test level and spawn a player into it
        LevelManager.Load("TestLevel");
        IsPlaying = true;
        SpawnPlayer();
    }

    /// <summary>
    /// Spawns a new player and returns its instance.
    /// If a player already exists, it will simply replace this new player.
    /// </summary>
    private static Player SpawnPlayer()
    {
        Debug.Log("GameManager.SpawnPlayer");

        DestroyPlayer();

        Player newPlayer = Instantiate(gameManager.playerPrefab);
        newPlayer.name = gameManager.playerPrefab.name;
        return newPlayer;
    }

    /// <summary>
    /// Stealthly destroys the player from the game.
    /// </summary>
    public static void DestroyPlayer()
    {
        Debug.Log("GameManager.DestroyPlayer");

        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            Destroy(player.gameObject);
        }
    }

    /// <summary>
    /// Makes the game stop playing.
    /// </summary>
    public static void Leave()
    {
        Debug.Log("GameManager.Leave");

        LevelManager.Clear();
        DestroyPlayer();
        Unpause();
        IsPlaying = false;
    }

    /// <summary>
    /// Quits and closes the game, duh.
    /// </summary>
    public static void Quit()
    {
        Debug.Log("GameManager.Quit");
        Application.Quit();
    }

    public static void Pause()
    {
        Time.timeScale = 0.001f;
    }

    public static void Unpause()
    {
        Time.timeScale = 1f;
    }
}