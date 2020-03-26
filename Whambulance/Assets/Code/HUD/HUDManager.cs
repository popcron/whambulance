using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private HUDElement[] elements = { };
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        elements = GetComponentsInChildren<HUDElement>(true);
    }

    private void Update()
    {
        foreach (HUDElement element in elements)
        {
            element.Canvas.worldCamera = cam;
            element.gameObject.SetActive(element.ShouldDisplay);
        }
    }
}
