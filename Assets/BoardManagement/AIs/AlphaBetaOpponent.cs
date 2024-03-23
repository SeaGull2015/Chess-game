using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AlphaBetaOpponent : AItemplate
{
    private virtualBoard vboard = new virtualBoard();
    int iterCounter = 0;
    public int searchDepth = 6;
    bool amWhite = true;
    int aggresionMod = 1;
    private Dictionary<string, int> pieceValuePairs = new Dictionary<string, int>() // too bad the only way to make a const dictionary is using a switch case, huh?
    {
        {"pawn", 10},
        {"knight", 25},
        {"bishop", 25},
        {"rook", 40},
        {"queen", 100},
        {"king", 2000},
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
            if (eval >= beta) return beta;
            alpha = math.max(eval, alpha);
        }

        return alpha;
    }

    public override Move getMove(bool thisWhite, string[,] brd, List<Move> mvs/*, List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces, PieceBehaviour[,] pieces*/)
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

            tres = -search(searchDepth, -res, 0, !thisWhite);
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

    public AlphaBetaOpponent()
    {

    }
    ~AlphaBetaOpponent()
    {

    }
}