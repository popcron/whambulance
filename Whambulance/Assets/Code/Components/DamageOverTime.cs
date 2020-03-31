using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    [SerializeField]
    private GameObject damageEffect;

    [SerializeField]
    private float damageInterval = 1f;

    [SerializeField]
    private string damageCause = "other";

    private float damageTime;
    private List<Health> healthsInside = new List<Health>();
    
    private void MakeDamageEffect(Vector2 position)
    {
        if (damageEffect)
        {
            GameObject instance = Instantiate(damageEffect, position, Quaternion.identity);
            Destroy(instance, 2f);
        }
    }

    private void FixedUpdate()
    {
        damageTime += Time.fixedDeltaTime;
        bool damage = false;
        if (damageTime > damageInterval)
        {
            damageTime = 0f;
            damage = true;
        }

        if (damage)
        {
            for (int i = 0; i < healthsInside.Count; i++)
            {
                healthsInside[i].Damage(1, damageCause);
            }
        }

        healthsInside.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Health health = collision.GetComponentInParent<Health>();
        if (health)
        {
            healthsInside.Add(health);
        }
    }
}