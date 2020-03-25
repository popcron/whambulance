using System.Collections.Generic;
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
    private List<Outline> outlines = new List<Outline>();

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
        outlines.Clear();

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

            //get the highest sorting order for later
            int maxSortingOrder = int.MinValue;
            int minSortingOrder = int.MaxValue;
            for (int r = 0; r < spriteRenderers.Length; r++)
            {
                maxSortingOrder = Mathf.Max(maxSortingOrder, spriteRenderers[r].sortingOrder);
                minSortingOrder = Mathf.Min(minSortingOrder, spriteRenderers[r].sortingOrder);
            }

            //create the renderers
            Image[] images = new Image[spriteRenderers.Length];
            for (int r = 0; r < spriteRenderers.Length; r++)
            {
                SpriteRenderer renderer = spriteRenderers[r];

                Image rendererObject = new GameObject(renderer.name, typeof(RectTransform)).AddComponent<Image>();
                rendererObject.rectTransform.SetParent(view);
                rendererObject.rectTransform.anchoredPosition3D = renderer.transform.position;
                rendererObject.rectTransform.localEulerAngles = new Vector3(0f, 0f, renderer.transform.eulerAngles.z);
                rendererObject.rectTransform.sizeDelta = renderer.transform.lossyScale;
                rendererObject.rectTransform.localScale = Vector3.one;
                rendererObject.sprite = renderer.sprite;

                rendererObject.color = renderer.color * obstacleColor;

                if (renderer.GetComponent<Collider2D>())
                {
                    //darken color if solid
                    rendererObject.color *= new Color(1f, 1f, 1f, 1f);

                    //also give outline
                    Outline outline = rendererObject.gameObject.AddComponent<Outline>();
                    outline.effectDistance = new Vector2(0.25f, 0.25f);
                    outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
                    outlines.Add(outline);
                }
                else
                {
                    //if its a city block, ignoreth
                    if (r > 0)
                    {
                        //not solid, so make it almost invisible
                        rendererObject.color *= new Color(1f, 1f, 1f, 0.3f);
                    }
                }

                rendererObject.transform.SetSiblingIndex(renderer.sortingOrder);
                images[r] = rendererObject;
            }

            //resort the renderers
            for (int o = minSortingOrder; o <= maxSortingOrder; o++)
            {
                for (int r = 0; r < spriteRenderers.Length; r++)
                {
                    if (spriteRenderers[r].sortingOrder == o)
                    {
                        images[r].transform.SetAsLastSibling();
                    }
                }
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

    /// <summary>
    /// Returns the target position of where the player should be going to.
    /// </summary>
    private Vector2 GetTarget()
    {
        if (!Player.Instance)
        {
            return default;
        }
        else
        {
            if (Player.Instance.CarryingObjective)
            {
                if (Destination.All.Count > 0)
                {
                    //get average destination position
                    Vector2 avg = default;
                    for (int i = 0; i < Destination.All.Count; i++)
                    {
                        avg += (Vector2)Destination.All[i].transform.position;
                    }

                    avg /= Destination.All.Count;
                    return avg;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                if (Objective.All.Count > 0)
                {
                    //get the first objective position
                    return Objective.All[0].transform.position;
                }
                else
                {
                    return default;
                }
            }
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

        //size the outlines properly
        int total = maxZoomLevel - minZoomLevel;
        float size = (total + minZoomLevel) - (zoomLevel - minZoomLevel);
        for (int i = 0; i < outlines.Count; i++)
        {
            outlines[i].effectDistance = Vector2.one * 0.08f * size;
        }
    }
}