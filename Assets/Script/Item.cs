using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public decimal healScore = 0.1m;
    public PlayerTriggerCheck playerCheck;
    public EnemyBehavior enemy = null;
    public BossBehavior boss = null;
    public int bossHpJudge;//ボスのHPが指定した数と同じになったら出現
    public AudioSource getItemSE;
   

    private SpriteRenderer sr;
    private BoxCollider2D box;
    private EdgeCollider2D edge;
    private Rigidbody2D rb;
    private bool pop = false;
    private bool added = false;
    private bool enemyDead = false;
    private bool bossDamaged = false;
    private int enemyhp;
    private Vector2 fixedenemyPos;


    private void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        box = gameObject.GetComponent<BoxCollider2D>();
        edge = gameObject.GetComponent<EdgeCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))//ポーズ中は起動させない
        {
            return;
        }

        if (boss != null)//ボスのスクリプトがある場合
        {
            enemyhp = boss.bossHp;//ボスのHPを常に取得
        }else if(enemy != null)
        {
            enemyhp = enemy.enemyHp;
        }
        //エネミーが消滅していないまたはボスがダメージを受けていない場合
        if ((enemy != null && !enemyDead) || (boss != null && !bossDamaged))
        {
            //ボス、またはエネミーについていく。このメソッド内で、enemyDeadとbossDamagedを判定
            judgeTrigger();
        }
        else//エネミーが消滅またはボスがダメージを受けた場合
        {
            if (playerCheck.isOn)//プレイヤーが範囲内に侵入した場合
            {
                if (getItemSE != null)
                {
                    getItemSE.Play();
                }
                if (!added)
                {
                    GameManager.instance.hpNum += healScore;
                    added = true;
                }
                gameObject.SetActive(false);
            }
        }


    }

    private void judgeTrigger()//このオブジェクトの挙動を判定
    {
        //ボスまたはエネミースクリプトがあり、踏まれていない場合
        if ((enemy != null && !enemy.playerStepOn) || (boss != null && !boss.playerStepOn2))
        {
            keepMoving();//ついていく
        }
        else//踏まれた場合
        {
            //エネミースクリプトがある場合またはボススクリプトがあり、指定した数とボスのHPが同じになった場合
            if ((enemy != null && enemyhp < 1) || (boss != null && enemyhp == bossHpJudge))
            {
                getEnable();//出現させる
            }
        }
    }

    private void keepMoving()//対象の敵についていく
    {
        if (enemy != null)
        {
            fixedenemyPos = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
            rb.MovePosition(enemy.transform.position);
        }
        else if (boss != null)
        {
            rb.MovePosition(boss.transform.position);
        }
        sr.enabled = false;
        box.enabled = false;
        edge.enabled = false;
        rb.isKinematic = true;
    }

    private void getEnable()
    {
        sr.enabled = true;
        box.enabled = true;
        edge.enabled = true;
        rb.isKinematic = false;
        if (!pop)
        {
            rb.velocity = new Vector2(-1f, 7f);
            pop = true;
        }
        else
        {
            rb.velocity = new Vector2(0, -5f);
        }
        if (enemy != null)
        {
            enemyDead = true;
        }
        else if (boss != null)
        {
            bossDamaged = true;
        }
    }
}
