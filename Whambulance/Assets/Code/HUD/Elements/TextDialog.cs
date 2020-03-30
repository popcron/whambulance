using TMPro;
using UnityEngine;

public class TextDialog : HUDElement
{
    public override bool ShouldDisplay => GameManager.IsPlaying;
    public static bool IsShowing => HUDManager.GetElement<TextDialog>().show;

    [SerializeField]
    private TMP_Text closeHint;

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text bodyText;

    [SerializeField]
    private RectTransform window;

    [SerializeField]
    private float showSpeed = 8f;

    private bool show;
    private string body;
    private float time;

    private void Awake()
    {
        window.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        window.gameObject.SetActive(false);
        show = false;
    }

    private void Update()
    {
        window.gameObject.SetActive(show);
        if (show)
        {
            time += Time.unscaledDeltaTime * showSpeed;

            //animate over time
            int length = (int)Mathf.Clamp(time, 0, body.Length);
            bool fullyShown = length == body.Length;
            bodyText.text = body.Substring(0, length);

            //press any key to essentially close
            if (Input.GetKey(KeyCode.Space) && fullyShown)
            {
                show = false;
                GameManager.Unpause();
            }

            //show the close hint after 3 seconds
            float extraSeconds = 3f * showSpeed;
            if (time > body.Length + extraSeconds)
            {
                Color color = closeHint.color;
                color.a = Mathf.Lerp(color.a, 0.8f, Time.unscaledDeltaTime * 2f);
                closeHint.color = color;
            }
        }
    }

    public static void Show(string title, string body)
    {
        //super smol to immitate a pause
        GameManager.Pause();

        TextDialog dialog = HUDManager.GetElement<TextDialog>();
        dialog.show = true;
        dialog.time = 0f;
        dialog.titleText.text = title;
        dialog.bodyText.text = null;
        dialog.body = body;

        Color color = dialog.closeHint.color;
        color.a = 0f;
        dialog.closeHint.color = color;
    }
}