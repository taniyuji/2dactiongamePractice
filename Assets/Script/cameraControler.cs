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
    public SpriteRenderer Logo;
    [HideInInspector] public bool cameraBack = false;

    private float newCamSize;
    private int followIdx = 1;
    private bool Play = false;
    private float waitTime = 0f;
    private float colorNum = 0f;

    private void Start()
    {
        newCamSize = Cam.m_Lens.OrthographicSize;
        if (Logo != null)
        {
            Logo.color = new Color(Logo.color.r, Logo.color.g, Logo.color.b, 0);
            Logo.enabled = false;
        }
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
            if (followObj[1].activeSelf == false && !pEnd.playerIn)
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
                waitTime = 0f;
                newCamSize -= 0.1f;
            }

            if (pEnd.dontFollow)
            {
                Cam.Follow = null;
                if(waitTime > 6f)
                {
                    Logo.enabled = true;
                    Logo.color = new Color(Logo.color.r, Logo.color.g, Logo.color.b, colorNum);
                    colorNum += 0.01f;
                    if(colorNum >= 1 && waitTime > 10f)
                    {
                        GameManager.instance.LogoAppeared = true;
                    }
                }
                waitTime += Time.deltaTime;
            }

            Cam.m_Lens.OrthographicSize = newCamSize;
        }
    }
}
