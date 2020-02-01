using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// The movement component attached to this player.
    /// </summary>
    public PlayerMovement Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
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

    void FindCar()
    {
        Collider2D[] carsHit = Physics2D.OverlapCircleAll(transform.position, 0.8f);
        if (carsHit.Length > 0)
        {
            for (int i = 0; i < carsHit.Length; i++)
            {
                if (carsHit[i].GetComponent<CarHit>())
                {
                    Debug.Log("HIT CAR");
                    carsHit[i].GetComponent<CarHit>().LaunchCar();
                }
            }
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.8f);
    }
}