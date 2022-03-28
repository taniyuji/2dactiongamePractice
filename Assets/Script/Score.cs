using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public bool isResult = false;
    public bool isfinalScore = false;

    private Text scoreText = null;
    private int oldScore = 0;

    void Start()
    {
        scoreText = GetComponent<Text>();
        if(GameManager.instance != null)
        {
            if (isResult)
            {
                scoreText.text = GameManager.instance.score.ToString();               
            }
        }
        else
        {
            Debug.Log("ゲームマネージャー置き忘れてるよ");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))//ポーズ中は起動させない
        {
            return;
        }

        if (oldScore != GameManager.instance.score && !isfinalScore && !isResult)
        {
            scoreText.text = "Score" + GameManager.instance.score;
            oldScore = GameManager.instance.score;
        }else if (isfinalScore)
        {
            scoreText.text = GameManager.instance.finalScore.ToString();
        }
    }
}
