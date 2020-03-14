using UnityEngine;

public partial class Helper
{
    //borrowed from http://csharphelper.com/blog/2014/08/determine-where-two-lines-intersect-in-c/
    /// <summary>
    /// Returns the point where these two lines meet at.
    /// </summary>
    public static Vector2? Intersects(Vector2 firstLineA, Vector2 firstLineB, Vector2 secondLineA, Vector2 secondLineB)
    {
        float dx12 = firstLineB.x - firstLineA.x;
        float dy12 = firstLineB.y - firstLineA.y;
        float dx34 = secondLineB.x - secondLineA.x;
        float dy34 = secondLineB.y - secondLineA.y;
        float denominator = (dy12 * dx34 - dx12 * dy34);
        float t1 = ((firstLineA.x - secondLineA.x) * dy34 + (secondLineA.y - firstLineA.y) * dx34) / denominator;

        //lines are perfectly parallel
        if (float.IsInfinity(t1))
        {
            return null;
        }

        float t2 = ((secondLineA.x - firstLineA.x) * dy12 + (firstLineA.y - secondLineA.y) * dx12) / -denominator;
        if ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1))
        {
            Vector2 point = new Vector2(firstLineA.x + dx12 * t1, firstLineA.y + dy12 * t1);
            return point;
        }

        return null;
    }

    //taken from https://forum.unity.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/
    /// <summary>
    /// Returns the closest point on this line.
    /// </summary>
    public static Vector2 ClosestPoint(Vector2 position, Vector2 lineA, Vector2 lineB)
    {
        Vector2 line = lineB - lineA;
        float len = line.magnitude;
        line.Normalize();

        Vector2 v = position - lineA;
        float d = Vector3.Dot(v, line);
        d = Mathf.Clamp(d, 0f, len);
        return lineA + line * d;
    }

    public static bool IsPointInRectangle(Vector2 position, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        return IsPointInTriangle(position, v1, v2, v4) || IsPointInTriangle(position, v1, v4, v3);
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    public static bool IsPointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        d1 = Sign(pt, v1, v2);
        d2 = Sign(pt, v2, v3);
        d3 = Sign(pt, v3, v1);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);
        return !(has_neg && has_pos);
    }
}
