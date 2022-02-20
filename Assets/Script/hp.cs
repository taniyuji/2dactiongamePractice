using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hp : MonoBehaviour
{
    public Image hpGage;
    private Text hpText = null;
    private float oldHpNum = 0;

    void Start()
    {
        hpText = GetComponent<Text>();
        if (GameManager.instance != null)
        {
            hpGage.fillAmount = GameManager.instance.hpNum;
        }
        else
        {
            Debug.Log("ゲームマネージャー置き忘れてるよ");
            Destroy(this);
        }
    }

    void Update()
    {
        if (oldHpNum != GameManager.instance.hpNum)
        {
            hpGage.fillAmount = GameManager.instance.hpNum;
            oldHpNum = GameManager.instance.hpNum;
        }
    }
}
