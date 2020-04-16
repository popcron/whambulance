using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed = 0.7f;
    public float projectileLifetime = 3f;
    public int projectileDamage = 1;

    Rigidbody2D rb;

    [SerializeField]
    private float timePassed;

    [SerializeField]
    private Collider2D[] colliders = { };

    private Vector3 direction;

    public Collider2D[] Colliders => colliders;

    public void Initialize(Vector2 direction)
    {
        this.direction = direction;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        timePassed = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timePassed += Time.fixedDeltaTime;
        if (timePassed >= projectileLifetime)
        {
            Die();
        }
        else
        {
            ProjectileTravel(-direction);
        }
    }

    public void ProjectileTravel(Vector3 direction)
    {
        rb.MovePosition(transform.position + (direction * projectileSpeed) * Time.deltaTime);
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //if theres a health component, hurt it
        Health health = other.gameObject.GetComponentInParent<Health>();
        if (health)
        {
            health.Damage(projectileDamage, "enemy");
        }

        int policeLayer = LayerMask.NameToLayer("Police");
        if (other.gameObject.layer != policeLayer && other.gameObject.layer != gameObject.layer)
        {
            Die();
        }
    }
}
