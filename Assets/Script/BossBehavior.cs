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
    public int stopWidth = 10;
    public List<GameObject> enemies;
    public List<GameObject> children;
    public GameObject JudgeReturnRight;
    public GameObject JudgeReturnLeft;
    public GameObject ReturnPos;
    public bool doNotAttack = false;
    public GameObject rightLim;
    public GameObject leftLim;
    [HideInInspector] public bool isAttack = false;
    [HideInInspector] public bool isGenerating = false;
    [HideInInspector] public bool playerStepOn2 = false; //敵を踏んだかどうか判断、インスペクターでは非表示
    [HideInInspector] public bool playerHit = false;
    [HideInInspector] public bool HitInvP = false;

    private Animator anim = null;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isSet = false;
    private GameObject player;
    private Player pSc;
    private int xVector;
    private float beforeSpeed = 1;
    private bool moveRight = false;
    private int attackNum;
    private bool AttackAnimFin = false;
    private float time = 0f;
    private float backTime = 0.0f;
    private bool inBoundary = false;
    private bool isReturn = false;
    private int xLocalScale;
    private int generateTime = 0;
    private int CountGenerate = 0;
    private Vector2 p;
    private Vector2 This;
    private Vector2 ReRight;
    private Vector2 ReLeft;
    private Vector2 ReturnVector;
    private Func<bool> boolFunction;
    private bool canBlink = false;
    private bool moveToReturn = false;
    private bool getDamageFin = false;
    private bool isDead = false;
    private bool attackSteped = false;
    private bool generateTrigger = false;
    private AudioSource beSteppedSE;

    private void Start()
    {
        beSteppedSE = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        beforeSpeed = enemySpeed;
        player = GameObject.Find("Player");
        ReRight = JudgeReturnRight.transform.position;
        ReLeft = JudgeReturnLeft.transform.position;
        ReturnVector = ReturnPos.transform.position;
    }

    void FixedUpdate()
    {
        p = player.transform.position;
        This = transform.position;

        
        //カメラに写っているか場合
        if (sr.isVisible || nonVisible)
        {

            GameManager.instance.bossIsvisble = true;
            judgeMoveDir();
           // Debug.Log("isReturn = " + isReturn);
            if (!isReturn)
            {
                //Debug.Log("player step on2 = " + playerStepOn2);
                if (!playerStepOn2)
                {
                    //Debug.Log("enter to !enemyStepOn");
                    if (!isDead)
                    {
                        pSc = player.GetComponent<Player>();
                        if (pSc.isDown || HitInvP)//HitInvPはBossAttackBehaviorでfalseにする。
                        {
                            SetInvincible();
                        }
                        else
                        {
                            UnSetInvincible();
                        }
                        nonVisible = true;
                        //Debug.Log("踏まれてないよ");
                        if (!doNotAttack)
                        {
                            BossAttackJudge();//攻撃動作が終了した場合isAttackをfalseにする
                        }
                        if (attackNum != 1)
                        {
                            //Debug.Log("boss move activate");
                            Move();/*JudgeisReturnPos()がtrueでplayerとあたった場合、
                            isReturnをtrueにする。*/
                        }

                        if(attackSteped)
                        {
                            GetBlink(sr);
                            if (isBlinkFin())
                            {
                                attackSteped = false;
                            }
                        }
                    }
                    else
                    {
                        setBossDead();
                    }                   
                }
                else//踏まれた場合
                {
                    //Debug.Log("enter to enemyStepOn");
                    if (attackNum == 1)
                    {
                        //Debug.Log("steped while attack");
                        beSteppedSE.Play();
                        bossHp--;
                        attackSteped = true;
                        isAttack = true;
                        playerStepOn2 = false;
                        if(bossHp == 10 || bossHp == 5)
                        {
                            generateTrigger = true;
                        }
                        else if(bossHp < 1)
                        {
                            isSet = false;
                            attackNum = 0;
                            playerStepOn2 = true;
                        }
                        if(bossHp > 0)
                        {
                            GameManager.instance.score += 100;
                        }
                        return;
                    }
                    //Debug.Log("踏まれたよ");
                    if (!getDamageFin)
                    {
                       // Debug.Log("Enter to GetDamageBehavior");
                        GetDamageBehavior();//回避動作が終了したらgetDamageFinフラグをtrueにする。
                    }
                    else
                    {
                        //Debug.Log("EnterTogetDamageFin");
                        JudgeCanGenerate();//Hpが生成作業対象の場合は、isGeneratingをtrueにする
                        if (isGenerating)//生成作業中の場合
                        {
                            //Debug.Log("生成開始");
                            GenerateEnemy();//生成作業が終了したらisGeneratingをfalseにする
                        }

                        if(!isGenerating)//生成作業終了後、すぐに起動させたいため上記のif文とは別々に記述
                        {
                            getDamageFin = false;
                            playerStepOn2 = false;//ここで!playerStepOn分岐を終了
                            isSet = false;//GetDamageBehaviorで使用する。仕様上ここでfalseにする
                            backTime = 0.0f; //仕様上ここでfalseにする
                        }
                    }
                }
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, leftLim.transform.position.x, rightLim.transform.position.x), transform.position.y, transform.position.z);
                transform.localScale = new Vector3(xLocalScale * Math.Abs(transform.localScale.x), transform.localScale.y);
                rb.velocity = new Vector2(xVector * Math.Abs(enemySpeed), -gravity);
            }
            else//isReturnの場合
            {
                //Debug.Log("EnterToIsreturn");
                TelepoteBehavior();//ここでplayerHitをfalseにする
                                   
                if (moveToReturn)//TelepoteBehaviorにてmoveToReturnがtrueにされた場合
                {
                    enemySpeed = beforeSpeed;
                    rb.MovePosition(ReturnVector);//移動したあと、TelepoteBehaviorにてisReturnがfalseになる
                }
            }
        }
        else//画面に入ってない場合
        {
            rb.Sleep();
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
        if (playerHit)//プレイヤーと衝突した場合
        {
            if (JudgeIsReturnPos())//戻り境界線の中にいた場合
            {
                Debug.Log("set isReturn");
                enemySpeed = 0;
                isReturn = true;
            }
            else if(!attackSteped)//戻り境界線にいなかった場合
            {
                xVector = moveRight ? -1 : 1;//0.5秒間後退する
                if (time > 0.5f)
                {
                    playerHit = false;
                    time = 0.0f;
                }
                time += Time.deltaTime;
            }
        }
        else//プレイヤーと衝突していない場合
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
        //Debug.Log("xVector = " + xVector + "xLocalScale = " + xLocalScale);
    }

    private bool JudgeIsReturnPos()
    {
        return This.x > ReRight.x || This.x < ReLeft.x;
    }

    private void GetDamageBehavior()
    {
        //Debug.Log(backTime);
        //一度起動させたら、次にfixedUpdateでisSetがfalseになるまで起動させない。
        if (!isSet)
        {
            //Debug.Log("GetDamageBehavior set");
            bossHp -= 1;
            beSteppedSE.Play();
            canBlink = true;
            getDamageFin = false;
            GameManager.instance.score += 100;
            isSet = true;
        }
           
        //一度終了したら、次に上記のif文でcanBlinkがtrueになるまで起動させない。
        if (canBlink)
        {
            //Debug.Log("start boss blink");
            GetBlink(sr);
            if (isBlinkFin())
            {
                canBlink = false;
            }
        }
        if(bossHp <= 0)
        {
            SetInvincible();
            anim.Play("Boss_Defeated");
            isDead = true;
        }

        if (!isDead)
        {
            //Debug.Log("enter to getDamageBehavior back Time = " + backTime);
            if (backTime < 1f)
            {
                anim.SetBool("telepote", true);
                SetInvincible();
                enemySpeed += 0.8f;
                xVector = moveRight ? -1 : 1;
            }
            else if (backTime >= 1f && backTime < 2f)
            {
                //Debug.Log("GetDamageBehavior's animation changed");
                enemySpeed = beforeSpeed;
                xVector = 0;
                anim.SetBool("telepote", false);
                anim.SetBool("GetBack", true);
            }
            else if (backTime >= 2f)
            {
                //Debug.Log("GetDamageBehaviorFin");
                anim.SetBool("GetBack", false);
                anim.Play("Boss_stand");
                UnSetInvincible();
                getDamageFin = true;           
            }
            backTime += Time.deltaTime;
        }
        else
        {
            if (!canBlink)
            {
                getDamageFin = true;
                isSet = false;
            }
        }
    }

    

    private void setBossDead()//ボス死亡時
    {
        if (!isSet)
        {
            GameManager.instance.isBossDead = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            isSet = true;
            //Debug.Log("set boss dead");
        }      
    }

    private void BossAttackJudge()//ボスの攻撃処理
    {
        if (attackNum != 1)
        {
            if (bossHp > 6)
            {
                attackNum = UnityEngine.Random.Range(1, 151);//151
            }
            else
            {
                attackNum = UnityEngine.Random.Range(1, 51);//51
            }
        }
        else
        {
            bossAttackBehavior();
            
        }
    }

    private void bossAttackBehavior()//ボスの攻撃動作
    {
        if (!isSet)
        {
            xVector = p.x > This.x ? 1 : -1;
            xLocalScale = xVector > 0 ? 1 : -1;
            AttackAnimFin = false;
            isSet = true;
        }
        if (AttackAnimFin == false)
        {
            anim.SetBool("Attack", true);
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);//再生中のアニメーションを取得
            if (currentState.IsName("Boss_Attack"))//ダウンアニメーションの場合
            {
               // Debug.Log("bossAttack normalizedTime = " + currentState.normalizedTime);
                if (currentState.normalizedTime >= 0.5 && currentState.normalizedTime < 1)
                {
                    isAttack = true;
                    enemySpeed += 1f;
                }
                else if (currentState.normalizedTime >= 1)//1で100%再生。再生し終わってるかを判断
                {
                    attackNum = 0;
                    isAttack = false;
                    isSet = false;
                    HitInvP = false;
                    anim.SetBool("Attack", false);
                    enemySpeed = beforeSpeed;
                    AttackAnimFin = true;
                    Debug.Log("boss attack fin");
                    if (generateTrigger)
                    {
                        Debug.Log("set generateTrigger");
                        playerStepOn2 = true;
                        getDamageFin = true;
                        generateTrigger = false;
                    }
                }
            }

        }
    }

    private void JudgeCanGenerate()
    {
        if (bossHp == 10 || bossHp == 5)
        {
            if ((CountGenerate == 0 && generateTime == 0) || (CountGenerate == 1 && generateTime == 5))
            {
                isGenerating = true;
            }
        }
    }

    private void GenerateEnemy()//エネミー生成攻撃の挙動
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        anim.SetBool("Generate", true);
        //１回目の生成攻撃か否かで何番目までのエネミーを生成するか判断
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
            //Debug.Log("生成終了");
            anim.SetBool("Generate", false);
            CountGenerate++;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY
                            | RigidbodyConstraints2D.FreezeRotation;
            time = 0.0f;
            isGenerating = false;
        }
        time += Time.deltaTime;
    }

    private void TelepoteBehavior()
    {
        //Debug.Log("enter to telepoteBehavior");
        SetInvincible();
        playerHit = false;

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
            //Debug.Log("帰還");
            moveToReturn = false;
            anim.SetBool("GetBack", false);
            anim.Play("Boss_stand");
            UnSetInvincible();
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
        children[0].layer = 15;
    }

    private IEnumerator DelayCoroutine(float sec, Action action)
    {
        yield return new WaitForSeconds(sec);
        action?.Invoke();
    }
}
