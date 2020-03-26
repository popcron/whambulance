using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameTimer : HUDElement
{
    GameSettings settings;
    InGameTimer inGameTimer;

    float maxDeliveryTime;
    float maxRescueTime;
    float totalTime;

    public Gradient timerGradient;
    public Image circleTimerImage;

    [Range(0, 1)]
    public float radialDecimal;

    // Start is called before the first frame update
    void Start()
    {
        settings = GameManager.Settings;

        maxDeliveryTime = settings.maxDeliveryTime;
        maxRescueTime = settings.maxRescueTime;
        totalTime = maxRescueTime + maxDeliveryTime;
    }

    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying;
        }
    }

    private void OnEnable()
    {
        inGameTimer = this;
        GameManager.onWon += OnWon;
    }

    private void OnDisable()
    {
        GameManager.onWon -= OnWon;
    }

    public void OnWon()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateTimeLeft();
        UpdateTimerColor();
    }

    private void CalculateTimeLeft()
    {
        if (GameManager.IsDelivering)
        {
            float deliveryTimePassed = maxDeliveryTime - GameManager.DeliveryTime;
            radialDecimal = deliveryTimePassed / maxDeliveryTime;
            circleTimerImage.fillAmount = radialDecimal;
        }
        else
        {
            float timePassed = maxRescueTime - GameManager.RescuingTime;
            radialDecimal = timePassed / maxRescueTime;
            circleTimerImage.fillAmount = radialDecimal;
        }
    }

    private void UpdateTimerColor()
    {
        Color timerColor = timerGradient.Evaluate(radialDecimal);
        GetComponentInChildren<Image>().color = timerColor;
    }
}
