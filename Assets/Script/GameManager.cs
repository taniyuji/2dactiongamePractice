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
        //Debug.Log("canContinue = " + canContinue);
        if(hpNum > 1m)//hpが１以上になった場合
        {
            hpNum = 1m;
        }

        if(score >= 1200)
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
        score -= 1200;
    }
}
