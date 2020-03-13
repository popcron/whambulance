using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettingsData : ScriptableObject
{
    public GameSettings settings = new GameSettings();
}