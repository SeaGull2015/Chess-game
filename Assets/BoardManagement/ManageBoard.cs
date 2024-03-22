using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
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
    public EvalOpponent blackAI = new EvalOpponent();
    public EvalOpponent whiteAI = new EvalOpponent();

    private string[,] board = new string[8,8];
    private SquareBehaviour[,] squares = new SquareBehaviour[8,8];
    private PieceBehaviour[,] pieces = new PieceBehaviour[8,8];
    private List<PieceBehaviour> whitePieces = new List<PieceBehaviour>();
    private List<PieceBehaviour> blackPieces = new List<PieceBehaviour>();
    private List<Move>[,] moves = new List<Move>[8,8];
    private string defStart = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private bool[] castles = new bool[4]
    {
        false, false, false, false
    }; // left black, right black, left white, right white.
    private System.Random rnd = new System.Random();
    private float time = 0; 


    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if ((!whiteTurn && isWhiteAI) || (whiteTurn && isBlackAI))
        {
            if (time > timeBetweenAIMoves)
            {
                nextMove();
                time = 0;
            }
        }
    }
}
