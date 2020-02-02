using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHit : MonoBehaviour
{
    //References
    public Player player;

    //Public Variables
    public float launchSpeed;
    public float spriteGrowth;


    //Local Variables
    bool didCollide;
    Rigidbody2D carBody;
    int iterations;
    int carMoveFrames = 90;

    // Start is called before the first frame update
    void Start()
    {
        //Gets the car's rigidbody
        carBody = GetComponent<Rigidbody2D>();
    }


    public void LaunchCar()
    {
        carMoveFrames = 90;
        //finds the direction from the center of the player versus the center of the vehicle hit
        Vector2 launchDirection = -(player.transform.position - transform.position).normalized;

        //adds an increase in velocity based on the direction
        carBody.velocity += (launchDirection * launchSpeed);
        StartCoroutine(SlowDownImpact(launchDirection));
    }

    IEnumerator SlowDownImpact(Vector2 direction)
    {
        //during the time specified by carMoveFrames, the car will slow to a stop
        while (carMoveFrames > 0)
        {
            carBody.velocity -= Vector2.one * direction * Time.deltaTime;
            carMoveFrames--;
            if (carMoveFrames <= 0 || didCollide)
            {
                carBody.velocity = Vector2.zero;
            }
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        didCollide = true;
        carMoveFrames = 0;
    }
}
