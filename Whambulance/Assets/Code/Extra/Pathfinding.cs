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

    private static List<Intersection> RetracePath(Intersection startNode, Intersection endNode)
    {
        List<Intersection> path = new List<Intersection>();
        Intersection currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Returns a path using A*
    /// </summary>
    public static List<Vector2> GetPath(Level level, Vector2 startPosition, Vector2 tagetPosition)
    {
        Intersection start = Intersection.GetClosest(startPosition);
        Intersection target = Intersection.GetClosest(tagetPosition);
        if (start == target)
        {
            return new List<Vector2>() { startPosition, tagetPosition };
        }

        List<Intersection> path = new List<Intersection>();
        List<Intersection> openSet = new List<Intersection>();
        HashSet<Intersection> closedSet = new HashSet<Intersection>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Intersection currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == target)
            {
                path = RetracePath(start, target);
                break;
            }

            foreach (Intersection neighbour in level.GetConnectedIntersections(currentNode))
            {
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, target);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        //sanitizie it
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
                    float path0Dist = Vector2.SqrMagnitude((Vector2)path[0].transform.position - tagetPosition);
                    float path1Dist = Vector2.SqrMagnitude((Vector2)path[1].transform.position - tagetPosition);
                    if (path1Dist > path0Dist)
                    {
                        //the second point is way closer to destination, so remove the first
                        path.RemoveAt(0);
                    }

                    list.Add(closest);
                }
            }
        }

        for (int i = 0; i < path.Count; i++)
        {
            list.Add(path[i].transform.position);
        }

        list.Add(tagetPosition);
        return list;
    }

    private static float GetDistance(Intersection nodeA, Intersection nodeB)
    {
        float dstX = Mathf.Abs(nodeA.transform.position.x - nodeB.transform.position.x);
        float dstY = Mathf.Abs(nodeA.transform.position.y - nodeB.transform.position.y);
        if (dstX > dstY)
        {
            return (14 * dstY) + (10 * (dstX - dstY));
        }
        else
        {
            return (14 * dstX) + (10 * (dstY - dstX));
        }
    }
}
