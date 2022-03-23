using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    public FadeScript fade;
    public GameObject player;
    public GameObject goBossPos;
    public bool isTitle = false;
    public bool isStage1 = false;
    public bool isBossScene = false;
    public AudioSource buttonSE;

    private bool isSet = false;
    private AsyncOperation asyncOperation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

    private void startLoadStage1Scene()
    {
        StartCoroutine("LoadStage1Scene");
    }

    IEnumerator LoadStage1Scene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Stage1-Forest-");
        asyncOperation.allowSceneActivation = false;
        fade.StartFadeOut();
        yield return new WaitForSeconds(1f);
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
        asyncOperation.allowSceneActivation = true;
    }
}
