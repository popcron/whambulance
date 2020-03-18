using System;
using UnityEngine;

[Serializable]
public class Advancement
{
    public string uniqueId = "blerh";
    public string displayName = "Advancement";

    [TextArea]
    public string description = "Description of this advancement";

    public Sprite icon;
    public float cost;
}
