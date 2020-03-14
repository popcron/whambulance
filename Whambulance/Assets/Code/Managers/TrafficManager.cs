﻿using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    private float nextSpawn;

    private void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + 2f;
            SpawnCarOffscreen();
        }

        //remove cars that are offscreen for too long
        foreach (Vehicle vehicle in Vehicle.All)
        {
            if (vehicle.TimeOffscreen > 2f && vehicle.LifeTime > 4f)
            {
                Destroy(vehicle.gameObject);
                break;
            }
        }
    }

    private void SpawnCarOffscreen()
    {
        if (Vehicle.All.Count > 16)
        {
            //too many cars on screen!
            return;
        }

        //find a road that is offscreen
        Level level = FindObjectOfType<Level>();
        if (level)
        {
            Camera camera = Camera.main;
            List<Road> roadsOffscreen = new List<Road>();
            foreach (Road road in level.roads)
            {
                Vector2 vp = camera.WorldToViewportPoint(road.Position);
                if (vp.x < 0f || vp.y < 0f || vp.x > 1f || vp.y > 1f)
                {
                    roadsOffscreen.Add(road);
                }
            }

            if (roadsOffscreen.Count > 0)
            {
                Road randomRoad = roadsOffscreen[Random.Range(0, roadsOffscreen.Count)];
                Vehicle randomVehicle = GameManager.Settings.vehicles[Random.Range(0, GameManager.Settings.vehicles.Count)];

                Vector2 positionOnRoad = randomRoad.GetRandomPosition(true);
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