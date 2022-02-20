using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float myScore = 0.1f;
    public PlayerTriggerCheck playerCheck;
    public EnemyBehavior enemy = null;
    private SpriteRenderer sr;
    private BoxCollider2D box;
    private Rigidbody2D rb;
    private bool pop = false;
    private bool added = false;
    private bool enemyDead = false;

    private void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        box = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (playerCheck.isOn){
            sr.enabled = false;
            box.enabled = false;
            if (!added)
                {
                    GameManager.instance.hpNum += myScore;
                    added = true;
                }   
        }

        if(enemy != null && !enemyDead)
        {
            if (!enemy.playerStepOn)
            {
                rb.MovePosition(enemy.transform.position);
                sr.enabled = false;
                box.enabled = false;
            }
            else
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
                enemyDead = true;
                
            }
        }
    }
}
