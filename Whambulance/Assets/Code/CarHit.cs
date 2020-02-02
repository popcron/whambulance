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
    public int carMoveFrames;

    //Local Variables
    bool isCarActive;
    Rigidbody2D carBody;
    int iterations;

    // Start is called before the first frame update
    void Start()
    {
        carBody = GetComponent<Rigidbody2D>();
    }


    public void LaunchCar()
    {
        Vector2 launchDirection = -(player.transform.position - transform.position).normalized;

        carBody.velocity += (launchDirection * launchSpeed);
        StartCoroutine(SlowDownImpact(launchDirection));
    }

    IEnumerator SlowDownImpact(Vector2 direction)
    {
        while (carMoveFrames > 0)
        {
            carBody.velocity -= Vector2.one * direction * Time.deltaTime;
            carMoveFrames--;
            if (carMoveFrames <= 0)
            {
                carBody.velocity = Vector2.zero;
            }
            yield return null;
        }
        
    }

}
