using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class ContinueScript : MonoBehaviour
{
    public Image ContinueUI;
    public Text gameOvertxt;
    public GameObject cursol;
    public List<GameObject> UIs;
    public List<GameObject> skippedUIs;
    public List<GameObject> CursolPos;
    public AudioSource SelectSE;
    public StageCtrl stct;
    public Player p;
    public GameObject pauseCtr;
    public LoadScenes loadScenes;

    private Text txt;
    private Color beforeColor;
    private BlinkObject blinkObject;
    private int objNum = 0;
    private int cursolNum = 0;
    private int judgeRightNum;
    private int judgeLeftNum;
    private bool RightPushed;
    private bool setParam;

    private void Start()
    {
        skippedUIs = UIs.Skip(1)//初めの要素からスタートさせたいため。
                        .ToList();
        skippedUIs.ForEach(skippedUIs =>//初めの要素以外を点滅させない。
        {
            blinkObject = skippedUIs.GetComponent<BlinkObject>();
            blinkObject.enabled = false;
        });
        //カーソルを始めの要素にセット
        cursol.transform.position = CursolPos[0].transform.position;
        if (GameManager.instance.canContinue)
        {
            objNum = 2;
            judgeRightNum = 2;
            judgeLeftNum = 2;
        }
        else
        {
            objNum = 0;
            judgeRightNum = 0;
            judgeLeftNum = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.canContinue)
        {
            txt = UIs[0].GetComponent<Text>();
            txt.enabled = false;
        }
        else
        {
            if (!setParam)
            {
                objNum = 0;
                judgeRightNum = 0;
                judgeLeftNum = 0;
                setParam = true;
            }
            txt = UIs[0].GetComponent<Text>();
            txt.enabled = true;
            txt = UIs[2].GetComponent<Text>();
            txt.enabled = false;
        }
        pauseCtr.SetActive(false);//コンティニュー画面中はポーズメニューが開かないようにする。
        //要素番号がUIのリストの最後尾でなく、右矢印が押された場合
        if (!(objNum == 1) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            RightPushed = true;
            // Debug.Log("moveCursolToRight");
            changeUI();
        }
        //要素番号がUIのリストの最前列でなく、左矢印が押された場合
        else if (!(objNum == judgeLeftNum) && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RightPushed = false;
            // Debug.Log("moveCursolToLeft");
            changeUI();
        }
        //カーソルの位置を新しい位置に移動
        cursol.transform.position = CursolPos[cursolNum].transform.position;
        //Debug.Log("CursolPos == " + cursol.transform.position);
        //Debug.Log("objNum = " + objNum);
        if (Input.GetKeyDown(KeyCode.Space))//スペースが押された場合。決定ボタン。
        {
            if (SelectSE != null)
            {
                SelectSE.Play();
            }
            if (objNum == judgeRightNum)//最初の要素(continue)の場合
            {
                //コンティニューする地点よりも格納されているコンティニュー地点の方が大きい場合
                if (stct.continuePoint.Length > stct.continuePos)
                {
                    if (!GameManager.instance.canContinue)
                    {
                        GameManager.instance.ResetPram();
                        loadScenes.startLoadStage1Scene();

                    }
                    else
                    {
                        stct.playerObj.transform.position = stct.continuePoint[stct.continuePos].transform.position;
                        p.ContinuePlayer();
                        pauseCtr.SetActive(true);
                        GameManager.instance.canContinue = false;
                    }
                }
                else
                {
                    Debug.Log("コンティニューポイントの設定が足りていない");
                }
            }
            else if (objNum == 1)//２つ目の要素(backToTitle)の場合
            {
                GameManager.instance.goBackTitle = true;
            }
            gameObject.SetActive(false);//何かが選択されたら、コンティニュー画面を終了する。
        }      
    }

    private void changeUI()//点滅するUIを変更するメソッド
    {
        txt = UIs[objNum].GetComponent<Text>();
        beforeColor = txt.color;
        txt.color = new Color(beforeColor.r, beforeColor.g, beforeColor.b, 1);//元の色に戻す
        blinkObject = UIs[objNum].GetComponent<BlinkObject>();
        blinkObject.enabled = false;//現在のUIの点滅状態を解除
        if (GameManager.instance.canContinue)
        {
            objNum = RightPushed ? --objNum : ++objNum;
        }
        else
        {
            objNum = RightPushed ? ++objNum : --objNum;
        }
        cursolNum = RightPushed ? ++cursolNum : --cursolNum;
        blinkObject = UIs[objNum].GetComponent<BlinkObject>();
        blinkObject.enabled = true;//次のUIの点滅を開始
    }
}

