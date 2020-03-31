using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject footstepEffect;

    [SerializeField]
    private GameObject waterSplash;

    [SerializeField]
    private float movementSpeed = 3f;

    [SerializeField]
    private float accelerationFactor = 100f;

    [SerializeField]
    private float decelerationFactor = 16f;

    [SerializeField]
    private float footstepRate = 1f;

    private float stunTimer;
    private float distanceMoved;

    /// <summary>
    /// The input that this player should move with.
    /// </summary>
    public Vector2 Input { get; set; }

    /// <summary>
    /// Is this player moving according to the input?
    /// </summary>
    public bool IsMoving => Input.sqrMagnitude > 0.5f;

    /// <summary>
    /// The rigidbody attached to this player.
    /// </summary>
    public Rigidbody2D Rigidbody { get; private set; }

    public void Stun(float stunDuration)
    {
        stunTimer = Mathf.Max(stunTimer, stunDuration);
    }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Rigidbody.gravityScale = 0f;
        Rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        stunTimer -= Time.deltaTime;
    }

    private void MakeFootstepEffect(Vector2 position)
    {
        GameObject prefab = footstepEffect;

        if (waterSplash)
        {
            //check if inside water
            for (int i = 0; i < Water.All.Count; i++)
            {
                if (Water.All[i].Contains(position))
                {
                    prefab = waterSplash;
                    break;
                }
            }
        }

        //spawn effect
        if (prefab)
        {
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            Destroy(instance, 8f);
        }
    }

    private void FixedUpdate()
    {
        if (stunTimer > 0f)
        {
            return;
        }

        float acceleration = IsMoving ? accelerationFactor : decelerationFactor;
        Vector2 target = Input.normalized * movementSpeed;
        Vector2 velocityChange = target - Rigidbody.velocity;

        if (IsMoving)
        {
            distanceMoved += Rigidbody.velocity.magnitude;
            if (distanceMoved > footstepRate)
            {
                distanceMoved = 0f;
                MakeFootstepEffect(transform.position);
            }
        }

        //apply a force that attempts to reach the target
        velocityChange.x = Mathf.Clamp(velocityChange.x, -acceleration, acceleration);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -acceleration, acceleration);
        Rigidbody.AddForce(velocityChange * Rigidbody.mass, ForceMode2D.Impulse);
    }
}