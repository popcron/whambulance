using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.onWon += OnWon;
    }

    private void OnDisable()
    {
        GameManager.onWon -= OnWon;
    }

    private void OnWon()
    {
        //game was won, so display the bill now
    }

    /// <summary>
    /// Reset the score to 0, as if the game has started from scratch.
    /// </summary>
    public static void Clear()
    {

    }

    /// <summary>
    /// Award the player with this many points, with the name of the type of offence.
    /// <code>
    /// Example: Road Cone Disruption, 100
    /// </code>
    /// </summary>
    public static void AwardPoints(string offenceName, int value)
    {

    }
}