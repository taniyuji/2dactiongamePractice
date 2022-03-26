using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCtrl : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject[] continuePoint;
    public List<GameObject> fallDeadPoint;
    public List<GameObject> RightMoveLim;
    public List<GameObject> LeftMoveLim;
    public GameObject DescendPosObj;
    public GameObject UnderGPosObj;
    public ContinueScript ContinueCtr;
    public int continuePos; //リスポーンする地点

    private Player p;
    private int nextSpawn = 1;
    private int fallDeadNum = 0;
    private bool ContinueCtrSet = false;
    private int RlimitNum = 0;
    private int LLimitNum = 0;
    private Vector2 DescendPos;
    private Vector2 UnderGPos;

    void Start()
    {
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0)
        {
            playerObj.transform.position = continuePoint[continuePos].transform.position;
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
        if (DescendPosObj != null && UnderGPosObj != null)
        {
            DescendPos = DescendPosObj.transform.position;
            UnderGPos = UnderGPosObj.transform.position;
        }
    }

    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))//ポーズ中は起動させない
        {
            return;
        }

        if (DescendPosObj != null && UnderGPosObj != null)
        {
            judgeMoveLimit();
        }

        if (fallDeadPoint.Count != 0)
        {
            JudgeIsFallDead();
        }
        
        if (continuePoint.Length > nextSpawn)
        {
            if (playerObj.transform.position.x <= continuePoint[nextSpawn].transform.position.x)
            {
                continuePos = nextSpawn;
                nextSpawn++;
            }

        }

            //プレイヤースクリプトの方で消滅アニメーションが終了した場合
        if ((p.IsDeadAnimEnd() || p.isFallDead) && !ContinueCtrSet)
        {
            //Debug.Log("ContinueCtrSetActive");
            ContinueCtr.activateParent = true;
            ContinueCtrSet = true;
        }

        if (!p.isDead)
        {
            ContinueCtrSet = false;
        }
    }

    private void judgeMoveLimit()
    {
        if(playerObj.transform.position.x < DescendPos.x && RlimitNum == 0)
        {
            RlimitNum++;
        }else if(playerObj.transform.position.y < UnderGPos.y && RlimitNum == 1 && LLimitNum == 0)
        {
            RlimitNum++;
            LLimitNum++;
        }
    }

    public float getRLimitXPos()
    {
        return RightMoveLim[RlimitNum].transform.position.x;
    }

    public float getLLimitXPos()
    {
        return LeftMoveLim[LLimitNum].transform.position.x;
    }

    public void JudgeIsFallDead()
    {
        if (fallDeadPoint == null)
        {
          return;
        }

        if (playerObj.transform.position.x < DescendPos.x && fallDeadNum == 0)
        {
            fallDeadNum++;
        }

        if (playerObj.transform.position.y < fallDeadPoint[fallDeadNum].transform.position.y)
        {
            p.isFallDead = true;
            //Debug.Log("set player fall dead");
        }
    }

}
