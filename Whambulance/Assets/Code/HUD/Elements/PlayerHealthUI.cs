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
    private Vector2 offset = new Vector2(0f, 1f);

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private Gradient healthColor;

    [SerializeField]
    private TMP_Text healthText;

    [SerializeField]
    private float damageFlashDuration = 0.4f;

    private float damagedTime;

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
            damagedTime = damageFlashDuration;
        }
    }

    private void Update()
    {
        damagedTime -= Time.deltaTime;
        Player player = Player.Instance;
        if (player)
        {
            //position onto player
            Vector2 shake = default;
            if (damagedTime > 0)
            {
                float t = damagedTime / damageFlashDuration * 0.5f;
                shake.x = (Mathf.PerlinNoise(Time.time * 40f, 0.3f) - 0.5f) * t;
                shake.y = (Mathf.PerlinNoise(0.6f, Time.time * 40f) - 0.5f) * t;
            }

            transform.position = player.transform.position + (Vector3)(offset + shake);

            //get % of hp
            float fillAmount = player.Health.HP / (float)player.Health.MaxHP;
            fillAmount = Mathf.Clamp01(fillAmount);

            Color color = default;
            if (damagedTime > 0)
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