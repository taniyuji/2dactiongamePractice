using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Linq;

public class EnemyBehavior : BlinkObject
{
    public float BoundHeight;    //敵を踏んだときの跳ねる高さ
    public float enemySpeed;
    public float gravity;
    public int enemyHp = 1;
    public bool nonVisible = false;//見えてないときでも動かすか
    public bool beNonVisible = false;//一度見えたらずっと動くようにするか
    public bool DirectionRight;//右方向に動かすか
    public bool isJamp = false;
    public int myScore;
    public GroudCheck g;
    public bool isFly;
    public bool ofBoss;
    public List<GameObject> children;
    
    [HideInInspector] public bool playerStepOn = false; //敵を踏んだかどうか判断、インスペクターでは非表示
    [HideInInspector] public bool isGenerated = false;
    [HideInInspector] public bool isDead;

    private Animator anim = null;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private GameObject Rlim;
    private GameObject LLim;
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
    private bool blinkStart = false;
    private float arrivedTime;
    private AudioSource beStepSE;
    private GameObject boss;

    private void Start()
    {
        beStepSE = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Rlim = GameObject.Find("RightMoveLimit");
        LLim = GameObject.Find("LeftMoveLimit");
        if (ofBoss)
        {
            sr.enabled = false;
            children.ForEach(o => o.SetActive(false));
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
        if(boss != null && GameManager.instance.isBossDead)
        {
            playerStepOn = true;
            isDead = true;
        }

        if (ofBoss)//ボスの生成対象の場合
        {
            boss = GameObject.FindWithTag("Boss");
            /*
            if (boss != null)
            {
                Debug.Log("getBossObject");
            }
            else
            {
                Debug.Log("couldntGetBossObj");
            }
            */
            if (boss != null && !isGenerated)//まだ生成されていない場合
            {
                rb.isKinematic = true;
                var fixedBossPos = new Vector2(boss.transform.position.x, boss.transform.position.y - 10);
                rb.MovePosition(fixedBossPos);
            }
            else//生成された場合
            {
                rb.isKinematic = false;
                ofBoss = false;
            }
        }
        else//生成された場合またはボスのエネミーじゃない場合
        {
            if (!playerStepOn)//踏まれていない場合
            {
                //カメラに写っているかどうか（シーンビューに映る際も適応される）
                if (sr.isVisible || nonVisible)
                {
                    Move();
                    if (beNonVisible)//一度見えたら止まらないようにするか
                    {
                        nonVisible = true;
                    }
                }
                else
                {
                    rb.Sleep();
                }
            }
            else//踏まれた場合
            {
                if (!isDead)//死んでいない場合
                {
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.score += myScore;//スコアを足す
                    }
                    if (enemyHp > 0)
                    {
                        if (!isSet)
                        {
                            // Debug.Log("踏まれた");
                            beStepSE.Play();
                            enemyHp--;
                            blinkStart = false;
                            isSet = true;
                        }
                        if (isJamp)//死亡時、ジャンプエネミーだけ落下させたいため
                        {
                            rb.isKinematic = true;
                        }
                        else
                        {
                            SetInvincible();
                        }
                        if (!blinkStart && enemyHp > 0)
                        {
                            GetBlink(sr);
                            if (isBlinkFin())
                            {
                                blinkStart = true;
                                UnSetInvincible();
                                isSet = false;
                                playerStepOn = false;
                            }
                        }
                    }
                    if (enemyHp <= 0)
                    {
                        isDead = true;
                    }
                }
                else
                {
                    anim.SetBool("Defeated", true);
                    AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
                    if (isJamp)
                    {
                        rb.velocity = new Vector2(0, -gravity);
                        transform.Rotate(new Vector3(0, 0, 5));
                        Destroy(gameObject, 2f);
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, 0);
                        if (checkDefeatedAnim(currentState))
                        {
                            if (currentState.normalizedTime >= 1)
                            {
                                gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "player")
        {
            DirectionRight = !DirectionRight;
        }
    }

    //setActiveがオフのままだと生成されるまでボスについて行く挙動ができないため用意
    public void generateItSelf()
    {
        sr.enabled = true;
        children.ForEach(o => o.SetActive(true));
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

    

    private void getYVector()
    {
        if (isJamp)
        {
            g.IsGround();
            anim.SetBool("Run", true);
            if (judgeTime < 1f)
            {
                yVector = 4.5f;
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
        else if (g.IsGround() || isJamp)
        {
            anim.SetBool("Run", true);
            xVector = DirectionRight ? 1 : -1;
            transform.localScale = new Vector3(xVector * Math.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void getFlyingBehavior()
    {
        player = GameObject.Find("Player");
        if (player != null)
        {
            if (!posSet)
            {
                beforePos = transform.position;
                posSet = true;
            }
            if (judgeTime < 0.5f)
            {
                p = new Vector2(player.transform.position.x, player.transform.position.y + 1.5f);
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
                        arrivedTime = judgeTime;
                    }
                }
            }
            else if (isPlayerPos && judgeTime >= arrivedTime + 1f)
            {
                toVector = Vector2.MoveTowards(transform.position, beforePos, enemySpeed * Time.deltaTime);
                if (ComparePos(beforePos))
                {
                    judgeTime = 0.0f;
                    isPlayerPos = false;
                    arrivedTime = 0.0f;
                }
            }
            judgeTime += Time.deltaTime;
        }
    }
    private bool checkDefeatedAnim(AnimatorStateInfo currentState)
    {
        return currentState.IsName("BigEnemyDefeated")
            || currentState.IsName("UltraBigEnemyDefeated")
            || currentState.IsName("FlyingEnemyDefeated");
    }

    private bool ComparePos(Vector2 v)
    {
        return (int)transform.position.x == (int)v.x && (int)transform.position.y == (int)transform.position.y;
    }

    private void SetInvincible()
    {
        gameObject.layer = 14;
        children.ForEach(o => o.layer = 14);
    }

    private void UnSetInvincible()
    {
        gameObject.layer = 6;
        children.ForEach(o => o.layer = 6);
        children[1].layer = 15;
        
    }

}
