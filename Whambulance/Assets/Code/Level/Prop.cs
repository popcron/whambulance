using UnityEngine;

public class Prop : MonoBehaviour
{
    /// <summary>
    /// Returns true when the player overlaps with any of the colliders of this object.
    /// </summary>
    public bool IsPlayerInside()
    {
        if (Player.Instance)
        {
            Bounds bounds = default;
            foreach (Collider2D collider in Colliders)
            {
                if (bounds == default)
                {
                    bounds = collider.bounds;
                }
                else
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }

            //expand by player radius
            bounds.Expand(0.5f);

            return bounds.Contains(Player.Instance.transform.position);
        }

        return false;
    }

    /// <summary>
    /// Returns true when the player overlaps with this radius.
    /// </summary>
    public bool IsPlayerInside(float propRadius)
    {
        if (Player.Instance)
        {
            //get vector on XY axis
            Vector2 vector = Player.Instance.transform.position - transform.position;
            float distance = vector.magnitude - Player.Radius;

            //compare radii
            return distance <= propRadius;
        }

        return false;
    }

    /// <summary>
    /// All the colliders that belong to this prop.
    /// </summary>
    public Collider2D[] Colliders { get; private set; }

    private void Awake()
    {
        Colliders = GetComponentsInChildren<Collider2D>();
    }
}
