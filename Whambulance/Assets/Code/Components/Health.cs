using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    /// <summary>
    /// List of all health components in the scene.
    /// </summary>
    public static List<Health> All { get; set; } = new List<Health>();

    public delegate void OnDied(Health health);
    public delegate void OnDamaged(Health health, int damage);
    public delegate void OnHealed(Health health, int heal);

    public static OnDied onDied;
    public static OnDamaged onDamaged;
    public static OnHealed onHealed;

    [SerializeField]
    private int health = 12;

    [SerializeField]
    private int maxHealth = 12;

    /// <summary>
    /// Is this health component considered dead?
    /// </summary>
    public bool IsDead => health <= 0;

    /// <summary>
    /// The current amount of health.
    /// </summary>
    public int HP => health;

    /// <summary>
    /// The maximum health.
    /// </summary>
    public int MaxHP => maxHealth;

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDestroy()
    {
        All.Remove(this);
    }

    /// <summary>
    /// Silently sets the health and max.
    /// </summary>
    public void Set(int health, int maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;
    }

    /// <summary>
    /// Damages this component.
    /// </summary>
    public void Damage(int amount)
    {
        if (amount > 0 && !IsDead)
        {
            int oldHealth = health;
            health = Mathf.Clamp(health - amount, 0, maxHealth);
            if (oldHealth > health)
            {
                onDamaged?.Invoke(this, amount);
            }

            if (health == 0)
            {
                onDied?.Invoke(this);
            }
        }
    }

    /// <summary>
    /// Heals this component.
    /// </summary>
    public void Heal(int amount)
    {
        if (amount > 0 && !IsDead)
        {
            int oldHealth = health;
            health = Mathf.Clamp(health + amount, 0, maxHealth);
            if (oldHealth < health)
            {
                onHealed?.Invoke(this, amount);
            }
        }
    }

    /// <summary>
    /// Instantly kills this health component.
    /// </summary>
    public void Kill()
    {
        if (!IsDead)
        {
            health = 0;
            onDied?.Invoke(this);
        }
    }
}