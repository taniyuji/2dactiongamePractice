using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int score;
    public int stageNum;//どのステージに居るか
    public decimal hpNum = 0.5m;
    public bool isBossDead;

    [HideInInspector]public bool canContinue;
    [HideInInspector] public bool judgeHp = false;
    [HideInInspector] public bool bossIsvisble = true;
    [HideInInspector] public bool goBackTitle = false;
    [HideInInspector] public bool startbackTitle = false;
    [HideInInspector] public bool Retry = false;
    [HideInInspector] public float playTime;
    [HideInInspector] public bool startPlayTime = false;
    [HideInInspector] public bool continueWait = false;
    [HideInInspector] public bool isPlayerDead = false;

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
            playTime = 0.0f;
            goBackTitle = false;
            startbackTitle = false;
            Retry = false;
            startPlayTime = false;
        }
        else if(startPlayTime && !continueWait && !isBossDead)
        {
            playTime += Time.deltaTime;
            Debug.Log("playTime = " + playTime);
        }



        //Debug.Log("canContinue = " + canContinue);
        if(hpNum > 1m)//hpが１以上になった場合
        {
            hpNum = 1m;
        }

        if(score >= 1500)
        {
            canContinue = true;
        }
        else
        {
            canContinue = false;
        }

        if (goBackTitle)//タイトルに戻ってもゲームマネージャーはリビルトされないため
        {
            ResetPram();
        }
    }

    public void ResetPram()
    {
        hpNum = 0.5m;
        score = 0;
        canContinue = false;
        bossIsvisble = false;
    }

    public void continueBehavior()
    {
        score -= 1500;
    }
}
