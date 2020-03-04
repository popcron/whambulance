using UnityEngine;

/// <summary>
/// This is the thing that is expecting an Objective to be dropped off at.
/// </summary>
public class Destination : Prop
{
    [SerializeField]
    private Vector2 area = new Vector2(2f, 2f);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, area);
    }

    private void Update()
    {
        if (!GameManager.IsConcluded)
        {
            if (IsPlayerInside(area))
            {
                GameManager.Win();
            }
        }
    }
}
