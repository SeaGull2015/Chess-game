using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public partial class ManageBoard
{
    public void lightUpSquares(Vector3 where)
    {
        int x = Convert.ToInt32(where.x - startpositionX);
        int y = Convert.ToInt32(where.y - startpositionY);
        squares[x, y].lightUp(lightSquareUpColorSelfy);
        foreach (var move in moves[x, y])
        {
            if (board[x + move.dx, y + move.dy] == "empty") squares[x + move.dx, y + move.dy].lightUp(lightSquareUpColorEmpty);
            else squares[x + move.dx, y + move.dy].lightUp(lightSquareUpColorEnemy);
        }
    }

    public void lightDownSquares(Vector3 where)
    {
        int x = Convert.ToInt32(where.x - startpositionX);
        int y = Convert.ToInt32(where.y - startpositionY);
        squares[x, y].updateColor();
        foreach (var move in moves[x, y])
        {
            squares[x + move.dx, y + move.dy].updateColor();
        }
    }

    bool isWhite(string s)
    {
        return char.IsLower(s[0]);
    }


    void nextMove()
    {
        foreach (var piece in pieces)
        {
            if (piece == null) continue;
            if (piece.isWhite == whiteTurn/* && !((whiteTurn && piece.isWhite && isWhiteAI) || (!whiteTurn && !piece.isWhite && isBlackAI))*/)
            { // if the piece colour matches the turn colour and this colour isn't AI controlled
                piece.canMove = true;
            }
            else
            {
                piece.canMove = false;
            }
        }
        for (int i = 0; i < 8; i++) // specifically, this probably makes the previous check useless, because pieces without moves shouldn't be able to move
        { // actually no, the previous check controls player input, depending on whether it's ai turn or nay
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == "empty") continue;
                else if (isWhite(board[i, j]) == whiteTurn)
                {
                    moves[i, j] = getMoves(i, j);
                }
                else
                {
                    moves[i, j] = new List<Move>();
                }
            }
        }

        if ((whiteTurn && isWhiteAI) || (!whiteTurn && isBlackAI))
        {
            Move epicMove = randomOpponent(whiteTurn, board, moves);
            MakeMove(epicMove);
        }

        whiteTurn = !whiteTurn;
    }

    public void extractMove(Vector3 from, Vector3 to, PieceBehaviour who)
    {
        int posxFrom = Convert.ToInt32(from.x - startpositionX);
        int posyFrom = Convert.ToInt32(from.y - startpositionY);
        int posxTo = Convert.ToInt32(to.x - startpositionX);
        int posyTo = Convert.ToInt32(to.y - startpositionY);

        pieces[posxFrom, posyFrom] = null;
        pieces[posxTo, posyTo] = who;

        board[posxTo, posyTo] = board[posxFrom, posyFrom];
        board[posxFrom, posyFrom] = "empty";
        //pieces[posxTo, posyTo].setColor(Color.red);
        //squares[posxTo, posyTo].lightUp(Color.red);

        nextMove();
    }

    private void eatPiece(int x, int y)
    {
        board[x, y] = "empty";
        Destroy(pieces[x, y].gameObject);
    }

    private void MakeMove(Move mv)
    {
        int targX = mv.startx + mv.dx;
        int targY = mv.starty + mv.dy;
        if (mv.additionalTargets != null) // this should be a basis for en passant? idk
        {
            foreach (Move target in mv.additionalTargets)
            {
                eatPiece(target.startx + target.dx, target.starty + target.dy);
            }
        }
        // TODO: add logic for castling aka rokirovka (duh)
        if (board[targX, targY] != "empty") eatPiece(targX, targY);
        board[targX, targY] = board[mv.startx, mv.starty];
        board[mv.startx, mv.starty] = "empty";
        pieces[targX, targY] = pieces[mv.startx, mv.starty];
        pieces[mv.startx, mv.starty] = null;

        pieces[targX, targY].move(mv.dx, mv.dy);
        //nextMove();
    }
}
