using TMPro;
using UnityEngine;

public class TextDialog : HUDElement
{
    public override bool ShouldDisplay => GameManager.IsPlaying;
    public static bool IsShowing => HUDManager.GetElement<TextDialog>().show;

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

    private void OnDisable()
    {
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
            if (!string.IsNullOrEmpty(Input.inputString) && fullyShown)
            {
                show = false;
                GameManager.Unpause();
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
    }
}