using UnityEngine;
using TMPro;

public class Victory : HUDElement
{
    public override bool ShouldDisplay
    {
        get
        {
            return GameManager.IsPlaying;
        }
    }

    [SerializeField]
    private RectTransform root;

    [SerializeField]
    private RectTransform entriesRoot;

    [SerializeField]
    private RectTransform entryTemplate;

    [SerializeField]
    private RectTransform noOffences;

    [SerializeField]
    private TMP_Text totalValueText;

    private bool show;

    private void OnEnable()
    {
        root.gameObject.SetActive(false);

        GameManager.onWon += OnWon;
        GameManager.onStartedPlaying += OnStartedPlaying;
        GameManager.onStoppedPlaying += OnStoppedPlaying;
    }

    private void OnDisable()
    {
        GameManager.onWon -= OnWon;
        GameManager.onStartedPlaying -= OnStartedPlaying;
        GameManager.onStoppedPlaying -= OnStoppedPlaying;
    }

    private void OnWon()
    {
        //game was won, so show this screen
        show = true;

        //build the bill printout shit
        BuildBill();
    }

    private void BuildBill()
    {
        ClearBill();

        float y = 0f;
        ScoreBill bill = ScoreManager.Bill;

        //in case the bill is empty, show the no offence case
        noOffences.gameObject.SetActive(bill.TotalValue == 0);
        string totalString = bill.TotalValue.ToString("C");
        totalValueText.text = totalString;

        for (int i = 0; i < bill.entries.Count; i++)
        {
            //create the ui element here
            ScoreBill.Entry entry = bill.entries[i];
            RectTransform newEntry = Instantiate(entryTemplate, entriesRoot);
            newEntry.name = $"Bill entry {i}";
            TMP_Text entryName = newEntry.Find("Name").GetComponent<TMP_Text>();
            TMP_Text entryValue = newEntry.Find("Value").GetComponent<TMP_Text>();

            //set the strings here
            entryValue.text = entry.value.ToString("C");

            if (entry.count > 1)
            {
                //more than 1 offence
                entryName.text = $"{entry.name} x {entry.count}";
            }
            else
            {
                //only 1 offence
                entryName.text = entry.name;
            }

            //set positions of the entry field
            newEntry.anchoredPosition = new Vector2(0, y);
            y -= entryTemplate.sizeDelta.y;

            //finally enable the thingy
            newEntry.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Clears the bill so that its empty
    /// </summary>
    private void ClearBill()
    {
        foreach (Transform child in entriesRoot)
        {
            //delete every kid here, except the template cause thats the prefab
            if (child != entryTemplate && child != noOffences)
            {
                Destroy(child.gameObject);
            }
        }

        //disable the prefab so that its invisibile
        entryTemplate.gameObject.SetActive(false);
        noOffences.gameObject.SetActive(false);

        //reset to default
        totalValueText.text = 0.ToString("C");
    }

    private void OnStoppedPlaying()
    {
        show = false;
    }

    private void OnStartedPlaying()
    {
        show = false;
    }

    public void ClickedAgain()
    {
        GameManager.Play();
    }

    public void ClickedLeave()
    {
        GameManager.Leave();
    }

    private void Update()
    {
        root.gameObject.SetActive(show);
    }
}