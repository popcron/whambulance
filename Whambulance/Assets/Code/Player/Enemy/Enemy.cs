using System.Collections;
using System.Collections.Generic;
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
    Transform player;

    [SerializeField]
    private float avoidanceRadius = 0.5f;

    [SerializeField]
    private float firingRate = 2f;

    private float firingCoolDown = 0;

    private bool canSee;
    private bool fired;
    private List<GameObject> objectsInRange;
    GameObject go;

    EnemyState currentState = EnemyState.FOLLOWING;

    public GameObject projectilePrefab;
    public float moveSpeed = 1.2f;
    public LayerMask playerMask;
    public int health;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = Player.Instance.transform;

        objectsInRange = new List<GameObject>();
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
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.fixedDeltaTime);
    }

    void RotateTowardsTarget()
    {
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

    bool IsPlayerInView()
    {
        if (!fired)
        {
            //Check all objects within the radius that the enemy can shoot
            for (int i = 0; i < objectsInRange.Count; i++)
            {
                go = objectsInRange[i];
                if (go && go.GetComponentInParent<Damage>() && go.GetComponentInParent<Player>())
                {
                    canSee = true;
                }
            }

            if (canSee)
            {
                //figure out whether or not the objects in the radius are the player or not
                if (go && go.GetComponentInParent<Damage>() && go.GetComponentInParent<Player>())
                {
                    currentState = EnemyState.SHOOTING;
                    Debug.Log("PLAYER IN SIGHT");
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
        Vector3 directionToPlayer = transform.position - player.position;
        return directionToPlayer;
    }

    void Firing()
    {
        if (!fired)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, transform.parent);
            currentState = EnemyState.FOLLOWING;
            fired = true;
        }
    }

    bool IsAlive()
    {
        if (health > 0)
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
        Destroy(this);
    }

    //GIZMO DRAWING FOR INTERNAL TESTING
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

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
