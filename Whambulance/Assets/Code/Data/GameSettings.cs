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
}
