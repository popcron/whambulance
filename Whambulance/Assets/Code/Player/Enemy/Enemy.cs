using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        FOLLOWING,
        SHOOTING,
        DEAD
    }

    Rigidbody2D rb;

    [SerializeField]
    private GameObject shootEffect;

    [SerializeField]
    private float avoidanceRadius = 0.5f;

    [SerializeField]
    private float firingRate = 2f;

    [SerializeField]
    private float viewRange = 5f;

    private float firingCoolDown = 0;
    private bool fired;
    private Health health;
    private EnemyState currentState = EnemyState.FOLLOWING;

    public GameObject projectilePrefab;
    public float moveSpeed = 1.2f;
    public LayerMask playerMask;

    public Collider2D[] Colliders { get; private set; }

    private void Awake()
    {
        Colliders = GetComponentsInChildren<Collider2D>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Health.onDied += OnDied;
    }

    private void OnDisable()
    {
        Health.onDied -= OnDied;
    }

    private void OnDied(Health health)
    {
        if (health == this.health)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateTowardsTarget();
        switch (currentState)
        {
            case EnemyState.FOLLOWING:
                FollowPlayer();
                break;

            case EnemyState.SHOOTING:
                Firing();
                break;

            case EnemyState.DEAD:
                Kill();
                break;
        }

        IsPlayerInView();
        if (fired)
        {
            firingCoolDown += Time.deltaTime;
            if (firingCoolDown >= firingRate)
            {
                firingCoolDown = 0f;
                fired = false;
            }
        }
    }

    void FollowPlayer()
    {
        Player player = Player.Instance;
        if (!player)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.fixedDeltaTime);
    }

    void RotateTowardsTarget()
    {
        Player player = Player.Instance;
        if (!player)
        {
            return;
        }

        float rotationSpeed = 10f;
        float offset = 90f;
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Returns true if a player is seen.
    /// Will also change the state to shooting if visible.
    /// </summary>
    private bool IsPlayerInView()
    {
        if (!fired)
        {
            //check if any player is visible within the view range
            for (int i = 0; i < Player.All.Count; i++)
            {
                if (!(Player.All[i] is Pedestrian))
                {
                    float sqrMagnitude = Vector2.SqrMagnitude(Player.All[i].transform.position - transform.position);
                    if (sqrMagnitude <= viewRange * viewRange)
                    {
                        currentState = EnemyState.SHOOTING;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public Vector3 FiringDirection()
    {
        Player player = Player.Instance;
        if (!player)
        {
            return default;
        }

        Vector3 directionToPlayer = transform.position - player.transform.position;
        return directionToPlayer;
    }

    void Firing()
    {
        Player player = Player.Instance;
        if (!player)
        {
            return;
        }

        if (!fired)
        {
            Vector2 spawnpoint = transform.position + transform.forward;
            Projectile projectile = Instantiate(projectilePrefab, spawnpoint, Quaternion.identity).GetComponent<Projectile>();
            projectile.Initialize(FiringDirection());
            currentState = EnemyState.FOLLOWING;
            fired = true;

            //ignore this projectile
            for (int i = 0; i < Colliders.Length; i++)
            {
                for (int p = 0; p < projectile.Colliders.Length; p++)
                {
                    Physics2D.IgnoreCollision(Colliders[i], projectile.Colliders[p], true);
                }
            }

            MakeShootEffect(spawnpoint);
        }
    }

    private void MakeShootEffect(Vector2 position)
    {
        if (shootEffect)
        {
            GameObject instance = Instantiate(shootEffect, position, Quaternion.identity);
            Destroy(instance, 2f);
        }
    }

    bool IsAlive()
    {
        //this was health > 0
        if (!health.IsDead)
        {
            return true;
        }
        else
        {
            currentState = EnemyState.DEAD;
            return false;
        }
    }

    void Kill()
    {
        Destroy(gameObject);
    }

    //GIZMO DRAWING FOR INTERNAL TESTING
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

        Player player = Player.Instance;
        if (player)
        {
            if (!IsPlayerInView())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, player.transform.position);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, player.transform.position);
            }
        }
    }
}
