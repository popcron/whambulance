using UnityEngine;

public class CityBlock : Prop
{
    private Bounds bounds;

    /// <summary>
    /// The bounds of this city block.
    /// </summary>
    public Bounds Bounds
    {
        get
        {
            if (bounds == default)
            {
                bounds = GetBounds();
            }

            return bounds;
        }
        private set => bounds = value;
    }

    private void OnEnable()
    {
        //cache thy bounds
        GetBounds();
    }

    /// <summary>
    /// Returns the overall bounds of this city block based on the sprite renderers.
    /// </summary>
    public Bounds GetBounds()
    {
        bounds = new Bounds((Vector2)transform.position, Vector3.zero);
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            bounds.Encapsulate(sprite.bounds);
        }

        return bounds;
    }

    /// <summary>
    /// Returns a position that a pedestrian could spawn on.
    /// </summary>
    public Vector2 GetRandomPointOnSidewalk()
    {
        Bounds bounds = GetBounds();
        Vector2 position;
        do
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            position = new Vector2(randomX, randomY);
        }
        while (!IsPointOnSidewalk(position));
        return position;
    }

    /// <summary>
    /// Returns true when this position is not obstructed by anything inside this city block.
    /// </summary>
    public bool IsPointOnSidewalk(Vector2 position)
    {
        foreach (Collider2D collider in Colliders)
        {
            //inside a collider, so no
            if (collider.bounds.Contains(position))
            {
                return false;
            }
        }

        return true;
    }
}
