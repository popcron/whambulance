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
        direction = PrefabInstance.direction;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timePassed += Time.fixedDeltaTime;
        if (timePassed >= projectileLifetime)
        {
            Destroy(this.gameObject);
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer != 10)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponentInParent<Damage>().PlayerTakeDamage(projectileDamage);
            }
            Destroy(this.gameObject);
        }
    }
}
