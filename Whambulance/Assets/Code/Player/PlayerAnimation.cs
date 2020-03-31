using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private float fps = 10f;

    [SerializeField]
    private Transform[] walkFrames = { };

    [SerializeField]
    private Transform[] idleFrames = { };

    [SerializeField]
    private Transform[] punchFrames = { };

    private float frameTime;
    private int punchF;
    private int walkF;
    private int idleF;
    private bool isMoving;

    public Player Player { get; private set; }

    private void Awake()
    {
        Player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        Player.onPunch += OnPunch;
    }

    private void OnDisable()
    {
        Player.onPunch += OnPunch;
    }

    private void OnPunch(Player player)
    {
        if (player == Player)
        {
            PlayPunchAnimation();
        }
    }

    private void DisableAllFrames()
    {
        for (int i = 0; i < walkFrames.Length; i++)
        {
            walkFrames[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < idleFrames.Length; i++)
        {
            idleFrames[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < punchFrames.Length; i++)
        {
            punchFrames[i].gameObject.SetActive(false);
        }
    }

    private void SetFrame(Transform frame)
    {
        DisableAllFrames();
        frame.gameObject.SetActive(true);
    }

    private void PlayPunchAnimation()
    {
        frameTime = 0f;
        SetFrame(punchFrames[punchF]);
        SafeIncrement(ref idleF, punchFrames.Length);
    }

    private void SafeIncrement(ref int frame, int limit)
    {
        frame++;
        if (frame >= limit)
        {
            frame = 0;
        }
    }

    private void Update()
    {
        float interval = 1f / fps;

        //moving state changed, so reset timer
        if (Player.Movement.IsMoving != isMoving)
        {
            isMoving = !isMoving;
            idleF = 0;
            walkF = 0;
            frameTime = interval;
        }

        frameTime += Time.deltaTime;
        if (frameTime > interval)
        {
            frameTime = 0f;

            //update frame if moving or not
            if (isMoving)
            {
                SetFrame(walkFrames[walkF]);
                SafeIncrement(ref walkF, walkFrames.Length);
            }
            else
            {
                SetFrame(idleFrames[idleF]);
                SafeIncrement(ref idleF, idleFrames.Length);
            }
        }
    }
}