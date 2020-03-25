using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    /// <summary>
    /// Returns the intersection that is the closest to this origin.
    /// </summary>
    private static Intersection GetClosest(List<Intersection> nodes, Intersection origin)
    {
        float closest = float.MaxValue;
        int index = -1;
        for (int i = 0; i < nodes.Count; i++)
        {
            float distance = Vector2.SqrMagnitude(origin.transform.position - nodes[i].transform.position);
            if (closest > distance)
            {
                index = i;
                closest = distance;
            }
        }

        return nodes[index];
    }

    /// <summary>
    /// Returns a path using A*
    /// </summary>
    public static List<Vector2> GetPath(Level level, Vector2 startPosition, Vector2 destinationPosition)
    {
        Intersection start = Intersection.GetClosest(startPosition);
        Intersection destination = Intersection.GetClosest(destinationPosition);
        if (start == destination)
        {
            return new List<Vector2>() { startPosition, destinationPosition };
        }

        List<Intersection> path = new List<Intersection>();
        path.Add(start);
        Intersection current = start;
        while (true)
        {
            //get all neighbors of current-node (nodes within transmission range)
            List<Intersection> allNeighbors = level.GetConnectedIntersections(current);

            //remove neighbors that are already added to path
            IEnumerable<Intersection> neighbors = from neighbor in allNeighbors
                                                  where !path.Contains(neighbor)
                                                  select neighbor;

            //stop if no neighbors or destination reached
            if (neighbors.Count() == 0)
            {
                break;
            }

            if (neighbors.Contains(destination))
            {
                path.Add(destination);
                break;
            }

            //choose next-node (the neighbor with shortest distance to destination)
            Intersection nearestNode = GetClosest(neighbors.ToList(), current);
            path.Add(nearestNode);
            current = nearestNode;
        }

        List<Vector2> list = new List<Vector2>();
        list.Add(startPosition);

        //start is too close to closest point on first road
        if (path.Count >= 2)
        {
            Road road = level.GetRoad(path[0], path[1]);
            Vector2 closest = road.ClosestPoint(startPosition);
            float sqrDistance = Vector2.SqrMagnitude(closest - startPosition);
            if (road != null && sqrDistance < 1f * 1f)
            {
                path.RemoveAt(0);
            }
            else
            {
                if (Vector2.SqrMagnitude(closest - (Vector2)path[0].transform.position) > 2f * 2f)
                {
                    list.Add(closest);
                }
            }
        }

        for (int i = 0; i < path.Count; i++)
        {
            list.Add(path[i].transform.position);
        }

        list.Add(destinationPosition);
        return list;
    }
}
