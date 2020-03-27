using TMPro;
using UnityEngine;

public class LoseScreen : HUDElement
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

    [SerializeField]
    private TMP_Text title;

    [SerializeField]
    private TMP_Text description;

    private bool show;

    private void OnEnable()
    {
        GameManager.onLost += OnLost;
        GameManager.onStartedPlaying += OnStartedPlaying;
        GameManager.onStoppedPlaying += OnStoppedPlaying;
    }

    private void OnDisable()
    {
        GameManager.onLost -= OnLost;
        GameManager.onStartedPlaying -= OnStartedPlaying;
        GameManager.onStoppedPlaying -= OnStoppedPlaying;
    }

    private void OnStoppedPlaying()
    {
        show = false;
    }

    private void OnStartedPlaying()
    {
        show = false;
    }

    private void OnLost()
    {
        show = true;
        title.text = "YOU LOSE";
        description.text = "Good luck next time!";
    }

    private void Update()
    {
        root.gameObject.SetActive(show);
    }

    public void ClickedAgain()
    {
        GameManager.Play();
    }

    public void ClickedLeave()
    {
        GameManager.Leave();
    }
}
