using UnityEngine;

public class InGame : HUDElement
{
    /// <summary>
    /// Only displays if the game is actually being played.
    /// </summary>
    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying;
        }
    }

    private void Update()
    {
        //tried to pause lmao
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.IsPaused)
            {
                GameManager.Unpause();
            }
            else
            {
                GameManager.Pause();
            }
        }
    }
}