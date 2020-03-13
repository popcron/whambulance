using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private static double nextUpdate = 0f;

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
        if (UnityEditor.EditorApplication.timeSinceStartup > nextUpdate)
        {
            BuildRoadLayout();
            nextUpdate = UnityEditor.EditorApplication.timeSinceStartup + 0.25f;
        }
#endif

        foreach (Road road in roads)
        {
            Vector2 dir = road.Direction;

            //rotate this vector by 90 degrees
            Gizmos.color = Color.white;
            Gizmos.DrawLine(road.StartA, road.EndA);
            Gizmos.DrawLine(road.StartB, road.EndB);

            Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
            Gizmos.DrawLine(road.Start, road.End);
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
                    if (Vector2.Angle(dir.normalized, intersection.transform.TransformDirection(roadDirection)) <= GameManager.Settings.maxRoadAngle)
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
                    Road newRoad = new Road();
                    newRoad.start = intersection;
                    newRoad.end = other;

                    bool roadInvalid = false;
                    foreach (Road road in roads)
                    {
                        //dont add if this identical road already exists
                        if (road.start == newRoad.start && road.end == newRoad.end)
                        {
                            roadInvalid = true;
                            break;
                        }
                        else if (road.start == newRoad.end && road.end == newRoad.start)
                        {
                            roadInvalid = true;
                            break;
                        }
                    }

                    if (!roadInvalid)
                    {
                        foreach (Road road in roads)
                        {
                            //check if this road interests another existing road
                            if (newRoad.Intersects(road))
                            {
                                roadInvalid = true;
                                break;
                            }
                        }
                    }

                    if (!roadInvalid)
                    {
                        roads.Add(newRoad);
                    }
                }
            }
        }
    }
}
