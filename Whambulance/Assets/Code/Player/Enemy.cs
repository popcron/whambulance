using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        FOLLOWING,
        ADJUSTING,
        SHOOTING,
        DEAD
    }

    Rigidbody2D rb;
    Transform player;

    [SerializeField]
    private float avoidanceRadius = 0.5f;

    [SerializeField]
    private float firingRadius = 1.5f;

    private bool canSee;
    private List<GameObject> objectsInRange;
    GameObject go;

    EnemyState currentState = EnemyState.FOLLOWING;

    public float moveSpeed = 1.2f;
    public LayerMask playerMask;

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

            case EnemyState.ADJUSTING:
                AdjustRotation();
                break;

            case EnemyState.SHOOTING:
                Firing(FiringDirection());
                break;

            case EnemyState.DEAD:
                Kill();
                break;
        }
        IsPlayerInView();
    }

    void FollowPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.fixedDeltaTime);
    }

    void RotateTowardsTarget()
    {
        if (currentState != EnemyState.ADJUSTING)
        {
            float rotationSpeed = 10f;
            float offset = 90f;
            Vector3 direction = player.position - transform.position;
            direction.Normalize();
            float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    void AdjustRotation()
    {
        //Adjusting state logic will go here
    }

    IEnumerator Rotate()
    {
        //jic we need a coroutine for rotating the enemy around obstacles
        yield return new WaitForEndOfFrame();
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
        //Check all objects within the radius that the enemy can shoot
        for (int i = 0; i < objectsInRange.Count; i++)
        {
            go = objectsInRange[i];
            if (go != null)
            {
                canSee = true;
            }
        }

        if (canSee)
        {
            //figure out whether or not the objects in the radius are the player or not
            if (go && Player.Instance && go.gameObject == Player.Instance.gameObject)
            {
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

    Vector3 FiringDirection()
    {
        Vector3 directionToPlayer = transform.position - player.position;
        return directionToPlayer;
    }

    void Firing(Vector3 direction)
    {

    }

    void Kill()
    {

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
