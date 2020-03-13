using UnityEngine;

public class Intersection : MonoBehaviour
{
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

    private Vector2 TransformPoint(float x, float y)
    {
        return transform.TransformPoint(x, y, 0f);
    }

    public float GetRoadWidth(Road road)
    {
        Vector2 dir = road.start.transform.position - road.end.transform.position;
        dir.Normalize();

        if (Vector2.Angle(dir, Vector2.left) <= 45f)
        {
            return Mathf.Abs(UpLeft.y - DownLeft.y);
        }
        else if (Vector2.Angle(dir, Vector2.right) <= 45f)
        {
            return Mathf.Abs(UpRight.y - DownRight.y);
        }
        else if (Vector2.Angle(dir, Vector2.up) <= 45f)
        {
            return Mathf.Abs(UpLeft.x - UpRight.x);
        }
        else if (Vector2.Angle(dir, Vector2.down) <= 45f)
        {
            return Mathf.Abs(DownLeft.x - DownRight.x);
        }

        return 0f;
    }

    public Vector2 ClosestPoint(Vector2 point)
    {
        Bounds bounds = new Bounds(transform.position, default);
        bounds.Encapsulate(UpLeft);
        bounds.Encapsulate(UpRight);
        bounds.Encapsulate(DownLeft);
        bounds.Encapsulate(DownRight);

        //depending on angle, give a closest point lol
        Vector2 dir = point - (Vector2)transform.position;
        dir.Normalize();

        if (Vector2.Angle(dir, Vector2.left) <= 45f)
        {
            return new Vector2(bounds.min.x, bounds.center.y);
        }
        else if (Vector2.Angle(dir, Vector2.right) <= 45f)
        {
            return new Vector2(bounds.max.x, bounds.center.y);
        }
        else if (Vector2.Angle(dir, Vector2.up) <= 45f)
        {
            return new Vector2(bounds.center.x, bounds.max.y);
        }
        else if (Vector2.Angle(dir, Vector2.down) <= 45f)
        {
            return new Vector2(bounds.center.x, bounds.min.y);
        }

        return bounds.ClosestPoint(point);
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
}
