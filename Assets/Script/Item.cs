using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public decimal healScore = 0.1m;
    public PlayerTriggerCheck playerCheck;
    public EnemyBehavior enemy = null;
    public BossBehavior boss = null;
    public int bossHpJudge;
    public AudioSource GetItemSound;

    private SpriteRenderer sr;
    private BoxCollider2D box;
    private Rigidbody2D rb;
    private bool pop = false;
    private bool added = false;
    private bool enemyDead = false;
    private bool bossDamaged = false;
    private int bosshp;


    private void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        box = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (boss != null)
        {
            bosshp = boss.bossHp;
        }

        if ((enemy != null && !enemyDead) || (boss != null && !bossDamaged))
        {
            judgeTrigger();
        }
        else
        {
            if (playerCheck.isOn)
            {
                sr.enabled = false;
                box.enabled = false;
                if (GetItemSound != null)
                {
                    GetItemSound.Play();
                }
                if (!added)
                {
                    GameManager.instance.hpNum += healScore;
                    added = true;
                }
            }
        }


    }

    private void judgeTrigger()
    {
        if (enemy != null && !enemy.playerStepOn)
        {
            keepMoving();
        }
        else if (boss != null && !boss.playerStepOn2)
        {
            keepMoving();
        }
        else
        {
            if (enemy != null)
            {
                getEnable();
            }
            else if (boss != null && bosshp == bossHpJudge)
            {
                getEnable();
            }
        }
    }

    private void keepMoving()
    {
        if (enemy != null)
        {
            rb.MovePosition(enemy.transform.position);
        }
        else if (boss != null)
        {
            rb.MovePosition(boss.transform.position);
        }
        sr.enabled = false;
        box.enabled = false;
    }

    private void getEnable()
    {
        sr.enabled = true;
        box.enabled = true;
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
