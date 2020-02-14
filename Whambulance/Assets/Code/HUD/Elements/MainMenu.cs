using UnityEngine;

public class MainMenu : HUDElement
{
    /// <summary>
    /// Only displays if the game isnt being played atm.
    /// </summary>
    public override bool ShouldDisplay
    {
        get
        {
            return !GameManager.IsPlaying;
        }
    }

    public void ClickedPlay()
    {
        Game.Play();
    }

    public void ClickedSettings()
    {
        Debug.Log("settings");
    }

    public void ClickedQuit()
    {
        Game.Quit();
    }

    private void Update()
    {

    }
}