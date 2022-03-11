using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stageNum : MonoBehaviour
{
    private Text stageText = null;
    private int oldStageNum = 0;

    void Start()
    {
        stageText = GetComponent<Text>();
        if (GameManager.instance != null)
        {
            stageText.text = "Stage" + GameManager.instance.stageNum;
        }
        else
        {
            Debug.Log("ゲームマネージャー置き忘れてるよ");
            Destroy(this);
        }
    }

    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))//ポーズ中は起動させない
        {
            return;
        }

        if (oldStageNum != GameManager.instance.stageNum)
        {
            stageText.text = "stage" + GameManager.instance.stageNum;
            oldStageNum = GameManager.instance.stageNum;
        }
    }
}
