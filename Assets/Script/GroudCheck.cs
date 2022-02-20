using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroudCheck : MonoBehaviour
{
    public bool checkPlatformGround;
    private string gTag = "Ground";
    private string platformTag = "MovingGround";
    private bool isGround;
    private bool isGroundEnter, isGroundStay, isGroundExit;
    private bool isMoving;

    public bool IsGround()
    {
        if(isGroundEnter || isGroundStay)
        {
            isGround = true;
        }else if (isGroundExit)
        {
            isGround = false;
        }

        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;
        return isGround;
    }

    public bool IsMovingGround()
    {
 
        return isMoving;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == gTag)
        {
            isGroundEnter = true;
           
        }else if(checkPlatformGround && collision.tag == platformTag)
        {
            isGroundEnter = true;

            isMoving = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == gTag)
        {
            isGroundStay = true;
           
        }
        else if (checkPlatformGround && collision.tag == platformTag)
        {
            isGroundStay = true;

            isMoving = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == gTag)
        {
            isGroundExit = true;
        }
        else if (checkPlatformGround && collision.tag == platformTag)
        {
            isGroundExit = true;
        }
    }
}
