using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public bool firstFadeInComp;//最初からフェードインを終了させておくか
    private Image img = null;
    private int frameCount = 0;
    private float timer = 0.0f;
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
        timer = 0.0f;
        img.color = new Color(1, 1, 1, 1);//１は白を表す
        img.fillAmount = 1;
        img.raycastTarget = true;//フェード中にボタンを押されないように
    }

    public bool IsFadeInComplete()//フェードインが終了しているか
    {
        return compFadeIn;
    }

    public void StartFadeOut()//フェードアウトを始める
    {
        if (fadeIn || fadeOut)
        {
            return;
        }
        fadeOut = true;
        compFadeOut = false;
        timer = 0.0f;
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 0;
        img.raycastTarget = true;
    }

    public bool IsFadeOutComplete()//フェードアウトが終了しているか
    {
        return compFadeOut;
    }
    
    void Start()
    {
        img = GetComponent<Image>();

        if (firstFadeInComp)//フェードインがいらない場合
        {
            FadeInComplete();
        }
        else
        {
            StartFadeIn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(frameCount > 2)//シーンロード中は思いため、2フレーム待ってから実行
        {
            if (fadeIn)//フェードインが開始している場合
            {
                FadeInUpdate();
            }else if (fadeOut)//フェードアウトが開始している場合
            {
                FadeOutUpdate();
            }
        }
        ++frameCount;    
    }

    private void FadeInUpdate()//フェードイン機能
    {
        if (timer < 1f)
        {
            img.color = new Color(1, 1, 1, 1 - timer);
            img.fillAmount = 1 - timer;
        }
        else
        {
            FadeInComplete();
        }
        timer += Time.deltaTime;
    }

    private void FadeOutUpdate()//フェードアウト機能
    {
        if(timer < 1f)//１秒でフェードさせる
        {
            img.color = new Color(1, 1, 1, timer);
            img.fillAmount = timer;
        }
        else
        {
            FadeOutComplete();
        }
        timer += Time.deltaTime;
    }

    private void FadeInComplete()//フェードインが終了
    {
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 0;
        img.raycastTarget = false;
        timer = 0.0f;
        fadeIn = false;
        compFadeIn = true;
    }

    private void FadeOutComplete()//フェードアウトが終了
    {
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 1;
        img.raycastTarget = false;
        timer = 0.0f;
        fadeIn = false;
        compFadeOut = true;
    }
}
