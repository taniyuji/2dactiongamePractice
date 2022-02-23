using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public FadeScript fade;
    private bool goNextScene = false;
    public static GameManager instance = null;
    public int score;
    public int stageNum;//どのステージに居るか
    public decimal hpNum = 0.5m;

    public CinemachineVirtualCamera Cam;
    public Camera cam;
    [HideInInspector] public bool bossIsvisble;
    [HideInInspector] public bool isBossDead = false;
    [HideInInspector] public bool goBossBattle = false;
    [HideInInspector] public bool isFallDead = false;


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
    }

    public void startFadeOut()
    {
        fade.StartFadeOut();
    }

    void Update()
    {

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
            if (Cam.m_Lens.OrthographicSize < 30)
            {
                Cam.m_Lens.OrthographicSize += 0.2f;
            }
        }

        if (isBossDead)
        {
          
            if (Cam.m_Lens.OrthographicSize > 20)
            {
                Cam.m_Lens.OrthographicSize -= 0.2f;
            }
        }
    }
    /*
                 if(BackColorR > 0.07f)
            {
                BackColorR -= 0.1f;
            }else if(BackColorG > 0.6f)
            {
                BackColorG -= 0.1f;
            }else if (BackColorB > 0.6f)
            {
                BackColorB -= 0.1f;
            }
            cam.backgroundColor = new Color(BackColorR, BackColorG, BackColorB, 0);
            if (LIntensity >= 0.4f && LIntensity <= 1f)
            {
                LIntensity -= 0.05f;
                slight.GetComponent<Light>().intensity = LIntensity;
            }

      if (BackColorR < 0.67f)
            {
                BackColorR += 0.1f;
            }
                BackColorG = 1f;
                BackColorB = 1f;
    
            cam.backgroundColor = new Color(BackColorR, BackColorG, BackColorB, 0);
            LIntensity = 1f;
            slight.GetComponent<Light>().intensity = LIntensity;
     */
}
