using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    private static Collider2D[] results = new Collider2D[4];

    [SerializeField]
    private float obstacleCheckRadius = 1f;

    [SerializeField]
    private float obstacleCheckDistance = 2f;

    private Vehicle vehicle;
    private Level level;
    private Road desiredRoad;
    private Intersection intersection;
    private float standstillTime;
    private bool reverseOut;
    private float reverseOutTime;
    private bool reverseLeft;

    private void Awake()
    {
        level = FindObjectOfType<Level>();
        vehicle = GetComponent<Vehicle>();
    }

    private void Start()
    {
        desiredRoad = level.GetRoad(transform.position);
    }

    private void OnDrawGizmos()
    {
        vehicle = GetComponent<Vehicle>();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(vehicle.FrontPosition + vehicle.Forward * obstacleCheckDistance, obstacleCheckRadius);
    }

    private void FixedUpdate()
    {
        if (reverseOut && reverseOutTime < 1.4f)
        {
            reverseOutTime += Time.fixedDeltaTime;
            vehicle.Gas = -1f;
            return;
        }

        float gas = 1f;

        //drive forward until an intersection is hit
        Intersection intersectionAhead = Intersection.Get(vehicle.FrontPosition + vehicle.Forward * 2f);
        if (intersectionAhead)
        {
            float sqrDistance = Vector2.SqrMagnitude(intersectionAhead.transform.position - transform.position);

            //only if not inside an intersection
            if (!intersection)
            {
                //intersection ahead, slow down a bit
                gas = 0.4f;
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
                if (connectedRoads.Count == 2)
                {
                    //pick the other one
                    if (desiredRoad == connectedRoads[0])
                    {
                        desiredRoad = connectedRoads[1];
                    }
                    else
                    {
                        desiredRoad = connectedRoads[0];
                    }
                }
                else if (connectedRoads.Count > 1)
                {
                    //pick any but the one were in
                    Road newRoad;
                    while (true)
                    {
                        newRoad = connectedRoads[Random.Range(0, connectedRoads.Count)];
                        if (newRoad != desiredRoad)
                        {
                            break;
                        }
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

            //depending on the steepness of the steering, slowdown
            if (angle > 35f)
            {
                gas *= 0.8f;
            }
            else if (angle > 45f)
            {
                gas *= 0.5f;
            }
            else if (angle > 55f)
            {
                gas *= 0.3f;
            }
            else if (angle > 65f && angle <= 100f)
            {
                gas *= 0.1f;
            }

            //check if theres an obstacle ahead
            int hits = Physics2D.OverlapCircleNonAlloc(vehicle.FrontPosition + vehicle.Forward * obstacleCheckDistance, obstacleCheckRadius, results);
            for (int i = 0; i < hits; i++)
            {
                Collider2D collision = results[i];
                if (collision.transform.IsChildOf(transform))
                {
                    //our self, ignore
                    continue;
                }

                //slow down yo
                gas = 0;
                break;
            }

            vehicle.Steer = angle;
            vehicle.Gas = Mathf.Lerp(vehicle.Gas, gas, Time.fixedDeltaTime * 4f);

            if (Mathf.Abs(gas) < 0.05f)
            {
                standstillTime += Time.fixedDeltaTime;
            }

            //been standing still for too long!
            if (standstillTime > 3f)
            {
                reverseOut = true;
                standstillTime = 0f;
                reverseOutTime = 0f;

                reverseLeft = !reverseLeft;
                vehicle.Steer = reverseLeft ? -vehicle.MaxSteerAngle * -0.5f : vehicle.MaxSteerAngle * 0.5f;
            }
        }
        else
        {
            vehicle.Steer = 0f;
            vehicle.Gas = 0f;

            //without a road, find the closest one
            int index = -1;
            float closest = float.MaxValue;
            for (int i = 0; i < level.Roads.Count; i++)
            {
                float distance = Vector2.SqrMagnitude(level.Roads[i].ClosestPoint(transform.position) - (Vector2)transform.position);
                if (closest > distance)
                {
                    closest = distance;
                    index = i;
                }
            }

            if (index != -1)
            {
                desiredRoad = level.Roads[index];
            }
        }
    }
}
