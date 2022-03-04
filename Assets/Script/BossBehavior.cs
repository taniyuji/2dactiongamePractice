using System.Collections.Generic;
using UnityEngine;
using System;

public class BossBehavior : BlinkObject
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
    public GameObject JudgeReturnRight;
    public GameObject JudgeReturnLeft;
    public GameObject ReturnPos;
    public EdgeCollider2D BodyEdge;
    public EdgeCollider2D HeadEdge;
    [HideInInspector] public bool hitGround = false;
    [HideInInspector] public bool isAttack = false;
    [HideInInspector] public bool isGenerating = false;

    [HideInInspector] public bool playerStepOn2 = false; //敵を踏んだかどうか判断、インスペクターでは非表示
    private Animator anim = null;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isSet = false;
    private GameObject player;
    private int xVector;
    private float beforeSpeed = 1;
    private bool moveRight = false;
    private bool startBlink = false;
    private int attackNum;
    private bool AttackAnimFin = false;
    private float time = 0f;
    private GameObject RLimitObj;
    private GameObject LLimitObj;
    private bool playerHit = false;
    private float backTime = 0.0f;
    private bool inBoundary = false;
    private bool isReturn = false;
    private int xLocalScale;
    private int generateTime = 0;
    private int CountGenerate = 0;
    private float generatingTime = 0.0f;
    private Vector2 returnPos;

    
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
        if (isSet)//HP減少がセットされた場合
        {
            if (!startBlink)
            {
                GetBlink(sr);
                if (isBlinkFin())
                {
                    startBlink = true;
                }
            }
            if(isBlinkFin())//点滅作業が終了した場合
            {
                if (bossHp > 0)
                {
                    //エネミー生成攻撃一回目の一体目、または二回目の一体目のエネミー生成の場合
                    if ((CountGenerate == 0 && generateTime == 0) || (CountGenerate == 1 && generateTime == 5))
                    {
                        isGenerating = true;//生成中のフラグを建てる
                    }

                    if ((bossHp == 8 || bossHp == 4) && isGenerating)//HPが8または4かつ生成作業中の場合
                    {
                        enemySpeed = 0;
                        GenerateEnemy();
                    }
                    else//HPが上記の該当外または、生成作業が終了した場合
                    {
                        enemySpeed = beforeSpeed;
                        anim.SetBool("Generate", false);
                        CountGenerate++;
                        playerStepOn2 = false;
                        isSet = false;
                        startBlink = false;
                    }
                }
                else//ボスのHPが0の場合
                {
                    setBossDead();
                }
            }
        }
       
        
    }

    void FixedUpdate()
    {
        if (!playerStepOn2)//プレイヤーが踏んでいない場合
        {
            //カメラに写っているかどうか（シーンビューに映る際も適応される）
            if (sr.isVisible || nonVisible)
            {
                GameManager.instance.bossIsvisble = true;
                judgeMoveDir();
                Move();
                if (!isReturn)
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
            if (!isAttack && !isGenerating && !isSet)//攻撃中でない、生成中でない、体力減少をセットしていない場合
            {
                bossHp -= 1;
                isSet = true;
            }
            if (!isGenerating)//生成攻撃中は動かさない
            {
                Move();
            }
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, LLimitObj.transform.position.x, RLimitObj.transform.position.x), transform.position.y, transform.position.z);
        if (isReturn)
        {
            Debug.Log("戻ります");
            returnPos = Vector2.MoveTowards(transform.position, ReturnPos.transform.position, enemySpeed * Time.deltaTime +1);
            Debug.Log(returnPos);
            rb.MovePosition(returnPos);
            if (transform.position.x == ReturnPos.transform.position.x)
            {
                BodyEdge.isTrigger = false;
                HeadEdge.isTrigger = false;
                isReturn = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(xVector * enemySpeed, -gravity);
        }
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

    private bool playerAbove()
    {
        return transform.position.y < player.transform.position.y;
    }

    private bool isReturnPos()
    {
        return transform.position.x > JudgeReturnRight.transform.position.x || transform.position.x < JudgeReturnLeft.transform.position.x;
    }

      ///メソッドたち///
    private void Move() //敵キャラの動き
    {
        if(isReturnPos())
        {
            BodyEdge.isTrigger = true;
            HeadEdge.isTrigger = true;
            isReturn = true;
        }
        else if (playerHit && !isAttack)//プレイヤーと衝突し、攻撃中ではない場合
        {
            //踏まれていない場合と踏まれた場合で下がる距離を分ける
            if ((!playerStepOn2 && backTime < 0.4f) || (playerStepOn2 && backTime < 0.6f))
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
            if(inBoundary && playerAbove())//境界間の内にいる場合
            {
                anim.SetBool("Run", false);
                xVector = 0;
                xLocalScale = moveRight ? 1 : -1;
            }
            else//プレイヤーが境界内にいない場合
            {
                anim.SetBool("Run", true);
                xVector = moveRight ? 1 : -1;
                xLocalScale = moveRight ? 1 : -1;
            }
            transform.localScale = new Vector3(xLocalScale * Math.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void setBossDead()//ボス死亡時
    {
        if (isBlinkFin())//点滅中でない場合
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
                else if (currentState.normalizedTime >= 1　|| playerHit)//1で100%再生。再生し終わってるかを判断
                {
                    playerHit = false;
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

    private void GenerateEnemy()//エネミー生成攻撃の挙動
    {
        anim.SetBool("Generate", true);
        //一回の攻撃につき、五体のエネミーを生成。
        if ((CountGenerate == 0 && generateTime < 5) || (CountGenerate == 1 && generateTime < 10))
        {
            var script = enemies[generateTime].GetComponent<EnemyBehavior>();
            if (generatingTime > 1f && !script.isGenerated)//ゲーム内時間１秒につき、一体生成
            {
                script.generateItSelf();//各エネミーのスクリプトでenableをtrueにする。
            }
            else if(script.isGenerated)//エネミースクリプト側で生成動作が終了した場合
            {
                generateTime++;
                generatingTime = 0.0f;
            }
        }
        else if (generateTime == 5 || generateTime == 10)//攻撃毎に、5体生成した場合
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
