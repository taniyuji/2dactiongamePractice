using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int score;
    public int stageNum;//どのステージに居るか
    public decimal hpNum = 0.5m;  
    public bool canContinue;
   
    [HideInInspector] public bool judgeHp = false;
    [HideInInspector] public bool bossIsvisble;
    [HideInInspector] public bool isBossDead = false;
    [HideInInspector] public bool goBackTitle = false;

     private int judgeCanContinue = 0;
    private bool setContinue = false;

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

        if (judgeHp)//hpが変化し、まだコンティニューしていない場合
        {
            if(!setContinue && hpNum <= 0.1m)
            {
                judgeCanContinue++;
                //Debug.Log("judgeCanContinue = " + judgeCanContinue);
            }
            judgeHp = false;
        }

        //hp残量が1以下の状態を4回繰り返すとコンティニューできるようになる。
        if(judgeCanContinue >= 4)
        {
            canContinue = true;//コンティニュースクリプトでfalseになる
            setContinue = true;//いちどだけコンティニューできるようにするため
            judgeCanContinue = 0;
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
        judgeCanContinue = 0;
        canContinue = false;
        setContinue = false;
        bossIsvisble = false;
    }
}
