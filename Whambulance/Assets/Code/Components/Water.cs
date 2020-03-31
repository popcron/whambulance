using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public static List<Water> All { get; set; } = new List<Water>();

    public Collider2D[] Colliders { get; private set; }

    private void Awake()
    {
        Colliders = GetComponentsInChildren<Collider2D>();
    }

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    /// <summary>
    /// Returns true if this water area contains this position.
    /// </summary>
    public bool Contains(Vector2 position)
    {
        for (int i = 0; i < Colliders.Length; i++)
        {
            if (Colliders[i].bounds.Contains(position))
            {
                return true;
            }
        }

        return false;
    }
}
