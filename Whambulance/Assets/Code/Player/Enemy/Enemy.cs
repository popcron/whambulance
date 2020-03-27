using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrefabInstance
{
    public static GameObject projectileInstance;
    public static Vector3 direction;
}

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        FOLLOWING,
        SHOOTING,
        DEAD
    }

    Rigidbody2D rb;
    Transform player;

    [SerializeField]
    private float avoidanceRadius = 0.5f;

    [SerializeField]
    private float firingRate = 2f;

    private float firingCoolDown = 0;

    private bool canSee;
    private bool fired;
    private Health health;
    private List<GameObject> objectsInRange;
    GameObject go;

    EnemyState currentState = EnemyState.FOLLOWING;

    public GameObject projectilePrefab;
    public float moveSpeed = 1.2f;
    public LayerMask playerMask;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        player = Player.Instance.transform;

        objectsInRange = new List<GameObject>();
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
                fired = false;
            }
        }
    }

    void FollowPlayer()
    {
        if (!player)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.fixedDeltaTime);
    }

    void RotateTowardsTarget()
    {
        if (!player)
        {
            return;
        }

        float rotationSpeed = 10f;
        float offset = 90f;
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (objectsInRange.Contains(other.gameObject)) return;
        objectsInRange.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        objectsInRange.Remove(other.gameObject);
        canSee = false;
    }

    /// <summary>
    /// Returns true if this object is a player.
    /// </summary>
    private bool IsObjectPlayer(GameObject gameObject)
    {
        if (!gameObject)
        {
            return false;
        }

        return gameObject.GetComponentInParent<Player>() == Player.Instance;
    }

    bool IsPlayerInView()
    {
        if (!fired)
        {
            //Check all objects within the radius that the enemy can shoot
            for (int i = 0; i < objectsInRange.Count; i++)
            {
                go = objectsInRange[i];
                if (IsObjectPlayer(go))
                {
                    canSee = true;
                }
            }

            if (canSee)
            {
                //figure out whether or not the objects in the radius are the player or not
                if (IsObjectPlayer(go))
                {
                    currentState = EnemyState.SHOOTING;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public Vector3 FiringDirection()
    {
        if (!player)
        {
            return default;
        }

        Vector3 directionToPlayer = transform.position - player.position;
        return directionToPlayer;
    }

    void Firing()
    {
        if (!player)
        {
            return;
        }

        if (!fired)
        {
            PrefabInstance.projectileInstance = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.identity);
            PrefabInstance.direction = FiringDirection();
            currentState = EnemyState.FOLLOWING;
            fired = true;
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

        if (player)
        {
            if (!IsPlayerInView())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, player.position);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }
}
