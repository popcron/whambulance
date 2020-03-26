using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Road
{
    public Intersection start;
    public Intersection end;

    private Vector2? startA;
    private Vector2? startB;
    private Vector2? endA;
    private Vector2? endB;

    public Vector2 Start => !start ? default : start.transform.position;
    public Vector2 End => !end ? default : (Vector2)end.transform.position;
    public Vector2 Position => Vector3.Lerp(Start, End, 0.5f);

    public Vector2 StartA
    {
        get
        {
            if (startA == null)
            {
                int index = -1;
                float closestCorner = float.MaxValue;
                Vector2 origin = End;
                List<Line> sides = start.Sides;
                for (int i = 0; i < sides.Count; i++)
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
                    startA = sides[index].a;
                }
                else
                {
                    startA = Start;
                }
            }

            return startA.Value;
        }
    }

    public Vector2 StartB
    {
        get
        {
            if (startB == null)
            {
                int index = -1;
                float closestCorner = float.MaxValue;
                Vector2 origin = End;
                List<Line> sides = start.Sides;
                for (int i = 0; i < sides.Count; i++)
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
                    startB = sides[index].b;
                }
                else
                {
                    startB = Start;
                }
            }

            return startB.Value;
        }
    }

    public Vector2 EndA
    {
        get
        {
            if (endA == null)
            {
                int index = -1;
                float closestCorner = float.MaxValue;
                Vector2 origin = Start;
                List<Line> sides = end.Sides;
                for (int i = 0; i < sides.Count; i++)
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
                    endA = sides[index].b;
                }
                else
                {
                    endA = End;
                }
            }

            return endA.Value;
        }
    }

    public Vector2 EndB
    {
        get
        {
            if (endB == null)
            {
                int index = -1;
                float closestCorner = float.MaxValue;
                Vector2 origin = Start;
                List<Line> sides = end.Sides;
                for (int i = 0; i < sides.Count; i++)
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
                    endB = sides[index].a;
                }
                else
                {
                    endB = End;
                }
            }

            return endB.Value;
        }
    }

    /// <summary>
    /// The length of the road.
    /// </summary>
    public float Length => Vector2.Distance(start.transform.position, end.transform.position);

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
    public Vector2 ClosestPoint(Vector2 position)
    {
        Vector2 leftPoint = Helper.ClosestPoint(position, Vector2.Lerp(StartA, Start, 0.5f), Vector2.Lerp(EndA, End, 0.5f));
        Vector2 rightPoint = Helper.ClosestPoint(position, Vector2.Lerp(StartB, Start, 0.5f), Vector2.Lerp(EndB, End, 0.5f));
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
    /// Returns the direction for this line on this road.
    /// </summary>
    public static Vector2? GetLaneDirection(Road road, Vector2 a, Vector2 b)
    {
        Vector2 dir = a - b;
        if (Vector2.Angle(dir.normalized, Vector2.left) <= 45f)
        {
            float avg = (a.y + b.y) * 0.5f;
            if (road.start.transform.position.y < avg || road.end.transform.position.y < avg)
            {
                return Vector2.left;
            }
            else
            {
                return Vector2.right;
            }
        }
        else if (Vector2.Angle(dir.normalized, Vector2.right) <= 45f)
        {
            float avg = (a.y + b.y) * 0.5f;
            if (road.start.transform.position.y > avg || road.end.transform.position.y > avg)
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }
        else if (Vector2.Angle(dir.normalized, Vector2.up) <= 45f)
        {
            float avg = (a.x + b.x) * 0.5f;
            if (road.start.transform.position.x < avg || road.end.transform.position.x < avg)
            {
                return Vector2.up;
            }
            else
            {
                return Vector2.down;
            }
        }
        else if (Vector2.Angle(dir.normalized, Vector2.down) <= 45f)
        {
            float avg = (a.x + b.x) * 0.5f;
            if (road.start.transform.position.x > avg || road.end.transform.position.x > avg)
            {
                return Vector2.down;
            }
            else
            {
                return Vector2.up;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns the closest point on the correct lane of the road.
    /// </summary>
    public Vector2 ClosestPoint(Vector2 position, Vector2 direction)
    {
        direction.Normalize();
        Vector2? laneDirection = GetLaneDirection(this, StartA, EndA);
        if (laneDirection.HasValue)
        {
            if (Vector2.Angle(laneDirection.Value, direction) < 90f)
            {
                Vector2 start = Vector2.Lerp(StartA, Start, 0.5f);
                Vector2 end = Vector2.Lerp(EndA, End, 0.5f);
                return Helper.ClosestPoint(position, start, end);
            }
        }

        laneDirection = GetLaneDirection(this, StartB, EndB);
        if (laneDirection.HasValue)
        {
            if (Vector2.Angle(laneDirection.Value, direction) < 90f)
            {
                Vector2 start = Vector2.Lerp(StartB, Start, 0.5f);
                Vector2 end = Vector2.Lerp(EndB, End, 0.5f);
                return Helper.ClosestPoint(position, start, end);
            }
        }

        return Helper.ClosestPoint(position, Start, End);
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
    public Vector2 GetRandomPosition(bool? rightHandSide = null)
    {
        float t = UnityEngine.Random.Range(0f, 1f);
        if (rightHandSide == null)
        {
            rightHandSide = UnityEngine.Random.Range(0f, 1f) > 0.5f;
        }

        Vector2 dir = end.transform.position - start.transform.position;
        Vector2 lookDir = ((StartA - StartB).normalized - dir.normalized).normalized;
        if (Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg > 45f)
        {
            //flip the right hand side parameter
            rightHandSide = !rightHandSide;
        }

        if (rightHandSide == true)
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

    public static bool operator ==(Road left, Road right)
    {
        bool leftNull = ReferenceEquals(left, null);
        if (!leftNull)
        {
            leftNull = !left.start || !left.end;
        }

        bool rightNull = ReferenceEquals(right, null);
        if (!rightNull)
        {
            rightNull = !right.start || !right.end;
        }

        if (leftNull && rightNull)
        {
            return true;
        }
        else if (leftNull != rightNull)
        {
            return false;
        }
        else
        {
            //a == b and b == a
            bool startMatch = left.start == right.start || left.start == right.end;
            bool endMatch = left.end == right.start || left.end == right.end;
            return startMatch && endMatch;
        }
    }

    public static bool operator !=(Road left, Road right)
    {
        bool leftNull = ReferenceEquals(left, null);
        if (!leftNull)
        {
            leftNull = !left.start || !left.end;
        }

        bool rightNull = ReferenceEquals(right, null);
        if (!rightNull)
        {
            rightNull = !right.start || !right.end;
        }

        if (leftNull && rightNull)
        {
            return false;
        }
        else if (leftNull != rightNull)
        {
            return true;
        }
        else
        {
            //a != b or b != a
            bool startMatch = left.start == right.start || left.start == right.end;
            bool endMatch = left.end == right.start || left.end == right.end;
            return !startMatch || !endMatch;
        }
    }
}
