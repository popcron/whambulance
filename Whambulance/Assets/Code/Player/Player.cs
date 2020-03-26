using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static List<Player> All { get; set; } = new List<Player>();

    public delegate void OnPickedUpObjective(Player player);

    public static OnPickedUpObjective onPickedUpObjective;

    /// <summary>
    /// The current player in existence.
    /// </summary>
    public static Player Instance { get; private set; }

    /// <summary>
    /// Returns the radius of the player.
    /// </summary>
    public static float Radius => Instance != null ? Instance.radius : 0.3f;

    [SerializeField]
    private Transform carriedObjectRoot;

    [SerializeField]
    private float punchRadius = 0.8f;

    [SerializeField]
    private float radius = 0.3f;

    /// <summary>
    /// The movement component attached to this player.
    /// </summary>
    public PlayerMovement Movement { get; private set; }

    public int health;

    /// <summary>
    /// The direction that the player should be moving in based on inputs.
    /// </summary>
    public virtual Vector2 MovementInput
    {
        get
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            Vector2 input = new Vector2(x, y);
            return input.normalized;
        }
    }

    /// <summary>
    /// Should this player be punching?
    /// </summary>
    public virtual bool Punch
    {
        get
        {
            return Input.GetButtonDown("Jump");
        }
    }

    /// <summary>
    /// The rotation in degrees that the player should be looking at.
    /// </summary>
    public float Rotation { get; protected set; }

    /// <summary>
    /// The objective that is being carried if any.
    /// </summary>
    public Objective CarryingObjective { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    public virtual void OnDrawGizmos()
    {
        //So we can see and adjust the OverlapCircle gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up, punchRadius);

        //also show the player radius just in case
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public virtual void Update()
    {
        //send inputs to the movement thingy
        Vector2 input = MovementInput;
        Movement.Input = input;
        if (Punch)
        {
            FindCar();
        }

        //rotate the root
        if (input.sqrMagnitude > 0.5f)
        {
            Rotation = (Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg) - 90f;
        }

        transform.eulerAngles = new Vector3(0f, 0f, Rotation);

        if (GetType() == typeof(Player))
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Puts this thing on the player to make it look like its being carried.
    /// </summary>
    public void Carry(Objective objective)
    {
        if (CarryingObjective != objective)
        {
            CarryingObjective = objective;
            CarryingObjective.BeganCarrying(this);

            //parent this to the back transform
            CarryingObjective.transform.SetParent(carriedObjectRoot);
            CarryingObjective.transform.localPosition = Vector3.zero;
            CarryingObjective.transform.localEulerAngles = Vector3.zero;
            onPickedUpObjective?.Invoke(this);
        }
    }

    /// <summary>
    /// Drops the current carried objective if any is present.
    /// </summary>
    public void Drop()
    {
        if (CarryingObjective)
        {
            CarryingObjective.Dropped(this);
            CarryingObjective.transform.SetParent(null);
        }
    }

    /// <summary>
    /// Finds the cars within the circle collider's range.
    /// </summary>
    private void FindCar()
    {
        //Collects car's hit info and stores it in array
        Collider2D[] carsHit = Physics2D.OverlapCircleAll(transform.position + transform.up, punchRadius);
        if (carsHit.Length > 0)
        {
            for (int i = 0; i < carsHit.Length; i++)
            {
                CarHit carHit = carsHit[i].GetComponentInParent<CarHit>();
                if (carHit)
                {
                    //Calls the hit car's launch function
                    carHit.LaunchCar();
                }
            }
        }
    }

    /// <summary>
    /// Returns the closest player that overlaps with this position.
    /// </summary>
    public static Player Get(Vector2 position, float extraRadius = 0f)
    {
        int index = -1;
        float closest = float.MaxValue;
        for (int i = 0; i < All.Count; i++)
        {
            float distance = Vector2.SqrMagnitude(position - (Vector2)All[i].transform.position);
            if (distance < closest)
            {
                closest = distance;
                index = i;
            }
        }

        if (index != -1)
        {
            Player player = All[index];
            if (closest <= (player.radius + extraRadius) * (player.radius + extraRadius))
            {
                return player;
            }
        }

        return null;
    }

    /// <summary>
    /// Stealthly destroys all players from the scene.
    /// </summary>
    public static void DestroyAll()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            Destroy(player.gameObject);
        }
    }
}