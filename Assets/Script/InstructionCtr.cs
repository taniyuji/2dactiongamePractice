using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionCtr : MonoBehaviour
{
    [SerializeField]
    private Player p;

    [SerializeField]
    private List<GameObject> instructPos;

    [SerializeField]
    private GameObject finInstructPos;

    [SerializeField]
    private List<TextMeshProUGUI> texts;

    private int idxNum = 0;
    private int beforeNum = 0;
    private float colorNum;
    private bool plusTime = true;
    private bool isSet = false;
    // Start is called before the first frame update
    void Start()
    {
        texts[0].enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))//ポーズ中は起動させない
        {
            return;
        }

        if (idxNum == texts.Count - 1 && beforeNum == idxNum)
            gameObject.SetActive(false);

        float pXPos = p.transform.position.x;
        float instructPosX = -1;
        if (idxNum < instructPos.Count)
                instructPosX = instructPos[idxNum].transform.position.x;
        

        if (pXPos < instructPosX)
        {
            //Debug.Log("change idxNum");
            texts[idxNum].enabled = false;
            idxNum++;
            texts[idxNum].enabled = true;
        }

        if (idxNum == beforeNum)
            return;

        Color color = texts[idxNum].color;
        texts[idxNum].color = new Color(color.r, color.g, color.b, colorNum);

        colorNum = plusTime ? colorNum + Time.deltaTime : colorNum - Time.deltaTime;
        //Debug.Log(colorNum);

        if (colorNum > 0.8 && !isSet) 
        {
            isSet = true;
            StartCoroutine(turnMinus(1.5f));
        }

        if(colorNum < 0)
        {
            //Debug.Log("finish changingColor");
            plusTime = true;
           // Debug.Log("set true");
            beforeNum = idxNum;
            colorNum = 0;
            isSet = false;
        }
    }

    private IEnumerator turnMinus(float s)
    {
        yield return new WaitForSeconds(s);
       // Debug.Log("set false");
        plusTime = false;
    }
}
