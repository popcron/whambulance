﻿using UnityEngine;

/// <summary>
/// This is the thing that the player is meant to pick up.
/// </summary>
public class Objective : Prop
{
    [SerializeField]
    private float radius = 0.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Update()
    {
        if (!GameManager.IsConcluded)
        {
            if (IsPlayerInside(radius))
            {
                GameManager.Win();
            }
        }
    }
}
