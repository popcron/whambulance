using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Road
{
    public List<CityBlock> cityBlocks = new List<CityBlock>();
    public Vector2 start;
    public float startWidth;

    public Vector2 end;
    public float endWidth;
}
