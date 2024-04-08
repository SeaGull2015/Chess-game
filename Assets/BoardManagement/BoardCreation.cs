using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public partial class ManageBoard
{
    void Start()
    {
        setAI();
        if (needBoard)
        {
            createBoard();
            board = FEN(defStart);
            putPieces(board);
        }
        if (isWhiteAI && isBlackAI) { timeBetweenAIMoves = 0.4f; }
        if (isWhiteAI) Invoke("nextMove", 0.1f);
        if (!isWhiteAI && !isBlackAI) Invoke("nextMove", 0.1f);
    }
    void createBoard()
    {
        for (int tY = 0; tY < 8; tY++)
        {
            for (int tX = 0; tX < 8; tX++)
            {
                GameObject sq = Instantiate(square, transform.position + new Vector3(tX + startpositionX, tY + startpositionY, +1), transform.rotation);
                sq.name = "square " + (tY * 8 + tX).ToString();
                SquareBehaviour sqBehav = sq.GetComponent<SquareBehaviour>();
                sqBehav.isLight = (tY + tX) % 2 != 0;
                sqBehav.updateColor();

                squares[tX, tY] = sqBehav;

                if (tX == 0)
                {
                    TMP_Text txt = Instantiate(indexPrefab, Camera.main.WorldToScreenPoint(transform.position + new Vector3(tX + startpositionX - 1, tY + startpositionY, +1)), transform.rotation, canvi.transform);
                    txt.SetText((tY+1).ToString());
                }
                if (tY == 0)
                {
                    TMP_Text txt = Instantiate(indexPrefab, Camera.main.WorldToScreenPoint(transform.position + new Vector3(tX + startpositionX, tY + startpositionY - 1, +1)), transform.rotation, canvi.transform);
                    txt.SetText(((char)('a' + tX)).ToString());
                }
            }
        }
    }

    string[,] FEN(string FENinp)
    {
        int x = 0, y = 0;
        string tempPiece = "";
        string[,] res = new string[8, 8];
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
        bool[] castles = new bool[4]; // left black, right black, left white, right white.
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
                        res[x, y] = "empty";
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
                    res[x, y] = tempPiece;
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
        castlesAllowed = new Castling(castles);

        return res;
    }

    void putPieces(string[,] tboard)
    {
        for (int tY = 0; tY < tboard.GetLength(0); tY++)
        {
            for (int tX = 0; tX < tboard.GetLength(1); tX++)
            {
                if (tboard[tX, tY] == "empty") continue;
                GameObject piece = Instantiate(boardPiece, transform.position + new Vector3(tX + startpositionX, tY + startpositionY), transform.rotation);
                PieceBehaviour pcBehaviour = piece.GetComponent<PieceBehaviour>();
                pieces[tX, tY] = pcBehaviour;
                if (isWhite(tboard[tX, tY]))
                {
                    pcBehaviour.isWhite = true;
                    //whitePieces.Add(pcBehaviour);
                }
                //else
                //{
                //    blackPieces.Add(pcBehaviour);
                //}
                if (!pcBehaviour.setType(tboard[tX, tY].ToLower()))
                {
                    throw new Exception("bad type when putting down pieces");
                }
            }
        }
        //nextMove();
    }

    void setAI()
    {
        string r = PlayerPrefs.GetString("blackAIstr");
        switch (r)
        {
            case "Player":
                isBlackAI = false;
                break;
            case "Easy":
                blackAI = new RandomerOpponent();
                isBlackAI = true;
                break;
            case "Medium":
                blackAI = new AlphaBetaOpponentInt(2, false);
                isBlackAI = true;
                break;
            case "Hard":
                blackAI = new AlphaBetaOpponentInt(4, false);
                isBlackAI = true;
                break;
            default:
                throw new Exception("player pref trouble");
        }
        switch (PlayerPrefs.GetString("whiteAIstr"))
        {
            case "Player":
                isWhiteAI = false;
                break;
            case "Easy":
                whiteAI = new RandomerOpponent();
                isWhiteAI = true;
                break;
            case "Medium":
                whiteAI = new AlphaBetaOpponentInt(2, true);
                isWhiteAI = true;
                break;
            case "Hard":
                whiteAI = new AlphaBetaOpponentInt(4, true);
                isWhiteAI = true;
                break;
            default:
                throw new Exception("player pref trouble");
        }
    }
}
