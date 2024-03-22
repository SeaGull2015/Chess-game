using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Pair<T1, T2>
{
    public T1 first
    {
        get;
        set;
    }
    public T2 second
    {
        get;
        set;
    }
    public Pair(T1 First, T2 Second)
    {
        first = First;
        second = Second;
    }
}
class virtualBoard
{
    bool whiteTurn = true;
    public string[,] board = new string[8, 8]; // "pawn", "knight", "bishop", "rook", "queen", "king"
    public Dictionary<string, int[]> pieceCountDict = new Dictionary<string, int[]>()
    {
        {"pawn", new int[2]},
        {"knight", new int[2]},
        {"bishop", new int[2]},
        {"rook", new int[2]},
        {"queen", new int[2]},
        {"king", new int[2]},
        {"empty", new int[2]}, //easier than ifs, but a little harmful to speed
    };
    //public int[] pawnsCount = new int[2];
    //public int[] knightCount = new int[2];
    //public int[] bishopCount = new int[2];
    //public int[] rookCount = new int[2];
    //public int[] queenCount = new int[2];
    //public int[] kingCount = new int[2];

    public int whiteIndex = 0;
    public int blackIndex = 1;



    public void virtualMove(Move mv)
    {
        pieceCountDict[board[mv.startx + mv.dx, mv.starty + mv.dy].ToLower()][ManageBoard.isWhite(board[mv.startx + mv.dx, mv.starty + mv.dy]) ? 0 : 1]--;
        if (mv.additionalTargets != null)
        {
            foreach (var targ in mv.additionalTargets)
            {
                board[targ.startx + targ.dx, targ.starty + targ.dy] = "empty";
                pieceCountDict[board[targ.startx + targ.dx, targ.starty + targ.dy].ToLower()][ManageBoard.isWhite(board[mv.startx + mv.dx, mv.starty + mv.dy]) ? 0 : 1]--;
            }
        }

        board[mv.startx + mv.dx, mv.starty + mv.dy] = board[mv.startx, mv.starty];
        board[mv.startx, mv.starty] = "empty";
    }

    public void virtualUnMove(Move mv)
    {
        if (mv.additionalTargets != null)
        {
            foreach (var targ in mv.additionalTargets)
            {
                board[targ.startx + targ.dx, targ.starty + targ.dy] = targ.target;
                pieceCountDict[board[targ.startx + targ.dx, targ.starty + targ.dy].ToLower()][ManageBoard.isWhite(board[mv.startx + mv.dx, mv.starty + mv.dy]) ? 0 : 1]++;
            }
        }
        board[mv.startx, mv.starty] = board[mv.startx + mv.dx, mv.starty + mv.dy];
        board[mv.startx + mv.dx, mv.starty + mv.dy] = mv.target;
        pieceCountDict[board[mv.startx + mv.dx, mv.starty + mv.dy].ToLower()][ManageBoard.isWhite(board[mv.startx + mv.dx, mv.starty + mv.dy]) ? 0 : 1]++;
    }

    public void setState(string[,] brd)
    {
        board = (string[,])brd.Clone();
        recount();
    }

    public void recount()
    {
        pieceCountDict = new Dictionary<string, int[]>()
        {
            {"pawn", new int[2]},
            {"knight", new int[2]},
            {"bishop", new int[2]},
            {"rook", new int[2]},
            {"queen", new int[2]},
            {"king", new int[2]},
            {"empty", new int[2]}, //easier than ifs, but a little harmful to speed
        };
        foreach (var pc in board)
        {
            pieceCountDict[pc.ToLower()][ManageBoard.isWhite(pc) ? 0 : 1]++;
        }
    }

    public virtualBoard(string[,] brd)
    {
        board = (string[,])brd.Clone();
        recount();
    }
    public virtualBoard()
    {
    }

}
public class EvalOpponent
{
    private virtualBoard vboard = new virtualBoard();
    int iterCounter = 0;
    int searchDepth = 4;
    bool amWhite = true;
    int aggresionMod = 1;
    private Dictionary<string, int> pieceValuePairs = new Dictionary<string, int>() // too bad the only way to make a const dictionary is using a switch case, huh?
    {
        {"pawn", 10},
        {"knight", 25},
        {"bishop", 25},
        {"rook", 40},
        {"queen", 100},
        {"king", 200},
        {"empty", 0} // don't care
    };


