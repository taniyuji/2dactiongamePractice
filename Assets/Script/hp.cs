using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hp : MonoBehaviour
{
    public Image hpGage;
    private decimal oldHpNum = 0m;

    void Start()
    {
        if (GameManager.instance != null)
        {
            hpGage.fillAmount = (float)GameManager.instance.hpNum;
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

        if (oldHpNum != GameManager.instance.hpNum)
        {
            hpGage.fillAmount = (float)GameManager.instance.hpNum;
            oldHpNum = GameManager.instance.hpNum;
        }
    }
}
