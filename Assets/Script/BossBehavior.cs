using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;

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
    public List<GameObject> children;
    public EnemyCollisionCheck enc;
    public GameObject JudgeReturnRight;
    public GameObject JudgeReturnLeft;
    public GameObject ReturnPos;
    public bool doNotAttack = false;
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
    private int attackNum;
    private bool AttackAnimFin = false;
    private float time = 0f;
    private bool playerHit = false;
    private float backTime = 0.0f;
    private bool inBoundary = false;
    private bool isReturn = false;
    private int xLocalScale;
    private int generateTime = 0;
    private int CountGenerate = 0;
    private Vector2 returnPos;
    private Vector2 p;
    private Vector2 This;
    private Vector2 ReRight;
    private Vector2 ReLeft;
    private Vector2 ReturnVector;
    private Func<bool> boolFunction;
    private bool isBlink = false;
    private bool isInvicble = false;
    private bool animSet = false;
    private bool moveToReturn = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        beforeSpeed = enemySpeed;
        player = GameObject.Find("Player");
        ReRight = JudgeReturnRight.transform.position;
        ReLeft = JudgeReturnLeft.transform.position;
        ReturnVector = ReturnPos.transform.position;
    }

    private void Update()
    {
        if (isSet)//HP減少がセットされた場合
        {
            if (isBlink)
            {
                GetBlink(sr);
                if (isBlinkFin())
                {
                    isBlink = false;
                }
            }
            else if(!playerHit)//点滅作業と回避行動が終了した場合
            {
                if (bossHp > 0)
                {
                    //updateのたびにtrueにしてしまうと永遠に生成してしまうため、一回だけフラグを立てる
                    if ((CountGenerate == 0 && generateTime == 0) || (CountGenerate == 1 && generateTime == 5))
                    {
                        isGenerating = true;
                    }

                    if ((bossHp == 8 || bossHp == 4) && isGenerating)//HPが8または4かつ生成作業中の場合
                    {
                        rb.constraints = RigidbodyConstraints2D.FreezeAll;
                        GenerateEnemy();

                    }
                    else//HPが上記の該当外または、生成作業が終了した場合
                    {
                        anim.SetBool("Generate", false);
                        CountGenerate++;
                        playerStepOn2 = false;
                        isSet = false;
                        rb.constraints = RigidbodyConstraints2D.FreezePositionY
                                        | RigidbodyConstraints2D.FreezeRotation;
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
        p = player.transform.position;
        This = transform.position;
        
        //カメラに写っているかどうか（シーンビューに映る際も適応される）
        if (sr.isVisible || nonVisible)
        {
            GameManager.instance.bossIsvisble = true;
            judgeMoveDir();

            if (isGenerating)
            {

            }
            else if (playerHit && !isAttack)//移動範囲外におり、プレイヤーに衝突した場合
            {
                if (JudgeIsReturnPos())
                {
                    isReturn = true;
                }
                else
                {
                    PlayerHitBehavior();
                }
            }
            else if(!playerHit)
            {
                if (!doNotAttack)
                {
                    BossAttackJudge();
                }
            }
            //PlayerHitBehaviorが終了したのち、すぐに動かしたいため上記のif分とは別に記述
            if (!playerHit && !isAttack && !isGenerating && !isReturn)
            {
                Move();
            }
       
            if (isReturn)
            {
                
                TelepoteBehavior();
                if (moveToReturn)
                { 
                   rb.MovePosition(ReturnVector);
                }
            }
            else
            {
                rb.velocity = new Vector2(xVector * enemySpeed, -gravity);
            }
        }
        else
        {
            rb.Sleep();
        }  
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGenerating || isReturn || playerStepOn2)
        {
            return;
        }
        else if (collision.collider.tag == "player")
        {
            playerHit = true;
        }
    }

    ///メソッド達///
    private void judgeMoveDir()//動く方向を判断
    {
        var leftBossStopWidth = transform.position.x - stopWidth;//左側の境界値
        var rightBossStopWidth = transform.position.x + stopWidth;//右側の境界値

        if(p.x < leftBossStopWidth || p.x > rightBossStopWidth)//プレイヤーが境界より外側にいた場合
        {
            inBoundary = false;
            moveRight = p.x > rightBossStopWidth;
        }
        else//両境界の内側にいた場合
        {
            inBoundary = true;
        }
    }

    private void Move() //敵キャラの動き
    {
        
        if (!isAttack && !isGenerating)//プレイヤーと衝突しておらず、攻撃中でない場合
        {
            boolFunction = () => This.y < p.y;
            if (inBoundary && boolFunction())//境界間の内におり、頭上にいる場合
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
        }
        transform.localScale = new Vector3(xLocalScale * Math.Abs(transform.localScale.x), transform.localScale.y);
    }

    private bool JudgeIsReturnPos()
    {
        return This.x > ReRight.x || This.x < ReLeft.x;
    }

    private void PlayerHitBehavior()
    {
        float Judgetime = playerStepOn2 ? 0.4f : 0.6f; //プレイヤーに踏まれたか、踏まれていないか


        if (playerStepOn2 && !isSet)//攻撃中でない、生成中でない、体力減少をセットしていない場合
        {
            bossHp -= 1;
            isSet = true;
            isBlink = true;
        }

        if (backTime < Judgetime)
        {
            SetInvincible();
            anim.SetBool("telepote", true);
            enemySpeed += 0.1f;
            xVector = moveRight ? -1 : 1;
        }
        else
        {
            anim.SetBool("telepote", false);
            anim.SetBool("GetBack", true);
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            if (currentState.IsName("Boss_GetBack") && currentState.normalizedTime >= 1)
            {
                Debug.Log("回避終了");
                UnSetInvincible();
                anim.SetBool("GetBack", false);
                anim.Play("Boss_stand");
                backTime = 0.0f;
                playerHit = false;
                enemySpeed = beforeSpeed;
            }
        }
        backTime += Time.deltaTime;
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

    private void BossAttackJudge()//ボスの攻撃処理
    {
        if (attackNum != 1)
        {
            attackNum = UnityEngine.Random.Range(1, 451);
        }
        else
        {
            bossAttackBehavior();
            
        }
    }

    private void bossAttackBehavior()//ボスの攻撃動作
    {
        if (time < 0.8f)
        {
            anim.SetBool("Run", false);
            AttackAnimFin = false;
            xVector = moveRight ? 1 : -1;
            xLocalScale = moveRight ? 1 : -1;
        }
        else if (time >= 0.8f && AttackAnimFin == false)
        {
            isAttack = true;
            anim.SetBool("Attack", true);
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);//再生中のアニメーションを取得
            if (currentState.IsName("Boss_Attack") && isAttack)//ダウンアニメーションの場合
            {
                if (currentState.normalizedTime >= 0.32 && currentState.normalizedTime < 1)
                {
                        enemySpeed += 1f;
                }
                else if (currentState.normalizedTime >= 1)//1で100%再生。再生し終わってるかを判断
                {
                    playerHit = false;
                    attackNum = 0;
                    time = 0f;
                    isAttack = false;
                    AttackAnimFin = true;
                    anim.SetBool("Attack", false);
                    enemySpeed = beforeSpeed;
                    rb.velocity = new Vector2(0, -gravity);
                    if (JudgeIsReturnPos())
                    {
                        isReturn = true;
                    }
                }
            }

        }
        time += Time.deltaTime;
    }

    private void GenerateEnemy()//エネミー生成攻撃の挙動
    {
        anim.SetBool("Generate", true);
        //１回目の攻撃か否かで何番目までのエネミーを生成するか判断
        int generateAmount = CountGenerate == 0 ? 5 : 10;

        if (generateTime < generateAmount)
        {
            var script = enemies[generateTime].GetComponent<EnemyBehavior>();
            if(time > 1f && !script.isGenerated)//ゲーム内時間１秒につき、一体生成
            {
                script.generateItSelf();//各エネミーのスクリプトでenableをtrueにする。
            }
            else if(script.isGenerated)//エネミースクリプト側で生成動作が終了した場合
            {
                generateTime++;
                time = 0.0f;
            }
        }
        else if (generateTime == 5 || generateTime == 10)//攻撃毎に、5体生成した場合
        {
            time = 0.0f;
            isGenerating = false;
        }
        time += Time.deltaTime;
    }

    private void TelepoteBehavior()
    {
        nonVisible = true;
        SetInvincible();
        isInvicble = true;


        if (!moveToReturn)
        {
            anim.SetBool("telepote", true);
        }
        else
        {
            anim.SetBool("telepote", false);
            anim.SetBool("GetBack", true);
        }

        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);

        if (currentState.IsName("Boss_Telepote") && currentState.normalizedTime >= 1 && !moveToReturn)
        {
            moveToReturn = true;
        }
        else if(currentState.IsName("Boss_GetBack") && currentState.normalizedTime >= 1)
        {
           // Debug.Log("帰還");
            moveToReturn = false;
            anim.SetBool("GetBack", false);
            anim.Play("Boss_stand");
            UnSetInvincible();
            isInvicble = false;
            playerHit = false;
            isReturn = false;
        }
    }

    private void SetInvincible()
    {
        gameObject.layer = 14;
        children.Select(o => o.layer = 14)
                .ToList();
    }

    private void UnSetInvincible()
    {
        gameObject.layer = 6;
        children.Select(o => o.layer = 6)
                .ToList();
    }

    private IEnumerator DelayCoroutine(float sec, Action action)
    {
        yield return new WaitForSeconds(sec);
        action?.Invoke();
    }
}
