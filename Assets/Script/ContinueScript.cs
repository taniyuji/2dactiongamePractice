using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class ContinueScript : MonoBehaviour
{
    public GameObject parent;
    public Text ContinueUI;
    public Text gameOvertxt;
    public GameObject cursol;
    public List<Text> UIs;
    public List<Text> skippedUIs;
    public List<GameObject> CursolPos;
    public AudioSource SelectSE;
    public StageCtrl stct;
    public Player p;
    public GameObject pauseCtr;
    public LoadScenes loadScenes;
    [HideInInspector] public bool activateParent = false;

    private Text txt;
    private Color beforeColor;
    private BlinkObject blinkObject;
    private int cursolNum = 0;
    private int firstPos;
    private int targetNum;
    private bool RightPushed;
    private Text targetUI;

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
    }
    // Update is called once per frame
    void Update()
    {
        if (targetNum != 1)
        {
            if (GameManager.instance.canContinue)//コンティニューできる場合
            {
                ContinueUI.enabled = true;
                gameOvertxt.enabled = false;
                targetUI = UIs[2];
                targetUI.enabled = false;
                targetUI = UIs[0];//これが現在選択中のUIになる。
                targetUI.enabled = true;
                firstPos = 0;
            }
            else//コンティニューできない場合
            {
                ContinueUI.enabled = false;
                gameOvertxt.enabled = true;
                targetUI = UIs[0];
                targetUI.enabled = false;
                targetUI = UIs[2];//これが現在選択中のUIになる。            
                targetUI.enabled = true;
                firstPos = 2;
            }
        }

        if (activateParent)//stageCtrでtrueにする。
        {
            GameManager.instance.continueWait = true;
            parent.SetActive(true);
            pauseCtr.SetActive(false);//コンティニュー画面中はポーズメニューが開かないようにする。
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
                if (cursolNum == 0 && Time.timeScale == 1)//最初の要素(continue)の場合
                {
                    //コンティニューする地点よりも格納されているコンティニュー地点の方が大きい場合
                    if (stct.continuePoint.Length > stct.continuePos)
                    {
                        if (!GameManager.instance.canContinue)
                        {
                            GameManager.instance.Retry = true;
                            loadScenes.startLoadStage1Scene();
                        }
                        else
                        {
                            stct.playerObj.transform.position = stct.continuePoint[stct.continuePos].transform.position;
                            p.ContinuePlayer();
                            pauseCtr.SetActive(true);
                            GameManager.instance.continueBehavior();
                        }
                    }
                    else
                    {
                        Debug.Log("コンティニューポイントの設定が足りていない");
                    }
                }
                else if (cursolNum == 1)//２つ目の要素(backToTitle)の場合
                {
                    GameManager.instance.goBackTitle = true;
                }
                GameManager.instance.continueWait = false;
                parent.SetActive(false);//何かが選択されたら、コンティニュー画面を終了する。
                activateParent = false;
            }
        }
    }

    private void changeUI()//点滅するUIを変更するメソッド
    {
        txt = targetUI.GetComponent<Text>();//現在のテキストコンポーネントを入手
        beforeColor = txt.color;
        txt.color = new Color(beforeColor.r, beforeColor.g, beforeColor.b, 1);//元の色に戻す
        blinkObject = targetUI.GetComponent<BlinkObject>();
        Debug.Log("beforetargetUI = " + targetUI);
        blinkObject.enabled = false;//現在のUIの点滅状態を解除
        cursolNum = RightPushed ? ++cursolNum : --cursolNum;//右矢印が押されたかで判断
        targetNum = RightPushed ? 1 : firstPos;  
        targetUI = UIs[targetNum];//次のオブジェクトを入手
        Debug.Log("aftertargetUI = " + targetUI);
        blinkObject = targetUI.GetComponent<BlinkObject>();
        blinkObject.enabled = true;//次のUIの点滅を開始
    }
}

