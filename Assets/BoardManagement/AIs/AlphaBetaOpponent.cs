using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// AlphaBetaOpponentInt, but worse in every single way. Good target for deletion.
/// </summary>
public class AlphaBetaOpponent : AItemplate
{
    private virtualBoard vboard = new virtualBoard();
    int iterCounter = 0;
    public int searchDepth = 5; // default value gets overrided during creation
    bool amWhite = true;
    int aggresionMod = 1;
    private Dictionary<string, int> pieceValuePairs = new Dictionary<string, int>() // too bad the only way to make a const dictionary is using a switch case, huh?
    {
        {"pawn", 100},
        {"knight", 250},
        {"bishop", 250},
        {"rook", 400},
        {"queen", 1000},
        {"king", 20000},
        {"empty", 0} // don't care
    };

    private Dictionary<string, int[,]> piecePositionPairs = new Dictionary<string, int[,]>() // too bad the only way to make a const dictionary is using a switch case, huh?
    {
        {"pawn", new int[8,8]},
        {"knight", new int[8,8]},
        {"bishop", new int[8,8]},
        {"rook", new int[8,8]},
        {"queen", new int[8,8]},
        {"king", new int[8,8]},
        {"empty", new int[8,8]} // don't care
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
            eval += piecePositionPairs[mv.piece.ToLower()][mv.starty + mv.dy, mv.startx + mv.dx];
#warning piecePositionPairs eat a lot of performance, please find another way to work with this

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
        if (amWhite != thisWhite) throw new Exception("alpha beta opponent is wrongly assigned colour");

        int res = -1000000;
        int tres = -1000000;
        int index = 0;
        int n = 0;
        int beth = -res;

        foreach (Move mv in mvs)
        {
            vboard.virtualMove(mv);

            tres = -search(searchDepth, 1000000, -1000000, !thisWhite);
            tres += piecePositionPairs[mv.piece.ToLower()][mv.starty + mv.dy, mv.startx + mv.dx];
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

    public AlphaBetaOpponent(int depth, bool isWhite)
    {
        searchDepth = depth;
        amWhite = isWhite;

        if (amWhite && searchDepth % 2 == 0) searchDepth--; // very ugly crutch, but I'm not sure why black only works if searchdepth is % 2 == 0
        if (!amWhite && searchDepth % 2 != 0) searchDepth--; // and white too

        setPiecePositionPairs();


        if (!amWhite)
        {
            var keys = new List<string>(piecePositionPairs.Keys);
            foreach (var val in keys)
            {
                piecePositionPairs[val] = rotatePiecePositionPairs(piecePositionPairs[val]);// I think it should be in-place
            }
        }
    }
    ~AlphaBetaOpponent()
    {

    }

    private void setPiecePositionPairs()
    {
        // analysis of positions is taken from https://www.chessprogramming.org/Simplified_Evaluation_Function , many thanks to the author
        piecePositionPairs["pawn"] = new int[8, 8]
            {{ pieceValuePairs["queen"],  pieceValuePairs["queen"],  pieceValuePairs["queen"],  pieceValuePairs["queen"],  pieceValuePairs["queen"],  pieceValuePairs["queen"],  pieceValuePairs["queen"],  pieceValuePairs["queen"]},
            {50, 50, 50, 50, 50, 50, 50, 50},
            {10, 10, 20, 30, 30, 20, 10, 10},
            { 5,  5, 10, 25, 25, 10,  5,  5},
            { 0,  0,  0, 20, 20,  0,  0,  0},
            { 5, -5,-10,  0,  0,-10, -5,  5},
            { 5, 10, 10,-20,-20, 10, 10,  5},
            { 0,  0,  0,  0,  0,  0,  0,  0}};
        piecePositionPairs["knight"] = new int[8, 8]
            {{-50, -40, -30, -30, -30, -30, -40, -50},
            {-40, -20,   0,   0,   0,   0, -20, -40},
            {-30,   0,  10,  15,  15,  10,   0, -30},
            {-30,   5,  15,  20,  20,  15,   5, -30},
            {-30,   0,  15,  20,  20,  15,   0, -30},
            {-30,   5,  10,  15,  15,  10,   5, -30},
            {-40, -20,   0,   5,   5,   0, -20, -40},
            {-50, -40, -30, -30, -30, -30, -40, -50}};
        piecePositionPairs["bishop"] = new int[8, 8]
            {{-20, -10, -10, -10, -10, -10, -10, -20},
            {-10,   0,   0,   0,   0,   0,   0, -10},
            {-10,   0,   5,  10,  10,   5,   0, -10},
            {-10,   5,   5,  10,  10,   5,   5, -10},
            {-10,   0,  10,  10,  10,  10,   0, -10},
            {-10,  10,  10,  10,  10,  10,  10, -10},
            {-10,   5,   0,   0,   0,   0,   5, -10},
            {-20, -10, -10, -10, -10, -10, -10, -20}};
        piecePositionPairs["rook"] = new int[8, 8]
            {{ 0,  0,  0,  0,  0,  0,  0,  0},
             { 5, 10, 10, 10, 10, 10, 10,  5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             { 0,  0,  0,  5,  5,  0,  0,  0}};
        piecePositionPairs["queen"] = new int[8, 8]
            {{-20, -10, -10,  -5,  -5, -10, -10, -20},
             {-10,   0,   0,   0,   0,   0,   0, -10},
             {-10,   0,   5,   5,   5,   5,   0, -10},
             { -5,   0,   5,   5,   5,   5,   0,  -5},
             {  0,   0,   5,   5,   5,   5,   0,  -5},
             {-10,   5,   5,   5,   5,   5,   0, -10},
             {-10,   0,   5,   0,   0,   0,   0, -10},
             {-20, -10, -10,  -5,  -5, -10, -10, -20}};
        piecePositionPairs["king"] = new int[8, 8]
            {{-30, -40, -40, -50, -50, -40, -40, -30},
             {-30, -40, -40, -50, -50, -40, -40, -30},
             {-30, -40, -40, -50, -50, -40, -40, -30},
             {-30, -40, -40, -50, -50, -40, -40, -30},
             {-20, -30, -30, -40, -40, -30, -30, -20},
             {-10, -20, -20, -20, -20, -20, -20, -10},
             { 20,  20,   0,   0,   0,   0,  20,  20},
             { 20,  30,  10,   0,   0,  10,  30,  20}};
        piecePositionPairs["kingEndgame"] = new int[8, 8] // this is for endgame, e.g. both sides have no queens or every side which has a queen has additionally no other pieces or one minorpiece maximum
            {{-50, -40, -30, -20, -20, -30, -40, -50},
             {-30, -20, -10,  0,   0, -10, -20, -30},
             {-30, -10,  20,  30,  30,  20, -10, -30},
             {-30, -10,  30,  40,  40,  30, -10, -30},
             {-30, -10,  30,  40,  40,  30, -10, -30},
             {-30, -10,  20,  30,  30,  20, -10, -30},
             {-30, -30,   0,   0,   0,   0, -30, -30},
             {-50, -30, -30, -30, -30, -30, -30, -50}};
    }

    private int[,] rotatePiecePositionPairs(int[,] inp)
    {
        var tmp = inp;
        int t = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                t = inp[i, j];
                inp[i, j] = inp[7 - i, j];
                inp[7 - i, j] = t;
            }
        }
        return tmp;
    }
}