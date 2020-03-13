using UnityEngine;

public class Intersection : MonoBehaviour
{
    [SerializeField]
    private float sideWalkSize = 0.5f;

    public Vector2 UpLeft => transform.TransformPoint(new Vector3(-transform.localScale.x, transform.localScale.y, 0f));
    public Vector2 UpRight => transform.TransformPoint(new Vector3(transform.localScale.x, transform.localScale.y, 0f));
    public Vector2 DownLeft => transform.TransformPoint(new Vector3(-transform.localScale.x, -transform.localScale.y, 0f));
    public Vector2 DownRight => transform.TransformPoint(new Vector3(transform.localScale.x, -transform.localScale.y, 0f));

    public float GetRoadWidth(Road road)
    {
        Vector2 dir = road.start - road.end;
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
        Gizmos.DrawLine(UpLeft + new Vector2(sideWalkSize, -sideWalkSize), UpRight + new Vector2(-sideWalkSize, -sideWalkSize));
        Gizmos.DrawLine(UpRight + new Vector2(-sideWalkSize, -sideWalkSize), DownRight + new Vector2(-sideWalkSize, sideWalkSize));
        Gizmos.DrawLine(DownRight + new Vector2(-sideWalkSize, sideWalkSize), DownLeft + new Vector2(sideWalkSize, sideWalkSize));
        Gizmos.DrawLine(DownLeft + new Vector2(sideWalkSize, sideWalkSize), UpLeft + new Vector2(sideWalkSize, -sideWalkSize));
    }
}
