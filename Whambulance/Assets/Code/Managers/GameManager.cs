using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameSettings defaultSettings = new GameSettings();
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
        get
        {
            if (!Manager)
            {
                return defaultSettings;
            }

            return Manager.gameSettings.settings;
        }
    }

    /// <summary>
    /// The amount of currency that the player has accumulated. Saved accross sessions.
    /// </summary>
    public static float Currency
    {
        get
        {
            return PlayerPrefs.GetFloat("currency", 0);
        }
        set
        {
            PlayerPrefs.SetFloat("currency", Mathf.Clamp(value, 0, 999999999));
        }
    }

    /// <summary>
    /// Has the player won or lost while playing the game.
    /// </summary>
    public static bool IsConcluded
    {
        get
        {
            if (!Manager)
            {
                return false;
            }

            return Manager.isPlaying && (Manager.won || Manager.lost);
        }
    }

    public static float RescuingTime => Manager.rescueTime;
    public static float DeliveryTime => Manager.deliveryTime;
    public static float TotalTime => Manager.rescueTime + Manager.deliveryTime;
    public static bool IsDelivering
    {
        get
        {
            if (Player.Instance)
            {
                return Player.Instance.CarryingObjective;
            }
            else
            {
                return false;
            }
        }
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

    private float rescueTime;
    private float deliveryTime;
    private bool shownIntroDialog;

    /// <summary>
    /// Starts the game, thats about it.
    /// </summary>
    public static void Play()
    {
        Leave();

        LevelManager.Load(Settings.levelToLoad);

        IsPlaying = true;
        SpawnPlayer();
        SpawnPatients(1);
        onStartedPlaying?.Invoke();
    }

    /// <summary>
    /// Spawns a new player and returns its instance.
    /// If a player already exists, it will simply replace this new player.
    /// </summary>
    private static Player SpawnPlayer()
    {
        Player newPlayer = Instantiate(Settings.playerPrefab);
        newPlayer.name = Settings.playerPrefab.name;

        //find a random spawnpoint, and put the player there
        Spawnpoint randomSpawnpoint = Spawnpoint.GetRandomSpawnpoint(true);
        if (randomSpawnpoint)
        {
            newPlayer.transform.position = randomSpawnpoint.transform.position;
        }

        return newPlayer;
    }

    /// <summary>
    /// Spawns an objective into the currently loaded level.
    /// </summary>
    private static void SpawnPatients(int patientsToSpawn)
    {
        for (int i = 0; i < patientsToSpawn; i++)
        {
            Objective prefab = Settings.patients[Random.Range(0, Settings.patients.Count)];
            Objective objective = Instantiate(prefab);
            objective.name = prefab.name;

            //find a level to parent to
            if (Level.All.Count > 0)
            {
                objective.transform.SetParent(Level.All[0].transform);
            }

            //find a spawnpoint
            Spawnpoint randomSpawnpoint = Spawnpoint.GetRandomSpawnpoint(false);
            if (randomSpawnpoint)
            {
                objective.transform.position = randomSpawnpoint.transform.position;
            }
        }
    }

    /// <summary>
    /// Makes the game stop playing.
    /// </summary>
    public static void Leave()
    {
        LevelManager.Clear();
        ScoreManager.Clear();
        Player.DestroyAll();

        Time.timeScale = 1f;
        Manager.shownIntroDialog = false;
        Manager.won = false;
        Manager.lost = false;
        Manager.rescueTime = 0f;
        Manager.deliveryTime = 0f;
        Unpause();

        if (IsPlaying)
        {
            IsPlaying = false;
            onStoppedPlaying?.Invoke();
        }
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
    public static void Win(string reason)
    {
        if (IsPlaying && !Manager.won)
        {
            Manager.won = true;
            Manager.lost = false;

            Time.timeScale = 0.5f;
            onWon?.Invoke();

            Analytics.Won(reason);

            //add scrore from bill to game
            Currency += ScoreManager.Bill.TotalValue;

        }
    }

    /// <summary>
    /// Makes the player lose no matter what.
    /// </summary>
    public static void Lose(string reason)
    {
        if (IsPlaying && !Manager.lost)
        {
            Manager.lost = true;
            Manager.won = false;

            Time.timeScale = 0.5f;
            onLost?.Invoke();

            Analytics.Lost(reason);
        }
    }

    private void Update()
    {
        if (IsPlaying && !IsConcluded)
        {
            if (IsDelivering)
            {
                deliveryTime += Time.deltaTime;

                //limit reached, die
                if (deliveryTime > Settings.maxDeliveryTime)
                {
                    Lose("Ran out of delivery time.");
                }
            }
            else
            {
                rescueTime += Time.deltaTime;

                //show call to action here after 1s
                if (rescueTime > 1f && !shownIntroDialog)
                {
                    shownIntroDialog = true;
                    string dialog = Settings.introDialogs[Random.Range(0, Settings.introDialogs.Length)];
                    TextDialog.Show("Hospital", dialog);
                }

                //limit reached, die
                if (rescueTime > Settings.maxRescueTime)
                {
                    Lose("Ran out of rescue time.");
                }
            }
        }
    }
}