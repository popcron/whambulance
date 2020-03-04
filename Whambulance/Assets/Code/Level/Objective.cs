using UnityEngine;

/// <summary>
/// This is the thing that the player is meant to pick up.
/// </summary>
public class Objective : Prop
{
    [SerializeField]
    private float radius = 0.5f;

    /// <summary>
    /// The player that this player is being carried by.
    /// </summary>
    public Player CarryingPlayer { get; private set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void BeganCarrying(Player player)
    {
        CarryingPlayer = player;
    }

    public void Dropped(Player player)
    {
        CarryingPlayer = null;
    }

    private void Update()
    {
        //only check if not being carried atm
        if (!CarryingPlayer)
        {
            if (IsPlayerInside(radius))
            {
                //put this object onto the player
                Player.Instance.Carry(this);
            }
        }
    }
}
