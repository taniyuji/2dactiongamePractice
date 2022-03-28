using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnding : MonoBehaviour
{
    public Animator anim;
    public GameObject youngSister;
    public GameObject player;
    public AudioSource vanishSE;

    private bool play = false;

    // Start is called before the first frame update
    void Start()
    {
        youngSister.SetActive(false);
        player.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))//pause中は起動させない
        {
            if (vanishSE != null)
            {
                vanishSE.Pause();
                play = false;
            }
            return;
        }
        if (IsDefeatedAnimFin())
        {
            //Debug.Log("boss Ending set false");
            player.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public bool IsDefeatedAnimFin()
    {
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("BossEnding_vanish"))
        {
            if(currentState.normalizedTime > 0.1f && !play)
            {
                vanishSE.Play();
                play = true;
            }
            if(currentState.normalizedTime >= 0.76f)
            {
                youngSister.SetActive(true);
            }

            if(currentState.normalizedTime >= 1)
            {
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}
