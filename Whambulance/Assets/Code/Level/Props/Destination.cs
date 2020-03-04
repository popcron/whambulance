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
                //only if player is carrying an objective
                if (Player.Instance.CarryingObjective)
                {
                    //drop the patient and win the game
                    Player.Instance.Drop();
                    GameManager.Win();
                }
            }
        }
    }
}
