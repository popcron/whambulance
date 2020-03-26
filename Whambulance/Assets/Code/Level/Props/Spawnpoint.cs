using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : Prop
{
    public static List<Spawnpoint> All { get; set; } = new List<Spawnpoint>();

    [SerializeField]
    private bool forPlayer = true;

    public bool ForPlayer => forPlayer;

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    private void OnDrawGizmos()
    {
        float radius = 0.5f;
        Gizmos.color = forPlayer ? Color.green : Color.blue;

        //this draws a plus
        Gizmos.DrawLine(transform.position + Vector3.up * radius, transform.position + Vector3.down * radius);
        Gizmos.DrawLine(transform.position + Vector3.right * radius, transform.position + Vector3.left * radius);
    }

    /// <summary>
    /// Returns a random spawnpoint.
    /// </summary>
    public static Spawnpoint GetRandomSpawnpoint(bool forPlayer)
    {
        if (All.Count == 0)
        {
            return null;
        }
        else if (All.Count == 1)
        {
            return All[0];
        }

        Spawnpoint randomSpawnpoint;
        while (true)
        {
            randomSpawnpoint = All[Random.Range(0, All.Count)];
            if (randomSpawnpoint.ForPlayer == forPlayer)
            {
                break;
            }
        }

        return randomSpawnpoint;
    }
}