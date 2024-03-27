using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

//{
//    {"pawn", 100}, 2
//    {"knight", 250}, 4
//    {"bishop", 250}, 8
//    {"rook", 400}, 16
//    {"queen", 1000}, 32
//    {"king", 20000}, 64
//    {"empty", 0} 0
//}

//{
//    {"PAWN", 100}, 130
//    {"KNIGHT", 250}, 132
//    {"BISHOP", 250}, 136
//    {"ROOK", 400}, 144
//    {"QUEEN", 1000}, 160
//    {"KING", 20000}, 192
//    {"empty", 0} 0
//}
class virtualBoardInt
{
    public int[,] board = new int[8, 8]; // "pawn", "knight", "bishop", "rook", "queen", "king"

    public int[] pieceCountArray = new int[MoveCalculatorInt.maxID];
    public int blackOffset = MoveCalculatorInt.blackID;
    public int whiteOffset = 0;

    public void virtualMove(MoveInt mv)
    {
        pieceCountArray[board[mv.startx + mv.dx, mv.starty + mv.dy]]--;
        if (mv.additionalTargets != null)
        {
            foreach (var targ in mv.additionalTargets)
            {
                board[targ.startx + targ.dx, targ.starty + targ.dy] = MoveCalculatorInt.emptyID;
                pieceCountArray[board[mv.startx + mv.dx, mv.starty + mv.dy]]--;
            }
        }

        board[mv.startx + mv.dx, mv.starty + mv.dy] = board[mv.startx, mv.starty];
        board[mv.startx, mv.starty] = MoveCalculatorInt.emptyID;
    }

    public void virtualUnMove(MoveInt mv)
    {
        if (mv.additionalTargets != null)
        {
            foreach (var targ in mv.additionalTargets)
            {
                board[targ.startx + targ.dx, targ.starty + targ.dy] = targ.target;
                pieceCountArray[board[targ.startx + targ.dx, targ.starty + targ.dy]]++;
            }
        }
        board[mv.startx, mv.starty] = board[mv.startx + mv.dx, mv.starty + mv.dy];
        board[mv.startx + mv.dx, mv.starty + mv.dy] = mv.target;
        pieceCountArray[board[mv.startx + mv.dx, mv.starty + mv.dy]]++;
    }

    public void setState(int[,] brd)
    {
        board = (int[,])brd.Clone();
        recount();
    }

    public void recount()
    {
        pieceCountArray = new int[MoveCalculatorInt.maxID];
        foreach (var pc in board)
        {
            pieceCountArray[pc]++;
        }
    }

    public virtualBoardInt(int[,] brd)
    {
        board = (int[,])brd.Clone();
        recount();
    }
    public virtualBoardInt()
    {
    }

}
public class AlphaBetaOpponentInt : AItemplate
{
    private virtualBoardInt vboard = new virtualBoardInt();
    int iterCounter = 0;
    public int searchDepth = 5; // default value gets overrided during creation
    bool amWhite = true;

    private int[] pieceValuePairsArray = new int[MoveCalculatorInt.maxID];
    private int[][,] piecePositionPairsArray = new int[MoveCalculatorInt.maxID][,];
   


    private int evaluate()
    {
        if (amWhite)
        {
            return countPieces(vboard.whiteOffset) - countPieces(vboard.blackOffset);
        }
        else
        {
            return countPieces(vboard.whiteOffset) - countPieces(vboard.blackOffset);
        }
    }

    private int countPieces(int offsetIndex)
    {
        int rs = 0;
        //vboard.recount(); // something's wrong with counting, so I recount
        //rs += vboard.pawnsCount[colourIndex] * pieceValuePairs["pawn"];
        //rs += vboard.knightCount[colourIndex] * pieceValuePairs["knight"];
        //rs += vboard.bishopCount[colourIndex] * pieceValuePairs["bishop"];
        //rs += vboard.rookCount[colourIndex] * pieceValuePairs["rook"];
        //rs += vboard.queenCount[colourIndex] * pieceValuePairs["queen"];
        //rs += vboard.kingCount[colourIndex] * pieceValuePairs["king"];
        foreach (var ID in MoveCalculatorInt.IDarray)
        {
            rs += vboard.pieceCountArray[ID + offsetIndex] * pieceValuePairsArray[ID];
        }
        return rs;
    }

