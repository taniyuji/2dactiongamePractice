using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionCheck : MonoBehaviour
{
    private EnemyBehavior enm;

    private void Start()
    {
        enm = transform.root.GetComponent<EnemyBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            if (enm != null)
            {
                //Debug.Log("敵の方向を転換");
                enm.DirectionRight = !enm.DirectionRight;
            }
        }
    }
}
