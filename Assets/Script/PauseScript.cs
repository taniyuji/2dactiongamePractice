using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class PauseScript : MonoBehaviour
{
    public GameObject player;
    public GameObject pauseUI;
    public GameObject cursol;
    public List<GameObject> UIs;
    public List<GameObject> CursolPos;
    public AudioSource SelectSE;
    [HideInInspector] public bool goback = false;

    private Image img;
    private Color BeforeColor;
    private BlinkObject blinkObject;
    private bool isPausing = false;
    private int objNum;
    private List<GameObject> skippedUIs;
    private bool DownAllowPushed;

    private void Start()
    {
        skippedUIs = UIs.Skip(1)
                        .ToList();
        skippedUIs.ForEach(skippedUIs =>
        {
            blinkObject = skippedUIs.GetComponent<BlinkObject>();
            blinkObject.enabled = false;
        });
        img = UIs[0].GetComponent<Image>();
        BeforeColor = img.color;
        cursol.transform.position = CursolPos[0].transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if (!goback)
        {
            if (!(objNum == UIs.Count - 1) && Input.GetKeyDown(KeyCode.DownArrow))
            {
                DownAllowPushed = true;
                changeUI();
            }
            else if (!(objNum == 0) && Input.GetKeyDown(KeyCode.UpArrow))
            {
                DownAllowPushed = false;
                changeUI();
            }
            cursol.transform.position = CursolPos[objNum].transform.position;

            if (isPausing)
            {
                pauseUI.SetActive(true);
                Time.timeScale = 0f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (SelectSE != null)
                    {
                        SelectSE.Play();
                    }
                    if (objNum == 0)
                    {
                        isPausing = false;
                    }
                    else if (objNum == 1)
                    {
                        Debug.Log("?????????????????????");
                        BackToTiTle();
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.X))
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

    public void BackToTiTle()//GameManagaer??????????????????????????????????????????????????????
    {
        player.SetActive(false);
        isPausing = false;
        GameManager.instance.goBackTitle = true;
        goback = true;
    }

    private void changeUI()
    {
        img = UIs[objNum].GetComponent<Image>();
        img.color = BeforeColor;
        blinkObject = UIs[objNum].GetComponent<BlinkObject>();
        blinkObject.enabled = false;
        objNum = DownAllowPushed ? ++objNum : --objNum;
        blinkObject = UIs[objNum].GetComponent<BlinkObject>();
        blinkObject.enabled = true;
    }
}
