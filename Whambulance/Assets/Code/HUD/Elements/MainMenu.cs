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
            return !GameManager.IsPlaying && !AdvancementsMenu.Show && !CreditsMenu.Show;
        }
    }

    [SerializeField]
    private TMPro.TMP_Text tip;

    private void OnEnable()
    {
        Tips tips = GameManager.Settings.tips.tips;
        tip.text = tips.tips[Random.Range(0, tips.tips.Length)];
    }

    public void ClickedPlay()
    {
        GameManager.Play();
    }

    public void ClickedCredits()
    {
        CreditsMenu.Show = true;
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