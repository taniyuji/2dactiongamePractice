using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckPlayerPenetration : MonoBehaviour
{
    public EdgeCollider2D head;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 11)
        {
            //Debug.Log("プレイヤー入ってきたよ");
            //上から踏みつけに行った場合も反応してしまうため、タイミングをずらす
            DelayCorutine(0.02f, () =>
            {
                head.enabled = false;
            });   
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player" || collision.gameObject.layer == 11)
        {
            //Debug.Log("プレイヤーいるよ");
                head.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            //Debug.Log("プレイヤーでたよ");
            head.enabled = true;
        }
    }

    private IEnumerator DelayCorutine(float sec, Action action)
    {
        yield return new WaitForSeconds(sec);
        action?.Invoke();
    }

}
