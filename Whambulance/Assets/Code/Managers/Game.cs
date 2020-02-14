using System;

public class Game
{
    public static bool IsPlaying => GameManager.IsPlaying;

    /// <summary>
    /// Starts the game, thats about it.
    /// </summary>
    public static void Play() => GameManager.Play();

    /// <summary>
    /// Quits the game, duh.
    /// </summary>
    public static void Quit() => GameManager.Quit();
}
