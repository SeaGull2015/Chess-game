using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ManageBoard : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject square;
    public GameObject boardPiece;
    public float startpositionX = -3.5f;
    public float startpositionY = -3.5f;
    public bool needBoard = true;
    //public bool isPlayerWhite = true;

    private string[,] board = new string[8,8];
    private SquareBehaviour[,] squares = new SquareBehaviour[8,8];
    private PieceBehaviour[,] pieces = new PieceBehaviour[8,8];
    private bool whiteTurn = true;
    private string defStart = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private bool[] castles = new bool[4]
    {
        false, false, false, false
    }; // left black, right black, left white, right white.
    void Start()
    {
        if (needBoard)
        {
            createBoard();
            board = FEN(defStart);
            putPieces(board);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void lightUpSquare(Vector3 where, Color color)
    {
        int x = Convert.ToInt32(where.x - startpositionX);
        int y = Convert.ToInt32(where.y - startpositionY);
        SquareBehaviour target = squares[y, x];
        target.lightUp(color);
    }

    public void lightDownSquare(Vector3 where)
    {
        int x = Convert.ToInt32(where.x - startpositionX);
        int y = Convert.ToInt32(where.y - startpositionY);
        SquareBehaviour target = squares[y, x];
        target.updateColor();
    }

    bool isWhite(string s)
    {
        return char.IsLower(s[0]);
    }

    void createBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject sq = Instantiate(square, transform.position + new Vector3(i + startpositionX, j + startpositionY, +1), transform.rotation);
                sq.name = "square " + (i+j).ToString();
                SquareBehaviour sqBehav = sq.GetComponent<SquareBehaviour>();
                sqBehav.isLight = (i + j) % 2 == 0;
                sqBehav.updateColor();

                squares[j, i] = sqBehav;
            }
        }
    }

    string[,] FEN(string FENinp)
    {
        int x = 0, y = 0;
        string tempPiece = "";
        string[,] res = new string[8,8];
        Dictionary<char, string> pieceTypes = new Dictionary<char, string>
        {
            {'p', "pawn"},
            {'n', "knight"},
            {'b', "bishop"},
            {'r', "rook"},
            {'q', "queen"},
            {'k', "king"},
        };

        int stage = 0;
        foreach (char c in FENinp)
        {
            if (c == ' ')
            {
                stage++;
                continue;
            }
            if (stage == 0)
            {
                if (char.IsNumber(c))
                {
                    int t = int.Parse(c.ToString());
                    while (t > 0)
                    {
                        res[y, x] = "empty";
                        x++;
                        t--;
                    }
                }
                else if (pieceTypes.ContainsKey(char.ToLower(c)))
                {
                    tempPiece = pieceTypes[char.ToLower(c)];
                    if (char.IsUpper(c))
                    {
                        tempPiece = tempPiece.ToUpper(); // black
                    }
                    res[y, x] = tempPiece;
                    x++;
                }
                else if (c == '/')
                {
                    x = 0;
                    y++;
                }
                if (y > 7)
                {
                    throw new Exception("wrong fen string");
                    break;
                }
            }
            if (stage == 1)
            {
                if (c == 'w') whiteTurn = true;
                else if (c == 'b') whiteTurn = false;
            }
            if (stage == 2) 
            { 
                if (c == '-')
                {
                    castles = new bool[4] { false, false, false, false };
                }
                else if (c == 'q') { castles[0] = true; }
                else if (c == 'k') { castles[1] = true; }
                else if (c == 'Q') { castles[2] = true; }
                else if (c == 'K') { castles[3] = true; }
            }
            // there is other stuff in FEN, but I don't want to implement it.
        }

        return res;
    }

    void putPieces(string[,] tboard)
    {
        for (int i = 0; i < tboard.GetLength(0); i++)
        {
            for(int j = 0; j < tboard.GetLength(1); j++)
            {
                if (tboard[j, i] == "empty") continue;
                GameObject piece = Instantiate(boardPiece, transform.position + new Vector3(i + startpositionX, j + startpositionY), transform.rotation);
                PieceBehaviour pcBehaviour = piece.GetComponent<PieceBehaviour>();
                pieces[Convert.ToInt32(j), Convert.ToInt32(i)] = pcBehaviour;
                if (isWhite(tboard[j, i]))
                {
                    pcBehaviour.isWhite = true;
                }
                if (!pcBehaviour.setType(tboard[j, i].ToLower()))
                {
                    throw new Exception("bad type when putting down pieces");
                }
            }
        }
        nextMove();
    }

    void nextMove()
    {
        foreach (var piece in pieces)
        {
            if (piece == null) continue;
            if (piece.isWhite == whiteTurn /*&& piece.isWhite == isPlayerWhite*/) {
                piece.canMove = true;
            }
            else
            {
                piece.canMove = false;
            }
        }
        whiteTurn = !whiteTurn;
    }

    public void extractMove(Vector3 from, Vector3 to, PieceBehaviour who)
    {
        int posxFrom = Convert.ToInt32(from.x - startpositionX);
        int posyFrom = Convert.ToInt32(from.y - startpositionY);
        int posxTo = Convert.ToInt32(to.x - startpositionX);
        int posyTo = Convert.ToInt32(to.y - startpositionY);

        pieces[posyFrom, posxFrom] = null;
        pieces[posyTo, posxTo] = who;

        nextMove();
    }
}
