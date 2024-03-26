using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceBehaviour : MonoBehaviour
{
    public BoxCollider2D thisCollider;
    public Rigidbody2D thisRigidbody;
    public SpriteRenderer thisSpriteRenderer;
    public Sprite[] sprites;
    public GameObject boardManager;
    public bool isWhite;
    public bool canMove = false;
    public AudioSource audioMove;
    public string[] allowedTypes = { "pawn", "knight", "bishop", "rook", "queen", "king" };// from https://www.chess.com/terms/chess-pieces
    public Vector3 initPoint;

    private promotionSelectScript dropDown;
    private ManageBoard board;
    private string figureType = "pawn";

    private Vector3 clickDragOffset;
    private GameObject collisionPiece;
    private bool isCollided = false;
    private float selfSpeed = 4f;
    private Vector3 target;
    private bool movementInProgress = false;
    private bool moveToExtract = false;
    private float extractDelay = 0.1f;
    private float extractionTime = 0;
    private bool selectionInProgress = false;

    // Start is called before the first frame update
    void Start()
    {
        boardManager = GameObject.FindWithTag("boardManager"); ;
        board = boardManager.GetComponent<ManageBoard>();
        name = isWhite ? figureType : figureType.ToUpper();
        board.addPieceToLists(this);
        target = transform.position;

        dropDown = GameObject.FindObjectsOfType<promotionSelectScript>(true)[0]; 
    }
    private void OnDestroy()
    {
        board.removePieceFromLists(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (movementInProgress)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, selfSpeed * Time.deltaTime * ((transform.position - target).magnitude + 1));
            if (transform.position == target)
            {
                audioMove.Play();
                movementInProgress = false;
            }
        }
        if (moveToExtract && !selectionInProgress)
        {
            extractionTime += Time.deltaTime;
            if (extractionTime > extractDelay)
            {
                moveToExtract = false;
                extractionTime = 0;
                board.triggerStop();
                board.extractMove(initPoint, transform.position, this);
            }
        }
    }

    public void recallPromotion(string result)
    {
        board.promote(this, result);
        selectionInProgress = false;
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
        if (canMove)
        {
            initPoint = transform.position;
            clickDragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            board.lightUpSquares(initPoint);
        }
    }

    private void OnMouseDrag()
    {
        if (canMove) transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - clickDragOffset;
    }

    private void OnMouseUp()
    {
        if (canMove)
        {
            board.lightDownSquares(initPoint);
            if (isCollided && (collisionPiece.transform.position - new Vector3(0, 0, 1) != initPoint))
            {
                transform.position = collisionPiece.transform.position - new Vector3(0, 0, 1);
                moveToExtract = true;
                eat();
            }
            else
            {
                transform.position = initPoint;
            }

            if (board.checkPromotion(this))
            {
                dropDown.setNewSelect(Camera.main.WorldToScreenPoint(transform.position).x, Camera.main.WorldToScreenPoint(transform.position).y, this);
                selectionInProgress = true;
            }

            audioMove.Play();
        }
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

    public void move(int dx, int dy)
    {
        target = transform.position + new Vector3(dx, dy, 0);
        movementInProgress = true;
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
            gameObject.name = type;
            updateSprite();
            return true;
        }
        else { return false; }
    }

    public void setColor(Color color)
    {
        thisSpriteRenderer.color = color;
    }
    private void updateSprite()
    {
        thisSpriteRenderer.sprite = sprites[Array.FindIndex(allowedTypes, x => x == figureType)];
        if (isWhite)
        {
            thisSpriteRenderer.color = Color.white;
        }
        else thisSpriteRenderer.color = Color.grey;   
    }


}
