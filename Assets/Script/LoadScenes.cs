using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    public GameObject player;
    public GameObject goBossPos;
    public FadeScript fade;
    public bool isTitle = false;
    public bool isStage1 = false;
    public bool isBossScene = false;
    public bool isEnding = false;
    public bool isResult = false;
    public AudioSource buttonSE;

    private bool isSet = false;
    private bool setbool = false;
    private AsyncOperation asyncOperation;

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

        if (isStage1 && !isSet)
        {
            if (!setbool)
            {
                GameManager.instance.startPlayTime = true;
                setbool = true;
            }
            if (player.transform.position.x < goBossPos.transform.position.x)
            {
                Debug.Log("ボス戦へ");
                startLoadBoss1Scene();
                isSet = true;
            }
        }

        if((isStage1 || isBossScene) && GameManager.instance.goBackTitle && !isSet)
        {
            startLoadTitleScene();
            GameManager.instance.startbackTitle = true;
            isSet = true;
        }

        if (isBossScene && GameManager.instance.isBossDead && !isSet)
        {
            startLoadEndingScene();
            isSet = true;
        }

        if(isEnding && GameManager.instance.LogoAppeared && !isSet)
        {
            startLoadResultScene();
            isSet = true;
        }

        if(isResult && Input.GetKeyDown(KeyCode.Space) && !isSet)
        {
            startLoadTitleScene();
            isSet = true;
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

    private void startLoadResultScene()
    {
        StartCoroutine("LoadResultScene");
    }

    IEnumerator LoadResultScene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Result");
        asyncOperation.allowSceneActivation = false;
        fade.StartFadeOut();
        yield return new WaitForSeconds(2f);
        asyncOperation.allowSceneActivation = true;
    }
}
