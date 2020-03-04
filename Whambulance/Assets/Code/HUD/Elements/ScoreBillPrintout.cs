using UnityEngine;

public class ScoreBillPrintout : HUDElement
{
    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying;
        }
    }

    [SerializeField]
    private RectTransform root;

    private bool show;

    private void OnEnable()
    {
        GameManager.onWon += OnWon;
        GameManager.onStartedPlaying += OnStartedPlaying;
        GameManager.onStoppedPlaying += OnStoppedPlaying;
    }

    private void OnDisable()
    {
        GameManager.onWon -= OnWon;
        GameManager.onStartedPlaying -= OnStartedPlaying;
        GameManager.onStoppedPlaying -= OnStoppedPlaying;
    }

    private void OnWon()
    {
        show = true;
    }

    private void OnStoppedPlaying()
    {
        show = false;
    }

    private void OnStartedPlaying()
    {
        show = false;
    }

    public void ClickedAgain()
    {
        GameManager.Play();
    }

    public void ClickedLeave()
    {
        GameManager.Leave();
    }

    private void Update()
    {
        root.gameObject.SetActive(show);
    }
}