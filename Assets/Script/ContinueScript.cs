using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueScript : MonoBehaviour
{
    public GameObject ContinueUI;
    public GameObject cursol;
    public List<GameObject> UIs;
    public List<GameObject> CursolPos;
    public AudioSource SelectSE;
    public StageCtrl stct;
    public Player p;
    public GameObject pauseCtr;

    private Text txt;
    private Color beforeColor;
    private BlinkObject blinkObject;
    private bool isPausing = false;
    private bool keyPushed = false;
    private int objNum;

    private void Start()
    {
        pauseCtr.SetActive(false);
        foreach (var i in UIs)
        {
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = false;
            objNum++;
        }
        blinkObject = UIs[0].GetComponent<BlinkObject>();
        blinkObject.enabled = true;
        cursol.transform.position = CursolPos[0].transform.position;
        objNum = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (!(objNum == UIs.Count - 1) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("moveCursolToRight");
            txt = UIs[objNum].GetComponent<Text>();
            beforeColor = txt.color;
            txt.color = new Color(beforeColor.r, beforeColor.g, beforeColor.b, 1);
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = false;
            objNum++;
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = true;
        }
        else if (!(objNum == 0) && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("moveCursolToLeft");
            txt = UIs[objNum].GetComponent<Text>();
            beforeColor = txt.color;
            txt.color = new Color(beforeColor.r, beforeColor.g, beforeColor.b, 1);
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = false;
            objNum--;
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = true;
        }
        cursol.transform.position = CursolPos[objNum].transform.position;
        Debug.Log("CursolPos == " + cursol.transform.position);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(SelectSE != null)
            {
                SelectSE.Play();
            }
            if(objNum == 0)
            {
                if (stct.continuePoint.Length > stct.continueNum)
                {
                    stct.playerObj.transform.position = stct.continuePoint[stct.continueNum].transform.position;
                    p.ContinuePlayer();
                    GameManager.instance.isFallDead = false;
                    pauseCtr.SetActive(true);                   
                }
                else
                {
                    Debug.Log("コンティニューポイントの設定が足りていない");
                }
            }else if(objNum == 1)
            {
                GameManager.instance.goBackTitle = true;
            }
            gameObject.SetActive(false);
        }
    }


}

