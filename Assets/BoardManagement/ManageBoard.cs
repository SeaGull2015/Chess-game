using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;


public partial class ManageBoard : MonoBehaviour
{

    // Start is called before the first frame update
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

    private string[,] board = new string[8,8];
    private SquareBehaviour[,] squares = new SquareBehaviour[8,8];
    private PieceBehaviour[,] pieces = new PieceBehaviour[8,8];
    private List<PieceBehaviour> whitePieces = new List<PieceBehaviour>();
    private List<PieceBehaviour> blackPieces = new List<PieceBehaviour>();
    private List<Move>[,] moves = new List<Move>[8,8];
    private int moveCounter;
    private string defStart = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private bool[] castles = new bool[4]
    {
        false, false, false, false
    }; // left black, right black, left white, right white.
    private System.Random rnd = new System.Random();
    private bool gameRestart = false;
    private int timeTillRestart = 5;
    private int timeTillEffect = 1;
    private bool effectDone = false;
    private float time = 0; 


    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (gameRestart)
        {
            if (time > timeTillEffect) // nested ifs cause I don't wanna go in the else if thing if it's not time yet
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
    
    public void promote(PieceBehaviour who, string toWhat)
    {
        if (!who.allowedTypes.Contains(toWhat)) throw new Exception("failed promotion - incorrect type");
        int posx = Convert.ToInt32(who.initPoint.x - startpositionX);
        int posy = Convert.ToInt32(who.initPoint.y - startpositionY);
        who.setType(toWhat);

        board[posx, posy] = setColour(toWhat, who.isWhite);        
    }

    public void promote(int x, int y, string toWhat)
    {
        int posx = x;
        int posy = y;
        pieces[x,y].setType(toWhat);

        board[posx, posy] = setColour(toWhat, isWhite(board[x,y]));
    }

    private string setColour(string what, bool toWhite)
    {
        if (toWhite) { return what.ToLower(); }
        else return what.ToUpper();
    }
}
