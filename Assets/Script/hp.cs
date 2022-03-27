using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hp : MonoBehaviour
{
    public Image hpGage;
    public GameObject bossHpGages;
    public BossBehavior boss;
    public bool ofBoss;

    private decimal oldHpNum;
    private float bossFirstHp;
    private bool isSet = false;

    void Start()
    {
        if (GameManager.instance != null)
        {
            if (!ofBoss)
            {
                hpGage.fillAmount = (float)GameManager.instance.hpNum;
                oldHpNum = 0m;
            }
        }
        else
        {
            Debug.Log("ゲームマネージャー置き忘れてるよ");
            Destroy(this);
        }
    }

    void Update()
    {
        if (ofBoss)
        {
            if (!GameManager.instance.bossIsvisble)
            {
                bossHpGages.SetActive(false);
            }
            else
            {
                bossHpGages.SetActive(true);
                if (!isSet)
                {
                    oldHpNum = boss.bossHp;
                    bossFirstHp = boss.bossHp;
                    isSet = true;
                }
            }

            if (GameManager.instance.isBossDead)
            {
                bossHpGages.SetActive(false);
            }
        }
        if (Mathf.Approximately(Time.timeScale, 0f))//ポーズ中は起動させない
        {
            return;
        }
        if (boss == null)
        {
            if (oldHpNum != GameManager.instance.hpNum)
            {
                hpGage.fillAmount = (float)GameManager.instance.hpNum;
                oldHpNum = GameManager.instance.hpNum;
            }
        }
        else
        {
            if (oldHpNum != boss.bossHp && GameManager.instance.bossIsvisble)
            {
                Debug.Log("bossHP = " + boss.bossHp);
                if (boss.bossHp != 0)
                {
                    hpGage.fillAmount = 1 * (boss.bossHp / bossFirstHp);
                }
                else
                {
                    hpGage.fillAmount = 0f;
                }
                //Debug.Log("bossHPgage = " + hpGage.fillAmount);
                oldHpNum = boss.bossHp;
            }

        }
    }
}
