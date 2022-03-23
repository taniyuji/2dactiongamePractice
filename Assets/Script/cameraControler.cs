using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class cameraControler : MonoBehaviour
{
    public CinemachineVirtualCamera Cam;
    public Camera cam;
    public List<GameObject> followObj;
    public BossBehavior bSc;
    public AudioSource BossBGM;
    [HideInInspector] public bool cameraBack = false;

    private float newCamSize;
    private int followIdx = 0;
    private bool Play = false;

    private void Start()
    {
        newCamSize = Cam.m_Lens.OrthographicSize;
    }

    private void Update()
    {

        if (Mathf.Approximately(Time.timeScale, 0f))//ポーズ中は起動させない
        {
            return;
        }

        if (GameManager.instance.isBossDead)
        {
            if (newCamSize > 18)
            {
                newCamSize -= 1f;
            }
            if (bSc.IsDefeatedAnimFin())
            {
                cameraBack = true;
                newCamSize = 15;
                GameManager.instance.isBossDead = false;
            }
            //Debug.Log("cameraBack = " + cameraBack);
        }

        followIdx = GameManager.instance.isBossDead ? 1 : 0;
        Cam.Follow = followObj[followIdx].transform;

        if (!cameraBack && GameManager.instance.bossIsvisble && !GameManager.instance.isBossDead)
        {
            if (newCamSize < 30)
            {
                newCamSize += 0.2f;
            }
        }
        Cam.m_Lens.OrthographicSize = newCamSize;


        if (GameManager.instance.bossIsvisble && !Play && BossBGM != null)
        {
            BossBGM.Play();
            Play = true;
        }

        if (GameManager.instance.isBossDead && BossBGM != null)
        {
            BossBGM.volume -= 0.01f;
        }
    }
}
