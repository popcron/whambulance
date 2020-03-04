using System;
using System.Collections.Generic;

[Serializable]
public class ScoreBill
{
    public List<Entry> entries = new List<Entry>();

    [Serializable]
    public class Entry
    {
        public string name;
        public int value;
    }
}
