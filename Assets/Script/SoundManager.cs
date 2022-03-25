using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] audioList;

    private bool played = false;

    void Update()
    {
        if (GameManager.instance.canContinue && !played)
        {
            audioList[0].Play();
            played = true;
        }
    }
}
