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
            return !GameManager.IsPlaying && !AdvancementsMenu.Show;
        }
    }

    public void ClickedPlay()
    {
        GameManager.Play();
    }

    public void ClickedSettings()
    {
    }

    public void ClickedAdvancements()
    {
        AdvancementsMenu.Show = true;
    }

    public void ClickedQuit()
    {
        GameManager.Quit();
    }

    private void Update()
    {

    }
}