using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseUI;

    private bool isPausing = false;
    private bool keyPushed = false;
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.goBackTitle)
        {
            if (isPausing)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    isPausing = false;
                    keyPushed = true;
                }else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    BackToTiTle();
                    keyPushed = true;
                }
                pauseUI.SetActive(true);

                Time.timeScale = 0.01f;
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
