using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// The current player in existence.
    /// </summary>
    public static Player Instance { get; private set; }

    /// <summary>
    /// The movement component attached to this player.
    /// </summary>
    public PlayerMovement Movement { get; private set; }

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

    //So we can see and adjust the OverlapCircle gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up, 0.8f);
    }
}