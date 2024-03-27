using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
static class MoveCalculatorInt
{
    static int[,] board = new int[8, 8];
    static bool thisWhite;
    static public int emptyID = 0; // kinda ugly but I don't wanna make adress some other singleton object or static because of potential performance issues
    static public int pawnID = 2; // also this should be somewhere else, imho. Maybe in board manager, idk
    static public int knightID = 4;
    static public int bishopID = 8;
    static public int rookID = 16;
    static public int queenID = 32;
    static public int kingID = 64;
    static public int blackID = 128;
    static public int maxID = 255;
    static public int[] IDarray = new int[] {emptyID, pawnID, knightID, bishopID, rookID, queenID, kingID, blackID, maxID };
    static public Dictionary<string, int> conversionIDdict = new Dictionary<string, int>()
    {
            {"pawn", pawnID}, 
            {"knight", knightID}, 
            {"bishop", bishopID}, 
            {"rook", rookID}, 
            {"queen", queenID}, 
            {"king", kingID}, 
            {"PAWN", pawnID + blackID}, 
            {"KNIGHT", knightID + blackID}, 
            {"BISHOP", bishopID + blackID}, 
            {"ROOK", rookID + blackID}, 
            {"QUEEN", queenID + blackID}, 
            {"KING", kingID + blackID}, 
            {"empty", emptyID} 
    };

    static public Dictionary<int, string> reverseConversionIDdict = new Dictionary<int, string>()
    {
         {pawnID, "pawn"},
         {knightID, "knight"},
         {bishopID, "bishop"},
         {rookID, "rook"},
         {queenID, "queen"},
         {kingID, "king"},
         {pawnID + blackID, "PAWN"},
         {knightID + blackID, "KNIGHT"},
         {bishopID + blackID, "BISHOP"},
         {rookID + blackID, "ROOK"},
         {queenID + blackID, "QUEEN"},
         {kingID + blackID, "KING"},
         {emptyID, "empty"}
    };


    static public bool checkWhite(int x)
    {
        return (x & blackID) == 0;
    }

    static public int getTypeWithoutColour(int x)
    {
        return ((x << 1) & maxID) >> 1;
    }

