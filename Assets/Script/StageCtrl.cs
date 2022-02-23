using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCtrl : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject[] continuePoint;
    public GameObject continueButton;
    public int continueNum; //リスポーンする地点
    
    private Player p;
    private int nextSpawn = 1;

    void Start()
    {
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0)
        {
            playerObj.transform.position = continuePoint[0].transform.position;
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
            }
            else
            {
                Debug.Log("コンティニューポイントの設定が足りていない");
            }
    }
}
