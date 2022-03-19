using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : BlinkObject
{
    //Unityのインスペクターに表示させる
    [Header("重力")] public float gravity;
    [Header("移動速度")] public float speed;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("ジャンプ時の高さの制限")] public float jumpHeightLimit;
    [Header("ジャンプ制限時間")] public float jumpLimitTime;
    [Header("踏みつけ判定の高さの割合")] public float stepOnRate;
    [Header("ダッシュの速さ表現")] public AnimationCurve DashCurve;
    [Header("ジャンプの速さ表現")] public AnimationCurve JampCurve;
    [Header("設置判定")] public GroudCheck ground;
    public JudgeIsEnemy judgeEnemySpace;
    [Header("頭をぶつけた判定")] public GroudCheck head;
    [HideInInspector] public bool EnemyCollision = false;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isDown = false;
    public GameObject RLimitObj;
    public GameObject LLimitObj;
    public GameObject deadPos = null;
    public GameObject playerFoots;
    public bool testMode = false;
    public AudioSource JampSE;
    public AudioSource JampDownSE;
    public AudioSource GetDamagedSE;
    public AudioSource RunningSE;

    //プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private CapsuleCollider2D capcol = null;
    private MoveObject moveObj = null;
    private bool isGround = false;
    private bool isHead = false;
    private bool isJump = false;
    private bool isJampDown = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isRolling = false;
    private bool isOtherJump = false;
    private bool isContinue = false;
    private bool isBoss = false;
    private bool isSet = false;
    private SpriteRenderer sr = null;
    private float jumpPos = 0.0f;
    private float jumpTime = 0.0f;
    private float downTime = 0.0f;
    private float dashTime = 0.0f;
    private float time = 0.0f;
    private float beforeKey = 0.0f;
    private float otherJumpHeight = 0.0f;
    private EnemyBehavior o = null;
    private BossBehavior b = null;
    private Vector2 addVelocity;
    private float xspeed;
    private float yspeed;
    private int xVector = 1;
    private AnimatorStateInfo currentState;
    private bool enemyOnRight = false;
    private bool wasJamp = false;
    private bool invincibleMode;//無敵状態
    private bool beforeDown = false;
    ///////////////////////////////////メイン///////////////////////////////////
    void Start()
    {
        //このオブジェクトのコンポーネントを入手
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capcol = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))//pause中は起動させない
        {
            return;
        }
    }

    private void FixedUpdate()
    {
        //接地しているか、頭があたってないかを判定
        isGround = ground.IsGround();
        isHead = head.IsGround();
        //アニメーションをセット
        SetAnim();      
        //プレイヤーがダウンしていない場合
        if (!isDown)
        {
            //x軸、y軸の速度を入手
            xspeed = GetXspeed();
            yspeed = GetYspeed(xspeed);
            //入手した速度を代入(Velocityは物理演算した結果算出した速度を格納しているところ。
            addVelocity = Vector2.zero;
            if (moveObj != null)
            {
                if (isGround)
                {
                    addVelocity = moveObj.GetVelocity();
                }
            }
            if (beforeDown　|| isContinue)
            {
                if(time < 2f)
                {
                    GetBlink(sr);
                }
                if(time >= 2f)
                {
                    sr.enabled = true;
                    UnSetInvincibleMode();
                    isSet = false;
                    time = 0.0f;
                    beforeDown = false;
                    isContinue = false;
                }
                time += Time.deltaTime;
            }
        }
        else if (isDown && !testMode)
        {//プレイヤーがダウンしている場合
            downBehavior();
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, LLimitObj.transform.position.x, RLimitObj.transform.position.x), transform.position.y, transform.position.z);
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * xVector, transform.localScale.y, 1);
        rb.velocity = new Vector2(xspeed, yspeed) + addVelocity;

    }

    
    //敵との接触判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        o = collision.transform.root.gameObject.GetComponent<EnemyBehavior>();
        b = collision.transform.root.gameObject.GetComponent<BossBehavior>();
        if(o == null)
        {
           // Debug.Log("failedgettingEnemyScript");
        }
        isBoss = b != null;
        enemyOnRight = collision.collider.transform.position.x > transform.position.x;

        if (collision.collider.tag == "Enemy_Head")
        {
            if (o != null || b != null)
            {
               
                jumpPos = transform.position.y;//ジャンプした位置を記録する
                isOtherJump = true;
                isJump = false;
                jumpTime = 0.0f;

                if(isBoss)//ボスの場合
                {
                    if (b.isAttack || b.isGenerating)//ボスが攻撃中の場合
                    {
                        if (gameObject.layer == 13)
                        {
                            if (GetDamagedSE != null)
                            {
                                GetDamagedSE.Play();
                            }
                            b.playerStepOn2 = false;
                            b.playerHit = true;
                            isDown = true;
                            GameManager.instance.hpNum -= 0.1m;
                        }
                    }
                    else
                    {
                        otherJumpHeight = b.BoundHeight;//ボススクリプトから跳ねる高さを取得
                        b.playerStepOn2 = true;//踏んだことをボスに通知
                    }
                }
                else//ザコ敵の場合
                {
                    otherJumpHeight = o.BoundHeight;//ザコ敵のスクリプトから跳ねる高さを取得
                    o.playerStepOn = true;//ザコ敵に踏んづけたことを通知
                    Debug.Log("踏んだ");
                }
            }
        }else if(collision.collider.tag == "Enemy_Body" && !invincibleMode)
        {
                beforeDown = true;
                isDown = true;       
        }else if(collision.collider.tag == "MovingGround")
        {
            moveObj = collision.gameObject.GetComponent<MoveObject>();
        }
    }
  

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "MovingGround")
        {
            moveObj = null;
        }
    }


    //////////////////////////////////////メソッド///////////////////////////////
    //
    private void SetInvincibleMode()
    {
        gameObject.layer = 11;
        //Debug.Log("Player is Invincible");
    }

    private void UnSetInvincibleMode()
    {
        gameObject.layer = 13;
        //Debug.Log("Un Set PlayerInvinciblemode");
    }

    private float GetXspeed()//X軸の移動
    {
        //定義
        float horizontalkey = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        float xspeed;

        //右矢印キーが押された場合
        if (horizontalkey > 0)
        {
            if (RunningSE != null)
            {
                if (isGround)
                {
                    if (dashTime < 0.02f || wasJamp)
                    {
                        RunningSE.Play();
                    }
                }
                else
                {
                    RunningSE.Pause();
                }
            }
            isRun = true;
            dashTime += Time.deltaTime;
            xspeed = speed;
            xVector = -1;
        }//左矢印キーが押された場合
        else if (horizontalkey < 0)
        {
            if (RunningSE != null)
            {
                if (isGround)
                {
                    if (dashTime < 0.02f || wasJamp)
                    {
                        RunningSE.Play();
                    }
                }
                else
                {
                    RunningSE.Pause();
                }
            }
            isRun = true;
            dashTime += Time.deltaTime;
            xspeed = -speed;
            xVector = 1;
        }//入力がない場合
        else
        {
            if (RunningSE != null)
            {
                RunningSE.Pause();
            }
            isRun = false;
            dashTime = 0.0f;
            xspeed = 0.0f;
        }

        //直前に押されていたキーと違うキーが押された場合
        if (horizontalkey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalkey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }

        //直前に押されていたキーを入手
        beforeKey = horizontalkey;

        return xspeed;
    }

    private float GetYspeed(float xspeed)//Y軸の移動
    {
        //定義
        float yspeed = -gravity;
        float VerticalKey = Input.GetAxisRaw("Vertical") * Time.deltaTime;
        float horizontalkey = Input.GetAxisRaw("Horizontal") * Time.deltaTime;

        if (isOtherJump)//敵を踏んでいる場合
        {
            //最後に飛んだ位置と高さの制限を足し合わせ、現在のポジションと比べる。
            //現在のポジションのほうが小さい場合、Trueとなる。
            bool canHeight = jumpPos + otherJumpHeight > transform.position.y;
            //ジャンプ制限時間とすでに飛んでいる時間を比べる。
            //すでに飛んでいる時間が小さい場合、True。
            bool canTime = jumpLimitTime > jumpTime;
            //ジャンプ制限位置以内、ジャンプ制限時間内かつ頭があたっていない場合
            if (canHeight && canTime && !isHead)
            {
                yspeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }//上記のどれか一つでも該当した場合
            else
            {
                isOtherJump = false;
                jumpTime = 0.0f;
            }
        }
        else if (isGround)//接地している場合
        {
            if (wasJamp && JampDownSE!= null)
            {
                JampDownSE.Play();
                wasJamp = false;
            }
            isJampDown = false;
            //上矢印キーが押された場合
            if (VerticalKey > 0)
            {
                yspeed = jumpSpeed;
                jumpPos = transform.position.y;
                //isJumpに飛ぶ
                isJump = true;
                jumpTime = 0.0f;
                if (JampSE != null)
                {
                    JampSE.Play();
                }
            }//下矢印キーが押された場合
            else if (VerticalKey < 0)
            {
                isCrouch = true;
                //下矢印キーが押されている状態で、右、または左矢印キーが押された場合
                if (horizontalkey > 0)
                {
                    xspeed = speed;
                    xVector = -1;
                    isRolling = true;
                }
                else if (horizontalkey < 0)
                {
                    xspeed = -speed;
                    xVector = 1;
                    isRolling = true;
                }//下矢印キーが押されているのみの場合
                else
                {
                    xspeed = 0.0f;
                    isRolling = false;
                }
            }//入力がない場合
            else
            {
                isCrouch = false;
                isRolling = false;
                isJampDown = false;
                isJump = false;
            }
        }else if(isJump)//飛んでいる状態の場合
        {
            //上矢印キーが押された場合
            bool pushUpKey = VerticalKey > 0;

            //最後に飛んだ位置と高さの制限を足し合わせ、現在のポジションと比べる。
            //現在のポジションのほうが小さい場合、Trueとなる。
            bool canHeight = jumpPos + jumpHeightLimit > transform.position.y;

            //ジャンプ制限時間とすでに飛んでいる時間を比べる。
            //すでに飛んでいる時間が小さい場合、True。
            bool canTime = jumpLimitTime > jumpTime;

            //上矢印が押され、ジャンプ制限位置以内、ジャンプ制限時間内かつ頭があたっていない場合
            if (pushUpKey && canHeight && canTime && !isHead)
            {
                yspeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }//上記のどれか一つでも該当した場合
            else
            {
                isJump = false;
                jumpTime = 0.0f;
            }
        }//飛んでおらず、接地していない場合
        else
        {
            isJampDown = true;
            wasJamp = true;
        }

        if (isJump || isOtherJump)//ジャンプ表現を付与
        {
            yspeed *= JampCurve.Evaluate(jumpTime);
        }
        return yspeed;
    }

    //アニメーションをセット
    private void SetAnim()
    {
        anim.SetBool("Jamp", isJump || isOtherJump);
        anim.SetBool("JampDown", isJampDown);
        anim.SetBool("Run", isRun);
        anim.SetBool("Crouch", isCrouch);
        anim.SetBool("Rolling", isRolling);
    }

    public void downBehavior()
    {
        if (!isSet)
        {
            if (GetDamagedSE != null)
            {
                GetDamagedSE.Play();
            }
            GameManager.instance.hpNum -= 0.1m;
            SetInvincibleMode();
            isSet = true;
        }
        RunningSE.Pause();
        if (isDead)
        {
            xspeed = 5f;
            yspeed = 1f;
            rb.isKinematic = true;
            capcol.enabled = false;
            beforeDown = false;
            anim.Play("Player_dead");
        }
        else
        {
            PlayDownAnimation();           
        }
    }

    public void PlayDownAnimation()
    {
        anim.Play("Player_Down");
        if (downTime < 0.5f)
        {
            xspeed = enemyOnRight ? -15: 15;
            xVector = enemyOnRight ? -1 : 1;
            yspeed = 1f;
        }else if(downTime >= 0.5f)
        {
            if (GameManager.instance.hpNum > 0m)
            {
                anim.Play("Player_Stand");
                isDown = false;
            }
            else
            {
                isDead = true;
            }
            downTime = 0.0f;
        }
        downTime += Time.deltaTime;
    }

    private bool IsDownAnimEnd()//ダウンアニメーションの最中か
    {
        if (!isDown)
        {
            return false;
        }
        else if(anim == null)
        {
            return false;
        }
        else//ダウン状態で、アニメーションコンポーネントがある場合
        { 
            currentState = anim.GetCurrentAnimatorStateInfo(0);//再生中のアニメーションを取得
            if (currentState.IsName("Player_Down"))//ダウンアニメーションの場合
            {
                if(currentState.normalizedTime >= 1)//1で100%再生。再生し終わってるかを判断
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsDeadAnimEnd()//trueの場合、stageCtrスクリプトに通知
    {
        if (!isDead)
        {
            return false;
        }
        else if (anim == null)
        {
            return false;
        }
        else//死亡状態で、アニメーションコンポーネントがある場合
        {
            currentState = anim.GetCurrentAnimatorStateInfo(0);//再生中のアニメーションを取得
            if (currentState.IsName("Player_dead"))//ダウンアニメーションの場合
            {
                if (currentState.normalizedTime >= 1)//1で100%再生。再生し終わってるかを判断
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    return true;
                }
            }
        }
        return false;
    }

    public void ContinuePlayer()//ダウンからの復帰。stageCtrlスクリプトで使用。
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.isKinematic = false;
        capcol.enabled = true;
        isBoss = false;
        GameManager.instance.hpNum = 0.5m;
        isDown = false;
        anim.Play("Player_Stand");
        isJump = false;
        isOtherJump = false;
        isRun = false;
        isContinue = true;
        isDead = false;
    }
}
    