using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public static List<Vehicle> All { get; set; } = new List<Vehicle>();

    [SerializeField]
    private Vector2 front = new Vector2(1f, 0f);

    [SerializeField]
    private float breakSpeed = 2f;

    [SerializeField]
    private float steerSpeed = 7f;

    [SerializeField]
    private float maxSteerAngle = 55f;

    [SerializeField]
    private float accelerationSpeed = 4f;

    [SerializeField]
    private float movementSpeed = 4f;

    private float steer;
    private float angle;
    private float speed;

    /// <summary>
    /// The amount of seconds that this vehicle has been alive for.
    /// </summary>
    public float LifeTime { get; private set; }

    /// <summary>
    /// Returns true if this vehicle is visible by any camera.
    /// </summary>
    public bool IsVisible
    {
        get
        {
            foreach (SpriteRenderer renderer in SpriteRenderers)
            {
                if (renderer.enabled && renderer.isVisible)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// The amount of seconds that this vehicle has been off screen for.
    /// </summary>
    public float TimeOffscreen { get; private set; }

    public VehicleFront Front { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }
    public SpriteRenderer[] SpriteRenderers { get; private set; }

    /// <summary>
    /// The front of the car in world space.
    /// </summary>
    public Vector2 FrontPosition => transform.TransformPoint(front);

    /// <summary>
    /// Returns the forward direction of this vehicle in world space.
    /// </summary>
    public Vector2 ForwardDirection => transform.TransformDirection(front.normalized);

    /// <summary>
    /// The steering amount in degrees.
    /// </summary>
    public float Steer
    {
        get => steer;
        set => steer = Mathf.Clamp(value, -maxSteerAngle, maxSteerAngle);
    }

    /// <summary>
    /// The amount of speed that should be applied. Range from -1 to 1.
    /// </summary>
    public float Gas { get; set; } = 0f;

    /// <summary>
    /// The maximum angle that this vehicle is able to steer with.
    /// </summary>
    public float MaxSteerAngle => maxSteerAngle;

    private void Awake()
    {
        SpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Front = GetComponentInChildren<VehicleFront>();
        Rigidbody = GetComponent<Rigidbody2D>();
        angle = Rigidbody.rotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(front));
    }

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    private void FixedUpdate()
    {
        //ensure the front is proper?
        if (Front)
        {
            if (Front.Rigidbody)
            {
                Front.Rigidbody.MovePosition(Rigidbody.position);
            }

            Front.transform.localEulerAngles = default;
        }

        //apply movement
        if (speed < Gas)
        {
            speed = Mathf.MoveTowards(speed, Gas, Time.fixedDeltaTime * accelerationSpeed);
        }
        else
        {
            speed = Mathf.MoveTowards(speed, Gas, Time.fixedDeltaTime * breakSpeed);
        }

        Rigidbody.velocity = ForwardDirection * Mathf.Lerp(-movementSpeed * 0.5f, movementSpeed, speed);
        angle += steer * Time.fixedDeltaTime * steerSpeed;
        Rigidbody.SetRotation(angle);
    }

    private void Update()
    {
        if (!IsVisible)
        {
            //this car is offscreen
            TimeOffscreen += Time.deltaTime;
        }
        else
        {
            TimeOffscreen = 0f;
        }

        LifeTime += Time.deltaTime;
    }
}