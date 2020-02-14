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
        Debug.Log("MainMenu.ClickedPlay");
        GameManager.Play();
    }

    public void ClickedSettings()
    {
        Debug.Log("MainMenu.ClickedSettings");
    }

    public void ClickedQuit()
    {
        Debug.Log("MainMenu.ClickedQuit");
        GameManager.Quit();
    }

    private void Update()
    {

    }
}