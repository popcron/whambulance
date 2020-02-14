using UnityEngine;

public class MainMenu : HUDElement
{
    /// <summary>
    /// Only displays if the player prefs setting for mainMenu is 1
    /// </summary>
    public override bool ShouldDisplay
    {
        get
        {
            return PlayerPrefs.GetInt("mainMenu") == 1;
        }
    }

    public void ClickedPlay()
    {

    }

    public void ClickedSettings()
    {

    }

    public void ClickedQuit()
    {

    }

    private void Update()
    {

    }
}