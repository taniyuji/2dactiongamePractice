using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkObject : MonoBehaviour
{
    [HideInInspector] public bool isBlink = false;
    [HideInInspector] public bool isBlinkFin = false;
    public static BlinkObject instance = null;
    private float blinkTime;
    private float continueTime;
    

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void blinkObject(SpriteRenderer sr)//点滅消滅
    {
        isBlink = true;
        //0.2秒以降は、人通りすべてのif条件を通るように
        if (blinkTime > 0.2f)
        {
            sr.enabled = true;//スプライトーレンダラーを表示
            blinkTime = 0.0f;
        }
        else if (blinkTime > 0.1f)
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
            isBlink = false;
            isBlinkFin = true;
        }
        else
        {
            blinkTime += Time.deltaTime;
            continueTime += Time.deltaTime;
        }
    }
}
