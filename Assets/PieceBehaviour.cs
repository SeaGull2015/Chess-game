using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This class manages the behaviour of a select piece on the board.
/// </summary>
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

    /// <summary>
    /// Sets up links to board manager and UI objects, sets up the position of the piece.
    /// </summary>
    void Start()
    {
        boardManager = GameObject.FindWithTag("boardManager"); ;
        board = boardManager.GetComponent<ManageBoard>();
        name = isWhite ? figureType : figureType.ToUpper();
        board.addPieceToLists(this);
        target = transform.position;

        dropDown = GameObject.FindObjectsOfType<promotionSelectScript>(true)[0]; 
    }
    /// <summary>
    /// Removes the piece from the board's piece lists upon destruction.
    /// </summary>
    private void OnDestroy()
    {
        board.removePieceFromLists(this);
    }

    /// <summary>
    /// Moves the piece according to its movement rules.
    /// </summary>
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

    /// <summary>
    /// Handles the promotion of the piece.
    /// </summary>
    /// <param name="result">The result of the promotion.</param>
    public void recallPromotion(string result)
    {
        board.promote(this, result);
        selectionInProgress = false;
    }

    /// <summary>
    /// Triggered when the piece collides with another collider.
    /// </summary>
    /// <param name="other">The collider the piece collided with.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        isCollided = true;
        collisionPiece = other.gameObject;
    }

    /// <summary>
    /// Triggered when the piece stops colliding with another collider.
    /// </summary>
    /// <param name="other">The collider the piece was colliding with.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        isCollided = false;
    }

    /// <summary>
    /// Triggered when the mouse button is pressed down while over the piece.
    /// </summary>
    void OnMouseDown()
    {
        if (canMove && !selectionInProgress)
        {
            initPoint = transform.position;
            clickDragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            board.lightUpSquares(initPoint);
        }
    }

    /// <summary>
    /// Triggered when the mouse is dragged while over the piece.
    /// </summary>
    private void OnMouseDrag()
    {
        if (canMove && !selectionInProgress) transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - clickDragOffset;
    }

    /// <summary>
    /// Triggered when the mouse button is released after being pressed down while over the piece.
    /// </summary>
    private void OnMouseUp()
    {
        if (canMove && !selectionInProgress)
        {
            board.lightDownSquares(initPoint);
            if (isCollided && (collisionPiece.transform.position - new Vector3(0, 0, 1) != initPoint) && board.checkeMovePossibility(initPoint, collisionPiece.transform.position, this))
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

    /// <summary>
    /// Eats the collided piece if applicable.
    /// </summary>
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

    /// <summary>
    /// Moves the piece by the given amount.
    /// </summary>
    /// <param name="dx">The amount to move along the x-axis.</param>
    /// <param name="dy">The amount to move along the y-axis.</param>
    public void move(int dx, int dy)
    {
        target = transform.position + new Vector3(dx, dy, 0);
        movementInProgress = true;
    }

    /// <summary>
    /// Eats a specific piece.
    /// </summary>
    /// <param name="Piece">The piece to eat.</param>
    private void eat(GameObject Piece)
    {
        Destroy(Piece);
    }

    /// <summary>
    /// Retrieves the type of the piece.
    /// </summary>
    /// <returns>The type of the piece.</returns>
    public string getType() { return figureType; }

    /// <summary>
    /// Sets the type of the piece.
    /// </summary>
    /// <param name="type">The type to set.</param>
    /// <returns>True if the type was successfully set, otherwise false.</returns>
    public bool setType(string type)
    {
        if (allowedTypes.Contains(type))
        {
            figureType = type;
            gameObject.name = type;
            updateSprite();
            return true;
        }
        else { return false; }
    }

    /// <summary>
    /// Sets the color of the piece.
    /// </summary>
    /// <param name="color">The color to set.</param>
    public void setColor(Color color)
    {
        thisSpriteRenderer.color = color;
    }

    /// <summary>
    /// Updates the sprite of the piece.
    /// </summary>
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
