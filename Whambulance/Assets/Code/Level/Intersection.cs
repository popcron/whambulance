using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    public static List<Intersection> All { get; set; } = new List<Intersection>();

    public Vector2[] Corners => new Vector2[] { UpLeft, UpRight, DownLeft, DownRight };
    public Line[] Sides
    {
        get
        {
            Line[] sides = new Line[4];
            sides[0] = new Line(UpLeft, UpRight);
            sides[1] = new Line(UpRight, DownRight);
            sides[2] = new Line(DownRight, DownLeft);
            sides[3] = new Line(DownLeft, UpLeft);
            return sides;
        }
    }

    public Vector2 UpLeft => TransformPoint(-transform.localScale.x, transform.localScale.y);
    public Vector2 UpRight => TransformPoint(transform.localScale.x, transform.localScale.y);
    public Vector2 DownLeft => TransformPoint(-transform.localScale.x, -transform.localScale.y);
    public Vector2 DownRight => TransformPoint(transform.localScale.x, -transform.localScale.y);

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
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
    /// Returns the closest intersection to this position.
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
