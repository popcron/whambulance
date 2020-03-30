using TMPro;
using UnityEngine;

public class InGameScore : HUDElement
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying;
        }
    }

    private void Update()
    {
        scoreText.text = ScoreManager.Bill.TotalValue.ToString("C");
    }
}
