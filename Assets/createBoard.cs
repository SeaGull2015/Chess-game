using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class createBoard : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject square;
    public float startpositionX = -3.5f;
    public float startpositionY = -3.5f;
    void Start()
    {
        for (float i = 0; i < 8; i++)
        {
            for(float j = 0; j < 8; j++)
            {
                GameObject sq = Instantiate(square, transform.position + new Vector3(i + startpositionX, j + startpositionY), transform.rotation);
                SquareBehaviour sqBehav = sq.GetComponent<SquareBehaviour>();
                sqBehav.isLight = (i + j) % 2 == 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
