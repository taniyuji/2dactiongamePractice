using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class ContinueScript : MonoBehaviour
{
    public GameObject ContinueUI;
    public GameObject cursol;
    public List<GameObject> UIs;
    public List<GameObject> skippedUIs;
    public List<GameObject> CursolPos;
    public AudioSource SelectSE;
    public StageCtrl stct;
    public Player p;
    public GameObject pauseCtr;

    private Text txt;
    private Color beforeColor;
    private BlinkObject blinkObject;
    private int objNum = 0;
    private bool RightPushed;

    private void Start()
    {
        skippedUIs = UIs.Skip(1)
                        .ToList();
        skippedUIs.ForEach(skippedUIs =>
        {
            blinkObject = skippedUIs.GetComponent<BlinkObject>();
            blinkObject.enabled = false;
        });                  
        cursol.transform.position = CursolPos[0].transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        pauseCtr.SetActive(false);//コンティニュー画面中はポーズメニューが開かないようにする。
        if (!(objNum == UIs.Count - 1) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            RightPushed = true;
            // Debug.Log("moveCursolToRight");
            changeUI();
        }
        else if (!(objNum == 0) && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RightPushed = false;
            // Debug.Log("moveCursolToLeft");
            changeUI();
        }
        cursol.transform.position = CursolPos[objNum].transform.position;
        //Debug.Log("CursolPos == " + cursol.transform.position);
        //Debug.Log("objNum = " + objNum);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SelectSE != null)
            {
                SelectSE.Play();
            }
            if (objNum == 0)
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
            }
            else if (objNum == 1)
            {
                GameManager.instance.goBackTitle = true;
            }
            gameObject.SetActive(false);
        }      
    }

    private void changeUI()
    {
        txt = UIs[objNum].GetComponent<Text>();
        beforeColor = txt.color;
        txt.color = new Color(beforeColor.r, beforeColor.g, beforeColor.b, 1);
        blinkObject = UIs[objNum].GetComponent<BlinkObject>();
        blinkObject.enabled = false;
        objNum = RightPushed ? ++objNum : --objNum;
        blinkObject = UIs[objNum].GetComponent<BlinkObject>();
        blinkObject.enabled = true;
    }
}

