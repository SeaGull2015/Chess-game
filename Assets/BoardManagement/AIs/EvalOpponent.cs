using System.Collections;
using System.Collections.Generic;
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
    string[,] board = new string[8,8];
    List<PieceBehaviour> whitePieces = new List<PieceBehaviour>();
    List<PieceBehaviour> blackPieces = new List<PieceBehaviour>();
    PieceBehaviour[,] pieces = new PieceBehaviour[8,8];

    public void virtualMove(Move mv)
    {

    }

    public void virtualUnMove()
    {

    }

    public void setState(bool thisWhite, string[,] brd, List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces, PieceBehaviour[,] pcs)
    {

    }

    public Pair<List<PieceBehaviour>, List<PieceBehaviour>> getState()
    {
        return new Pair<List<PieceBehaviour>, List<PieceBehaviour>>(whitePieces, blackPieces);
    }

    public virtualBoard()
    {

    }

}
class EvalOpponent
{
    private Dictionary<string, int> pieceValuePairs = new Dictionary<string, int>()
    {
        {"pawn", 10},
        {"knight", 25},
        {"bishop", 25},
        {"rook", 40},
        {"queen", 100},
        {"king", 200},
    };

    
    private int evaluate(List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces)
    {
        int res = 0;
        foreach (var piece in wPieces) // can optimise that by keeping track of the amount of corresponding pieces existing
        {
            res += pieceValuePairs[piece.getType()];
        }
        foreach (var piece in bPieces)
        {
            res -= pieceValuePairs[piece.getType()];
        }
        return res;
    }
    
    public Move getMove(bool thisWhite, string[,] brd, List<Move>[,] mvs, List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces, PieceBehaviour[,] pieces) { 
        Move resMove = new Move();

        return resMove;
    }

    public EvalOpponent()
    {

    }
    ~EvalOpponent()
    {

    }
}
