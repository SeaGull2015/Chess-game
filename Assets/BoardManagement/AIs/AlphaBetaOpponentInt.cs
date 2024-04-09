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
/// <summary>
/// Class for emulating virtual board for calculations.
/// </summary>
class virtualBoardInt
{
    /// <summary>
    /// Represents the chess board with piece IDs.
    /// </summary>
    public int[,] board = new int[8, 8]; // "pawn", "knight", "bishop", "rook", "queen", "king"

    /// <summary>
    /// Array to keep track of the count of each type of piece.
    /// </summary>
    public int[] pieceCountArray = new int[MoveCalculatorInt.maxID];

    /// <summary>
    /// Offset for black pieces.
    /// </summary>
    public int blackOffset = MoveCalculatorInt.blackID;

    /// <summary>
    /// Offset for white pieces.
    /// </summary>
    public int whiteOffset = 0;

    /// <summary>
    /// Simulates a move on the virtual board.
    /// </summary>
    /// <param name="mv">The move to be made.</param>
    public void virtualMove(MoveInt mv)
    {
        pieceCountArray[board[mv.startx + mv.dx, mv.starty + mv.dy]]--;
        if (mv.additionalTargets != null)
        {
            if ((mv.piece & MoveCalculatorInt.kingID) == MoveCalculatorInt.kingID)
            {
                foreach (var targ in mv.additionalTargets) // separate cycle cause I don't wanna to do a check every time
                {
                    virtualMove(targ);
                }
            }
            else
            {
                foreach (var targ in mv.additionalTargets)
                {
                    board[targ.startx + targ.dx, targ.starty + targ.dy] = MoveCalculatorInt.emptyID;
                    pieceCountArray[board[mv.startx + mv.dx, mv.starty + mv.dy]]--;
                }
            }
        }

        board[mv.startx + mv.dx, mv.starty + mv.dy] = board[mv.startx, mv.starty];
        board[mv.startx, mv.starty] = MoveCalculatorInt.emptyID;
    }

    /// <summary>
    /// Reverts a move made on the virtual board.
    /// </summary>
    /// <param name="mv">The move to be reverted.</param>
    public void virtualUnMove(MoveInt mv)
    {
        if (mv.additionalTargets != null)
        {
            if ((mv.piece & MoveCalculatorInt.kingID) == MoveCalculatorInt.kingID)
            {
                foreach (var targ in mv.additionalTargets)
                {
                    virtualUnMove(targ);
                }
            }
            else
            {
                foreach (var targ in mv.additionalTargets)
                {
                    board[targ.startx + targ.dx, targ.starty + targ.dy] = targ.target;
                    pieceCountArray[board[targ.startx + targ.dx, targ.starty + targ.dy]]++;
                }
            }
        }
        board[mv.startx, mv.starty] = board[mv.startx + mv.dx, mv.starty + mv.dy];
        board[mv.startx + mv.dx, mv.starty + mv.dy] = mv.target;
        pieceCountArray[board[mv.startx + mv.dx, mv.starty + mv.dy]]++;
    }

    /// <summary>
    /// Sets the state of the virtual board with the given board configuration.
    /// </summary>
    /// <param name="brd">The board configuration to set.</param>
    public void setState(int[,] brd)
    {
        board = (int[,])brd.Clone();
        recount();
    }

