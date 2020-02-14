using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private Canvas canvas;
    private HUDElement[] elements = { };

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        elements = canvas.GetComponentsInChildren<HUDElement>();
    }

    private void Update()
    {
        foreach (HUDElement element in elements)
        {
            element.gameObject.SetActive(element.ShouldDisplay);
        }
    }
}
