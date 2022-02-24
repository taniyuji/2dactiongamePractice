using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class EnemyBehavior : MonoBehaviour
{
    public float BoundHeight;    //敵を踏んだときの跳ねる高さ
    public float enemySpeed;
    public float gravity;
    public int DirectionChangeCount;//移動方向を変更するまでの時間
    public bool nonVisible = false;//見えてないときでも動かすか
    public bool DirectionRight = true;//右方向に動かすか
    public bool isJamp = false;
    public int myScore;
    public GroudCheck g;
    public bool isFly;
    [HideInInspector]public bool playerStepOn = false; //敵を踏んだかどうか判断、インスペクターでは非表示

    private int count2;
    private Animator anim = null;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private GameObject[] children;
    private GameObject Rlim;
    private GameObject LLim;
    private bool isDead;
    private bool posSet = false;
    private Vector2 beforePos;
    private float xVector;
    private float yVector;
    private float judgeTime;
    private bool isJamping = false;
    private GameObject player;
    private Vector2 p;
    private Vector2 toVector;

    private void Start()
    {
        anim = GetComponent<Animator>();
        count2 = DirectionChangeCount;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        getAllChildren();
        Rlim = GameObject.Find("RightMoveLimit");
        LLim = GameObject.Find("LeftMoveLimit");
        if (isJamp)
        {
            isFly = false;
        }else if (isFly)
        {
            isJamp = false;
        }

        if (isJamp)
        {
            isJamping = true;
        }
    }

    void FixedUpdate()
    {
        if (playerStepOn == false)
        {
            //カメラに写っているかどうか（シーンビューに映る際も適応される）
            if (sr.isVisible || nonVisible)
            {
                Move();
            }
            else
            {
                rb.Sleep();
            }
        }
        else//踏まれた場合
        {
            if (!isDead)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.score += myScore;
                }
                isDead = true;
                anim.SetBool("Defeated", true);
                rb.velocity = new Vector2(0, -gravity);
                foreach (var i in children)//スプライトがあるため、親オブジェクトだけ消したくない
                {
                    i.SetActive(false);
                }
                Destroy(gameObject, 3f);
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, 5));
            }
        }
    }

    private void Move() //敵キャラの動き
    {
        getYVector();
        getXVector();
        if (isFly == true)
        {
            rb.MovePosition(toVector);
        }
        else
        {
            rb.velocity = new Vector2(xVector * enemySpeed, yVector);
        }
    }

    private void getAllChildren()
    {
        children = new GameObject[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            children[i] = gameObject.transform.GetChild(i).gameObject;
        }
    }

    private void getYVector()
    {
        if (isJamp)
        {
            g.IsGround();
            anim.SetBool("Run", true);
            if (judgeTime < 1f)
            {
                yVector = 5f;
            }
            else if (judgeTime >= 1f && g.IsGround() == false)
            {
                yVector = -gravity;
            }
            else if (g.IsGround() == true)
            {
                judgeTime = 0.0f;
            }
            judgeTime += Time.deltaTime;
        }
        else if (isFly)
        {

            player = GameObject.Find("Player");
            if (!posSet)
            {
                beforePos = transform.position;
                posSet = true;
            }
            if (judgeTime < 1.5f)
            {
                if (player.activeSelf == true)
                {
                    p = player.transform.position;
                    toVector = Vector2.MoveTowards(transform.position, p, enemySpeed * Time.deltaTime);
                }
            }
            else if (judgeTime >= 1.5f && judgeTime < 2.5f)
            {
                toVector = Vector2.MoveTowards(transform.position, beforePos, enemySpeed * Time.deltaTime);
            }
            else
            {
                judgeTime = 0.0f;
            }
            judgeTime += Time.deltaTime;
        }
        else//飛行もジャンプもしない場合
        {
            yVector = -gravity;
        }
    }

    private void getXVector()
    {
        if (isJamp == false && isFly == false && g.IsGround() == false)
        {
            anim.SetBool("Run", false);
            xVector = 0;
        }
        else if (g.IsGround() == true)
        {
            anim.SetBool("Run", true);
            if (DirectionRight)//動く方向が右方向の場合
            {

                xVector = 1;
                transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y);
                count2--;
                if (count2 == 0 || transform.position.x >= Rlim.transform.position.x)//移動方向を転換
                {
                    count2 = 0;
                    DirectionRight = false;
                }
            }
            else//動く方向が左方向の場合
            {
                xVector = -1;
                transform.localScale = new Vector2(-(Math.Abs(transform.localScale.x)), transform.localScale.y);
                count2++;
                if (count2 == DirectionChangeCount || transform.position.x <= LLim.transform.position.x)//移動方向を転換
                {
                    count2 = DirectionChangeCount;
                    DirectionRight = true;
                }
            }
        }
    }
}
