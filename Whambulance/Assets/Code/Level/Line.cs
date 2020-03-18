using System;
using UnityEngine;

[Serializable]
public class Line
{
    public Vector2 a;
    public Vector2 b;

    public Vector2 Position => Vector2.Lerp(a, b, 0.5f);

    public Line(Vector2 a, Vector2 b)
    {
        this.a = a;
        this.b = b;
    }
}
