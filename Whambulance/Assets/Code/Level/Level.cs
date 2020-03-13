using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<Road> roads = new List<Road>();

    private Intersection[] intersections = { };
    private CityBlock[] cityBlocks = { };

    private void Awake()
    {
        BuildRoadLayout();
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        BuildRoadLayout();
#endif

        foreach (Road road in roads)
        {
            Vector2 dir = road.end - road.start;

            //rotate this vector by 90 degrees
            Vector2 perpDir = new Vector2(dir.y, dir.x).normalized;
            Gizmos.color = Color.white;
            Gizmos.DrawLine(road.start + perpDir * road.startWidth * 0.5f, road.end + perpDir * road.endWidth * 0.5f);
            Gizmos.DrawLine(road.start - perpDir * road.startWidth * 0.5f, road.end - perpDir * road.endWidth * 0.5f);

            Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
            Gizmos.DrawLine(road.start, road.end);
        }

        Gizmos.color = Color.black;
        foreach (CityBlock block in cityBlocks)
        {
            Bounds bounds = block.Bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }

    /// <summary>
    /// Asks this level to rebuild the layout for the roads.
    /// </summary>
    public void BuildRoadLayout()
    {
        Vector2[] directions =
        {
            Vector2.left,
            Vector2.right,
            Vector2.up,
            Vector2.down
        };

        roads.Clear();
        intersections = GetComponentsInChildren<Intersection>();
        cityBlocks = GetComponentsInChildren<CityBlock>();
        foreach (CityBlock block in cityBlocks)
        {
            block.GetBounds();
        }

        //for every intersection, try to find a connecting intersection by "raycasting"
        foreach (Intersection intersection in intersections)
        {
            foreach (Vector2 roadDirection in directions)
            {
                float closestDistance = float.MaxValue;
                int index = -1;
                for (int i = 0; i < intersections.Length; i++)
                {
                    Intersection other = intersections[i];
                    if (intersection == other)
                    {
                        continue;
                    }

                    //is this intersection kinda close in angle?
                    Vector2 dir = intersection.transform.position - other.transform.position;
                    if (Vector2.Angle(dir.normalized, roadDirection) <= 6f)
                    {
                        if (closestDistance > dir.sqrMagnitude)
                        {
                            closestDistance = dir.sqrMagnitude;
                            index = i;
                        }
                    }
                }

                if (index != -1)
                {
                    Intersection other = intersections[index];
                    Road road = new Road();
                    road.start = intersection.ClosestPoint(other.transform.position);
                    road.startWidth = intersection.GetRoadWidth(road);

                    road.end = other.ClosestPoint(intersection.transform.position);
                    road.endWidth = other.GetRoadWidth(road);
                    roads.Add(road);
                }
            }
        }
    }

    /// <summary>
    /// Returns true when these two city blocks are close.
    /// </summary>
    private bool IsClose(CityBlock block, CityBlock other, Vector2 dir, float gap)
    {
        Vector2 blockClosest = block.Bounds.ClosestPoint(other.transform.position);
        Vector2 otherBlockClosest = other.Bounds.ClosestPoint(block.transform.position);
        if (Vector2.SqrMagnitude(blockClosest - otherBlockClosest) > gap * gap)
        {
            return false;
        }

        //right check
        if (dir.x == 1 && block.Bounds.max.x - other.Bounds.min.x <= gap * gap)
        {
            return true;
        }

        //left check
        if (dir.y == -1 && other.Bounds.max.x - block.Bounds.min.x <= gap * gap)
        {
            return true;
        }

        //up check
        if (dir.y == 1 && block.Bounds.max.y - other.Bounds.min.y <= gap * gap)
        {
            return true;
        }

        //down check
        if (dir.y == -1 && other.Bounds.max.y - block.Bounds.min.y <= gap * gap)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Creates a new road at this direction relative to the city block.
    /// </summary>
    private Road GetRoad(Bounds bounds, Vector2 direction)
    {
        Vector2 rotatedDirection = new Vector2(direction.y, direction.x).normalized;
        Vector2 startFarAway = rotatedDirection * 100000f;
        Vector2 endFarAway = rotatedDirection * -100000f;

        Road road = new Road();
        road.start = bounds.ClosestPoint(startFarAway);
        road.startWidth = 2f;
        road.end = bounds.ClosestPoint(endFarAway);
        road.endWidth = 4f;

        return road;
    }

    /// <summary>
    /// Returns all roads that belong this city block.
    /// </summary>
    public List<Road> GetRoadsOfCityBlock(CityBlock block)
    {
        List<Road> roads = new List<Road>();
        foreach (Road road in roads)
        {
            if (road.cityBlocks.Contains(block))
            {
                roads.Add(road);
            }
        }

        return roads;
    }
}
