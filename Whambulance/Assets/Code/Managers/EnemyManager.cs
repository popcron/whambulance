using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private float nextSpawn;
    private float time;

    private void OnEnable()
    {
        Player.onPickedUpObjective += OnPickedUp;
        LevelManager.onLevelLoaded += OnLevelLoaded;
        LevelManager.onCleared += OnCleared;
    }

    private void OnDisable()
    {
        Player.onPickedUpObjective -= OnPickedUp;
        LevelManager.onLevelLoaded -= OnLevelLoaded;
        LevelManager.onCleared -= OnCleared;
    }

    private void OnPickedUp(Player player)
    {
        time = 0f;
    }

    private void OnLevelLoaded(Level level)
    {
        time = 0f;
    }

    private void OnCleared()
    {
        Clear();
    }

    private void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + 1f;
            Spawn();
        }

        time += Time.deltaTime;
    }

    private void Spawn()
    {
        if (Level.All.Count > 0 && Player.Instance)
        {
            Level level = Level.All[0];

            //get current amount
            int enemies = FindObjectsOfType<Enemy>().Length;
            float t = Mathf.Clamp01(time / GameManager.Settings.maxRescueTime);
            t *= 0.5f;

            //if the player has the patient, then delivery phase
            if (Player.Instance.CarryingObjective)
            {
                t = Mathf.Clamp01(time / GameManager.Settings.maxDeliveryTime);
                t *= 0.5f;
                t += 0.5f;
            }

            int maxEnemies = (int)(GameManager.Settings.maxEnemiesOverTime.Evaluate(t) * GameManager.Settings.maxEnemiesAlive);
            if (enemies >= maxEnemies)
            {
                //too many
                return;
            }

            //find a random city block
            CityBlock randomBlock = level.CityBlocks[Random.Range(0, level.CityBlocks.Length)];
            Vector2 position = randomBlock.GetRandomPointOnSidewalk();

            //pick a random enemy and spawn them
            GameObject randomEnemy = GameManager.Settings.enemyUnits[Random.Range(0, GameManager.Settings.enemyUnits.Count)];
            GameObject newEnemy = Instantiate(randomEnemy, position, Quaternion.identity);
            newEnemy.name = randomEnemy.name;
            //newPedestrian.CityBlock = randomBlock;
        }
    }

    /// <summary>
    /// Removes all pedestrians from the scene.
    /// </summary>
    public static void Clear()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }
}