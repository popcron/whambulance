using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private HUDElement[] elements = { };

    private void Awake()
    {
        elements = GetComponentsInChildren<HUDElement>(true);
    }

    private void Update()
    {
        foreach (HUDElement element in elements)
        {
            element.gameObject.SetActive(element.ShouldDisplay);
        }
    }
}
