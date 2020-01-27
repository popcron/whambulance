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
        //get velocity, lerp it to destination velocity, then assign it back
        Vector3 velocity = Rigidbody.velocity;
        float acceleration = IsMoving ? accelerationFactor : decelerationFactor;
        velocity = Vector3.Lerp(velocity, Input.normalized * movementSpeed, Time.fixedDeltaTime * acceleration);
        Rigidbody.velocity = velocity;
    }
}