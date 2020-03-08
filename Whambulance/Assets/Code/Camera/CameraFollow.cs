using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform player;
    Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    [Range(0.01f, 0.99f)]
    public float smoothing;

    private bool scriptBegun;

    //Function called when the player's singleton is created
    public void BeginCameraScript()
    {
        scriptBegun = true;
        if (Player.Instance != null)
        {
            player = Player.Instance.transform;
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
            offset = transform.position - player.position;
        }
        else
        {
            Debug.LogError("THE PLAYER INSTANCE HAS NOT BEEN CREATED");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (scriptBegun)
        {
            Vector3 newCamPos = (player.position + offset);
            transform.position = Vector3.SmoothDamp(transform.position, newCamPos, ref velocity, smoothing);
        }
    }
}
