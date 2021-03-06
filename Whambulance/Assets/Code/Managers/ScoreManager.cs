﻿using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreBill defaultBill = new ScoreBill();
    private static ScoreManager scoreManager;

    /// <summary>
    /// Returns the current bill with all the scores.
    /// </summary>
    public static ScoreBill Bill
    {
        get
        {
            if (!Manager)
            {
                return defaultBill;
            }

            return Manager.bill;
        }
    }

    private static ScoreManager Manager
    {
        get
        {
            if (!scoreManager)
            {
                scoreManager = FindObjectOfType<ScoreManager>();
            }

            return scoreManager;
        }
    }

    [SerializeField]
    private ScoreBill bill = new ScoreBill();

    private void OnEnable()
    {
        scoreManager = this;
        GameManager.onWon += OnWon;
    }

    private void OnDisable()
    {
        GameManager.onWon -= OnWon;
    }

    private void OnWon()
    {
        //game was won, add to score
        GameManager.Currency += bill.TotalValue;
    }

    /// <summary>
    /// Reset the score to 0, as if the game has started from scratch.
    /// </summary>
    public static void Clear()
    {
        Manager.bill = new ScoreBill();
    }

    /// <summary>
    /// Deduct money from the bill.
    /// </summary>
    public static void Deduct(string offenceName, float value)
    {
        if (GameManager.IsConcluded)
        {
            return;
        }

        //invalid offence
        if (value <= 0f)
        {
            return;
        }

        //find an existing entry on the bill
        ScoreBill bill = Bill;
        for (int i = 0; i < bill.entries.Count; i++)
        {
            //its already here!, so just add to the existing value
            if (bill.entries[i].name.Equals(offenceName, StringComparison.OrdinalIgnoreCase))
            {
                bill.entries[i].value -= value;
                bill.entries[i].count++;
                return;
            }
        }

        //an existing entry wasnt found, so add a new one
        ScoreBill.Entry newEntry = new ScoreBill.Entry
        {
            name = offenceName,
            value = -value,
            count = 1
        };

        bill.entries.Add(newEntry);
    }

    /// <summary>
    /// Award the player with this many points, with the name of the type of offence.
    /// <code>
    /// Example: Road Cone Disruption, 100.23
    /// </code>
    /// </summary>
    public static void AwardPoints(string offenceName, float value)
    {
        if (GameManager.IsConcluded)
        {
            return;
        }

        //invalid offence
        if (value <= 0f)
        {
            return;
        }

        //find an existing entry on the bill
        ScoreBill bill = Bill;
        for (int i = 0; i < bill.entries.Count; i++)
        {
            //its already here!, so just add to the existing value
            if (bill.entries[i].name.Equals(offenceName, StringComparison.OrdinalIgnoreCase))
            {
                bill.entries[i].value += value;
                bill.entries[i].count++;
                return;
            }
        }

        //an existing entry wasnt found, so add a new one
        ScoreBill.Entry newEntry = new ScoreBill.Entry
        {
            name = offenceName,
            value = value,
            count = 1
        };

        bill.entries.Add(newEntry);
    }
}
