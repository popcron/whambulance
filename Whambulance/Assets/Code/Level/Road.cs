using System;
using UnityEngine;

[Serializable]
public class Road
{
    public Intersection start;
    public Intersection end;

    public Vector2 Start => !start ? default : start.transform.position;
    public Vector2 End => !end ? default : (Vector2)end.transform.position;
    public Vector2 Position => Vector3.Lerp(Start, End, 0.5f);

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

        return Helper.Intersects(roadStartA, roadEndA, road.StartA, road.EndA).HasValue || Helper.Intersects(roadStartB, roadEndB, road.StartB, road.EndB).HasValue
            || Helper.Intersects(roadStartA, roadEndA, road.StartB, road.EndB).HasValue || Helper.Intersects(roadStartB, roadEndB, road.StartA, road.EndA).HasValue;
    }

    /// <summary>
    /// Returns the closest point on this road.
    /// </summary>
    public Vector2 ClosestPoint(Vector2 position, bool? rightHandSide = null)
    {
        if (rightHandSide != null)
        {
            Vector2 lookDir = ((StartA - StartB).normalized - Direction).normalized;
            if (Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg > 45f)
            {
                //flip the right hand side parameter
                rightHandSide = !rightHandSide;
            }
        }

        Vector2 leftPoint = Helper.ClosestPoint(position, Vector2.Lerp(StartA, Start, 0.5f), Vector2.Lerp(EndA, End, 0.5f));
        if (rightHandSide == false)
        {
            return leftPoint;
        }

        Vector2 rightPoint = Helper.ClosestPoint(position, Vector2.Lerp(StartB, Start, 0.5f), Vector2.Lerp(EndB, End, 0.5f));
        if (rightHandSide == true)
        {
            return rightPoint;
        }

        float leftDistance = Vector2.SqrMagnitude(leftPoint - position);
        float rightDistance = Vector2.SqrMagnitude(rightPoint - position);
        if (leftDistance < rightDistance)
        {
            return leftPoint;
        }
        else
        {
            return rightPoint;
        }
    }

    /// <summary>
    /// Returns true if this road has this position inside it.
    /// </summary>
    public bool Contains(Vector2 position)
    {
        return Helper.IsPointInRectangle(position, StartA, EndA, EndB, StartB);
    }

    /// <summary>
    /// Returns a random position on this road.
    /// </summary>
    public Vector2 GetRandomPosition(bool rightHandSide)
    {
        float t = UnityEngine.Random.Range(0f, 1f);
        Vector2 dir = end.transform.position - start.transform.position;
        Vector2 lookDir = ((StartA - StartB).normalized - dir.normalized).normalized;
        if (Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg > 45f)
        {
            //flip the right hand side parameter
            rightHandSide = !rightHandSide;
        }

        if (rightHandSide)
        {
            Vector2 right = Vector2.Lerp(StartB, Start, 0.5f);
            return right + dir * t;
        }
        else
        {
            Vector2 left = Vector2.Lerp(StartA, Start, 0.5f);
            return left + dir * t;
        }
    }

    public override string ToString()
    {
        return $"Road {start.name} to {end.name}";
    }
}
