using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{

    public int yeet = 0;
    bool isJessDumb;
   

    void Start()
    {
        yeet = 0;
        isJessDumb = false;
    }

    void Update()
    {

        if (yeet != 0)
        {
            yeet++;
            isJessDumb = true;
        }
        else
        {
            yeet = 0;
            isJessDumb = false;
        }

    }
}
