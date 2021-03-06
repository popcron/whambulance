﻿using UnityEngine;

public class AwardIfPlayerCollided : Prop
{
    [SerializeField]
    private string offenceName = "Nuisance";

    [SerializeField]
    private int value = 100;

    private bool disturbed = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //already disturbed, dont bother checking
        if (disturbed)
        {
            return;
        }

        Player player = collision.collider.GetComponentInParent<Player>();
        if (player && player.GetType() == typeof(Player))
        {
            //collided with the player, therefor, got disturbed, therefor, give points
            disturbed = true;
            ScoreManager.AwardPoints(offenceName, value);
        }
    }
}
