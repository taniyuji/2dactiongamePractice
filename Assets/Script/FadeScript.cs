using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    [HideInInspector] public bool goLoadScene = false;
    public bool firstFadeInComp;//最初からフェードインを終了させておくか
    private Image img = null;
    private bool fadeIn = false;
    private bool compFadeIn = false;
    private bool fadeOut = false;
    private bool compFadeOut = false;

    public void StartFadeIn()//フェードインを始める
    {
        //フェードイン、アウトの最中だった場合
        if(fadeIn || fadeOut)
        {
            return;
        }
        fadeIn = true;
        compFadeIn = false;
        img.raycastTarget = true;//フェード中にボタンを押されないように
        StartFadeInUpdate();
    }

    public bool IsFadeInComplete()//フェードインが終了しているか
    {
        return compFadeIn;
    }

    public void StartFadeOut()//フェードアウトを始める
    {
        if (fadeIn || fadeOut)
        {
            Debug.Log("fadeOut中断");
            return;
        }
        //Debug.Log("StartFadeOut");
        img.enabled = true;
        fadeOut = true;
        compFadeOut = false;
        img.raycastTarget = true;
        StartFadeOutUpdate();
    }

    public bool IsFadeOutComplete()//フェードアウトが終了しているか
    {
        return compFadeOut;
    }
    
    void Start()
    {
        img = GetComponent<Image>();
        StartFadeIn();
        goLoadScene = false;
    }

    /*
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))//ポーズ中は起動させない
        {
            return;
        }
        if (frameCount > 2)//シーンロード中は思いため、2フレーム待ってから実行
        {
            if (fadeIn)//フェードインが開始している場合
            {
                StartCoroutine(FadeInUpdate());
            }else if (fadeOut)//フェードアウトが開始している場合
            {
                StartCoroutine(FadeOutUpdate());
            }
        }
        ++frameCount;    
    }
    */

    private void StartFadeInUpdate()
    {
        StartCoroutine(FadeInUpdate());
    }

    private void StartFadeOutUpdate()
    {
        StartCoroutine(FadeOutUpdate());
    }

    IEnumerator FadeInUpdate()
    {
        for(decimal i = 0m; i <= 1m; i += 0.01m)
        {
            decimal g = i;
            img.color = new Color(1, 1, 1, 1 - (float)g);
            //Debug.Log("FadeInが" + img.color + "進んでます");
            if(i == 1m)
            {
                //Debug.Log("終わり");
                FadeInComplete();
            }
            yield return null;
        }  
    }

    IEnumerator FadeOutUpdate()
    {
        for (decimal i = 0m; i <= 1m; i += 0.01m)
        {
            decimal g = i;
            img.color = new Color(1, 1, 1, (float)g);
            //Debug.Log("FadeOutが" + img.color + "進んでます");
            if (i == 1m)
            {
                //Debug.Log("終わり");
                FadeOutComplete();
            }
            yield return null;
        }

    }

    private void FadeInComplete()//フェードインが終了
    {
        
        img.color = new Color(1, 1, 1, 0);
        img.raycastTarget = false;
        fadeIn = false;
        compFadeIn = true;
    }

    private void FadeOutComplete()//フェードアウトが終了
    {
        
        img.color = new Color(1, 1, 1, 1);
        img.raycastTarget = false;
        fadeIn = false;
        compFadeOut = true;
    }
}
