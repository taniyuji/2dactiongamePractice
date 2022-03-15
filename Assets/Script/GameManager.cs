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
    public AudioSource buttonSE;
    [HideInInspector] public bool bossIsvisble;
    [HideInInspector] public bool isBossDead = false;
    [HideInInspector] public bool goBossBattle = false;
    [HideInInspector] public bool isFallDead = false;
    [HideInInspector] public bool goNextScene = false;
    [HideInInspector] public bool goBackTitle = false;

    private bool isSet = false;
    private GameObject goBossPos;
    private bool Play = false;
    private float BackColorR;
    private float BackColorG;
    private float BackColorB;
    private AsyncOperation asyncOperation;
    private bool keyPushed = false;

    private void Awake()
    {
        Time.timeScale = 1f;
         //インスタンスが存在しない場合
        if(instance == null)
        {
            instance = this;
        }
        else//存在した場合
        {
            Destroy(this.gameObject);
        }
        BackColorR = cam.backgroundColor.r;
        BackColorG = cam.backgroundColor.g;
        BackColorB = cam.backgroundColor.b;
        goBossPos = GameObject.Find("goBossBattlePoint");
    }

    public void startFadeOut()
    {
        if (buttonSE != null)
        {
            buttonSE.Play();
        }
        startLoadStage1Scene();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene" && Input.anyKey && !keyPushed)
        {
            startFadeOut();
            keyPushed = true;
        }
        if (!isSet && goBossPos != null)
        {
            if( player.transform.position.x < goBossPos.transform.position.x)
            {
                Debug.Log("ボス戦へ");
                goBossBattle = true;
                isSet = true;
            }
        }
        if(Mathf.Approximately(Time.timeScale, 0f))//ポーズ中は起動させない
        {
            return;
        }

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
            startLoadBoss1Scene();
            goBossBattle = false;
        }

        if (goBackTitle)
        {
            startLoadTitleScene();
            goBackTitle = false;
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

    private void startLoadStage1Scene()
    {
        StartCoroutine("LoadStage1Scene");
    }

    IEnumerator LoadStage1Scene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Stage1-Forest-");
        asyncOperation.allowSceneActivation = false;
        fade.StartFadeOut();
        yield return new WaitForSeconds(1f);
        asyncOperation.allowSceneActivation = true;
    }

    private void startLoadTitleScene()
    {
        StartCoroutine("LoadTitleScene");
    }

    IEnumerator LoadTitleScene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("TitleScene");
        asyncOperation.allowSceneActivation = false;
        fade.StartFadeOut();
        yield return new WaitForSeconds(2f);
        asyncOperation.allowSceneActivation = true;
    }

    private void startLoadBoss1Scene()
    {
        StartCoroutine("LoadBoss1Scene");
    }

    IEnumerator LoadBoss1Scene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Boss1");
        asyncOperation.allowSceneActivation = false;
        fade.StartFadeOut();
        yield return new WaitForSeconds(2f);
        asyncOperation.allowSceneActivation = true;
    }
}
