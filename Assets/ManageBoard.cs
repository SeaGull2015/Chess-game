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

    private string[,] board = new string[8,8];
    private string defStart = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
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

    bool isWhite(string s)
    {
        return char.IsLower(s[0]);
    }

    void createBoard()
    {
        for (float i = 0; i < 8; i++)
        {
            for (float j = 0; j < 8; j++)
            {
                GameObject sq = Instantiate(square, transform.position + new Vector3(i + startpositionX, j + startpositionY, +1), transform.rotation);
                SquareBehaviour sqBehav = sq.GetComponent<SquareBehaviour>();
                sqBehav.isLight = (i + j) % 2 == 0;
                sqBehav.updateColor();
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
        foreach (char c in FENinp)
        {
            if (char.IsNumber(c))
            {
                int t = int.Parse(c.ToString());
                while (t > 0)
                {
                    res[y,x] = "empty";
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
            else if (c == ' ') break;

            if (y > 7)
            {
                throw new Exception("wrong fen string");
                break;
            }
        }

        return res;
    }

    void putPieces(string[,] tboard)
    {
        for (int i = 0; i < tboard.GetLength(0); i++)
        {
            for(int j = 0; j < tboard.GetLength(1); j++)
            {
                if (tboard[i, j] == "empty") continue;
                GameObject piece = Instantiate(boardPiece, transform.position + new Vector3(j + startpositionX, i + startpositionY), transform.rotation);
                PieceBehaviour pcBehaviour = piece.GetComponent<PieceBehaviour>();
                if (isWhite(tboard[i, j]))
                {
                    pcBehaviour.isWhite = true;
                }
                if (!pcBehaviour.setType(tboard[i, j].ToLower()))
                {
                    throw new Exception("bad type when putting down pieces");
                }
            }
        }
    }
}
