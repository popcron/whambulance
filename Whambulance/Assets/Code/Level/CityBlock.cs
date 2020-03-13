using UnityEngine;

public class CityBlock : Prop
{
    private Bounds bounds;

    /// <summary>
    /// The cached bounds from when you do GetBounds() on this object.
    /// </summary>
    public Bounds Bounds
    {
        get => bounds;
        private set => bounds = value;
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
}
