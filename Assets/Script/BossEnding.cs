using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnding : MonoBehaviour
{
    public Animator anim;
    public GameObject youngSister;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        youngSister.SetActive(false);
        player.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDefeatedAnimFin())
        {
            //Debug.Log("boss Ending set false");
            GameManager.instance.isBossDead = false;
            player.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public bool IsDefeatedAnimFin()
    {
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("BossEnding_vanish"))
        {
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
