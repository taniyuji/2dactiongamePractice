using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayTime : MonoBehaviour
{
    public Text txt;
    public bool isResult;
    private int castedTime;
    private int judgeTime = 600;
    private int bonusScore;

    // Start is called before the first frame update
    void Start()
    {
        if (isResult)
        {
            bonusScore = 10 * (judgeTime - (int)GameManager.instance.playTime);
            GameManager.instance.finalScore = GameManager.instance.score + bonusScore;
            txt.text = bonusScore.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isResult)
        {
            castedTime = (int)GameManager.instance.playTime - 60 * GameManager.instance.minute;
            if (castedTime >= 60)
            {
                ++GameManager.instance.minute;
                castedTime -= 60 * GameManager.instance.minute;
            }
            txt.text = string.Format($"{GameManager.instance.minute:00}") + ":" + string.Format($"{castedTime:00}");
        }
    }
}
