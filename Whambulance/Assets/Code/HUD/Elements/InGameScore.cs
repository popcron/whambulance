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
        float value = ScoreManager.Bill.TotalValue;
        if (value < 0f)
        {
            scoreText.text = $"-{Mathf.Abs(value).ToString("C")}";
        }
        else
        {
            scoreText.text = value.ToString("C");
        }
    }
}
