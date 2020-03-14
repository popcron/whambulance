using UnityEngine;

public class VehicleFront : MonoBehaviour
{
    public Rigidbody2D Rigidbody { get; private set; }
    public Vehicle Vehicle { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Vehicle = GetComponentInParent<Vehicle>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Vehicle.HitObject(collision);
    }
}