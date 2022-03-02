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
    public int enemyHp = 1;
    public bool nonVisible = false;//見えてないときでも動かすか
    public bool DirectionRight;//右方向に動かすか
    public bool isJamp = false;
    public int myScore;
    public GroudCheck g;
    public bool isFly;
    public bool ofBoss;
    public GameObject boss;
    [HideInInspector]public bool playerStepOn = false; //敵を踏んだかどうか判断、インスペクターでは非表示
    [HideInInspector]public bool isGenerated = false;

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
    private GameObject player;
    private Vector2 p;
    private Vector2 toVector;
    private bool isPlayerPos = false;
    private bool isSet = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        getAllChildren();
        Rlim = GameObject.Find("RightMoveLimit");
        LLim = GameObject.Find("LeftMoveLimit");
        if (ofBoss)
        {
            sr.enabled = false;
            foreach(var i in children)
            {
                i.SetActive(false);
            }
        }
        //飛行とジャンプを両方入力してしまった場合の修正
        if (isFly) 
        {
            isJamp = false;
        }

        if (isJamp)
        {
            isFly = false;
        }

    }

    void FixedUpdate()
    {
        if (ofBoss)
        {
            if (!isGenerated)
            {
                Debug.Log(boss);
                var fixedBossPos = new Vector2(boss.transform.position.x, boss.transform.position.y - 21);
                rb.MovePosition(fixedBossPos);
            }
            else
            {
                ofBoss = false;
            }
        }
        if (!playerStepOn)
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
                if (enemyHp > 0)
                {
                    if (!isSet)
                    {
                        enemyHp -= 1;
                        isSet = true;
                    }
                    rb.isKinematic = true;
                    BlinkObject.instance.blinkObject(sr);
                    if (BlinkObject.instance.isBlink)
                    {
                        foreach (var i in children)//スプライトがあるため、親オブジェクトだけ消したくない
                        {
                            i.SetActive(false);
                        }
                    }
                    else
                    {
                        foreach (var i in children)
                        {
                            i.SetActive(true);
                        }
                        rb.isKinematic = false;
                        playerStepOn = false;
                        isSet = false;
                    }
                }
                else
                {
                    isDead = true;
                    anim.SetBool("Defeated", true);
                    rb.velocity = new Vector2(0, -gravity);
                    foreach (var i in children)//スプライトがあるため、親オブジェクトだけ消したくない
                    {
                        i.SetActive(false);
                    }
                    Destroy(gameObject, 3f);
                }
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, 5));
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "player")
        {
            DirectionRight = !DirectionRight;
        }
    }
    public void generateItSelf()
    {
        sr.enabled = true;
        foreach(var i in children)
        {
            i.SetActive(true);
        }
        isGenerated = true;
    }

    private void Move() //敵キャラの動き
    {
        if (isFly)
        {
            getFlyingBehavior();
            rb.MovePosition(toVector);
        }
        else
        {
            getYVector();
            getXVector();
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
            else if (judgeTime >= 1f && !g.IsGround())
            {
                yVector = -gravity;
            }
            else if (g.IsGround())
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
        if (!isJamp && !isFly && !g.IsGround())
        {
            anim.SetBool("Run", false);
            xVector = 0;
        }
        else if (g.IsGround())
        {
            anim.SetBool("Run", true);
            if (DirectionRight)//動く方向が右方向の場合
            {
                xVector = 1;
                transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y);
            }
            else//動く方向が左方向の場合
            {
                xVector = -1;
                transform.localScale = new Vector2(-(Math.Abs(transform.localScale.x)), transform.localScale.y);
            }
        }
    }

    private void getFlyingBehavior()
    {
        player = GameObject.Find("Player");
        if (!posSet)
        {
            beforePos = transform.position;
            posSet = true;
        }
        if (judgeTime < 0.5f)
        {
            p = player.transform.position;
            toVector = transform.position;
        }
        else if (judgeTime >= 0.5f && !isPlayerPos)
        {
            if (p != null)
            {
                toVector = Vector2.MoveTowards(transform.position, p, enemySpeed * Time.deltaTime);
                if (ComparePos(p))
                {
                    isPlayerPos = true;
                }
            }
        }
        else if (isPlayerPos)
        {
            toVector = Vector2.MoveTowards(transform.position, beforePos, enemySpeed * Time.deltaTime);
            if (ComparePos(beforePos))
            {
                judgeTime = 0.0f;
                isPlayerPos = false;
            }
        }
        judgeTime += Time.deltaTime;
    }

    private bool ComparePos(Vector2 v)
    {
        return (int)transform.position.x == (int)v.x && (int)transform.position.y == (int)transform.position.y;
    }
}
