using TMPro;
using UnityEngine;

public class Advancements : HUDElement
{
    /// <summary>
    /// Should the advancements menu show?
    /// </summary>
    public static bool Show
    {
        get
        {
            return PlayerPrefs.GetInt("showAdvancements", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("showAdvancements", value ? 1 : 0);
        }
    }

    public override bool ShouldDisplay => Show;

    [SerializeField]
    private TMP_Text currencyText;

    private string originalCurrencyText;

    private void Awake()
    {
        originalCurrencyText = currencyText.text;
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
    }

    public void ClickedBack()
    {
        Show = false;
    }
}