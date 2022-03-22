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
    public GameObject boss;
    public List<GameObject> followObj;
    public BossBehavior bSc;
    public AudioSource buttonSE;
    [HideInInspector] public bool bossIsvisble;
    [HideInInspector] public bool isBossDead = false;
    [HideInInspector] public bool goBossBattle = false;
    [HideInInspector] public bool isFallDead = false;
    [HideInInspector] public bool goNextScene = false;
    [HideInInspector] public bool goBackTitle = false;
    [HideInInspector] public bool cameraBack = false;

    private bool isSet = false;
    private GameObject goBossPos;
    private bool Play = false;
    private AsyncOperation asyncOperation;
    private bool keyPushed = false;
    private GameObject fixedBossPoss;
    private int followIdx = 0;
    private float newCamSize;

    private void Awake()
    {
        Time.timeScale = 1f;
        //インスタンスが存在しない場合
        if (instance == null)
        {
            instance = this;
        }
        else//存在した場合
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        followObj.Add(player);
        followObj.Add(boss);
        goBossPos = GameObject.Find("goBossBattlePoint");
        newCamSize = Cam.m_Lens.OrthographicSize;
    }

    void Update()
    {
        if (isBossDead)
        {
            if (newCamSize > 18)
            {
               newCamSize -= 1f;
            }
            if (bSc.IsDefeatedAnimFin())
            {
                cameraBack = true;
                newCamSize = 15;
                isBossDead = false;
            }
            Debug.Log("cameraBack = " + cameraBack);
        }

        followIdx = isBossDead ? 1 : 0;
        Cam.Follow = followObj[followIdx].transform;

        if (!cameraBack && bossIsvisble && !isBossDead)
        {
            if (!Play && BossBGM != null)
            {
                BossBGM.Play();
                Play = true;
            }
            if (newCamSize < 30)
            {
                newCamSize += 0.2f;
            }
        }
        Cam.m_Lens.OrthographicSize = newCamSize;

        if (SceneManager.GetActiveScene().name == "TitleScene" && Input.anyKey && !keyPushed)
        {
            Debug.Log("起動開始");
            buttonSE.Play();
            startLoadStage1Scene();
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

        if (isBossDead && BossBGM != null)
        {
            BossBGM.volume -= 0.01f;
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