    /// <summary>
    /// Recalculates the piece count array based on the current board configuration.
    /// </summary>
    public void recount()
    {
        pieceCountArray = new int[MoveCalculatorInt.maxID];
        foreach (var pc in board)
        {
            pieceCountArray[pc]++;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="virtualBoardInt"/> class with the given board configuration.
    /// </summary>
    /// <param name="brd">The initial board configuration.</param>
    public virtualBoardInt(int[,] brd)
    {
        board = (int[,])brd.Clone();
        recount();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="virtualBoardInt"/> class.
    /// </summary>
    public virtualBoardInt()
    {
    }
}

/// <summary>
/// Represents an AI opponent using the Alpha-Beta pruning algorithm for decision-making in a chess game.
/// </summary>
public class AlphaBetaOpponentInt : AItemplate
{
    private virtualBoardInt vboard = new virtualBoardInt(); // Instance of the virtual chessboard
    int iterCounter = 0; // Counter for iterations during search
    public int searchDepth = 5; // Default search depth for the Alpha-Beta algorithm
    bool amWhite = true; // Flag indicating if the opponent is playing as white

    private int[] pieceValuePairsArray = new int[MoveCalculatorInt.maxID]; // Array storing the values of each chess piece
    private int[][,] piecePositionPairsArray = new int[MoveCalculatorInt.maxID][,]; // Array storing positional values for each piece


    /// <summary>
    /// Evaluates the current board state.
    /// </summary>
    /// <returns>The evaluation score of the board.</returns>
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

    /// <summary>
    /// Counts the total value of pieces with a given offset.
    /// </summary>
    /// <param name="offsetIndex">The offset for piece counting.</param>
    /// <returns>The total value of pieces with the specified offset.</returns>
    private int countPieces(int offsetIndex)
    {
        int rs = 0;
        foreach (var ID in MoveCalculatorInt.IDtypeArray)
        {
            rs += vboard.pieceCountArray[ID + offsetIndex] * pieceValuePairsArray[ID];
        }
        return rs;
    }

    /// <summary>
    /// Performs the Alpha-Beta search algorithm to determine the best move.
    /// </summary>
    /// <param name="depth">The current depth of the search.</param>
    /// <param name="beta">The current beta value.</param>
    /// <param name="alpha">The current alpha value.</param>
    /// <param name="whiteTurn">Flag indicating if it's white's turn.</param>
    /// <param name="possibleMoves">List of possible moves.</param>
    /// <returns>The evaluation score of the best move.</returns>
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

            vboard.virtualUnMove(mv);
            if (eval >= beta) return beta;
            alpha = Math.Max(eval, alpha);
        }

        return alpha;
    }

    /// <summary>
    /// Gets the best move for the opponent.
    /// </summary>
    /// <param name="thisWhite">Flag indicating if the opponent is playing as white.</param>
    /// <param name="brd">The current board state.</param>
    /// <param name="mves">List of possible moves.</param>
    /// <returns>The best move determined by the Alpha-Beta algorithm.</returns>
    public override Move getMove(bool thisWhite, string[,] brd, List<Move> mves)
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

    /// <summary>
    /// Initializes a new instance of the <see cref="AlphaBetaOpponentInt"/> class.
    /// </summary>
    /// <param name="depth">The search depth for the Alpha-Beta algorithm.</param>
    /// <param name="isWhite">Flag indicating if the opponent plays as white.</param>
    public AlphaBetaOpponentInt(int depth, bool isWhite)
    {
        searchDepth = depth;
        amWhite = isWhite;

        if (amWhite && searchDepth % 2 == 0) searchDepth--; // very ugly crutch, but I'm not sure why black only works if searchdepth is % 2 == 0
        if (!amWhite && searchDepth % 2 != 0) searchDepth--; // and white too

        setPiecePositionPairsArray();
        setpieceValuePairsArray();


        foreach (var ID in MoveCalculatorInt.IDtypeArray)
        {
            piecePositionPairsArray[ID + MoveCalculatorInt.blackID] = rotatePiecePositionPairs(piecePositionPairsArray[ID]);
        }
    }
    ~AlphaBetaOpponentInt()
    {

    }

    /// <summary>
    /// Sets piece-position pairs array based on analysis from a specified source.
    /// </summary>
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
        piecePositionPairsArray[MoveCalculatorInt.kingID + 1] = new int[8, 8]; // this is for endgame, e.g. both sides have no queens or every side which has a queen has additionally no other pieces or one minorpiece maximum
        piecePositionPairsArray[MoveCalculatorInt.kingID + 1] = new int[8, 8]
            {{-50, -40, -30, -20, -20, -30, -40, -50},
         {-30, -20, -10,  0,   0, -10, -20, -30},
         {-30, -10,  20,  30,  30,  20, -10, -30},
         {-30, -10,  30,  40,  40,  30, -10, -30},
         {-30, -10,  30,  40,  40,  30, -10, -30},
         {-30, -10,  20,  30,  30,  20, -10, -30},
         {-30, -30,   0,   0,   0,   0, -30, -30},
         {-50, -30, -30, -30, -30, -30, -30, -50}};
    }

    /// <summary>
    /// Sets piece-value pairs array with specified values.
    /// </summary>
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

    /// <summary>
    /// Rotates the piece-position pairs array.
    /// </summary>
    /// <param name="inp">Input array to be rotated.</param>
    /// <returns>Rotated array.</returns>
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

    /// <summary>
    /// Converts a 2D string array representing a board to a corresponding 2D integer array.
    /// </summary>
    /// <param name="brd">2D string array representing the board.</param>
    /// <returns>2D integer array representing the board.</returns>
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

    /// <summary>
    /// Converts a list of Move objects to a list of MoveInt objects.
    /// </summary>
    /// <param name="lst">List of Move objects to be converted.</param>
    /// <returns>List of MoveInt objects.</returns>
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