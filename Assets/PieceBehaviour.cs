using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{
    public BoxCollider2D thisCollider;
    private Vector3 initPoint;
    private Vector3 clickDragOffset;
    private GameObject collisionPiece;
    private bool isCollided = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        isCollided = true;
        collisionPiece = other.gameObject;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        isCollided = false;
    }
    private void eat()
    {
        LayerMask tempL = thisCollider.excludeLayers;
        //thisCollider.excludeLayers = new LayerMask();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, (thisCollider.size.x + thisCollider.size.y)/2 * 0.64f);
        if (colliders.Length > 1)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject != gameObject && collider.gameObject.CompareTag("Piece"))
                {
                    Destroy(collider.gameObject);
                }
            }
        }
        thisCollider.excludeLayers = tempL;
    }
    void OnMouseDown()
    {
        initPoint = transform.position;
        clickDragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - clickDragOffset;
    }

    private void OnMouseUp()
    {
        if (isCollided)
        {
            transform.position = collisionPiece.transform.position;
            eat();
        }
        else
        {
            transform.position = initPoint;
        }
    }
}
