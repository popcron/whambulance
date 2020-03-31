using UnityEngine;

public class CreditsMenu : HUDElement
{
    /// <summary>
    /// Should the credits menu show?
    /// </summary>
    public static bool Show { get; set; }

    public override bool ShouldDisplay => Show;

    [SerializeField]
    private RectTransform[] people = { };

    private Vector2[] originalPositions = { };

    private void Awake()
    {
        originalPositions = new Vector2[people.Length];
        for (int i = 0; i < originalPositions.Length; i++)
        {
            originalPositions[i] = people[i].anchoredPosition;
        }    
    }

    public void ClickedBack()
    {
        Show = false;
    }

    private void Update()
    {
        for (int i = 0; i < people.Length; i++)
        {
            RectTransform transform = people[i];
            float offset = Mathf.Abs(i.GetHashCode()) * 23f;
            float angle = (Time.time * 43f + offset) % 360f;
            angle *= i % 2 == 0 ? 1 : -1;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            transform.anchoredPosition = originalPositions[i] + dir * 6f;
        }
    }
}