    private int evaluate()
    {
        if (amWhite)
        {
            return countPieces(vboard.whiteIndex) - countPieces(vboard.blackIndex) * aggresionMod;
        }
        else
        {
            return countPieces(vboard.whiteIndex) * aggresionMod - countPieces(vboard.blackIndex);
        }
    }

    private int countPieces(int colourIndex)
    {
        int rs = 0;
        //vboard.recount(); // something's wrong with counting, so I recount
        //rs += vboard.pawnsCount[colourIndex] * pieceValuePairs["pawn"];
        //rs += vboard.knightCount[colourIndex] * pieceValuePairs["knight"];
        //rs += vboard.bishopCount[colourIndex] * pieceValuePairs["bishop"];
        //rs += vboard.rookCount[colourIndex] * pieceValuePairs["rook"];
        //rs += vboard.queenCount[colourIndex] * pieceValuePairs["queen"];
        //rs += vboard.kingCount[colourIndex] * pieceValuePairs["king"];
        foreach (var pc in vboard.pieceCountDict)
        {
            rs += pc.Value[colourIndex] * pieceValuePairs[pc.Key];
        }
        return rs;
    }

    int search(int depth, int beta, int alpha, bool whiteTurn, List<Move> possibleMoves = null)
    {
        iterCounter++;
        if (depth == 0) return evaluate();


        if (possibleMoves == null) possibleMoves = MoveCalculator.generateAllMovesListInterestingFirst(vboard.board, whiteTurn);
        if (possibleMoves.Count == 0) return 0;


        foreach (Move mv in possibleMoves)
        {
            vboard.virtualMove(mv);

            int eval = -search(depth - 1, -alpha, -beta, !whiteTurn);

            vboard.virtualUnMove(mv);
            if (alpha > beta) return alpha;
            alpha = math.max(eval, alpha);
        }

        return alpha;
    }
    public Move getMove(bool thisWhite, Move lastMove, List<Move> mvs/*, List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces, PieceBehaviour[,] pieces*/)
    {
        var st = Time.realtimeSinceStartup;
        if (!lastMove.nullMove) vboard.virtualMove(lastMove);
        amWhite = thisWhite;

        int res = -100000;
        int tres = -100000;
        int index = 0;
        int n = 0;
        int beth = -res;

        foreach (Move mv in mvs)
        {
            vboard.virtualMove(mv);

            tres = -search(searchDepth, -res, evaluate(), !thisWhite);
            if (tres > res)
            {
                beth = res;
                res = tres;
                index = n;
            }

            vboard.virtualUnMove(mv);
            n++;
        }
        Debug.Log(iterCounter);
        iterCounter = 0;
        Debug.Log(Time.realtimeSinceStartup - st);
        return mvs[index];
    }

    public EvalOpponent(string[,] brd)
    {
        vboard.setState(brd);
    }
    ~EvalOpponent()
    {

    }

    // better check if that works lol
    public Move getMove(bool thisWhite, string[,] brd, List<Move> mvs/*, List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces, PieceBehaviour[,] pieces*/)
    {
        var st = Time.realtimeSinceStartup;
        vboard.setState(brd);
        amWhite = thisWhite;

        int res = -100000;
        int tres = -100000;
        int index = 0;
        int n = 0;
        int beth = -res;

        foreach (Move mv in mvs)
        {
            vboard.virtualMove(mv);

            tres = -search(searchDepth, -beth, evaluate(), !thisWhite);
            if (tres > res)
            {
                beth = res;
                res = tres;
                index = n;
            }

            vboard.virtualUnMove(mv);
            n++;
        }
        Debug.Log(iterCounter);
        iterCounter = 0;
        Debug.Log(Time.realtimeSinceStartup - st);
        return mvs[index];
    }
}