using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the thing that is expecting an Objective to be dropped off at.
/// </summary>
public class Destination : Prop
{
    public static List<Destination> All { get; set; } = new List<Destination>();

    [SerializeField]
    private Vector2 area = new Vector2(2f, 2f);

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, area);
    }

    private void Update()
    {
        if (!GameManager.IsConcluded)
        {
            if (IsPlayerInside(area))
            {
                //only if player is carrying an objective
                Objective obj = Player.Instance.CarryingObjective;
                if (obj)
                {
                    //drop the patient and win the game
                    Player.Instance.Drop();
                    GameManager.Win("Player has delivering the patient.");
                    Destroy(obj.gameObject);
                }
            }
        }
    }
}
