using UnityEngine;

public class PedestrianManager : MonoBehaviour
{
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

    private void OnCleared()
    {
        Clear();
    }

    private void SpawnAllPedestrians(Level level)
    {
        for (int i = 0; i < GameManager.Settings.maxPedestrians; i++)
        {
            //find a random point in this city block
            CityBlock randomBlock = level.CityBlocks[Random.Range(0, level.CityBlocks.Length)];
            Vector2 position = randomBlock.GetRandomPointOnSidewalk();

            //pick a random pedestrian and spawn them
            Pedestrian randomPedestrian = GameManager.Settings.pedestrians[Random.Range(0, GameManager.Settings.pedestrians.Count)];
            Pedestrian newPedestrian = Instantiate(randomPedestrian, position, Quaternion.identity);
            newPedestrian.name = randomPedestrian.name;
            newPedestrian.CityBlock = randomBlock;
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