    static public List<MoveInt>[,] generateAllMoves(int[,] brd, bool whiteTurn)
    {
        List<MoveInt>[,] moves = new List<MoveInt>[8, 8];
        board = brd;
        thisWhite = whiteTurn;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == emptyID) continue;
                else if (checkWhite(board[i,j]) == whiteTurn)
                {
                    moves[i, j] = getMoves(i, j);
                }
                else
                {
                    moves[i, j] = new List<MoveInt>();
                }
            }
        }
        return moves;
    }

    static public List<MoveInt> generateAllMovesList(int[,] brd, bool whiteTurn)
    {
        List<MoveInt> moves = new List<MoveInt>();
        board = brd;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == 0) continue;
                else if (checkWhite(board[i, j]) == whiteTurn)
                {
                    moves.AddRange(getMoves(i, j));
                }
            }
        }
        return moves;
    }

    static public List<MoveInt> generateAllMovesListInterestingFirst(int[,] brd, bool whiteTurn)
    {
        List<MoveInt> moves = new List<MoveInt>();
        board = brd;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == emptyID) continue;
                else if (checkWhite(board[i, j]) == whiteTurn)
                {
                    List<MoveInt> tlist = getMoves(i, j);
                    foreach (MoveInt mv in tlist)
                    {
                        if (mv.target != emptyID) moves.Insert(0, mv);
                        else moves.Add(mv);
                    }
                }
            }
        }
        //moves.Sort((x,y)=> -pieceValuePairs[x.target.ToLower()].CompareTo(pieceValuePairs[y.target.ToLower()])); // basicly I want moves with higher price targets first to improve alpha-beta pruning performance
        // note though that this doesn't work for en passants, cause bad code
        // consider also instead of sorting just checking if it has a target, then moving it to top
        return moves;
    }


    static public List<MoveInt> getMoves(int x, int y, int[,] brd)
    {
        board = brd;
        int type = getTypeWithoutColour(board[x,y]); // basicly getting rid off the leading 1 or 0, because colour doesn't matter here
        int[] sliders = { bishopID, queenID, rookID };
        if (sliders.Contains(type)) return getSlidingMoves(x, y, type);
        else if (type == pawnID) return getPawnMoves(x, y);
        else if (type == kingID) return getKingMoves(x, y);
        else if (type == knightID) return getKnightMoves(x, y);
        else throw new Exception("getMoves() bad type"); /*return new List<MoveInt>();*/
    }

    static private List<MoveInt> getMoves(int x, int y)
    {
        int type = ((board[x, y] << 1) & maxID) >> 1; // basicly getting rid off the leading 1, because colour doesn't matter here
        int[] sliders = { bishopID, queenID, rookID };
        if (sliders.Contains(type)) return getSlidingMoves(x, y, type);
        else if (type == pawnID) return getPawnMoves(x, y);
        else if (type == kingID) return getKingMoves(x, y);
        else if (type == knightID) return getKnightMoves(x, y);
        else throw new Exception("getMoves() bad type"); /*return new List<MoveInt>();*/
    }
    static private MoveInt directSlidingMove(int distance, int dir, int sx, int sy) // this kinda sucks
    {
        if (dir == 0) return new MoveInt(0, distance, sx, sy, board[sx, sy], board[sx, sy + distance]);
        if (dir == 1) return new MoveInt(distance, 0, sx, sy, board[sx, sy], board[sx + distance, sy]);
        if (dir == 2) return new MoveInt(0, -distance, sx, sy, board[sx, sy], board[sx, sy - distance]);
        if (dir == 3) return new MoveInt(-distance, 0, sx, sy, board[sx, sy], board[sx - distance, sy]);
        if (dir == 4) return new MoveInt(distance, distance, sx, sy, board[sx, sy], board[sx + distance, sy + distance]);
        if (dir == 5) return new MoveInt(distance, -distance, sx, sy, board[sx, sy], board[sx + distance, sy - distance]);
        if (dir == 6) return new MoveInt(-distance, -distance, sx, sy, board[sx, sy], board[sx - distance, sy - distance]);
        if (dir == 7) return new MoveInt(-distance, distance, sx, sy, board[sx, sy], board[sx - distance, sy + distance]);
        else throw new Exception("directMove() wrong input dir");
    }

    static private List<MoveInt> getSlidingMoves(int x, int y, int type)
    {
        List<MoveInt> res = new List<MoveInt>();

        int[] dirs = dirLen(x, y);

        if (type == bishopID || type == queenID) // the following code somewhat sucks, but as of now I don't have ideas how to implement it better
        {
            for (int i = 4; i < 8; i++)
            {
                for (int j = 1; j <= dirs[i]; j++)
                {
                    MoveInt tMove = directSlidingMove(j, i, x, y);
                    if (board[x + tMove.dx, y + tMove.dy] == emptyID) res.Add(tMove);
                    else if (checkWhite(board[x + tMove.dx, y + tMove.dy]) == thisWhite) break;
                    else if (checkWhite(board[x + tMove.dx, y + tMove.dy]) != thisWhite)
                    {
                        res.Add(directSlidingMove(j, i, x, y));
                        break;
                    }
                }
            }
        }
        if (type == rookID || type == queenID)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= dirs[i]; j++)
                {
                    MoveInt tMove = directSlidingMove(j, i, x, y);
                    if (board[x + tMove.dx, y + tMove.dy] == emptyID) res.Add(tMove);
                    else if (checkWhite(board[x + tMove.dx, y + tMove.dy]) == thisWhite) break;
                    else if (checkWhite(board[x + tMove.dx, y + tMove.dy]) != thisWhite)
                    {
                        res.Add(directSlidingMove(j, i, x, y));
                        break;
                    }
                }
            }
        }

        return res;
    }

    static private List<MoveInt> getPawnMoves(int x, int y)
    {
        List<MoveInt> res = new List<MoveInt>();
        bool thisWhite = checkWhite(board[x, y]);
        int dir = thisWhite ? 1 : -1;
        int yLim = thisWhite ? 7 : 0;
        int startYlvl = thisWhite ? 1 : 6;

        if (y != yLim && board[x, y + dir] == emptyID)
        {
            res.Add(new MoveInt(0, dir, x, y, board[x, y], board[x, y + dir]));
            if (y == startYlvl && board[x, y + dir * 2] == emptyID)
            {
                res.Add(new MoveInt(0, dir * 2, x, y, board[x, y], board[x, y + dir * 2]));
            }
        }

        if (y != yLim && x < 7 && board[x + 1, y + dir] != emptyID && thisWhite != checkWhite(board[x + 1, y + dir])) // I would rather have y < 7 or y > 0, but it would be ugly, so y != yLim
        {
            res.Add(new MoveInt(1, dir, x, y, board[x, y], board[x + 1, y + dir]));
        }
        if (y != yLim && x > 0 && board[x - 1, y + dir] != emptyID && thisWhite != checkWhite(board[x - 1, y + dir]))
        {
            res.Add(new MoveInt(-1, dir, x, y, board[x, y], board[x - 1, y + dir]));
        }

        if (false) // TODO: insert en passant here
        {

        }
        if (false) // TODO: insert upgrade behaviour here
        {

        }


        return res;
    }

    static private List<MoveInt> getKingMoves(int x, int y)
    {
        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        List<MoveInt> res = new List<MoveInt>();
        bool thisWhite = checkWhite(board[x, y]);

        for (int i = 0; i < 8; i++)
        {
            if (x + dx[i] >= 0 && x + dx[i] < 8 &&
                y + dy[i] >= 0 && y + dy[i] < 8 &&
                (board[x + dx[i], y + dy[i]] == emptyID ||
                checkWhite(board[x + dx[i], y + dy[i]]) != thisWhite))
            {
                res.Add(new MoveInt(dx[i], dy[i], x, y, board[x, y], board[x + dx[i], y + dy[i]]));
            }
        }

        return res;
    }

    static private List<MoveInt> getKnightMoves(int x, int y)
    {
        List<MoveInt> res = new List<MoveInt>();
        bool thisWhite = checkWhite(board[x, y]);

        int[] dx = { -2, -1, 1, 2, 2, 1, -1, -2 };
        int[] dy = { -1, -2, -2, -1, 1, 2, 2, 1 };

        for (int i = 0; i < 8; i++)
        {
            if (x + dx[i] >= 0 && x + dx[i] < 8 &&
                y + dy[i] >= 0 && y + dy[i] < 8 &&
                (board[x + dx[i], y + dy[i]] == emptyID ||
                checkWhite(board[x + dx[i], y + dy[i]]) != thisWhite))
            {
                res.Add(new MoveInt(dx[i], dy[i], x, y, board[x, y], board[x + dx[i], y + dy[i]]));
            }
        }

        return res;
    }
    static private int[] dirLen(int x, int y)
    {
        int[] res = new int[8];
        res[0] = 7 - y; // north
        res[1] = 7 - x; // east
        res[2] = y; // south
        res[3] = x; // west
        res[4] = 7 - (x > y ? x : y); // NE: 7 - max(x, y)
        res[5] = ((7 - x) > y ? y : 7 - x); // SE: min(x, 7 - y) - if something is wrong, SE and NW might be swapped
        res[6] = (x > y ? y : x); // SW - just minimum between x and y
        res[7] = (x > (7 - y) ? 7 - y : x);  // NW - min(7 - x, y)
        return res;
    }


}
