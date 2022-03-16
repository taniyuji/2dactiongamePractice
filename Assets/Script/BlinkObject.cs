using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkObject : MonoBehaviour
{
    public bool isUI = false;

    private bool isBlinkfin;
    private float blinkTime = 0.0f;
    private float continueTime;
    private Text txt;
    private Image img;
    private Color color;
    private bool PlusTime = true;

    private void Start()
    {
        if (isUI)
        {
            txt = gameObject.GetComponent<Text>();
            img = gameObject.GetComponent<Image>();
            if (txt != null)
            {
                color = txt.color;
            }
            else
            {
                color = img.color;
            }
        }
    }
    private void Update()
    {
        if (isUI)
        {
            BlinkUI();
        }
    }

    public void BlinkUI()
    {
        if (blinkTime < 1f)
        {
            float subTime = PlusTime ? blinkTime : 1 - blinkTime;
            if (txt != null)
            {
                txt.color = new Color(color.r, color.g, color.b, subTime);
            }
            else
            {
                img.color = new Color(color.r, color.g, color.b, subTime);
            }
        }
        else
        {
            PlusTime = !PlusTime;
            blinkTime = 0.0f;
        }
        blinkTime += Time.unscaledDeltaTime;
    }
    public void GetBlink(SpriteRenderer sr)//点滅消滅
    {
        isBlinkfin = false;
        //0.2秒以降は、人通りすべてのif条件を通るように
        if (blinkTime > 0.2f)
        {
            sr.enabled = true;//スプライトーレンダラーを表示
            blinkTime = 0.0f;//ここでリセット
        }
        else if (blinkTime > 0.1f && blinkTime <= 1.0f)
        {
            sr.enabled = false;//スプライトーレンダラーを非表示
        }
        else
        {
            sr.enabled = true;
        }

        if (continueTime > 1.0f)//リスポーン表現の時間が1秒より大きくなった場合
        {
            blinkTime = 0f;
            continueTime = 0f;
            sr.enabled = true;
            isBlinkfin = true;
        }
        else
        {
            blinkTime += Time.deltaTime;
            continueTime += Time.deltaTime;
        }
    }

    public bool isBlinkFin()
    {
        return isBlinkfin;
    }
}
