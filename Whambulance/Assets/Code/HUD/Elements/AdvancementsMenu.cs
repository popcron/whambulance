using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvancementsMenu : HUDElement
{
    /// <summary>
    /// Should the advancements menu show?
    /// </summary>
    public static bool Show { get; set; }

    public override bool ShouldDisplay => Show;

    [SerializeField]
    private Color canBuy = Color.green;

    [SerializeField]
    private Color alreadyHas = Color.blue;

    [SerializeField]
    private Color tooExpensive = Color.gray;

    [SerializeField]
    private TMP_Text currencyText;

    [SerializeField]
    private RectTransform contentTransform;

    [SerializeField]
    private RectTransform templatePrefab;

    private string originalCurrencyText;

    private void Awake()
    {
        originalCurrencyText = currencyText.text;
    }

    private void OnEnable()
    {
        FillInAdvancements();
    }

    private void Clear()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void FillInAdvancements()
    {
        Clear();

        for (int i = 0; i < GameManager.Settings.advancements.Count; i++)
        {
            Advancement advancement = GameManager.Settings.advancements[i].advancement;
            RectTransform item = Instantiate(templatePrefab, contentTransform);
            item.anchoredPosition = new Vector2(0f, i * -templatePrefab.sizeDelta.y);

            //get the kids
            TMP_Text nameText = item.Find("Content/Name").GetComponent<TMP_Text>();
            TMP_Text descriptionText = item.Find("Content/Description").GetComponent<TMP_Text>();
            Image icon = item.Find("Content/Icon").GetComponent<Image>();
            Button buyButton = item.Find("Content/Buy").GetComponent<Button>();
            Image buyImage = buyButton.targetGraphic as Image;
            TMP_Text costText = item.Find("Content/Buy/Cost").GetComponent<TMP_Text>();

            //assign information
            nameText.text = advancement.displayName;
            descriptionText.text = advancement.description;
            icon.sprite = advancement.icon;

            //show cost
            costText.text = advancement.cost.ToString("C");

            //color the button based on the cost
            if (HasAdvancement(advancement.uniqueId))
            {
                buyImage.color = alreadyHas;
                icon.color = alreadyHas;
                nameText.color = alreadyHas;
                descriptionText.color = alreadyHas;
            }
            else
            {
                buyImage.color = GameManager.Currency >= advancement.cost ? canBuy : tooExpensive;
            }

            //sub to the button
            buyButton.onClick.RemoveAllListeners();
            if (GameManager.Currency >= advancement.cost && !HasAdvancement(advancement.uniqueId))
            {
                buyButton.interactable = true;
                buyButton.onClick.AddListener(() =>
                {
                    //this is called an anonymous method/delegate
                    UnlockAdvancement(advancement.uniqueId);
                    FillInAdvancements();
                });
            }
            else
            {
                buyButton.interactable = false;
            }
        }

        //resize the scroll view content
        contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, GameManager.Settings.advancements.Count * templatePrefab.sizeDelta.y);
    }

    private void Update()
    {
        string text = originalCurrencyText.Replace("{currency}", GameManager.Currency.ToString("C"));
        currencyText.text = text;

        //pressed escape, so leave
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClickedBack();
        }

#if UNITY_EDITOR
        //r = rich
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Currency = int.MaxValue - 1;
            FillInAdvancements();
        }

        //p = poor
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Currency = int.MinValue;
            FillInAdvancements();
        }

        //l = lock all
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerPrefs.SetString("advancements", "");
            FillInAdvancements();
        }
#endif
    }

    public void ClickedBack()
    {
        Show = false;
    }

    /// <summary>
    /// Unlocks this advancement. Will also subtract currency from the player.
    /// </summary>
    public static void UnlockAdvancement(string name)
    {
        int cost = 20000;
        GameManager.Currency -= cost;

        string data = PlayerPrefs.GetString("advancements", "");
        if (string.IsNullOrEmpty(data))
        {
            data = name;
        }
        else
        {
            List<string> unlocked = data.Split('|').ToList();
            unlocked.Add(name);
            data = string.Join("|", unlocked);
        }

        PlayerPrefs.SetString("advancements", data);
    }

    /// <summary>
    /// Is this advancement unlocked?
    /// </summary>
    public static bool HasAdvancement(string name)
    {
        string data = PlayerPrefs.GetString("advancements", "");
        if (string.IsNullOrEmpty(data))
        {
            return false;
        }
        else
        {
            string[] unlocked = data.Split('|');
            for (int i = 0; i < unlocked.Length; i++)
            {
                if (unlocked[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }
}