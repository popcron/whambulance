using System;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    public static List<Intersection> All { get; set; } = new List<Intersection>();

    public float angleLeniency = 22.5f;
    public bool blockLeft = false;
    public bool blockRight = false;
    public bool blockUp = false;
    public bool blockDown = false;

    //pathfinding only thing, useless otherwise
    [HideInInspector, NonSerialized]
    public Intersection parent;

    [HideInInspector, NonSerialized]
    public float gCost;

    [HideInInspector, NonSerialized]
    public float hCost;

    private Vector2? upLeft;
    private Vector2? upRight;
    private Vector2? downLeft;
    private Vector2? downRight;

    public float FCost => gCost + hCost;
    public Vector2[] Corners => new Vector2[] { UpLeft, UpRight, DownLeft, DownRight };

    /// <summary>
    /// Sides of the intersection with no particular order.
    /// </summary>
    public List<Line> Sides
    {
        get
        {
            List<Line> sides = new List<Line>();
            sides.Add(new Line(UpLeft, UpRight));
            sides.Add(new Line(UpRight, DownRight));
            sides.Add(new Line(DownRight, DownLeft));
            sides.Add(new Line(DownLeft, UpLeft));
            return sides;
        }
    }

    public Vector2 UpLeft
    {
        get
        {
            if (upLeft == null)
            {
                upLeft = TransformPoint(-transform.localScale.x, transform.localScale.y);
            }

            return upLeft.Value;
        }
    }

    public Vector2 UpRight
    {
        get
        {
            if (upRight == null)
            {
                upRight = TransformPoint(transform.localScale.x, transform.localScale.y);
            }

            return upRight.Value;
        }
    }

    public Vector2 DownLeft
    {
        get
        {
            if (downLeft == null)
            {
                downLeft = TransformPoint(-transform.localScale.x, -transform.localScale.y);
            }

            return downLeft.Value;
        }
    }

    public Vector2 DownRight
    {
        get
        {
            if (downRight == null)
            {
                downRight = TransformPoint(transform.localScale.x, -transform.localScale.y);
            }

            return downRight.Value;
        }
    }

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    /// <summary>
    /// Is this direction blocked for this intersection?
    /// </summary>
    public bool IsDirectionBlocked(Vector2 direction)
    {
        if (direction == Vector2.right)
        {
            return blockRight;
        }
        else if (direction == Vector2.left)
        {
            return blockLeft;
        }
        else if (direction == Vector2.up)
        {
            return blockUp;
        }
        else if (direction == Vector2.down)
        {
            return blockDown;
        }

        return false;
    }

    private Vector2 TransformPoint(float x, float y)
    {
        return transform.TransformPoint(x, y, 0f);
    }

    /// <summary>
    /// Returns true when this position is inside this intersection.
    /// </summary>
    public bool Contains(Vector2 position)
    {
        return Helper.IsPointInRectangle(position, UpLeft, UpRight, DownLeft, DownRight);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(UpLeft, UpRight);
        Gizmos.DrawLine(UpRight, DownRight);
        Gizmos.DrawLine(DownRight, DownLeft);
        Gizmos.DrawLine(DownLeft, UpLeft);

        //shrink and draw again
        float adjust = GameManager.Settings.sideWalkSize;
        Vector2 upLeft = TransformPoint(-transform.localScale.x + adjust, transform.localScale.y - adjust);
        Vector2 upRight = TransformPoint(transform.localScale.x - adjust, transform.localScale.y - adjust);
        Vector2 downLeft = TransformPoint(-transform.localScale.x + adjust, -transform.localScale.y + adjust);
        Vector2 downRight = TransformPoint(transform.localScale.x - adjust, -transform.localScale.y + adjust);

        Gizmos.DrawLine(upLeft, upRight);
        Gizmos.DrawLine(upRight, downRight);
        Gizmos.DrawLine(downRight, downLeft);
        Gizmos.DrawLine(downLeft, upLeft);
    }

    /// <summary>
    /// Returns an intersection that is closest to this position.
    /// </summary>
    public static Intersection GetClosest(Vector2 position)
    {
        int index = -1;
        float closest = float.MaxValue;
        for (int i = 0; i < All.Count; i++)
        {
            float sqrMagnitude = Vector2.SqrMagnitude(position - (Vector2)All[i].transform.position);
            if (sqrMagnitude < closest)
            {
                closest = sqrMagnitude;
                index = i;
            }
        }

        if (index != -1)
        {
            return All[index];
        }

        return null;
    }

    /// <summary>
    /// Returns an intersection that contains this position.
    /// </summary>
    public static Intersection Get(Vector2 position)
    {
        int index = -1;
        float closest = float.MaxValue;
        for (int i = 0; i < All.Count; i++)
        {
            float sqrMagnitude = Vector2.SqrMagnitude(position - (Vector2)All[i].transform.position);
            if (sqrMagnitude < closest)
            {
                if (All[i].Contains(position))
                {
                    closest = sqrMagnitude;
                    index = i;
                }
            }
        }

        if (index != -1)
        {
            return All[index];
        }

        return null;
    }
}
