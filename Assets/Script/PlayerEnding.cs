using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnding : MonoBehaviour
{
    public float speed;
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

    // Start is called before the first frame update
    void Start()
    {
        xspeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.x < startEndPos.transform.position.x)
        {
            playerIn = true;
            anim.SetBool("walk", false);
            if (!isset)
            {
                speed = 0;
                isset = true;
            }
            if(youngSister.transform.position.y < gameObject.transform.position.y + 1.5 && waitTime < 3f)
            {
                anim.SetBool("chach", true);
                waitTime += Time.deltaTime;
            }else if (waitTime >= 3f)
            {
                anim.SetBool("withSis", true);
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
                if (walkSE != null)
                {
                    walkSE.Play();
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
                }
                speed = 0.0f;
            }
        }
        //Debug.Log("playerXspeed = " + xspeed);
        gameObject.transform.localScale = new Vector3(xVector, 1, 1);
        rb.velocity = new Vector2(speed, -1);
    }
    
}
