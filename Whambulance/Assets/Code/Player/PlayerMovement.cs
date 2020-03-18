using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 3f;

    [SerializeField]
    private float accelerationFactor = 100f;

    [SerializeField]
    private float decelerationFactor = 16f;

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

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Rigidbody.gravityScale = 0f;
        Rigidbody.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        float acceleration = IsMoving ? accelerationFactor : decelerationFactor;
        Vector2 target = Input.normalized * movementSpeed;
        Vector2 velocityChange = target - Rigidbody.velocity;

        //apply a force that attempts to reach the target
        velocityChange.x = Mathf.Clamp(velocityChange.x, -acceleration, acceleration);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -acceleration, acceleration);
        Rigidbody.AddForce(velocityChange * Rigidbody.mass, ForceMode2D.Impulse);
    }
}