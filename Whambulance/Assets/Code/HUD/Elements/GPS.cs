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
        for (int i = 0; i < level.Roads.Count; i++)
        {
            Road road = level.Roads[i];

            Image roadObject = new GameObject(road.ToString(), typeof(RectTransform)).AddComponent<Image>();
            roadObject.rectTransform.SetParent(view);
            roadObject.rectTransform.anchoredPosition3D = road.Position;

            Vector2 direction = road.Direction;
            float distance = road.Length + roadWidth;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            roadObject.rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
            roadObject.rectTransform.sizeDelta = new Vector2(distance, roadWidth);
            roadObject.rectTransform.localScale = Vector3.one;
        }

        //zoom the view
        view.localScale = Vector3.one * zoomLevel;
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
        int scroll = GetMouseScroll();
        if (scroll != 0)
        {
            int lastZoomLevel = zoomLevel;
            zoomLevel = Mathf.Clamp(zoomLevel + scroll, minZoomLevel, maxZoomLevel);
            if (zoomLevel != lastZoomLevel)
            {
                //zoom level has changed, so just rebuild
                rebuild = true;
            }
        }

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
        view.anchoredPosition3D = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f) * -zoomLevel;

        //position the player based on the discrepancy between cam and player
        if (Player.Instance)
        {
            Vector2 disc = (cam.transform.position - Player.Instance.transform.position) * -zoomLevel;
            playerMarker.anchoredPosition = disc;
            playerMarker.gameObject.SetActive(true);

        }
        else
        {
            playerMarker.gameObject.SetActive(false);
        }
    }
}