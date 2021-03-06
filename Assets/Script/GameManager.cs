using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int score;
    public int stageNum;//どのステージに居るか
    public decimal hpNum = 0.5m;
    public bool isBossDead;
    public bool startPlayTime = false;

    [HideInInspector]public bool canContinue;
    [HideInInspector] public bool bossIsvisble = true;
    [HideInInspector] public bool goBackTitle = false;
    [HideInInspector] public bool startbackTitle = false;
    [HideInInspector] public bool Retry = false;
    [HideInInspector] public float playTime;
    [HideInInspector] public bool continueWait = false;
    [HideInInspector] public bool isPlayerDead = false;
    [HideInInspector] public bool LogoAppeared = false;
    [HideInInspector] public int minute = 0;
    [HideInInspector] public int finalScore;

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
        DontDestroyOnLoad(instance);
    }

    void Update()
    {
        if (startbackTitle || Retry)
        {
            ResetPram();           
        }
        else if(startPlayTime && !continueWait && !isBossDead)
        {
            playTime += Time.deltaTime;
            //Debug.Log("playTime = " + playTime);
        }



        //Debug.Log("canContinue = " + canContinue);
        if(hpNum > 1m)//hpが１以上になった場合
        {
            hpNum = 1m;
        }

        if(score >= 2000)
        {
            canContinue = true;
        }
        else
        {
            canContinue = false;
        }
    }

    public void ResetPram()
    {
        hpNum = 0.5m;
        score = 0;
        canContinue = false;
        bossIsvisble = false;
        isBossDead = false;
        LogoAppeared = false;
        startbackTitle = false;
        startPlayTime = false;
        goBackTitle = false;
        Retry = false;
        playTime = 0;
        minute = 0;
    }

    public void continueBehavior()
    {
        score -= 2000;
    }
}
