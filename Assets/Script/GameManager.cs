using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int score;
    public int stageNum;//どのステージに居るか
    public decimal hpNum = 1m;  
    public bool canContinue;

    [HideInInspector] public bool bossIsvisble;
    [HideInInspector] public bool isBossDead = false;
    [HideInInspector] public bool goBackTitle = false;

     private int judgeCanContinue = 0;

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
        if(hpNum > 1m)
        {
            hpNum = 1m;
            judgeCanContinue++;
            Debug.Log("judgeCanContinue = " + judgeCanContinue);
        }
        if(judgeCanContinue >= 5)
        {
            canContinue = true;
            judgeCanContinue = 0;
        }
    }

    public void ResetPram()
    {
        hpNum = 0.5m;
        score = 0;
        judgeCanContinue = 0;
    }
}
