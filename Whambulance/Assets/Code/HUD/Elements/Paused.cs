using UnityEngine;

public class Paused : HUDElement
{
    /// <summary>
    /// Only displays if the game is being played and the time scale is stupidly small.
    /// </summary>
    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying && GameManager.IsPaused;
        }
    }

    public void ClickedResume()
    {
        GameManager.Unpause();
    }

    public void ClickedSettings()
    {
    }

    public void ClickedLeave()
    {
        GameManager.Leave();
    }
}