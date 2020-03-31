using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PedestrianRandomizer : MonoBehaviour
{
    [Serializable]
    public class RandomizedPart
    {
        public SpriteRenderer[] spriteRenderer = { };
        public Color[] colors = { Color.white };
    }

    [SerializeField]
    private RandomizedPart[] parts = { };

    private void Awake()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            Color[] colors = parts[i].colors;
            Color color = colors[Random.Range(0, colors.Length)];
            for (int s = 0; s < parts[i].spriteRenderer.Length; s++)
            {
                SpriteRenderer renderer = parts[i].spriteRenderer[s];
                renderer.color = color;
            }
        }
    }
}
