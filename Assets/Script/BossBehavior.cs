using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BossBehavior : MonoBehaviour
{
    public float BoundHeight;    //敵を踏んだときの跳ねる高さ
    public float enemySpeed;
    public float gravity;
    public bool nonVisible = false;//見えてないときでも動かすか
    public int myScore;
    public int bossHp;
    public int stopWidth = 25;
    public List<GameObject> enemies;
    public EnemyCollisionCheck enc;
    [HideInInspector] public bool hitGround = false;
    [HideInInspector] public bool isAttack = false;
    [HideInInspector] public bool isGenerating = false;

    [HideInInspector] public bool playerStepOn2 = false; //敵を踏んだかどうか判断、インスペクターでは非表示
    private int count2;
    private Animator anim = null;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isDead;
    private bool isBlink = false;
    private bool isStamped = false;
    private float blinkTime = 0.0f;
    private float continueTime = 0.0f;
    private GameObject player;
    private int xVector;
    private float beforeSpeed = 1;
    private bool moveRight = false;
    private int attackNum;
    private bool AttackAnimFin = false;
    private float time = 0f;
    private GameObject RLimitObj;
    private GameObject LLimitObj;
    private bool playerOnRight = true;
    private bool playerHit = false;
    private float backTime = 0.0f;
    private bool inBoundary = false;
    private int xLocalScale;
    private int generateTime = 0;
    private int CountGenerate = 0;
    private float generatingTime = 0.0f;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        RLimitObj = GameObject.Find("RightMoveLimit");
        LLimitObj = GameObject.Find("LeftMoveLimit");
        beforeSpeed = enemySpeed;
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        if (isStamped)
        {
            if (!BlinkObject.instance.isBlinkFin)
            {
                BlinkObject.instance.blinkObject(sr);
            }
            else
            {
                if (bossHp > 0)
                {
                    if ((CountGenerate == 0 && generateTime == 0) || (CountGenerate == 1 && generateTime == 5))
                    {
                        isGenerating = true;
                    }

                    if ((bossHp == 8 || bossHp == 4) && isGenerating)
                    {
                        Debug.Log("はいった");
                        enemySpeed = 0;
                        GenerateEnemy();
                    }
                    else
                    {
                        enemySpeed = beforeSpeed;
                        anim.SetBool("Generate", false);
                        CountGenerate++;
                        playerStepOn2 = false;
                        BlinkObject.instance.isBlinkFin = false;
                        isStamped = false;
                    }
                }
                else
                {
                    setBossDead();
                }
            }
        }
       
        
    }

    void FixedUpdate()
    {
        if (!playerStepOn2)
        {
            //カメラに写っているかどうか（シーンビューに映る際も適応される）
            if (sr.isVisible || nonVisible)
            {
                GameManager.instance.bossIsvisble = true;
                judgeMoveDir();
                Move();
                if (hitGround == false)
                {
                    bossAttack();
                }
            }
            else
            {
                rb.Sleep();
            }
        }
        else//踏まれた場合
        {
            if (!isAttack && !isGenerating && !isStamped)
            {
                bossHp -= 1;
                isStamped = true;
            }
            if (!isGenerating)
            {
                Move();
            }
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, LLimitObj.transform.position.x, RLimitObj.transform.position.x), transform.position.y, transform.position.z);
        rb.velocity = new Vector2(xVector * enemySpeed, -gravity);
    }

    private void judgeMoveDir()//動く方向を判断
    {
        var leftBossStopWidth = transform.position.x - stopWidth;//左側の境界値
        var rightBossStopWidth = transform.position.x + stopWidth;//右側の境界値
        float pPos = player.transform.position.x;

        if(pPos < leftBossStopWidth || pPos > rightBossStopWidth)//プレイヤーが境界より外側にいた場合
        {
            inBoundary = false;
            moveRight = pPos > rightBossStopWidth;
        }
        else//両境界の内側にいた場合
        {
            inBoundary = true;
        }
    }
      ///メソッドたち///
    private void Move() //敵キャラの動き
    {
        if (playerHit && !isAttack)//プレイヤーと衝突し、攻撃中ではない場合
        {
            if ((!playerStepOn2 && backTime < 0.3f) || (playerStepOn2 && backTime < 0.6f))
            {
                enemySpeed += 2f;
                xVector = moveRight ? -1 : 1;
            }
            else
            {
                backTime = 0.0f;
                playerHit = false;
                enemySpeed = beforeSpeed;
            }
            backTime += Time.deltaTime;
        }
        else if(!isAttack && !playerHit)//プレイヤーと衝突しておらず、攻撃中でない場合
        {
            xLocalScale = 1;
            if (!moveRight)//プレイヤーが左側にいる場合
            {
                anim.SetBool("Run", true);
                xVector = -1;
                xLocalScale = -1;
            }
            else if(moveRight)//プレイヤーが右側にいる場合
            {
                anim.SetBool("Run", true);
                xVector = 1;
                xLocalScale = 1;
            }
            else if(inBoundary)//境界間の内にいる場合
            {
                anim.SetBool("Run", false);
                xVector = 0;
                xLocalScale = moveRight ? 1 : -1;
            }
            transform.localScale = new Vector3(xLocalScale * Math.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void setBossDead()//ボス死亡時
    {
        if (!isBlink)
        {
            GameManager.instance.isBossDead = true;
            gameObject.SetActive(false);
            playerStepOn2 = false;
        }
    }

    private void bossAttack()//ボスの攻撃処理
    {
        if (gameObject.tag != "Enemy")
        {
            if (attackNum != 1)
            {
                attackNum = UnityEngine.Random.Range(1, 351);
            }
            else
            {
                bossAttackBehavior();
            }
        }
    }

    private void bossAttackBehavior()//ボスの攻撃動作
    {
        if (time < 1f)
        {
            anim.SetBool("Run", false);
            AttackAnimFin = false;
            xVector = moveRight ? 1 : -1;
            xLocalScale = moveRight ? 1 : -1;
        }
        else if (time >= 1f && AttackAnimFin == false)
        {
            isAttack = true;
            anim.SetBool("Attack", true);
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);//再生中のアニメーションを取得
            if (currentState.IsName("Boss_Attack"))//ダウンアニメーションの場合
            {
                if (currentState.normalizedTime >= 0.32 && currentState.normalizedTime < 1)
                {
                        enemySpeed += 1f;
                }
                else if (currentState.normalizedTime >= 1)//1で100%再生。再生し終わってるかを判断
                {
                    attackNum = 0;
                    time = 0f;
                    isAttack = false;
                    AttackAnimFin = true;
                    anim.SetBool("Attack", false);
                    enemySpeed = beforeSpeed;
                    rb.velocity = new Vector2(0, -gravity);
                }
            }

        }
        time += Time.deltaTime;
    }

    private void GenerateEnemy()
    {
        anim.SetBool("Generate", true);
            if ((CountGenerate == 0 && generateTime < 5) || (CountGenerate == 1 && generateTime < 10))
            {
                var script = enemies[generateTime].GetComponent<EnemyBehavior>();
                if (generatingTime > 1f && !script.isGenerated)
                {
                    script.generateItSelf();
                }
                else if(script.isGenerated)
                {
                    generateTime++;
                    generatingTime = 0.0f;
                }
            }
            else if (generateTime == 5 || generateTime == 10)
            {
                generatingTime = 0.0f;
                isGenerating = false;
            }
        generatingTime += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.collider.tag == "player")
        {
            playerHit = true;
        }
    }
}
