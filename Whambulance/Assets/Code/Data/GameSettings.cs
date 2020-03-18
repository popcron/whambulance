using System;
using System.Collections.Generic;

[Serializable]
public class GameSettings
{
    public string levelToLoad = "TestLevel";
    public Player playerPrefab;
    public float maxRoadAngle = 14f;
    public float sideWalkSize = 0.5f;
    public List<Level> levels = new List<Level>();
    public List<Vehicle> vehicles = new List<Vehicle>();
    public List<Pedestrian> pedestrians = new List<Pedestrian>();
    public List<AdvancementAsset> advancements = new List<AdvancementAsset>();
    public int maxVehicles = 48;
    public float vehicleLifeTime = 16f;
    public int maxPedestrians = 48;
}
