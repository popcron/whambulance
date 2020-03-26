using UnityEngine;

public class PedestrianManager : MonoBehaviour
{
    private float nextUpdate;

    private void OnEnable()
    {
        LevelManager.onLevelLoaded += OnLevelLoaded;
        LevelManager.onCleared += OnCleared;
    }

    private void OnDisable()
    {
        LevelManager.onLevelLoaded -= OnLevelLoaded;
        LevelManager.onCleared -= OnCleared;
    }

    private void OnLevelLoaded(Level level)
    {
        //spawn peds everywhere
        SpawnAllPedestrians(level);
    }

    private void FixedUpdate()
    {
        if (nextUpdate > Time.time)
        {
            nextUpdate = Time.time + 0.1f;
            if (Level.All.Count > 0)
            {
                Level level = Level.All[0];
                if (level)
                {
                    float maxPerBlock = GameManager.Settings.maxPedestrians / (float)level.CityBlocks.Length;
                    for (int i = 0; i < level.CityBlocks.Length; i++)
                    {
                        CityBlock block = level.CityBlocks[i];
                        int count = PedestriansInCityBlock(block);
                        if (count < maxPerBlock)
                        {
                            //spawn a new one to replace it
                            Spawn(block);
                        }
                    }
                }
            }
        }
    }

    private int PedestriansInCityBlock(CityBlock block)
    {
        int c = 0;
        for (int i = 0; i < Player.All.Count; i++)
        {
            if (Player.All[i] is Pedestrian ped)
            {
                if (ped.CityBlock == block)
                {
                    c++;
                }
            }
        }

        return c;
    }

    private void OnCleared()
    {
        Clear();
    }

    private void Spawn(CityBlock block)
    {
        //find a random point in this city block
        Vector2 position = block.GetRandomPointOnSidewalk();

        //pick a random pedestrian and spawn them
        Pedestrian randomPedestrian = GameManager.Settings.pedestrians[Random.Range(0, GameManager.Settings.pedestrians.Count)];
        Pedestrian newPedestrian = Instantiate(randomPedestrian, position, Quaternion.identity);
        newPedestrian.name = randomPedestrian.name;
        newPedestrian.CityBlock = block;
    }

    private void SpawnAllPedestrians(Level level)
    {
        for (int i = 0; i < GameManager.Settings.maxPedestrians; i++)
        {
            CityBlock randomBlock = level.CityBlocks[Random.Range(0, level.CityBlocks.Length)];
            Spawn(randomBlock);
        }
    }

    /// <summary>
    /// Removes all pedestrians from the scene.
    /// </summary>
    public static void Clear()
    {
        Pedestrian[] pedestrians = FindObjectsOfType<Pedestrian>();
        foreach (Pedestrian pedestrian in pedestrians)
        {
            Destroy(pedestrian.gameObject);
        }
    }
}
