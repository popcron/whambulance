using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : HUDElement
{
    /// <summary>
    /// Only displays if the game is actually being played and the player is living.
    /// </summary>
    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying && Player.Instance;
        }
    }

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private Gradient healthColor;

    [SerializeField]
    private TMP_Text healthText;

    [SerializeField]
    private float damageFlashDuration = 0.4f;

    private float flashTime;

    private void OnEnable()
    {
        Health.onDamaged += OnDamaged;
    }

    private void OnDisable()
    {
        Health.onDamaged -= OnDamaged;
    }

    private void OnDamaged(Health health, int damage)
    {
        //our player took damage, flash for this much
        if (Player.Instance && health == Player.Instance.Health)
        {
            flashTime = damageFlashDuration;
        }
    }

    private void Update()
    {
        flashTime -= Time.deltaTime;
        Player player = Player.Instance;
        if (player)
        {
            //get % of hp
            float fillAmount = player.Health.HP / (float)player.Health.MaxHP;
            fillAmount = Mathf.Clamp01(fillAmount);

            Color color = default;
            if (flashTime > 0)
            {
                bool flash = Mathf.RoundToInt(Time.time * 20f) % 2 == 0;
                color = flash ? Color.white : Color.black;
            }
            else
            {
                color = healthColor.Evaluate(1f - fillAmount);
            }

            //assign fill and color based on %
            healthBar.fillAmount = fillAmount;
            healthBar.color = color;
            healthText.text = $"{player.Health.HP} / {player.Health.MaxHP}";
        }
    }
}