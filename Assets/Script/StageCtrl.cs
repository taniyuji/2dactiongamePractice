using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCtrl : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject[] continuePoint;
    public GameObject[] fallDeadPoint;
    public GameObject continueButton;
    public int continueNum; //リスポーンする地点
    
    private Player p;
    private int nextSpawn = 1;
    private int nextFallDeadPos = 1;
    private int fallDeadPos = 0;
    private FallDeadNum Fnum;

    void Start()
    {
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0)
        {
            playerObj.transform.position = continuePoint[continueNum].transform.position;
            p = playerObj.GetComponent<Player>();
            if(p == null)
            {
                Debug.Log("プレイヤーじゃないものがあたっちされている");
            }
        }
        else
        {
            Debug.Log("設定が足りてないよ");
        }
    }

    void Update()
    {
        if (fallDeadPoint.Length > nextFallDeadPos)
        {
            Fnum = fallDeadPoint[nextFallDeadPos].GetComponent<FallDeadNum>();
        }
        else
        {
            Fnum.connectContinuePos = -1;
        }

        if (Fnum.connectContinuePos == continueNum)
        {
            fallDeadPos = nextFallDeadPos;
            Debug.Log(fallDeadPos);
            nextFallDeadPos++;
        }

        isFallDead();

        if (continuePoint.Length > nextSpawn)
        {
            if (playerObj.transform.position.x <= continuePoint[nextSpawn].transform.position.x)
            {
                continueNum = nextSpawn;
                nextSpawn++;
            }
            
        }

        if (p != null && p.IsContinueWating())
        {
            continueButton.SetActive(true);
        }
    }

    public void onClickContinue()
    {
            if (continuePoint.Length > continueNum)
            {
                playerObj.transform.position = continuePoint[continueNum].transform.position;
                p.ContinuePlayer();
                continueButton.SetActive(false);
                GameManager.instance.isFallDead = false;
            }
            else
            {
                Debug.Log("コンティニューポイントの設定が足りていない");
            }
    }

    public void isFallDead()
    {
        if (fallDeadPoint == null)
        {
            return;
        }else if (playerObj.transform.position.y <= fallDeadPoint[fallDeadPos].transform.position.y)
        {
            GameManager.instance.isFallDead = true;
        }
    }

}
