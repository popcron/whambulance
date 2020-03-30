using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private static bool hotReloaded = true;
    private static HUDManager instance;

    private HUDElement[] elements = { };
    private Camera cam;

    private void OnEnable()
    {
        hotReloaded = false;
        instance = this;
    }

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

    /// <summary>
    /// Returns a HUD element of this type.
    /// </summary>
    public static T GetElement<T>() where T : HUDElement
    {
        if (hotReloaded)
        {
            instance = FindObjectOfType<HUDManager>();
        }

        for (int i = 0; i < instance.elements.Length; i++)
        {
            if (instance.elements[i] is T t)
            {
                return t;
            }
        }

        return null;
    }
}
