using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Cinemachine;
using UnityEngine.UI;

public class ChangeBackGroundColor : MonoBehaviour
{

    public Tilemap Ground;
    public List<SpriteRenderer> sr;
    public List<Image> UIs;
    public GameObject changePos;
    public GameObject p;
    public Camera cam;
    public GameObject bl;

    private float colorNum = 0.99f;
    private float BackColorR;
    private float BackColorG;
    private float BackColorB;

    private void Start()
    {
        bl.SetActive(false);
        BackColorR = cam.backgroundColor.r;
        BackColorG = cam.backgroundColor.g;
        BackColorB = cam.backgroundColor.b;
    }

    private void Update()
    {
        if(p.transform.position.y < changePos.transform.position.y)
        {
            if (colorNum > 0.2f)
            {
                Ground.color = new Color(colorNum, colorNum, colorNum, 1);
                sr.Select(i => i.color = new Color(colorNum, colorNum, colorNum, 1))
                    .ToList();
                colorNum -= 0.003f;
            }

            if(colorNum > 0.85f)
            {
                UIs.Select(i => i.color = new Color(colorNum, colorNum, colorNum, 0.6f))
                    .ToList();
            }

            
            if (BackColorR > 0.08f)
            {
                BackColorR -= 0.003f;
            }
            else if (BackColorG > 0.055f)
            {
                BackColorG -= 0.003f;
                if(BackColorG < 0.06f)
                {
                    bl.SetActive(true);//初めからオンにすると画面がすごいことになるため
                }
            }
            else if (BackColorB > 0.31f)
            {
                BackColorB -= 0.003f;
            }
            cam.backgroundColor = new Color(BackColorR, BackColorG, BackColorB, 0);
        }
        
    }
}
