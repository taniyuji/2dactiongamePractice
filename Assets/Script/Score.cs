using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private Text scoreText = null;
    private int oldScore = 0;

    void Start()
    {
        scoreText = GetComponent<Text>();
        if(GameManager.instance != null)
        {
            scoreText.text = "Score" + GameManager.instance.score;
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

        if (oldScore != GameManager.instance.score)
        {
            scoreText.text = "Score" + GameManager.instance.score;
            oldScore = GameManager.instance.score;
        }
    }
}
