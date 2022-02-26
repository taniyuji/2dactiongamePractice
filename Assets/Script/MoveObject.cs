using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public GameObject[] movePoint;

    public int nowPoint = 0;
    public bool onPlay = false;
    public float speed = 20.0f;
    public bool stopMove = false;
    public bool Move = true;

    private Rigidbody2D rb = null;
    private GameObject pObj;
    private Player player;
    private bool returnPoint = false;
    private Vector2 oldPos = Vector2.zero;
    private Vector2 mvVelocity = Vector2.zero;
    private bool playerOn = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(movePoint != null && movePoint.Length > 0 && rb != null)//オブジェクトを初期位置に設定
        {
            rb.position = movePoint[0].transform.position;
            oldPos = rb.position;
        }
        pObj = GameObject.Find("Player");
        player = pObj.GetComponent<Player>();
    }

    public Vector2 GetVelocity()//外部からこのオブジェクトのvelocityを入手
    {
        return mvVelocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movePoint != null && movePoint.Length > 1 && rb != null)
        {
            if (Move == true)
            {
                if (!returnPoint)//帰ってない場合
                {
                    Vector2 toVector;
                    int nextPoint;
                    if (onPlay && (player.isDead || GameManager.instance.isFallDead))//onPlayでプレイヤーが死亡した場合
                    {
                        toVector = new Vector2(movePoint[0].transform.position.x, movePoint[0].transform.position.y);
                        nowPoint = 0;
                        playerOn = false;
                        rb.MovePosition(toVector);
                    }
                    else if (!onPlay || (onPlay && playerOn))//プレイヤーが死亡していない場合
                    {
                        nextPoint = nowPoint + 1;
                        if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                        {
                            toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);
                        }
                        else
                        {
                            toVector = movePoint[nextPoint].transform.position;
                            ++nowPoint;
                        }
                        rb.MovePosition(toVector);
                    }
                    if (nowPoint + 1 > movePoint.Length - 1)
                    {
                        if (stopMove)
                        {
                            Move = false;
                        }
                        else
                        {
                            returnPoint = true;
                        }
                    }
                }
                else
                {
                    int nextPoint;
                    Vector2 toVector;
                    if (onPlay && (player.isDead || GameManager.instance.isFallDead))
                    {
                        toVector = new Vector2(movePoint[0].transform.position.x, movePoint[0].transform.position.y);
                        nowPoint = 0;
                        rb.MovePosition(toVector);
                        playerOn = false;
                    }
                    else if (!onPlay || (onPlay && playerOn))
                    {
                        nextPoint = nowPoint - 1;
                        if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                        {
                            toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);
                        }
                        else
                        {
                            toVector = movePoint[nextPoint].transform.position;
                            --nowPoint;
                        }
                        rb.MovePosition(toVector);
                    }
                    if (nowPoint - 1 < 0)
                    {
                        returnPoint = false;
                    }
                }
            }
        }
        mvVelocity = (rb.position - oldPos) / Time.deltaTime;
        oldPos = rb.position;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "player")
        {
            playerOn = true;
        }
    }
}
