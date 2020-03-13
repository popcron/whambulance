using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettingsAsset : ScriptableObject
{
    public GameSettings settings = new GameSettings();
}