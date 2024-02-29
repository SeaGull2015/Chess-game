using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceBehaviour : MonoBehaviour
{
    public BoxCollider2D thisCollider;
    public SpriteRenderer thisSpriteRenderer;
    public Sprite[] sprites;
    public GameObject boardManager;
    public Color lightUpColor = new Color(20, 20, 20);
    public bool isWhite;
    private ManageBoard board;
    private string figureType = "pawn";
    private string[] allowedTypes = { "pawn", "knight", "bishop", "rook", "queen", "king" }; // from https://www.chess.com/terms/chess-pieces
    private Vector3 initPoint;
    private Vector3 clickDragOffset;
    private GameObject collisionPiece;
    private bool isCollided = false;

    // Start is called before the first frame update
    void Start()
    {
        boardManager = GameObject.FindWithTag("boardManager"); ;
        board = boardManager.GetComponent<ManageBoard>();
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
    void OnMouseDown()
    {
        initPoint = transform.position;
        clickDragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        board.lightUpSquare(initPoint, lightUpColor); // it is pretty illogical that pawn defines what colour the square should be, not the board manager, but I don't want to breed light up functions for different cases
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - clickDragOffset;
    }

    private void OnMouseUp()
    {
        if (isCollided)
        {
            transform.position = collisionPiece.transform.position - new Vector3(0,0,1);
            eat();
            move();
        }
        else
        {
            transform.position = initPoint;
        }
        board.lightDownSquare(initPoint);
    }

    private void eat()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, (thisCollider.size.x + thisCollider.size.y) / 2 * 0.64f);
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
    }

    private void move()
    {
        return;
    }

    private void eat(GameObject Piece)
    {
        Destroy(Piece);
    }

    public string getType () { return figureType; }
    public bool setType(string type) { 
        if (allowedTypes.Contains(type))
        {
            figureType = type;
            updateSprite();
            return true;
        }
        else { return false; }
    }

    void updateSprite()
    {
        thisSpriteRenderer.sprite = sprites[Array.FindIndex(allowedTypes, x => x == figureType)];
        if (isWhite)
        {
            thisSpriteRenderer.color = Color.white;
        }
        else thisSpriteRenderer.color = Color.grey;   
    }
}
