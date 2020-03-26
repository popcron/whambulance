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

    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timePassed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= projectileLifetime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            ProjectileTravel(-PrefabInstance.direction);
        }
    }

    public void ProjectileTravel(Vector3 direction)
    {
        rb.MovePosition(transform.position + (direction * projectileSpeed) * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //if theres a health component, hurt it
        Health health = other.gameObject.GetComponentInParent<Health>();
        if (health)
        {
            health.Damage(projectileDamage, "enemy");
        }

        int layer = LayerMask.NameToLayer("Police");
        if (other.gameObject.layer != layer)
        {
            Destroy(gameObject);
        }
    }
}
