using UnityEngine;

public class Player : MonoBehaviour
{
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
    private float radius = 0.3f;

    /// <summary>
    /// The movement component attached to this player.
    /// </summary>
    public PlayerMovement Movement { get; private set; }

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
        Instance = this;
    }

    private void Update()
    {
        //send inputs to the movement thingy
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(x, y);
        Movement.Input = input;

        if (Input.GetButtonDown("Jump"))
        {
            FindCar();
        }
    }

    /// <summary>
    /// Puts this thing on the player to make it look like its being carried.
    /// </summary>
    public void Carry(Objective objective)
    {
        CarryingObjective = objective;
        CarryingObjective.BeganCarrying(this);

        //parent this to the back transform
        CarryingObjective.transform.SetParent(carriedObjectRoot);
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

    //finds the cars within the circle collider's range
    void FindCar()
    {
        //Collects car's hit info and stores it in array
        Collider2D[] carsHit = Physics2D.OverlapCircleAll(transform.position + transform.up, 0.8f);
        if (carsHit.Length > 0)
        {
            for (int i = 0; i < carsHit.Length; i++)
            {
                if (carsHit[i].GetComponent<CarHit>())
                {
                    //Calls the hit car's launch function
                    carsHit[i].GetComponent<CarHit>().LaunchCar();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        //So we can see and adjust the OverlapCircle gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up, 0.8f);

        //also show the player radius just in case
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}