using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{

    public int yeet = 100;
    bool isJessDumb;
    bool isJasmineDumb;

    void Start()
    {
        yeet = 1;
        isJessDumb = true;
        isJasmineDumb = false;
        
    }

    void Update()
    {

        if (yeet != 1)
        {
            yeet++;
            isJessDumb = false;
        }
        else {

            isJasmineDumb = true;

        }

    }
}
