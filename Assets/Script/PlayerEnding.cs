using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnding : MonoBehaviour
{
    
    public GameObject floatSE;
    public GameObject moveLim;
    public float speed;
    public AudioSource catchSE;
    public AudioSource walkSE;
    public Rigidbody2D rb;
    public Animator anim;
    public GameObject startEndPos;
    public GameObject youngSister;
    [HideInInspector] public bool dontFollow = false;
    [HideInInspector] public bool playerIn = false;

    private int xVector = 1;
    private float xspeed;
    private float waitTime;
    private bool isset = false;
    private bool stoppedWalk = false;
    private bool setCatch = false;

    // Start is called before the first frame update
    void Start()
    {
        xspeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))//pause中は起動させない
        {
            if (walkSE != null)
            {
                walkSE.Pause();
                stoppedWalk = true;
            }
            return;
        }
        if (gameObject.transform.position.x < startEndPos.transform.position.x)
        {
            playerIn = true;
            anim.SetBool("walk", false);
            if (!isset)
            {
                walkSE.Pause();
                stoppedWalk = true;
                speed = 0;
                isset = true;
            }
            if (youngSister.transform.position.y < gameObject.transform.position.y + 2 && waitTime < 3f)
            {
                anim.SetBool("chach", true);
                if (!setCatch)
                {
                    //Debug.Log("きゃっちSEPlay");
                    catchSE.Play();
                    setCatch = true;
                }
                waitTime += Time.deltaTime;
            }
            else if (waitTime >= 3f)
            {
                if (waitTime < 3.5f)
                {
                    anim.SetBool("withSis", true);
                    if (stoppedWalk)
                    {
                        walkSE.Play();
                        stoppedWalk = false;
                    }
                    floatSE.SetActive(false);
                    waitTime += Time.deltaTime;
                }
                else
                {
                    walkSE.volume -= 0.001f;
                }
                speed = -xspeed + 2;
                dontFollow = true;
            }
        }
        else
        {
            //定義
            float horizontalkey = Input.GetAxisRaw("Horizontal") * Time.deltaTime;

            if (horizontalkey != 0)//入力がある場合
            {
                anim.SetBool("walk", true);
                if (walkSE != null && stoppedWalk)
                {
                    walkSE.Play();
                    stoppedWalk = false;
                }
                xVector = horizontalkey > 0 ? -1 : 1;//ここで右か左かを判断
                                                     //スプライトの向きと進む方向があべこべになってしまっているため
                speed = -xVector * xspeed;
            }//入力がない場合
            else
            {
                anim.SetBool("walk", false);
                if (walkSE != null)
                {
                    walkSE.Pause();
                    stoppedWalk = true;
                }
                speed = 0.0f;
            }
        }
        //Debug.Log("playerXspeed = " + xspeed);
        gameObject.transform.position = new Vector3(Mathf.Clamp(gameObject.transform.position.x, -100, moveLim.transform.position.x), gameObject.transform.position.y, gameObject.transform.position.z);
        gameObject.transform.localScale = new Vector3(xVector, 1, 1);
        rb.velocity = new Vector2(speed, -1);
    }
    
}
