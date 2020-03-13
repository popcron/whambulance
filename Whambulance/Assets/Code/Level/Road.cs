using System;
using UnityEngine;

[Serializable]
public class Road
{
    public Intersection start;
    public Intersection end;

    public Vector2 Start => start.transform.position;
    public Vector2 End => end.transform.position;

    public Vector2 StartA
    {
        get
        {
            int index = -1;
            float closestCorner = float.MaxValue;
            Vector2 origin = End;
            Line[] sides = start.Sides;
            for (int i = 0; i < sides.Length; i++)
            {
                float sqrDistance = Vector2.SqrMagnitude(sides[i].Position - origin);
                if (closestCorner > sqrDistance)
                {
                    closestCorner = sqrDistance;
                    index = i;
                }
            }

            if (index != -1)
            {
                return sides[index].a;
            }
            else
            {
                return Start;
            }
        }
    }

    public Vector2 StartB
    {
        get
        {
            int index = -1;
            float closestCorner = float.MaxValue;
            Vector2 origin = End;
            Line[] sides = start.Sides;
            for (int i = 0; i < sides.Length; i++)
            {
                float sqrDistance = Vector2.SqrMagnitude(sides[i].Position - origin);
                if (closestCorner > sqrDistance)
                {
                    closestCorner = sqrDistance;
                    index = i;
                }
            }

            if (index != -1)
            {
                return sides[index].b;
            }
            else
            {
                return Start;
            }
        }
    }

    public Vector2 EndA
    {
        get
        {
            int index = -1;
            float closestCorner = float.MaxValue;
            Vector2 origin = Start;
            Line[] sides = end.Sides;
            for (int i = 0; i < sides.Length; i++)
            {
                float sqrDistance = Vector2.SqrMagnitude(sides[i].Position - origin);
                if (closestCorner > sqrDistance)
                {
                    closestCorner = sqrDistance;
                    index = i;
                }
            }

            if (index != -1)
            {
                return sides[index].b;
            }
            else
            {
                return End;
            }
        }
    }

    public Vector2 EndB
    {
        get
        {
            int index = -1;
            float closestCorner = float.MaxValue;
            Vector2 origin = Start;
            Line[] sides = end.Sides;
            for (int i = 0; i < sides.Length; i++)
            {
                float sqrDistance = Vector2.SqrMagnitude(sides[i].Position - origin);
                if (closestCorner > sqrDistance)
                {
                    closestCorner = sqrDistance;
                    index = i;
                }
            }

            if (index != -1)
            {
                return sides[index].a;
            }
            else
            {
                return End;
            }
        }
    }

    /// <summary>
    /// Normalized direction of this road.
    /// </summary>
    public Vector2 Direction
    {
        get
        {
            if (!end || !start)
            {
                return default;
            }

            return (end.transform.position - start.transform.position).normalized;
        }
    }

    /// <summary>
    /// Returns true when this road intersects the other road.
    /// </summary>
    public bool Intersects(Road road)
    {
        Vector2 roadStartA = StartA + Direction * 0.1f;
        Vector2 roadStartB = StartB + Direction * 0.1f;
        Vector2 roadEndA = EndA - Direction * 0.1f;
        Vector2 roadEndB = EndB - Direction * 0.1f;

        return Intersects(roadStartA, roadEndA, road.StartA, road.EndA).HasValue || Intersects(roadStartB, roadEndB, road.StartB, road.EndB).HasValue
            || Intersects(roadStartA, roadEndA, road.StartB, road.EndB).HasValue || Intersects(roadStartB, roadEndB, road.StartA, road.EndA).HasValue;
    }

    //borrowed from http://csharphelper.com/blog/2014/08/determine-where-two-lines-intersect-in-c/
    private Vector2? Intersects(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float dx12 = p2.x - p1.x;
        float dy12 = p2.y - p1.y;
        float dx34 = p4.x - p3.x;
        float dy34 = p4.y - p3.y;
        float denominator = (dy12 * dx34 - dx12 * dy34);
        float t1 = ((p1.x - p3.x) * dy34 + (p3.y - p1.y) * dx34) / denominator;

        //lines are perfectly parallel
        if (float.IsInfinity(t1))
        {
            return null;
        }

        float t2 = ((p3.x - p1.x) * dy12 + (p1.y - p3.y) * dx12) / -denominator;
        if ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1))
        {
            Vector2 point = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);
            return point;
        }

        return null;
    }
}
