using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeIsEnemy : MonoBehaviour
{
    private bool isEnemy;

    public bool IsEnemy()
    {
        return isEnemy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy" || collision.tag == "Boss")
        {
            Debug.Log("引き飛ぶ");
            isEnemy = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.tag == "Boss")
        {
            isEnemy = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("引き跳びおわり");
        if (collision.tag == "Enemy" || collision.tag == "Boss")
        {
            isEnemy = false;
        }
    }
}