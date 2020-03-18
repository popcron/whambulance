using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private static double nextUpdate = 0f;

    /// <summary>
    /// List of all levels loaded into the scene right now.
    /// </summary>
    public static List<Level> All { get; set; } = new List<Level>();

    [SerializeField]
    private List<Road> roads = new List<Road>();

    [SerializeField]
    private Intersection[] intersections = { };

    [SerializeField]
    private CityBlock[] cityBlocks = { };

    /// <summary>
    /// All of the roads in this level.
    /// </summary>
    public List<Road> Roads => roads;

    /// <summary>
    /// All of the intersections in this level.
    /// </summary>
    public Intersection[] Intersections => intersections;

    /// <summary>
    /// All of the city blocks in this level.
    /// </summary>
    public CityBlock[] CityBlocks => cityBlocks;

    private void Awake()
    {
        BuildRoadLayout();
    }

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
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
            if (!road.start || !road.end)
            {
                continue;
            }

            Vector2 dir = road.Direction;

            //rotate this vector by 90 degrees
            Gizmos.color = Color.white;
            Gizmos.DrawLine(road.StartA, road.EndA);
            Gizmos.DrawLine(road.StartB, road.EndB);

            Vector2? dirA = Road.GetLaneDirection(road, road.StartA, road.EndA);
            if (dirA.HasValue)
            {
                Vector2 start = Vector2.Lerp(road.StartA, road.Start, 0.5f);
                Vector2 end = Vector2.Lerp(road.EndA, road.End, 0.5f);
                Helper.DrawGizmoArrow(Vector2.Lerp(start, end, 0.5f), dirA.Value, 1f);
            }

            Vector2? dirB = Road.GetLaneDirection(road, road.StartB, road.EndB);
            if (dirB.HasValue)
            {
                Vector2 start = Vector2.Lerp(road.StartB, road.Start, 0.5f);
                Vector2 end = Vector2.Lerp(road.EndB, road.End, 0.5f);
                Helper.DrawGizmoArrow(Vector2.Lerp(start, end, 0.5f), dirB.Value, 1f);
            }

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
    /// Returns a road that has this position in it.
    /// </summary>
    public Road GetRoad(Vector2 position)
    {
        for (int i = 0; i < roads.Count; i++)
        {
            if (roads[i].Contains(position))
            {
                return roads[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all of the roads that are connected to this intersection.
    /// </summary>
    public List<Road> GetConnectedRoads(Intersection intersection)
    {
        List<Road> connected = new List<Road>();
        for (int i = 0; i < roads.Count; i++)
        {
            //start or end, doesnt matter
            if (roads[i].start == intersection || roads[i].end == intersection)
            {
                //make the sure the list contains unique roads
                if (!connected.Contains(roads[i]))
                {
                    connected.Add(roads[i]);
                }
            }
        }

        return connected;
    }

    /// <summary>
    /// Asks this level to rebuild the layout for the roads. Very expensive.
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
