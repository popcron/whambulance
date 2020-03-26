using UnityEngine;

public class AwardIfGotPunched : MonoBehaviour
{
    [SerializeField]
    private string offenceName = "Nuisance";

    [SerializeField]
    private int value = 100;

    public void GotPunched()
    {
        ScoreManager.AwardPoints(offenceName, value);
    }
}