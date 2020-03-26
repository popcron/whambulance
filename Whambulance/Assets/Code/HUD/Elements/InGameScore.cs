using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameScore : HUDElement
{
    public TextMeshProUGUI scoreText;
    private bool inGame = true;
    InGameScore inGameScore;

    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inGame)
            UpdateScore();
    }

    private void OnEnable()
    {
        inGameScore = this;
        GameManager.onWon += OnWon;
    }

    private void OnDisable()
    {
        GameManager.onWon -= OnWon;
    }

    private void OnWon()
    {
        inGame = false;
    }

    void UpdateScore()
    {
       scoreText.text = "$" + ScoreManager.Bill.TotalValue.ToString();
    }
}
