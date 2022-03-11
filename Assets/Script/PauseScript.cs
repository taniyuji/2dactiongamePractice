using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseUI;

    private bool isPausing = false;
    // Update is called once per frame
    void Update()
    {
        if (isPausing)
        {
            pauseUI.SetActive(true);

            Time.timeScale = 0.01f;
        }
        else
        {
            pauseUI.SetActive(false);
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

    public void BackToTiTle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
