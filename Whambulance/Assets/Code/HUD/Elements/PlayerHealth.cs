using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : HUDElement
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

    private void Update()
    {
        Player player = Player.Instance;
        if (player)
        {
            float fillAmount = player.Health.HP / (float)player.Health.MaxHP;
            healthBar.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }
}