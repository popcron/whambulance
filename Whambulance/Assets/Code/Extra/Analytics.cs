using System.Collections.Generic;
using UnityEngine;
using A = UnityEngine.Analytics.Analytics;

/// <summary>
/// A shim that sends proper events out.
/// </summary>
public static class Analytics
{
    static Analytics()
    {
        GameManager.onStartedPlaying += () =>
        {
            PedestriansHit = 0;
            VehiclesHit = 0;
        };
    }

    public static int PedestriansHit { get; set; } = 0;
    public static int VehiclesHit { get; set; } = 0;

    private static List<string> controlsUsed = new List<string>();

    /// <summary>
    /// Reports a win.
    /// </summary>
    public static void Won(string reason)
    {
        A.CustomEvent("won", new Dictionary<string, object>
        {
            { "time", GameManager.TotalTime },
            { "reason", reason },
            { "score", ScoreManager.Bill.TotalValue },
            { "pedestriansHit", PedestriansHit },
            { "vehiclesHit", VehiclesHit },
        });
    }

    public static void Healed(Player player)
    {
        A.CustomEvent("healed", new Dictionary<string, object>
        {
            { "time", GameManager.TotalTime },
            { "health", player.Health.HP },
            { "score", ScoreManager.Bill.TotalValue },
        });
    }

    /// <summary>
    /// Reports how the player controls.
    /// </summary>
    public static void UsedControl(string controlTried)
    {
        if (controlsUsed.Contains(controlTried))
        {
            return;
        }

        controlsUsed.Add(controlTried);
        A.CustomEvent("controls", new Dictionary<string, object>
        {
            { "controlTried", controlTried },
            { "time", GameManager.TotalTime }
        });
    }

    /// <summary>
    /// Reports a loss.
    /// </summary>
    public static void Lost(string reason)
    {
        A.CustomEvent("lost", new Dictionary<string, object>
        {
            { "time", GameManager.TotalTime },
            { "isDelivering", GameManager.IsDelivering },
            { "reason", reason },
            { "score", ScoreManager.Bill.TotalValue },
            { "pedestriansHit", PedestriansHit },
            { "vehiclesHit", VehiclesHit },
        });
    }

    /// <summary>
    /// Reports a player death.
    /// </summary>
    public static void PlayerDeath(string reason)
    {
        A.CustomEvent("playerDeath", new Dictionary<string, object>
        {
            { "time", GameManager.TotalTime },
            { "isDelivering", GameManager.IsDelivering },
            { "cause", reason }
        });
    }

    /// <summary>
    /// Reports a pedestrian death.
    /// </summary>
    public static void PedestrianDeath(string reason)
    {
        A.CustomEvent("pedestrianDeath", new Dictionary<string, object>
        {
            { "time", GameManager.TotalTime },
            { "isDelivering", GameManager.IsDelivering },
            { "cause", reason }
        });
    }

    /// <summary>
    /// Reports the time when the player started delivering.
    /// </summary>
    public static void NowDelivering(Player player)
    {
        A.CustomEvent("delivering", new Dictionary<string, object>
        {
            { "playerHealth", player.Health.HP },
            { "timeToRescue", GameManager.RescuingTime }
        });
    }
}
