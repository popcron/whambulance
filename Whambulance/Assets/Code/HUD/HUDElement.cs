using UnityEngine;

public class HUDElement : MonoBehaviour
{
    private Canvas elementCanvas;

    public virtual bool ShouldDisplay => false;

    public Canvas Canvas
    {
        get
        {
            if (!elementCanvas)
            {
                elementCanvas = GetComponent<Canvas>();
            }

            return elementCanvas;
        }
    }
}