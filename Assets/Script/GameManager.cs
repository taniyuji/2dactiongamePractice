using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public FadeScript fade;
    
    public static GameManager instance = null;
    public int score;
    public int stageNum;//どのステージに居るか
    public decimal hpNum = 0.5m;
    public AudioSource BossBGM = null;
    public CinemachineVirtualCamera Cam;
    public Camera cam;
    public GameObject BackGroundChangePos;
    public GameObject player;
    [HideInInspector] public bool bossIsvisble;
    [HideInInspector] public bool isBossDead = false;
    [HideInInspector] public bool goBossBattle = false;
    [HideInInspector] public bool isFallDead = false;

    private bool Play = false;
    private bool goNextScene = false;
    private float BackColorR;
    private float BackColorG;
    private float BackColorB;



    private void Awake()
    {
         //インスタンスが存在しない場合
        if(instance == null)
        {
            instance = this;
           // DontDestroyOnLoad(this.gameObject);
        }
        else//存在した場合
        {
            Destroy(this.gameObject);
        }
        BackColorR = cam.backgroundColor.r;
        BackColorG = cam.backgroundColor.g;
        BackColorB = cam.backgroundColor.b;
    }

    public void startFadeOut()
    {
        fade.StartFadeOut();
    }

    void Update()
    {
        if (player != null)
        {
            if (player.transform.position.y < BackGroundChangePos.transform.position.y)
            {
                if (BackColorR > 0.08f)
                {
                    BackColorR -= 0.01f;
                }
                else if (BackColorG > 0.055f)
                {
                    BackColorG -= 0.01f;
                }
                else if (BackColorB > 0.31f)
                {
                    BackColorB -= 0.01f;
                }
                cam.backgroundColor = new Color(BackColorR, BackColorG, BackColorB, 0);
            }
        }

        if(hpNum >= 1m)
        {
            hpNum = 1m;
        }

        if (goBossBattle)
        {
            goBossBattle = false;
            SceneManager.LoadScene("Boss1");
        }
        //次のシーンに言っておらず、フェードアウトが終了していた場合
        if (!goNextScene && fade.IsFadeOutComplete())
        {
            SceneManager.LoadScene("Stage1-Forest-");
            goNextScene = true;
        }

        if(bossIsvisble && !isBossDead)
        {
            if (!Play && BossBGM != null)
            {
                BossBGM.Play();
                Play = true;
            }
            if (Cam.m_Lens.OrthographicSize < 30)
            {
                Cam.m_Lens.OrthographicSize += 0.2f;
            }
        }

        if (isBossDead && BossBGM != null)
        {
            BossBGM.volume -= 0.01f;
            if (Cam.m_Lens.OrthographicSize > 20)
            {
                Cam.m_Lens.OrthographicSize -= 0.2f;
            }
        }
    }
}
