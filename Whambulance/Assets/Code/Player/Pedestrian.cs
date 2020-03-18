using UnityEngine;

public class Pedestrian : Player
{
    public override Vector2 MovementInput
    {
        get
        {
            return Vector2.zero;
        }
    }

    public override bool Punch
    {
        get
        {
            return false;
        }
    }
}
