using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;


/// <summary>
/// Manages the chessboard and game logic.
/// </summary>
public partial class ManageBoard : MonoBehaviour
{
    public GameObject square;
    public GameObject boardPiece;
    public float startpositionX = -3.5f;
    public float startpositionY = -3.5f;
    public bool needBoard = true;
    public Color lightSquareUpColorSelfy = new Color(20, 20, 20);
    public Color lightSquareUpColorEmpty = new Color(20, 20, 20);
    public Color lightSquareUpColorEnemy = new Color(20, 20, 20);
    public bool isWhiteAI = true;
    public bool isBlackAI = true;
    public bool isPlayerWhite = true;
    public bool whiteTurn = true;
    public float timeBetweenAIMoves = 0f;
    public AItemplate blackAI = new EvalOpponent(2, false);
    public AItemplate whiteAI = new EvalOpponent(2, true);
    public VictoryTextLogic victoryTexter;
    public TMP_Text indexPrefab;
    public Canvas canvi;

    private string[,] board = new string[8, 8];
    private SquareBehaviour[,] squares = new SquareBehaviour[8, 8];
    private PieceBehaviour[,] pieces = new PieceBehaviour[8, 8];
    private List<PieceBehaviour> whitePieces = new List<PieceBehaviour>();
    private List<PieceBehaviour> blackPieces = new List<PieceBehaviour>();
    private List<Move>[,] moves = new List<Move>[8, 8];
    private int moveCounter;
    private string defStart = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private Castling castlesAllowed;
    private System.Random rnd = new System.Random();
    private bool gameRestart = false;
    private int timeTillRestart = 5;
    private int timeTillEffect = 1;
    private bool effectDone = false;
    private float time = 0;

    private Move lastMove;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (gameRestart)
        {
            if (time > timeTillEffect)
            {
                if (!effectDone) gameoverEffect();
                if (time > timeTillRestart)
                {
                    Physics2D.gravity = new Vector2();
                    SceneManager.LoadScene("mainMenu");
                }
            }
        }
        else if ((!whiteTurn && isWhiteAI) || (whiteTurn && isBlackAI))
        {
            if (time > timeBetweenAIMoves)
            {
                nextMove();
                time = 0;
            }
        }
    }

    /// <summary>
    /// Checks if a pawn needs to be promoted.
    /// </summary>
    /// <param name="who">The pawn to check.</param>
    /// <returns>True if promotion is needed, otherwise false.</returns>
    public bool checkPromotion(PieceBehaviour who)
    {
        if (who.getType().ToLower() != "pawn") return false;
        int posy = Convert.ToInt32(who.transform.position.y - startpositionY);
        if ((who.isWhite && posy == 7) || (!who.isWhite && posy == 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Promotes a pawn to a different piece.
    /// </summary>
    /// <param name="who">The pawn to promote.</param>
    /// <param name="toWhat">The piece type to promote to.</param>
    public void promote(PieceBehaviour who, string toWhat)
    {
        if (!who.allowedTypes.Contains(toWhat)) throw new Exception("failed promotion - incorrect type");
        int posx = Convert.ToInt32(who.initPoint.x - startpositionX);
        int posy = Convert.ToInt32(who.initPoint.y - startpositionY);
        who.setType(toWhat);

        board[posx, posy] = setColour(toWhat, who.isWhite);
    }

    /// <summary>
    /// Promotes a pawn at a specific position to a different piece.
    /// </summary>
    /// <param name="x">The x-coordinate of the pawn.</param>
    /// <param name="y">The y-coordinate of the pawn.</param>
    /// <param name="toWhat">The piece type to promote to.</param>
    public void promote(int x, int y, string toWhat)
    {
        int posx = x;
        int posy = y;
        pieces[x, y].setType(toWhat);

        board[posx, posy] = setColour(toWhat, isWhite(board[x, y]));
    }

    /// <summary>
    /// Sets the color of the piece based on player color.
    /// </summary>
    /// <param name="what">The piece type.</param>
    /// <param name="toWhite">Whether the piece belongs to the white player.</param>
    /// <returns>The piece type with appropriate casing.</returns>
    private string setColour(string what, bool toWhite)
    {
        if (toWhite) { return what.ToLower(); }
        else return what.ToUpper();
    }

    /// <summary>
    /// Checks if a move is possible for a piece.
    /// </summary>
    /// <param name="from">The starting position of the piece.</param>
    /// <param name="to">The destination position of the piece.</param>
    /// <param name="who">The piece making the move.</param>
    /// <returns>True if the move is possible, otherwise false.</returns>
    public bool checkeMovePossibility(Vector3 from, Vector3 to, PieceBehaviour who)
    {
        int posxFrom = Convert.ToInt32(from.x - startpositionX);
        int posyFrom = Convert.ToInt32(from.y - startpositionY);
        int posxTo = Convert.ToInt32(to.x - startpositionX);
        int posyTo = Convert.ToInt32(to.y - startpositionY);

        Move tmove = new Move(posxTo - posxFrom, posyTo - posyFrom, posxFrom, posyFrom, who.name, board[posxTo, posyTo]);

        foreach (Move mv in moves[posxFrom, posyFrom])
        {
            if (mv.Equals(tmove)) return true;
        }
        return false;
    }
}