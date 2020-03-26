using System.Collections;
using UnityEngine;

public class CarHit : MonoBehaviour
{
    //References
    Player player;

    //Public Variables
    public float launchSpeed;
    public float spriteGrowth;
    public float decelModifier;
    [Range(0.01f, 2.5f)]
    public float objectMass;


    //Local Variables
    bool didCollide;
    Rigidbody2D carBody;
    int iterations;
    int carMoveFrames = 90;
    float originalDecel;

    // Start is called before the first frame update
    void Start()
    {
        //Gets the car's rigidbody
        carBody = GetComponent<Rigidbody2D>();
        originalDecel = decelModifier;
        player = Player.Instance;
    }


    public void LaunchCar()
    {
        return;
        decelModifier = originalDecel;
        carMoveFrames = 90;
        didCollide = false;

        //finds the direction from the center of the player versus the center of the vehicle hit
        Vector2 launchDirection = -(player.transform.position - transform.position).normalized;
        Vector2 distanceFromCar = player.transform.position - transform.position;

        if (Mathf.Abs(distanceFromCar.y) > 1.5f || Mathf.Abs(distanceFromCar.x) > 1.5f)
        {
            decelModifier -= 0.5f;
        }

        //adds an increase in velocity based on the direction
        Vector2 force = launchDirection * new Vector2(launchSpeed - Mathf.Abs(distanceFromCar.x), launchSpeed - Mathf.Abs(distanceFromCar.y));
        carBody.velocity += force;

        StopAllCoroutines();
        StartCoroutine(SlowDownImpact(launchDirection));
    }

    IEnumerator SlowDownImpact(Vector2 direction)
    {
        //during the time specified by carMoveFrames, the car will slow to a stop
        while (carMoveFrames > 0)
        {
            //determines the speed at which the object is sent after a punch
            carBody.velocity -= (Vector2.one * decelModifier) * direction * Time.deltaTime;
            carMoveFrames--;
            Debug.Log("SLOWING DOWN");
            if (carMoveFrames <= 0 || didCollide)
            {
                carBody.velocity = Vector2.zero;
            }
            yield return null;
        }

        Debug.Log("DONE SLOWING DOWN");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        didCollide = true;
        carMoveFrames = 0;
    }
}
