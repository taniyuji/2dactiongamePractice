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
    public EnemyCollisionCheck enc;
    [HideInInspector] public bool hitGround = false;
    [HideInInspector] public bool isAttack = false;

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
    private Transform player;
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


    private void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        RLimitObj = GameObject.Find("RightMoveLimit");
        LLimitObj = GameObject.Find("LeftMoveLimit");
        beforeSpeed = enemySpeed;
    }

    private void Update()
    {
        if (isStamped)
        {
            blink();//このメソッド内でisStampedをfalseにすることで一回だけ実行させる
            if (bossHp > 0)
            {
                playerStepOn2 = false;
            }

            if (bossHp <= 0)
            {
                if (!isBlink)
                {
                    GameManager.instance.isBossDead = true;
                    gameObject.SetActive(false);
                    playerStepOn2 = false;
                }
            }
        }
       
        
    }

    void FixedUpdate()
    {


        if (playerStepOn2 == false)
        {
            //カメラに写っているかどうか（シーンビューに映る際も適応される）
            if (sr.isVisible || nonVisible)
            {
                GameManager.instance.bossIsvisble = true;
                player = GameObject.Find("Player").transform;
                if (player.transform.position.x < transform.position.x)
                {
                    playerOnRight = false;
                }
                else
                {
                    playerOnRight = true;
                }
                Move();

                if (hitGround == false)
                {
                    if (attackNum != 1)
                    {
                        attackNum = UnityEngine.Random.Range(1, 401);
                    }
                    else
                    {
                        bossAttack();
                    }
                }
            }
            else
            {
                rb.Sleep();
            }
        }
        else//踏まれた場合
        {
            if (!isBlink)
            {
                bossHp -= 1;
                isStamped = true;
            }
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, LLimitObj.transform.position.x, RLimitObj.transform.position.x), transform.position.y, transform.position.z);
        rb.velocity = new Vector2(xVector * enemySpeed, -gravity);
    }

    private void Move() //敵キャラの動き
    {
        if (hitGround == true)
        {
            anim.SetBool("Run", false);
            //Debug.Log("静止します");
            xVector = 0;
            if ((moveRight == true && !playerOnRight)
                || (moveRight == false && playerOnRight))
            {
                hitGround = false;
            }
        }
        else if (playerHit && !isAttack)
        {
            if (backTime < 0.3f)
            {
                enemySpeed += 2f;
                if (playerOnRight)
                {
                    xVector = -1;
                    //霧散状態アニメーションを追加
                }
                else
                {
                    xVector = 1;
                }
            }
            else
            {
                backTime = 0.0f;
                playerHit = false;
                enemySpeed = beforeSpeed;
            }
            backTime += Time.deltaTime;
        }
        else
        {
            if (transform.position.x >= player.position.x - 20 && transform.position.x <= player.position.x + 20)
            {
                anim.SetBool("Run", false);
                xVector = 0;
            }
            else
            {
                anim.SetBool("Run", true);
                if (transform.position.x < player.position.x - 20)
                {

                    moveRight = true;
                    xVector = 1;
                }
                else if (transform.position.x > player.position.x + 20)
                {
                    moveRight = false;
                    xVector = -1;
                }
                transform.localScale = new Vector3(xVector * Math.Abs(transform.localScale.x), transform.localScale.y);
            }
        }
    }


    public void blink()//点滅消滅
    {
        isBlink = true;
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
            //Debug.Log("blinkEnd");
            blinkTime = 0f;
            continueTime = 0f;
            sr.enabled = true;
            isBlink = false;
            isStamped = false;
        }
        else
        {
            blinkTime += Time.deltaTime;
            continueTime += Time.deltaTime;
        }
    }

    public void bossAttack()
    {
        if (time < 1f)
        {
            anim.SetBool("Run", false);
            AttackAnimFin = false;

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

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.collider.tag == "player")
        {
            playerHit = true;
        }
    }
}
