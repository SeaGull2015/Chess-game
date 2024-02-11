using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehaviour : MonoBehaviour
{
    public bool isLight = false;
    public Color ColorLight = Color.white;
    public Color ColorDark = Color.black;
    public SpriteRenderer render;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLight)
        {
            render.color = ColorLight;
        }
        else
        {
            render.color = ColorDark;
        }
    }
}
