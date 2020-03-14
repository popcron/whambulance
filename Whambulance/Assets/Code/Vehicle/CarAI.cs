using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    private Vehicle vehicle;
    private Level level;
    private Road desiredRoad;
    private Intersection intersection;

    private void Awake()
    {
        level = FindObjectOfType<Level>();
        vehicle = GetComponent<Vehicle>();
    }

    private void Start()
    {
        desiredRoad = level.GetRoad(transform.position);
    }

    private void FixedUpdate()
    {
        //drive forward until an intersection is hit
        Intersection intersectionAhead = Intersection.Get(vehicle.FrontPosition);
        if (intersection != intersectionAhead)
        {
            intersection = intersectionAhead;
            if (intersection)
            {
                //pick a random road to go to now
                List<Road> connectedRoads = level.GetConnectedRoads(intersectionAhead);
                if (connectedRoads.Count > 1)
                {
                    //pick any but the one were in
                    Road newRoad = null;
                    while (newRoad == null || desiredRoad == newRoad)
                    {
                        newRoad = connectedRoads[Random.Range(0, connectedRoads.Count)];
                    }

                    desiredRoad = newRoad;
                }
                else
                {
                    desiredRoad = null;
                }
            }
        }

        if (desiredRoad != null)
        {
            //try to arrive to the closest position on the road first
            Vector2 velocity = vehicle.Rigidbody.velocity.normalized;
            Vector2 closestPoint = desiredRoad.ClosestPoint(vehicle.FrontPosition + velocity, velocity);
            Vector2 dirToPoint = closestPoint - vehicle.FrontPosition;
            float angle = -Vector2.SignedAngle(dirToPoint.normalized, velocity);
            angle *= 0.75f;

            Debug.DrawRay(vehicle.FrontPosition, vehicle.ForwardDirection, Color.red);
            Debug.DrawRay(vehicle.FrontPosition, velocity, Color.cyan);
            Debug.DrawLine(vehicle.FrontPosition, closestPoint, Color.black);

            Vector2 right = new Vector2(vehicle.ForwardDirection.y, vehicle.ForwardDirection.x);
            Debug.DrawRay(vehicle.FrontPosition, right * angle / vehicle.MaxSteerAngle, Color.yellow);

            vehicle.Steer = angle;
            vehicle.Gas = Mathf.Lerp(vehicle.Gas, 1f, Time.fixedDeltaTime);
        }
        else
        {
            vehicle.Steer = 0f;
            vehicle.Gas = 0f;
        }
    }
}
