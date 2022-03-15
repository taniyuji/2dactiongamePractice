using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseUI;
    public List<GameObject> UIs;
    private Image img;
    private BlinkObject blinkObject;

    private bool isPausing = false;
    private bool keyPushed = false;
    private int objNum;

    private void Start()
    {
        foreach (var i in UIs)
        {
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = false;
            objNum++;
        }
        blinkObject = UIs[0].GetComponent<BlinkObject>();
        blinkObject.enabled = true;
        objNum = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (!(objNum == UIs.Count - 1) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            img = UIs[objNum].GetComponent<Image>();
            img.color = new Color(1, 1, 1, 1);
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = false;
            objNum++;
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = true;
        }
        else if (!(objNum == 0) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            img = UIs[objNum].GetComponent<Image>();
            img.color = new Color(1, 1, 1, 1);
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = false;
            objNum--;
            blinkObject = UIs[objNum].GetComponent<BlinkObject>();
            blinkObject.enabled = true;
        }

        if (!GameManager.instance.goBackTitle)
        {
            if (isPausing)
            {
                pauseUI.SetActive(true);
                Time.timeScale = 0.01f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (objNum == 0)
                    {
                        isPausing = false;
                        keyPushed = true;
                    }
                    else if (objNum == 1)
                    {
                        BackToTiTle();
                        keyPushed = true;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isPausing = true;
                }
                pauseUI.SetActive(false);
                Time.timeScale = 1f;
            }
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void PauseGame()
    {
        isPausing = true;
    }

    public void ReturnToGame()
    {
        isPausing = false;
    }

    public void BackToTiTle()//GameManagaerスクリプト側でフェードとシーンを移動
    {
        if (GameManager.instance.goBackTitle)
        {
            return;
        }
        isPausing = false;
        GameManager.instance.goBackTitle = true;
    }
}
