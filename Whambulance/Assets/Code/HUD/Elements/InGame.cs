using UnityEngine;
using UnityEngine.SceneManagement;

public class InGame : HUDElement
{
    /// <summary>
    /// Only displays if the game is actually being played.
    /// </summary>
    public override bool ShouldDisplay
    {
        get
        {
            return Game.IsPlaying;
        }
    }

    private void Update()
    {
        
    }
}