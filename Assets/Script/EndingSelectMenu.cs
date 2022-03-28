using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingSelectMenu : MonoBehaviour
{
    public GameObject cursol;
    public List<Text> UIs;
    public List<GameObject> CursolPos;
    public AudioSource SelectSE;
    public LoadScenes loadScenes;
    [HideInInspector] public bool isset = false;
    [HideInInspector] public bool openRanking = false;

    private Text txt;
    private Color beforeColor;
    private BlinkObject blinkObject;
    private int cursolNum = 0;
    private bool RightPushed;
    private Text targetUI;

    private void Start()
    {
        blinkObject = UIs[1].GetComponent<BlinkObject>();//初期位置じゃないオブジェクトは点滅させない
        blinkObject.enabled = false;
        //カーソルを始めの要素にセット
        cursol.transform.position = CursolPos[0].transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if (!openRanking) { 
        //カーソルポジションの要素番号が最後尾でなく、右矢印が押された場合
        if (!(cursolNum == 1) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            RightPushed = true;
            // Debug.Log("moveCursolToRight");
            changeUI();
        }
        //要素番号がUIのリストの最前列でなく、左矢印が押された場合
        else if (!(cursolNum == 0) && Input.GetKeyDown(KeyCode.LeftArrow))
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
                if (cursolNum == 0)//最初の要素(continue)の場合
                {
                    if (!isset)
                    {
                        loadScenes.LoadRankingScene();
                        openRanking = true;
                        isset = true;
                    }
                }
                else if (cursolNum == 1)//２つ目の要素(backToTitle)の場合
                {
                    GameManager.instance.goBackTitle = true;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isset = false;
                }
            }
        }
    }

    private void changeUI()//点滅するUIを変更するメソッド
    {
        txt = UIs[cursolNum].GetComponent<Text>();//現在のテキストコンポーネントを入手
        beforeColor = txt.color;
        txt.color = new Color(beforeColor.r, beforeColor.g, beforeColor.b, 1);//元の色に戻す
        blinkObject = UIs[cursolNum].GetComponent<BlinkObject>();
        Debug.Log("beforetargetUI = " + targetUI);
        blinkObject.enabled = false;//現在のUIの点滅状態を解除
        cursolNum = RightPushed ? ++cursolNum : --cursolNum;//右矢印が押されたかで判断
        Debug.Log("aftertargetUI = " + targetUI);
        blinkObject = UIs[cursolNum].GetComponent<BlinkObject>();
        blinkObject.enabled = true;//次のUIの点滅を開始
    }
}
