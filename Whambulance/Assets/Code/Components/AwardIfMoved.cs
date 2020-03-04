using UnityEngine;

public class AwardIfMoved : Prop
{
    [SerializeField]
    private string offenceName = "Nuisance";

    [SerializeField]
    private int value = 100;

    [SerializeField]
    private float moveThreshold = 0.2f;

    private bool disturbed = false;
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalPosition.z = 0f;
    }

    private void Update()
    {
        //only once
        if (disturbed)
        {
            return;
        }

        //if moved this much, then donzo
        float sqrDistance = (originalPosition - transform.localPosition).sqrMagnitude;
        if (originalPosition != default && sqrDistance >= moveThreshold * moveThreshold)
        {
            disturbed = true;
            ScoreManager.AwardPoints(offenceName, value);
        }
    }
}