using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public GameObject[] movePoint;

    public int nowPoint = 0;
    public bool onPlay = false;

    private Rigidbody2D rb = null;

    private float speed = 20.0f;
    private bool returnPoint = false;
    private Vector2 oldPos = Vector2.zero;
    private Vector2 mvVelocity = Vector2.zero;
    private bool playerOn = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(movePoint != null && movePoint.Length > 0 && rb != null)
        {
            rb.position = movePoint[0].transform.position;
            oldPos = rb.position;
        }
    }

    public Vector2 GetVelocity()
    {
        return mvVelocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (onPlay == true && playerOn == false)
        {
        }
        else if(onPlay == false || (onPlay == true && playerOn == true)){

            if (movePoint != null && movePoint.Length > 1 && rb != null)
            {
                if (!returnPoint)
                {
                    int nextPoint = nowPoint + 1;
                    if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                    {
                        Vector2 toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);

                        rb.MovePosition(toVector);
                    }
                    else
                    {
                        rb.MovePosition(movePoint[nextPoint].transform.position);
                        ++nowPoint;

                        if (nowPoint + 1 >= movePoint.Length)
                        {
                            returnPoint = true;
                        }
                    }
                }
                else
                {
                    int nextPoint = nowPoint - 1;

                    if (Vector2.Distance(transform.position, movePoint[nextPoint].transform.position) > 0.1f)
                    {
                        Vector2 toVector = Vector2.MoveTowards(transform.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);

                        rb.MovePosition(toVector);
                    }
                    else
                    {
                        rb.MovePosition(movePoint[nextPoint].transform.position);
                        --nowPoint;

                        if (nowPoint - 1 < 0)
                        {
                            returnPoint = false;
                        }
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

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "player")
        {
            playerOn = true;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "player")
        {
            playerOn = false;
        }
    }
}
