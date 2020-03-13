using System;
using System.Collections.Generic;

[Serializable]
public class GameSettings
{
    public string levelToLoad = "TestLevel";
    public Player playerPrefab;
    public List<Level> levels = new List<Level>();
}
