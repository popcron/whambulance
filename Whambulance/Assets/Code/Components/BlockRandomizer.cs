using UnityEngine;

public class BlockRandomizer : MonoBehaviour
{
    [SerializeField]
    private Color[] colors = { };

    [SerializeField]
    private SpriteRenderer[] spriteRenderers = { };

    [SerializeField]
    private bool yeet;

    private void Awake()
    {
        //assign each sprite renderer a random color from the array
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            //check if it exists just in case
            if (spriteRenderer)
            {
		//get random index, then random color, then assign it lastly
		int randomIndex = Random.Range(0, colors.Length);
		Color color = colors[randomIndex];
                spriteRenderer.color = color;
            }
        }

        yeet = true;

    }
}