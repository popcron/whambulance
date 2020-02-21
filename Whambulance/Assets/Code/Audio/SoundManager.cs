using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
        //Variables for player footsteps on two different textures.
        public GameObject footStepTriggerGrass;
        public GameObject footStepTriggerConcrete;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //This method is called when player starts to run. Player send the message about whether it is running(speed), and its textureInt(grass:1, concrete:2)
        //If player is running, then trigger the corresponding footstep sound.
        public void FootStep(int speed, int textureInt)
        {

            if (speed == 1 && textureInt == 1) footStepTriggerGrass.SetActive(true);
            else if (speed == 1 && textureInt == 2) footStepTriggerConcrete.SetActive(true);
            //if player stops running, stop any footstep audio.
            else if (speed == 0)
            {

                footStepTriggerGrass.SetActive(false);
                footStepTriggerConcrete.SetActive(false);

            }
        }

        //A function to stop any footstep audio that is currently playing.
        public void ResetAudio()
        {

            footStepTriggerGrass.SetActive(false);
            footStepTriggerConcrete.SetActive(false);

        }

    }
