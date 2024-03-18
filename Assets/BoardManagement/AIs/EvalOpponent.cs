using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Pair<T1, T2>
{
    public T1 First
    {
        get;
        set;
    }
    public T2 Second
    {
        get;
        set;
    }
    public Pair(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }
}
class virtualBoard
{
    bool whiteTurn = true;
    public string[,] board = new string[8,8]; // "pawn", "knight", "bishop", "rook", "queen", "king"
    public int[] pawnsCount = new int[2];
    public int[] knightCount = new int[2];
    public int[] bishopCount = new int[2];
    public int[] rookCount = new int[2];
    public int[] queenCount = new int[2];
    public int[] kingCount = new int[2];

    public int whiteIndex = 0;
    public int blackIndex = 0;



    public void virtualMove(Move mv)
    {

    }

    public void virtualUnMove(Move mv)
    {

    }

    public void setState(string[,] brd)
    {
        recount();
    }

    private void recount()
    {

    }

    public virtualBoard()
    {

    }

}
class EvalOpponent
{
    private virtualBoard vboard = new virtualBoard();
    bool amWhite = true;
    private Dictionary<string, int> pieceValuePairs = new Dictionary<string, int>()
    {
        {"pawn", 10},
        {"knight", 25},
        {"bishop", 25},
        {"rook", 40},
        {"queen", 100},
        {"king", 200},
    };

    
    private int evaluate()
    {
        return countPieces(vboard.whiteIndex) - countPieces(vboard.blackIndex);
    }

    private int countPieces(int colourIndex)
    {
        int rs = 0;
        rs += vboard.pawnsCount[colourIndex] * pieceValuePairs["pawn"];
        rs += vboard.knightCount[colourIndex] * pieceValuePairs["knight"];
        rs += vboard.bishopCount[colourIndex] * pieceValuePairs["bishop"];
        rs += vboard.rookCount[colourIndex] * pieceValuePairs["rook"];
        rs += vboard.queenCount[colourIndex] * pieceValuePairs["queen"];
        rs += vboard.kingCount[colourIndex] * pieceValuePairs["king"];
        return rs;
    }

    int search(int depth, bool whiteTurn)
    {
        if (depth == 0) return evaluate();

        
        List<Move> possibleMoves = MoveCalculator.generateAllMovesList(vboard.board, whiteTurn);
        if (possibleMoves.Count == 0) return 0;

        int res = -100000;

        foreach (Move mv in possibleMoves)
        {
            vboard.virtualMove(mv);

            res = math.max(res, -search(depth - 1, !whiteTurn));

            vboard.virtualUnMove(mv);
        }

        return res;
    }
    
    public Move getMove(bool thisWhite, string[,] brd, List<Move>[,] mvs, List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces, PieceBehaviour[,] pieces) { 
        Move resMove = new Move();
        vboard.setState(brd);
        amWhite = thisWhite;

        return resMove;
    }

    public EvalOpponent()
    {

    }
    ~EvalOpponent()
    {

    }
}
