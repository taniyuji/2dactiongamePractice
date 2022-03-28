using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingSE : MonoBehaviour
{
    public AudioSource floatSE;
    public AudioSource floatSE2;

    private float playTime;
    private bool play = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(playTime);
        if (playTime > 4f && playTime < 7f)
        {
            if (!play)
            {
                floatSE.Play();
                play = true;
            }
        }
        else if(playTime >= 6f)
        {
            if (play)
            {
                floatSE2.Play();
                play = false;
            }
            playTime = 0f;
        }
        playTime += Time.deltaTime;
    }
}
