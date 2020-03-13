using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;

    public delegate void OnWon();
    public delegate void OnLost();
    public delegate void OnStoppedPlaying();
    public delegate void OnStartedPlaying();

    /// <summary>
    /// Happens when the player wins the game.
    /// </summary>
    public static OnWon onWon;

    /// <summary>
    /// Happens when the player loses.
    /// </summary>
    public static OnLost onLost;

    /// <summary>
    /// Happens when the Leave method is called.
    /// </summary>
    public static OnStoppedPlaying onStoppedPlaying;

    /// <summary>
    /// Happens when the Play method is called.
    /// </summary>
    public static OnStartedPlaying onStartedPlaying;

    private static GameManager Manager
    {
        get
        {
            if (!gameManager)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            return gameManager;
        }
    }

    /// <summary>
    /// The game settings to use here.
    /// </summary>
    public static GameSettings Settings
    {
        get => Manager.gameSettings.settings;
    }

    /// <summary>
    /// Has the player won or lost while playing the game.
    /// </summary>
    public static bool IsConcluded
    {
        get => Manager.isPlaying && (Manager.won || Manager.lost);
    }

    /// <summary>
    /// Is the game considered to be in the play state?
    /// </summary>
    public static bool IsPlaying
    {
        get => Manager.isPlaying;
        private set => Manager.isPlaying = value;
    }

    /// <summary>
    /// Is the game currently considered to be paused?
    /// </summary>
    public static bool IsPaused
    {
        //time scale so small that time is practically stopped
        get => Mathf.Abs(Time.timeScale) <= 0.001f;
    }

    [SerializeField]
    private GameSettingsAsset gameSettings;

    [SerializeField]
    private bool isPlaying;

    [SerializeField]
    private bool won;

    [SerializeField]
    private bool lost;

    /// <summary>
    /// Starts the game, thats about it.
    /// </summary>
    public static void Play()
    {
        Unpause();
        Time.timeScale = 1f;
        Manager.won = false;
        Manager.lost = false;

        LevelManager.Load(Settings.levelToLoad);
        ScoreManager.Clear();

        IsPlaying = true;
        SpawnPlayer();

        onStartedPlaying?.Invoke();
    }

    /// <summary>
    /// Spawns a new player and returns its instance.
    /// If a player already exists, it will simply replace this new player.
    /// </summary>
    private static Player SpawnPlayer()
    {
        DestroyPlayer();

        Player newPlayer = Instantiate(Settings.playerPrefab);
        newPlayer.name = Settings.playerPrefab.name;

        //find a random spawnpoint, and put the player there
        Spawnpoint[] spawnpoints = FindObjectsOfType<Spawnpoint>();
        if (spawnpoints.Length > 0)
        {
            Spawnpoint randomSpawnpoint = spawnpoints[Random.Range(0, spawnpoints.Length)];
            newPlayer.transform.position = randomSpawnpoint.transform.position;
        }

        return newPlayer;
    }

    /// <summary>
    /// Stealthly destroys the player from the game.
    /// </summary>
    public static void DestroyPlayer()
    {
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
        LevelManager.Clear();
        ScoreManager.Clear();

        Time.timeScale = 1f;
        Manager.won = false;
        Manager.lost = false;
        DestroyPlayer();
        Unpause();
        IsPlaying = false;

        onStoppedPlaying?.Invoke();
    }

    /// <summary>
    /// Quits and closes the game, duh.
    /// </summary>
    public static void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Pauses the game, only if the game hasnt concluded.
    /// </summary>
    public static void Pause()
    {
        if (!IsConcluded)
        {
            Time.timeScale = 0.001f;
        }
    }

    /// <summary>
    /// Unpauses teh game, only if the game hasnt concluded.
    /// </summary>
    public static void Unpause()
    {
        if (!IsConcluded)
        {
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Makes the player win no matter what.
    /// </summary>
    public static void Win()
    {
        if (IsPlaying && !Manager.won)
        {
            Manager.won = true;
            Manager.lost = false;

            Time.timeScale = 0.5f;
            onWon?.Invoke();
        }
    }

    /// <summary>
    /// Makes the player lose no matter what.
    /// </summary>
    public static void Lose()
    {
        if (IsPlaying && !Manager.lost)
        {
            Manager.lost = true;
            Manager.won = false;

            Time.timeScale = 0.5f;
            onLost?.Invoke();
        }
    }
}