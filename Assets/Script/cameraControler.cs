using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class cameraControler : MonoBehaviour
{
    public bool isBossBattle;
    public CinemachineVirtualCamera Cam;
    public Camera cam;
    public List<GameObject> followObj;
    public BossEnding BESc;
    public AudioSource BossBGM;
    public PlayerEnding pEnd;
    [HideInInspector] public bool cameraBack = false;

    private float newCamSize;
    private int followIdx = 1;
    private bool Play = false;
    private float waitTime = 0f;

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

        if (isBossBattle)
        {
            if (GameManager.instance.bossIsvisble)
            {
                if (BossBGM != null && !Play)
                {
                    BossBGM.Play();
                    Play = true;
                }
                if (newCamSize < 30)
                {
                    newCamSize += 0.1f;
                }
            }
            Cam.m_Lens.OrthographicSize = newCamSize;         
        }
        else
        {
            if (!GameManager.instance.isBossDead && !pEnd.playerIn)
            {
                if (waitTime > 3f)
                {
                   // Debug.Log("set followIdx = 0");
                    newCamSize = 15;
                    followIdx = 0;
                }
                waitTime += Time.deltaTime;
                Cam.Follow = followObj[followIdx].transform;
            }
            else if (pEnd.playerIn && newCamSize > 10)
            {
                newCamSize -= 0.1f;
            }

            if (pEnd.dontFollow)
            {
                Cam.Follow = null;
            }

            Cam.m_Lens.OrthographicSize = newCamSize;



        }
    }
}
