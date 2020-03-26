using UnityEngine;

public class AwardIfTookDamage : MonoBehaviour
{
    [SerializeField]
    private string offenceName = "Nuisance";

    [SerializeField]
    private int value = 100;

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
        if (health.gameObject == gameObject)
        {
            ScoreManager.AwardPoints(offenceName, value);
        }
    }
}