    int search(int depth, int beta, int alpha, bool whiteTurn, List<MoveInt> possibleMoves = null)
    {
        iterCounter++;
        if (depth == 0) return evaluate();


        if (possibleMoves == null) possibleMoves = MoveCalculatorInt.generateAllMovesListInterestingFirst(vboard.board, whiteTurn);
        if (possibleMoves.Count == 0) return 0;


        foreach (MoveInt mv in possibleMoves)
        {
            vboard.virtualMove(mv);

            int eval = -search(depth - 1, -alpha, -beta, !whiteTurn);
            eval += piecePositionPairsArray[mv.piece][mv.starty + mv.dy, mv.startx + mv.dx];
#warning piecePositionPairs eat a lot of performance, please find another way to work with this

            vboard.virtualUnMove(mv);
            if (eval >= beta) return beta;
            alpha = math.max(eval, alpha);
        }

        return alpha;
    }

    public override Move getMove(bool thisWhite, string[,] brd, List<Move> mves/*, List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces, PieceBehaviour[,] pieces*/)
    {
        var st = Time.realtimeSinceStartup;
        if (amWhite != thisWhite) throw new Exception("alpha beta opponent is wrongly assigned colour");
        vboard.setState(boardConversion(brd));
        List<MoveInt> mvs = convertListToInt(mves);

        int res = -1000000;
        int tres = -1000000;
        int index = 0;
        int n = 0;
        int beth = -res;

        foreach (MoveInt mv in mvs)
        {
            vboard.virtualMove(mv);

            tres = -search(searchDepth, 1000000, -1000000, !thisWhite);
            tres += piecePositionPairsArray[mv.piece][mv.starty + mv.dy, mv.startx + mv.dx];
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
        return new Move(mvs[index]);
    }

    public AlphaBetaOpponentInt(int depth, bool isWhite)
    {
        searchDepth = depth;
        amWhite = isWhite;

        if (amWhite && searchDepth % 2 == 0) searchDepth--; // very ugly crutch, but I'm not sure why black only works if searchdepth is % 2 == 0
        if (!amWhite && searchDepth % 2 != 0) searchDepth--; // and white too

        setPiecePositionPairsArray();
        setpieceValuePairsArray();


        if (!amWhite)
        {
            foreach (var ID in MoveCalculatorInt.IDarray)
            {
                if (ID == MoveCalculatorInt.emptyID || ID == MoveCalculatorInt.maxID) continue;
                piecePositionPairsArray[ID + MoveCalculatorInt.blackID] = rotatePiecePositionPairs(piecePositionPairsArray[ID]);
            }

            //piecePositionPairsArray[MoveCalculatorInt.kingID + 1] = rotatePiecePositionPairs(piecePositionPairsArray[MoveCalculatorInt.kingID + 1]);
        }
    }
    ~AlphaBetaOpponentInt()
    {

    }

    private void setPiecePositionPairsArray()
    {
        // analysis of positions is taken from https://www.chessprogramming.org/Simplified_Evaluation_Function , many thanks to the author
        piecePositionPairsArray[MoveCalculatorInt.pawnID] = new int[8, 8]
            {{ pieceValuePairsArray[MoveCalculatorInt.queenID],  pieceValuePairsArray[MoveCalculatorInt.queenID],  pieceValuePairsArray[MoveCalculatorInt.queenID],  pieceValuePairsArray[MoveCalculatorInt.queenID],  pieceValuePairsArray[MoveCalculatorInt.queenID],  pieceValuePairsArray[MoveCalculatorInt.queenID],  pieceValuePairsArray[MoveCalculatorInt.queenID],  pieceValuePairsArray[MoveCalculatorInt.queenID]},
            {50, 50, 50, 50, 50, 50, 50, 50},
            {10, 10, 20, 30, 30, 20, 10, 10},
            { 5,  5, 10, 25, 25, 10,  5,  5},
            { 0,  0,  0, 20, 20,  0,  0,  0},
            { 5, -5,-10,  0,  0,-10, -5,  5},
            { 5, 10, 10,-20,-20, 10, 10,  5},
            { 0,  0,  0,  0,  0,  0,  0,  0}};
        piecePositionPairsArray[MoveCalculatorInt.knightID] = new int[8, 8]
            {{-50, -40, -30, -30, -30, -30, -40, -50},
            {-40, -20,   0,   0,   0,   0, -20, -40},
            {-30,   0,  10,  15,  15,  10,   0, -30},
            {-30,   5,  15,  20,  20,  15,   5, -30},
            {-30,   0,  15,  20,  20,  15,   0, -30},
            {-30,   5,  10,  15,  15,  10,   5, -30},
            {-40, -20,   0,   5,   5,   0, -20, -40},
            {-50, -40, -30, -30, -30, -30, -40, -50}};
        piecePositionPairsArray[MoveCalculatorInt.bishopID] = new int[8, 8]
            {{-20, -10, -10, -10, -10, -10, -10, -20},
            {-10,   0,   0,   0,   0,   0,   0, -10},
            {-10,   0,   5,  10,  10,   5,   0, -10},
            {-10,   5,   5,  10,  10,   5,   5, -10},
            {-10,   0,  10,  10,  10,  10,   0, -10},
            {-10,  10,  10,  10,  10,  10,  10, -10},
            {-10,   5,   0,   0,   0,   0,   5, -10},
            {-20, -10, -10, -10, -10, -10, -10, -20}};
        piecePositionPairsArray[MoveCalculatorInt.rookID] = new int[8, 8]
            {{ 0,  0,  0,  0,  0,  0,  0,  0},
             { 5, 10, 10, 10, 10, 10, 10,  5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             {-5,  0,  0,  0,  0,  0,  0, -5},
             { 0,  0,  0,  5,  5,  0,  0,  0}};
        piecePositionPairsArray[MoveCalculatorInt.queenID] = new int[8, 8]
            {{-20, -10, -10,  -5,  -5, -10, -10, -20},
             {-10,   0,   0,   0,   0,   0,   0, -10},
             {-10,   0,   5,   5,   5,   5,   0, -10},
             { -5,   0,   5,   5,   5,   5,   0,  -5},
             {  0,   0,   5,   5,   5,   5,   0,  -5},
             {-10,   5,   5,   5,   5,   5,   0, -10},
             {-10,   0,   5,   0,   0,   0,   0, -10},
             {-20, -10, -10,  -5,  -5, -10, -10, -20}};
        piecePositionPairsArray[MoveCalculatorInt.kingID] = new int[8, 8]
            {{-30, -40, -40, -50, -50, -40, -40, -30},
             {-30, -40, -40, -50, -50, -40, -40, -30},
             {-30, -40, -40, -50, -50, -40, -40, -30},
             {-30, -40, -40, -50, -50, -40, -40, -30},
             {-20, -30, -30, -40, -40, -30, -30, -20},
             {-10, -20, -20, -20, -20, -20, -20, -10},
             { 20,  20,   0,   0,   0,   0,  20,  20},
             { 20,  30,  10,   0,   0,  10,  30,  20}};
        piecePositionPairsArray[MoveCalculatorInt.kingID + 1] = new int[8, 8] // this is for endgame, e.g. both sides have no queens or every side which has a queen has additionally no other pieces or one minorpiece maximum
            {{-50, -40, -30, -20, -20, -30, -40, -50},
             {-30, -20, -10,  0,   0, -10, -20, -30},
             {-30, -10,  20,  30,  30,  20, -10, -30},
             {-30, -10,  30,  40,  40,  30, -10, -30},
             {-30, -10,  30,  40,  40,  30, -10, -30},
             {-30, -10,  20,  30,  30,  20, -10, -30},
             {-30, -30,   0,   0,   0,   0, -30, -30},
             {-50, -30, -30, -30, -30, -30, -30, -50}};
    }

    private void setpieceValuePairsArray()
    {
        pieceValuePairsArray[MoveCalculatorInt.emptyID] = 0;
        pieceValuePairsArray[MoveCalculatorInt.pawnID] = 100;
        pieceValuePairsArray[MoveCalculatorInt.knightID] = 250;
        pieceValuePairsArray[MoveCalculatorInt.bishopID] = 250;
        pieceValuePairsArray[MoveCalculatorInt.rookID] = 400;
        pieceValuePairsArray[MoveCalculatorInt.queenID] = 1000;
        pieceValuePairsArray[MoveCalculatorInt.kingID] = 20000;
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

    private int[,] boardConversion(string[,] brd)
    {
        int[,] res = new int[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                res[j, i] = MoveCalculatorInt.conversionIDdict[brd[j, i]];
            }
        }
        return res;
    }

    private List<MoveInt> convertListToInt(List<Move> lst)
    {
        List<MoveInt> res = new List<MoveInt>();
        foreach (Move move in lst)
        {
            res.Add(new MoveInt(move)); 
        }

        return res;
    }
}