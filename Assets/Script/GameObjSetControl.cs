using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjSetControl : MonoBehaviour
{
    public GameObject Obj;
    public SpriteRenderer sr;
    public PlayerTriggerCheck pt;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim.enabled = false;
        sr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (pt.isOn)
        {
            anim.enabled = true;
            sr.enabled = true;
        }
    }
}
