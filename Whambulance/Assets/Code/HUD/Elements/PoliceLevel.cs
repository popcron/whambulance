using UnityEngine;
using UnityEngine.UI;

public class PoliceLevel : HUDElement
{
    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying;
        }
    }

    [SerializeField]
    private int maxLevels = 5;

    [SerializeField]
    private float size = 100f;

    [SerializeField]
    private Gradient flashingColor;

    [SerializeField]
    private float flashingSpeed = 6f;

    [SerializeField]
    private Color idleColor;

    [SerializeField]
    private RectTransform prefab;

    private Image[] sirens = { };

    private void OnEnable()
    {
        Clear();
        Create();
    }

    private void Clear()
    {
        prefab.gameObject.SetActive(false);
        foreach (Transform transform in prefab.parent)
        {
            if (transform != prefab)
            {
                Destroy(transform.gameObject);
            }
        }
    }

    private void Create()
    {
        prefab.gameObject.SetActive(true);
        Vector2 min = Vector2.left * ((maxLevels * size * 0.5f) + size * 0.5f);
        Vector2 max = Vector2.right * ((maxLevels * size * 0.5f) - size * 0.5f);
        sirens = new Image[maxLevels];
        for (int i = 0; i < maxLevels; i++)
        {
            float t = i / (maxLevels - 1f);
            RectTransform newSiren = Instantiate(prefab, prefab.parent);
            newSiren.anchoredPosition = Vector2.Lerp(min, max, t);
            sirens[i] = newSiren.Find("Siren").GetComponent<Image>();
        }

        prefab.gameObject.SetActive(false);
    }

    private void Update()
    {
        float difficulty = EnemyManager.Difficulty;
        int policeLevel = Mathf.FloorToInt(difficulty * maxLevels);
        Vector2 min = Vector2.left * ((maxLevels * size * 0.5f) + size * 0.5f);
        Vector2 max = Vector2.right * ((maxLevels * size * 0.5f) - size * 0.5f);
        for (int i = 0; i < sirens.Length; i++)
        {
            //calculate color
            float t = i / (maxLevels - 1f);
            bool isFlashing = i < policeLevel;
            if (isFlashing)
            {
                float time = Time.time + Mathf.Abs(i.GetHashCode() * 0.8f);
                float color = (time * flashingSpeed) % 1f;
                sirens[i].color = flashingColor.Evaluate(color);
            }
            else
            {
                sirens[i].color = idleColor;
            }

            //position
            RectTransform parent = sirens[i].transform.parent.GetComponent<RectTransform>();
            parent.anchoredPosition = Vector2.Lerp(min, max, t);
        }
    }
}
