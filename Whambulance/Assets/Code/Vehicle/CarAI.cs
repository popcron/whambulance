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
        Intersection intersectionAhead = Intersection.Get(vehicle.FrontPosition + vehicle.Forward);
        if (intersectionAhead)
        {
            float sqrDistance = Vector2.SqrMagnitude(intersectionAhead.transform.position - transform.position);

            //only if not inside an intersection
            if (!intersection)
            {
                //intersection ahead, slow down a bit
                vehicle.Gas = Mathf.MoveTowards(vehicle.Gas, 0.3f, Time.fixedDeltaTime);
            }

            //too far, pretend its null
            if (sqrDistance > 2f * 2f)
            {
                intersectionAhead = null;
            }
        }

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
            float angle = Vector2.Dot(dirToPoint.normalized, vehicle.Right);
            angle *= 90f;

            Debug.DrawRay(vehicle.FrontPosition, vehicle.Forward, Color.red);
            Debug.DrawRay(vehicle.FrontPosition, velocity, Color.cyan);
            Debug.DrawRay(vehicle.FrontPosition, dirToPoint, Color.black);

            Debug.DrawRay(vehicle.FrontPosition, vehicle.Right * angle / vehicle.MaxSteerAngle, Color.yellow);

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
