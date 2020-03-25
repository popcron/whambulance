using UnityEngine;
using UnityEngine.UI;

public class GPS : HUDElement
{
    /// <summary>
    /// Only displays if the game is actually being played.
    /// </summary>
    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying;
        }
    }

    [SerializeField]
    private RectTransform view;

    [SerializeField]
    private RectTransform playerMarker;

    [SerializeField]
    private Color roadColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [SerializeField]
    private Color obstacleColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    [SerializeField]
    private float roadWidth = 4f;

    [SerializeField]
    private int zoomLevel = 2;

    [SerializeField]
    private int minZoomLevel = 1;

    [SerializeField]
    private int maxZoomLevel = 3;

    private Level level;
    private Camera cam;

    private void OnEnable()
    {
        cam = Camera.main;
        level = null;
        ClearGPS();
    }

    private void ClearGPS()
    {
        foreach (Transform child in view)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Builds the GPS by creating a huge texture to work with.
    /// </summary>
    private void BuildGPSMap(Level level)
    {
        //build the roads
        for (int i = 0; i < level.Roads.Count; i++)
        {
            Road road = level.Roads[i];

            Image roadObject = new GameObject(road.ToString(), typeof(RectTransform)).AddComponent<Image>();
            roadObject.rectTransform.SetParent(view);
            roadObject.rectTransform.anchoredPosition3D = road.Position;
            roadObject.color = roadColor;

            Vector2 direction = road.Direction;
            float distance = road.Length + roadWidth;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            roadObject.rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
            roadObject.rectTransform.sizeDelta = new Vector2(distance, roadWidth);
            roadObject.rectTransform.localScale = Vector3.one;
        }

        //now build the physical obstacles
        for (int i = 0; i < level.CityBlocks.Length; i++)
        {
            CityBlock block = level.CityBlocks[i];
            SpriteRenderer[] spriteRenderers = block.GetComponentsInChildren<SpriteRenderer>();
            for (int c = 0; c < spriteRenderers.Length; c++)
            {
                SpriteRenderer renderer = spriteRenderers[c];

                Image colliderObject = new GameObject(renderer.name, typeof(RectTransform)).AddComponent<Image>();
                colliderObject.rectTransform.SetParent(view);
                colliderObject.rectTransform.anchoredPosition3D = renderer.transform.position;
                colliderObject.rectTransform.localEulerAngles = new Vector3(0f, 0f, renderer.transform.eulerAngles.z);
                colliderObject.rectTransform.sizeDelta = renderer.transform.lossyScale;
                colliderObject.rectTransform.localScale = Vector3.one;
                colliderObject.sprite = renderer.sprite;
                colliderObject.color = renderer.color * obstacleColor;
            }
        }
    }

    private int GetMouseScroll()
    {
        //this is to avoid unitys sign function, which never returns 0
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll > 0)
        {
            return 1;
        }
        else if (scroll < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    private void Update()
    {
        Level current = null;
        if (Level.All.Count > 0)
        {
            current = Level.All[0];
        }

        bool rebuild = false;
#if UNITY_EDITOR
        rebuild |= Input.GetKeyDown(KeyCode.M);
#endif

        //adjust zoom level
        zoomLevel = Mathf.Clamp(zoomLevel + GetMouseScroll(), minZoomLevel, maxZoomLevel);

        //level has changed, so rebuild
        if (current != level)
        {
            level = current;
            rebuild = true;
        }

        //update the gps information
        if (rebuild)
        {
            ClearGPS();
            BuildGPSMap(level);
        }

        //make sure the view follows the camera position, not the player
        view.localScale = Vector3.one * zoomLevel;
        view.anchoredPosition3D = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f) * -zoomLevel;

        //position the player based on the discrepancy between cam and player
        if (Player.Instance)
        {
            Vector2 disc = (cam.transform.position - Player.Instance.transform.position) * -zoomLevel;
            playerMarker.anchoredPosition = disc;
            playerMarker.gameObject.SetActive(true);
            playerMarker.localScale = Vector3.one * zoomLevel * 0.15f;

        }
        else
        {
            playerMarker.gameObject.SetActive(false);
        }
    }
}