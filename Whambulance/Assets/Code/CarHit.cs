using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHit : MonoBehaviour
{
    //References
    public Player player;

    //Public Variables
    public float launchDistance;
    public float spriteGrowth;

    //Local Variables
    bool isCarActive;
    Rigidbody2D carBody;

    // Start is called before the first frame update
    void Start()
    {
        carBody = GetComponent<Rigidbody2D>();
    }

    public void LaunchCar()
    {
        Vector2 launchDirection = -(player.transform.position - transform.position).normalized;

        carBody.velocity += (launchDirection * launchDistance);
    }

}
