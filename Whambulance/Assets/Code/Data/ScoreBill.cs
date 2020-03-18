using System;
using System.Collections.Generic;

[Serializable]
public class ScoreBill
{
    public List<Entry> entries = new List<Entry>();

    /// <summary>
    /// The total value of this bill.
    /// </summary>
    public float TotalValue
    {
        get
        {
            float value = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                value += entries[i].value;
            }

            return value;
        }
    }

    [Serializable]
    public class Entry
    {
        public string name;
        public float value;
        public int count;
    }
}
