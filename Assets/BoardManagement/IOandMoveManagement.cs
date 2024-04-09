using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public partial class ManageBoard
{
    /// <summary>
    /// Manages the next move in the game, including controlling piece movement and AI decisions.
    /// </summary>
    private void nextMove()
    {
        if (gameRestart)
        {
            foreach (var piece in pieces)
            {
                if (piece != null) piece.canMove = false;
            }
            return;
        }
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

        moves = MoveCalculator.generateAllMoves(board, whiteTurn, lastMove, castlesAllowed); // specifically, this probably makes the previous check useless, because pieces without moves shouldn't be able to move
                                                                                             // actually no, the previous check controls player input, depending on whether it's ai turn or nay
                                                                                             // gotta also make a lister out of it instead of running it twice for ai
        if ((whiteTurn && isWhiteAI) || (!whiteTurn && isBlackAI))
        {
            Move epicMove;
            if (whiteTurn)
            {
                epicMove = whiteAI.getMove(true, board, MoveCalculator.generateAllMovesListInterestingFirst(board, true, lastMove, castlesAllowed));
            }
            else
            {
                epicMove = blackAI.getMove(false, board, MoveCalculator.generateAllMovesListInterestingFirst(board, false, lastMove, castlesAllowed));
            }
            MakeMove(epicMove);
            checkKingDeath(epicMove);
        }

        whiteTurn = !whiteTurn;
    }

    /// <summary>
    /// Highlights available squares for movement.
    /// </summary>
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

    /// <summary>
    /// Removes highlights from squares.
    /// </summary>
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

    /// <summary>
    /// Checks if the piece color is white.
    /// </summary>
    public static bool isWhite(string s)
    {
        return char.IsLower(s[0]);
    }

    /// <summary>
    /// Extracts move information and updates game state accordingly.
    /// </summary>
    public void extractMove(Vector3 from, Vector3 to, PieceBehaviour who)
    {
        int posxFrom = Convert.ToInt32(from.x - startpositionX);
        int posyFrom = Convert.ToInt32(from.y - startpositionY);
        int posxTo = Convert.ToInt32(to.x - startpositionX);
        int posyTo = Convert.ToInt32(to.y - startpositionY);

        lastMove = new Move(posxTo - posxFrom, posyTo - posyFrom, posxFrom, posyFrom, who.name, board[posxTo, posyTo]);
        checkKingDeath(board[posxTo, posyTo]);
        checkEnPassant(posxFrom, posyFrom, posxTo - posxFrom, posyTo - posyFrom, who.name);
        checkCastlesChange(posxFrom, posyFrom, posxTo - posxFrom, posyTo - posyFrom, who.name);
        checkCastleDone(posxFrom, posyFrom, posxTo - posxFrom, posyTo - posyFrom, who.name);

        pieces[posxFrom, posyFrom] = null;
        pieces[posxTo, posyTo] = who;

        board[posxTo, posyTo] = board[posxFrom, posyFrom];
        board[posxFrom, posyFrom] = "empty";

        nextMove();
    }

    /// <summary>
    /// Checks for en passant move possibility.
    /// </summary>
    private void checkEnPassant(int sx, int sy, int dx, int dy, string who)
    {
        if (who.ToLower() == "pawn") // this kinda sucks
        {
            if (Math.Abs(dx) > 0 && board[sx + dx, sy + dy] == "empty")
            {
                eatPiece(sx + dx, sy);
            }
        }
    }

    /// <summary>
    /// Updates castling availability.
    /// </summary>
    private void checkCastlesChange(int sx, int sy, int dx, int dy, string who)
    {
        if (who.ToLower() == "king") castlesAllowed.voidUniversal(who);
        if (who.ToLower() == "rook")
        {
            if (isWhite(who))
            {
                if (sx == 0) castlesAllowed.leftWhite = false;
                else castlesAllowed.rightWhite = false;
            }
            else
            {
                if (sx == 0) castlesAllowed.leftBlack = false;
                else castlesAllowed.rightBlack = false;
            }
        }
    }

    /// <summary>
    /// Performs castling if the move is a castle.
    /// </summary>
    private void checkCastleDone(int sx, int sy, int dx, int dy, string who)
    {
        if (who.ToLower() == "king" && Math.Abs(dx) > 1)
        {
            if (dx < 0) // left castle
            {
                pieces[0, sy].move(sx - 1, 0);

                pieces[sx - 1, sy] = pieces[0, sy];
                pieces[0, sy] = null;
                board[sx - 1, sy] = board[0, sy];
                board[0, sy] = "empty";
            }
            else
            {
                pieces[7, sy].move(sx + 1 - 7, 0);

                pieces[sx + 1, sy] = pieces[7, sy];
                pieces[7, sy] = null;
                board[sx + 1, sy] = board[7, sy];
                board[7, sy] = "empty";
            }
        }
    }

    /// <summary>
    /// Removes a piece from the board and game.
    /// </summary>
    private void eatPiece(int x, int y)
    {
        board[x, y] = "empty";
        Destroy(pieces[x, y].gameObject);
    }

    /// <summary>
    /// Makes a move on the board.
    /// </summary>
    private void MakeMove(Move mv)
    {
        int targX = mv.startx + mv.dx;
        int targY = mv.starty + mv.dy;
        if (mv.additionalTargets != null)
        {

            foreach (Move target in mv.additionalTargets)
            {
                if (mv.piece.ToLower() == "king") MakeMove(target); // bruh
                else eatPiece(target.startx + target.dx, target.starty + target.dy);
            }
        }

        checkCastlesChange(mv.startx, mv.starty, mv.dx, mv.dy, mv.piece);

        if (board[targX, targY] != "empty") eatPiece(targX, targY);
        board[targX, targY] = board[mv.startx, mv.starty];
        board[mv.startx, mv.starty] = "empty";
        pieces[targX, targY] = pieces[mv.startx, mv.starty];
        pieces[mv.startx, mv.starty] = null;

        pieces[targX, targY].move(mv.dx, mv.dy);

        if (board[targX, targY].ToLower() == "pawn" && (isWhite(board[targX, targY]) ? targY == 7 : targY == 0))
        {
            promote(targX, targY, "queen");
        }
        lastMove = mv;
    }

    /// <summary>
    /// Removes a piece from the corresponding list.
    /// </summary>
    public void removePieceFromLists(PieceBehaviour pc)
    {
        if (pc.isWhite)
        {
            whitePieces.Remove(pc);
        }
        else
        {
            blackPieces.Remove(pc);
        }
    }

    /// <summary>
    /// Adds a piece to the corresponding list.
    /// </summary>
    public void addPieceToLists(PieceBehaviour pc)
    {
        if (pc.isWhite)
        {
            whitePieces.Add(pc);
        }
        else
        {
            blackPieces.Add(pc);
        }
    }

    /// <summary>
    /// Stops all piece movements.
    /// </summary>
    public void triggerStop()
    {
        foreach (var pieces in pieces)
        {
            if (pieces != null) pieces.canMove = false;
        }
    }
}
