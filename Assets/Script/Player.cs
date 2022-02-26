using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
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
    [Header("頭をぶつけた判定")] public GroudCheck head;
    [HideInInspector] public bool EnemyCollision = false;
    [HideInInspector] public bool isDead = false;
    public GameObject RLimitObj;
    public GameObject LLimitObj;
    public GameObject deadPos = null;
    public bool testMode = false;
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
    private bool isDown = false;
    private bool isOtherJump = false;
    private bool isContinue = false;
    private bool isMovingGround = false;
    private bool isBoss = false;
    private float continueTime = 0.0f;
    private float blinkTime = 0.0f;
    private SpriteRenderer sr = null;
    private float jumpPos = 0.0f;
    private float jumpTime = 0.0f;
    private float downTime = 0.0f;
    private float dashTime = 0.0f;
    private float beforeKey = 0.0f;
    private float otherJumpHeight = 0.0f;
    private string enemyTag = "Enemy";
    private EnemyBehavior o = null;
    private BossBehavior b = null;
    private Vector2 addVelocity;
    private float xspeed;
    private float yspeed;
    private GameObject goBossPos;
    private AnimatorStateInfo currentState;
    private bool enemyOnRight = false;
    ///////////////////////////////////メイン///////////////////////////////////
    void Start()
    {
        //このオブジェクトのコンポーネントを入手
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capcol = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        goBossPos = GameObject.Find("goBossBattlePoint");
    }

    private void Update()
    {
        if (isContinue)//コンティニュー表現中か
        {
            blink();//プレイヤーを点滅
        }
    }

    private void FixedUpdate()
    {
        //接地しているか、頭があたってないかを判定
        isGround = ground.IsGround();
        isHead = head.IsGround();
        //アニメーションをセット
        SetAnim();

        if(transform.position.x <= goBossPos.transform.position.x)
        {
            GameManager.instance.goBossBattle = true;
        }
      
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
           
        }
        else if (isDown && !testMode)
        {//プレイヤーがダウンしている場合
            downBehavior();
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, LLimitObj.transform.position.x, RLimitObj.transform.position.x), transform.position.y, transform.position.z);

        rb.velocity = new Vector2(xspeed, yspeed) + addVelocity;    
    }

    
    //敵との接触判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        o = collision.transform.root.gameObject.GetComponent<EnemyBehavior>();
        b = collision.transform.root.gameObject.GetComponent<BossBehavior>();

        isBoss = b != null;
        enemyOnRight = collision.collider.transform.position.x <= transform.position.x;

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
                    if (b.isAttack)//ボスが攻撃中の場合
                    {
                        b.playerStepOn2 = false;
                        anim.Play("Player_Down");
                        isDown = true;
                        GameManager.instance.hpNum -= 0.1m;
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
                }
            }
        }else if(collision.collider.tag == "Enemy_Body" && !testMode)
        {
            anim.Play("Player_Down");
            isDown = true;
            GameManager.instance.hpNum -= 0.1m;
        }else if(collision.collider.tag == "MovingGround")
        {
            moveObj = collision.gameObject.GetComponent<MoveObject>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "MovingGround")
        {
            moveObj = null;
        }
    }



    //////////////////////////////////////メソッド///////////////////////////////
    ///
    private float GetXspeed()//X軸の移動
    {
        //定義
        float horizontalkey = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        float xspeed;

            //右矢印キーが押された場合
            if (horizontalkey > 0)
            {
                transform.localScale = new Vector3(-1 * Math.Abs(transform.localScale.x), transform.localScale.y, 1);
                isRun = true;
                dashTime += Time.deltaTime;
                xspeed = speed;
            }//左矢印キーが押された場合
            else if (horizontalkey < 0)
            {
                transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, 1);
                isRun = true;
                dashTime += Time.deltaTime;
                xspeed = -speed;
            }//入力がない場合
            else
            {
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
        //移動表現を代入
        xspeed *= DashCurve.Evaluate(dashTime);

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

            isJampDown = false;
            //上矢印キーが押された場合
            if (VerticalKey > 0)
            {
                yspeed = jumpSpeed;
                jumpPos = transform.position.y;
                //isJumpに飛ぶ
                isJump = true;
                jumpTime = 0.0f;
            }//下矢印キーが押された場合
            else if (VerticalKey < 0)
            {
                isCrouch = true;
                //下矢印キーが押されている状態で、右、または左矢印キーが押された場合
                if (horizontalkey > 0)
                {
                    xspeed = speed;
                    transform.localScale = new Vector3(-1 * Math.Abs(transform.localScale.x), transform.localScale.y, 1);
                    isRolling = true;
                }
                else if (horizontalkey < 0)
                {
                    xspeed = -speed;
                    transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, 1);
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
        if (isDead)
        {
            xspeed = 0;
            yspeed = -gravity;
        }
        else
        {
            if (isBoss)
            {
                bossDownBehavior();
            }
            else if (!isBoss)
            {
                enemyDownBehavior();
            }
            downTime += Time.deltaTime;
        }
    }

    public void enemyDownBehavior()
    {

        if (downTime < 0.2f)
        {
            if (enemyOnRight)
            {
                xspeed = speed + 0.3f;
                yspeed = 10f;
            }
            else
            {
                xspeed = -(speed + 0.3f);
                yspeed = 10f;
            }
        }
        else if (downTime >= 0.2f && downTime < 0.6f && !isGround)
        {
            if (enemyOnRight)
            {
                xspeed = speed;
                yspeed = -10f;
            }
            else
            {
                xspeed = -speed;
                yspeed = -10f;
            }
        }else if(downTime >= 0.6f || isGround)
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
    }

    public void bossDownBehavior()
    {
        if (downTime < 0.2f)
        {
            if (enemyOnRight)
            {
                xspeed = speed + 1f;
                yspeed = 20f;
            }
            else
            {
                xspeed = -(speed + 1f);
                yspeed = 20f;
            }
        }
        else if (downTime >= 0.2f && downTime < 0.6f && !isGround)
        {
            if (enemyOnRight)
            {
                xspeed = speed;
                yspeed = -20f;
            }
            else
            {
                xspeed = -speed;
                yspeed = -20f;
            }
        }
        else if (downTime >= 0.6f || isGround)
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
            isBoss = false;
        }
    }

    public bool IsContinueWating()//コンティニュー待ちか。結果をstagectrlのupdateに送る。
    {
        if (GameManager.instance.hpNum <= 0 && !GameManager.instance.isFallDead)
        {
            return IsDownAnimEnd();
        }else if (GameManager.instance.isFallDead)
        {
            gameObject.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
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

    public void ContinuePlayer()//ダウンからの復帰。stageCtrlスクリプトで使用。
    {
        GameManager.instance.hpNum = 0.5m;
        gameObject.SetActive(true);
        isDown = false;
        anim.Play("Player_Stand");
        isJump = false;
        isOtherJump = false;
        isRun = false;
        isContinue = true;
        isDead = false;
    }

    public void blink()//点滅表現
    {
        //0.2秒以降は、人通りすべてのif条件を通るように
        if (blinkTime > 0.2f)
        {
            sr.enabled = true;//スプライトーレンダラーを表示
            blinkTime = 0.0f;
        }
        else if (blinkTime > 0.1f)
        {
            sr.enabled = false;//スプライトーレンダラーを非表示
        }
        else
        {
            sr.enabled = true;
        }

        if (continueTime > 1.0f)//リスポーン表現の時間が1秒より大きくなった場合
        {
            isContinue = false;
            blinkTime = 0f;
            continueTime = 0f;
            sr.enabled = true;
        }
        else
        {
            blinkTime += Time.deltaTime;
            continueTime += Time.deltaTime;
        }
    }
/*
        if (collision.collider.tag == enemyTag || collision.collider.tag == "Boss")
        {
            //踏みつけ判定になる高さ
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));
            //=0.5

            //踏みつけ判定のワールド座標
            float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

            //一つでも敵にあたってしまった衝突データがないか調べる
            foreach (ContactPoint2D p in collision.contacts)
            {
                Debug.Log("衝突データのY座標(p) = " + p.point.y);
                Debug.Log("踏みつけ判定の座標(judgePos) = " + judgePos);
                if (p.point.y < judgePos)//衝突データが踏みつけ判定座標より下の場合
                {

                    EnemyBehavior o = collision.gameObject.GetComponent<EnemyBehavior>();
                    BossBehavior b = collision.gameObject.GetComponent<BossBehavior>();
                    if (o != null || b != null)
                    {
                        otherJumpHeight = o.BoundHeight;//踏んづけたものから跳ねる高さを取得
                        jumpPos = transform.position.y;//ジャンプした位置を記録する
                        isOtherJump = true;
                        isJump = false;
                        jumpTime = 0.0f;
                        if(o != null) //踏んづけたものに対して踏んづけたことを通知
                        {
                            o.playerStepOn = true;
                        }else if(b != null)
                        {
                            b.playerStepOn2 = true;
                        }
                    }
                }
                else//衝突データが足元よりも上だった場合
                {
                    anim.Play("Player_Down");
                    isDown = true;
                    GameManager.instance.hpNum -= 1;
                    break;
                }
            }
        }
        */
}
    