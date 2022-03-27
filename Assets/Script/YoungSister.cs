using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoungSister : MonoBehaviour
{
    public GameObject player;

    private float posY;
    private PlayerEnding pEnd;
    // Start is called before the first frame update
    void Start()
    {
        posY = gameObject.transform.position.y;
        pEnd = player.GetComponent<PlayerEnding>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (pEnd.playerIn && gameObject.transform.position.y > player.transform.position.y + 1.5)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, posY);
            posY -= 0.03f;
        }
        else if(pEnd.playerIn)
        {
            gameObject.SetActive(false);
        }
    }
}
