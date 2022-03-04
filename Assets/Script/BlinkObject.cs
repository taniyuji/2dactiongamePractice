using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkObject : MonoBehaviour
{
    private bool isBlinkfin;
    private float blinkTime;
    private float continueTime;

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
