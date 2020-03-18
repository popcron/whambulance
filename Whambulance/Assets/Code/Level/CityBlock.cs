using System;
using System.Collections.Generic;
using UnityEngine;

public class CityBlock : Prop
{
    private Bounds bounds;
    private Waypoint[] waypoints = { };

    /// <summary>
    /// The bounds of this city block.
    /// </summary>
    public Bounds Bounds
    {
        get
        {
            if (bounds == default)
            {
                bounds = GetBounds();
            }

            return bounds;
        }
        private set => bounds = value;
    }

    /// <summary>
    /// The pedestrian waypoints.
    /// </summary>
    public Waypoint[] Waypoints => waypoints;

    private void Awake()
    {
        waypoints = GetComponentsInChildren<Waypoint>();
    }

    private void OnEnable()
    {
        //cache thy bounds
        GetBounds();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);

        Gizmos.color = Color.white;
        waypoints = GetComponentsInChildren<Waypoint>();
        if (waypoints.Length > 2)
        {
            //draw lines
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Waypoint a = waypoints[i];
                Waypoint b = waypoints[i + 1];
                Gizmos.DrawLine(a.transform.position, b.transform.position);
            }

            //draw circles around waypoints that are connected
            foreach (Waypoint a in waypoints)
            {
                foreach (Waypoint b in waypoints)
                {
                    if (a != b)
                    {
                        //very close to each other, thats considered connected
                        if (Vector2.SqrMagnitude(a.transform.position - b.transform.position) < Waypoint.ConnectedDistance * Waypoint.ConnectedDistance)
                        {
                            Gizmos.DrawLine(a.transform.position, b.transform.position);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns the waypoint that is closest to this position.
    /// </summary>
    public Waypoint ClosestWaypoint(Vector2 position, float? maxDistance = null)
    {
        int index = -1;
        float closest = float.MaxValue;
        for (int i = 0; i < waypoints.Length; i++)
        {
            float distance = Vector2.SqrMagnitude((Vector2)waypoints[i].transform.position - position);
            if (closest > distance)
            {
                if (maxDistance == null || distance < maxDistance * maxDistance)
                {
                    closest = distance;
                    index = i;
                }
            }
        }

        if (index != -1)
        {
            return waypoints[index];
        }

        return null;
    }

    /// <summary>
    /// Returns all of the waypoints that are connected to this one.
    /// </summary>
    public List<Waypoint> GetConnectedWaypoints(Waypoint waypoint)
    {
        //find all of the origin waypoints that are grouped up
        //then find all of the waypoints that are neighbours of these origins
        List<Waypoint> neighbours = new List<Waypoint>();
        foreach (Waypoint wp in waypoints)
        {
            float distance = Vector2.SqrMagnitude(wp.transform.position - waypoint.transform.position);
            if (distance < Waypoint.ConnectedDistance * Waypoint.ConnectedDistance)
            {
                int index = Array.IndexOf(waypoints, waypoint);
                if (index > 0)
                {
                    neighbours.Add(waypoints[index - 1]);
                }

                if (index < waypoints.Length - 1)
                {
                    neighbours.Add(waypoints[index + 1]);
                }
            }
        }

        return neighbours;
    }

    /// <summary>
    /// Returns the overall bounds of this city block based on the sprite renderers.
    /// </summary>
    public Bounds GetBounds()
    {
        bounds = new Bounds((Vector2)transform.position, Vector3.zero);
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            bounds.Encapsulate(sprite.bounds);
        }

        return bounds;
    }

    /// <summary>
    /// Returns a position that a pedestrian could spawn on.
    /// </summary>
    public Vector2 GetRandomPointOnSidewalk()
    {
        Bounds bounds = GetBounds();
        Vector2 position;
        do
        {
            float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            float randomY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
            position = new Vector2(randomX, randomY);
        }
        while (!IsPointOnSidewalk(position));
        return position;
    }

    /// <summary>
    /// Returns true when this position is not obstructed by anything inside this city block.
    /// </summary>
    public bool IsPointOnSidewalk(Vector2 position)
    {
        foreach (Collider2D collider in Colliders)
        {
            //inside a collider, so no
            if (collider.bounds.Contains(position))
            {
                return false;
            }
        }

        return true;
    }
}
