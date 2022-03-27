using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    public GameObject fadeObj;
    public GameObject player;
    public GameObject goBossPos;
    public bool isTitle = false;
    public bool isStage1 = false;
    public bool isBossScene = false;
    public AudioSource buttonSE;

    private bool isSet = false;
    private FadeScript fade;
    private AsyncOperation asyncOperation;

    private void Awake()
    {
        fadeObj.SetActive(true);
        fade = fadeObj.GetComponent<FadeScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!fade.IsFadeInComplete())
        {
            return;
        }

        if (isTitle)
        {
            if (Input.anyKey && !isSet)
            {
                Debug.Log("起動開始");
                buttonSE.Play();
                startLoadStage1Scene();
                isSet = true;
            }
        }

        if (isStage1)
        {
            if (!isSet)
            {
                if (player.transform.position.x < goBossPos.transform.position.x)
                {
                    Debug.Log("ボス戦へ");
                    startLoadBoss1Scene();
                    isSet = true;
                }
            }
        }

        if(isStage1 || isBossScene)
        {

            if (GameManager.instance.goBackTitle)
            {
                startLoadTitleScene();
                GameManager.instance.goBackTitle = false;
            }
        }

        if (isBossScene && GameManager.instance.isBossDead)
        {
            if (!isSet)
            {
                startLoadEndingScene();
                isSet = true;
            }
        }

    }

    private void startLoadBoss1Scene()
    {
        StartCoroutine("LoadBoss1Scene");
    }

    IEnumerator LoadBoss1Scene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Boss1");
        asyncOperation.allowSceneActivation = false;
        fade.StartFadeOut();
        yield return new WaitForSeconds(2f);
        asyncOperation.allowSceneActivation = true;
    }

    public void startLoadStage1Scene()
    {
        StartCoroutine("LoadStage1Scene");
    }

    IEnumerator LoadStage1Scene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Stage1-Forest-");
        asyncOperation.allowSceneActivation = false;
        fade.StartFadeOut();
        yield return new WaitForSeconds(1f);
        if (!GameManager.instance.canContinue)
        {
            GameManager.instance.ResetPram();
        }
        asyncOperation.allowSceneActivation = true;
    }

    private void startLoadTitleScene()
    {
        StartCoroutine("LoadTitleScene");
    }

    IEnumerator LoadTitleScene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("TitleScene");
        asyncOperation.allowSceneActivation = false;
        fade.StartFadeOut();
        yield return new WaitForSeconds(2f);
        GameManager.instance.ResetPram();
        asyncOperation.allowSceneActivation = true;
    }

    private void startLoadEndingScene()
    {
        StartCoroutine("LoadEndingScene");
    }

    IEnumerator LoadEndingScene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Ending");
        asyncOperation.allowSceneActivation = false;
        fade.StartFadeOut();
        yield return new WaitForSeconds(2f);
        asyncOperation.allowSceneActivation = true;
    }
}
