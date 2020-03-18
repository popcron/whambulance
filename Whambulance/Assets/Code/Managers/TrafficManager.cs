using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    private float nextSpawn;

    private void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + 0.6f;
            SpawnCarOffscreen();
        }

        //remove cars that are too old and off screen
        foreach (Vehicle vehicle in Vehicle.All)
        {
            if (vehicle.LifeTime >= GameManager.Settings.vehicleLifeTime && vehicle.TimeOffscreen > 6f)
            {
                Destroy(vehicle.gameObject);
                break;
            }
        }
    }

    private bool IsOnScreen(Vector2 position, float padding = 16f)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(position);
        if (screenPoint.x > -padding && screenPoint.y > -padding && screenPoint.x < Screen.width + padding && screenPoint.y < Screen.height - padding)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpawnCarOffscreen()
    {
        if (Vehicle.All.Count > GameManager.Settings.maxVehicles)
        {
            //too many cars on screen!
            return;
        }

        //find a road that is offscreen
        Level level = FindObjectOfType<Level>();
        if (level)
        {
            List<Road> roadsOffscreen = new List<Road>();
            foreach (Road road in level.Roads)
            {
                if (!IsOnScreen(road.Start) && !IsOnScreen(road.End) && !IsOnScreen(road.Position))
                {
                    if (!roadsOffscreen.Contains(road))
                    {
                        roadsOffscreen.Add(road);
                    }
                }
            }

            if (roadsOffscreen.Count > 0)
            {
                Road randomRoad = roadsOffscreen[Random.Range(0, roadsOffscreen.Count)];
                Vehicle randomVehicle = GameManager.Settings.vehicles[Random.Range(0, GameManager.Settings.vehicles.Count)];

                //get random point on road
                Vector2 positionOnRoad = randomRoad.GetRandomPosition(true);
                while (true)
                {
                    //ensure that nothing is in the way
                    bool valid = true;
                    foreach (Vehicle vehicle in Vehicle.All)
                    {
                        float distance = Vector2.SqrMagnitude((Vector2)vehicle.transform.position - positionOnRoad);
                        if (distance < 5f * 5f)
                        {
                            //theres another car that is too close
                            valid = true;
                            break;
                        }
                    }

                    if (valid)
                    {
                        break;
                    }
                    else
                    {
                        //postion wasnt changed, so get new position
                        positionOnRoad = randomRoad.GetRandomPosition(true);
                    }
                }

                Vector2 roadDir = randomRoad.Direction;
                float lookAngle = Mathf.Atan2(roadDir.y, roadDir.x) * Mathf.Rad2Deg;
                Quaternion lookDirection = Quaternion.Euler(0f, 0f, lookAngle);
                Vehicle newVehicle = Instantiate(randomVehicle, positionOnRoad, lookDirection);
            }
        }
    }

    /// <summary>
    /// Removes all vehicles from the level.
    /// </summary>
    public static void Clear()
    {
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();
        foreach (Vehicle vehicle in vehicles)
        {
            Destroy(vehicle.gameObject);
        }
    }
}