//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public partial class ManageBoard
//{
//    Dictionary<string, int> pieceValuePairs = new Dictionary<string, int>()
//    { 
//        {"pawn", 10},
//        {"knight", 25},
//        {"bishop", 25},
//        {"rook", 40},
//        {"queen", 100},
//        {"king", 200},
//    };

//    string[,] virtualMove(string[,] brd, Move mv) // brd might be passed as a reference, not as value, idk.
//    {


//        return brd;
//    }

//    private int evaluate(List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces)
//    {
//        int res = 0;
//        foreach (var piece in wPieces) // can optimise that by keeping track of the amount of corresponding pieces existing
//        {
//            res += pieceValuePairs[piece.getType()];
//        }
//        foreach (var piece in bPieces)
//        {
//            res -= pieceValuePairs[piece.getType()];
//        }
//        return res;
//    }
//    private Move EvalOpponent(bool thisWhite, string[,] brd, List<Move>[,] mvs, List<PieceBehaviour> wPieces, List<PieceBehaviour> bPieces)
//    {

//    }
//}
