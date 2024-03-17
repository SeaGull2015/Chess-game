using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public partial class ManageBoard
{
    private Move directSlidingMove(int distance, int dir, int sx, int sy) // this kinda sucks
    {
        if (dir == 0) return new Move(0, distance, sx, sy);
        if (dir == 1) return new Move(distance, 0, sx, sy);
        if (dir == 2) return new Move(0, -distance, sx, sy);
        if (dir == 3) return new Move(-distance, 0, sx, sy);
        if (dir == 4) return new Move(distance, distance, sx, sy);
        if (dir == 5) return new Move(distance, -distance, sx, sy);
        if (dir == 6) return new Move(-distance, -distance, sx, sy);
        if (dir == 7) return new Move(-distance, distance, sx, sy);
        else throw new Exception("directMove() wrong input dir");
    }

    private List<Move> getMoves(int x, int y)
    {
        string type = board[x, y];
        type = type.ToLower();
        string[] sliders = { "bishop", "queen", "rook" };
        if (sliders.Contains(type)) return getSlidingMoves(x, y);
        else if (type == "pawn") return getPawnMoves(x, y);
        else if (type == "king") return getKingMoves(x, y);
        else if (type == "knight") return getKnightMoves(x, y);
        else throw new Exception("getMoves() bad type"); /*return new List<Move>();*/
    }

    private List<Move> getSlidingMoves(int x, int y)
    {
        List<Move> res = new List<Move>();
        string type = board[x, y];
        bool thisWhite = isWhite(type);
        type = type.ToLower();

        int[] dirs = dirLen(x, y);

        if (type == "bishop" || type == "queen") // the following code somewhat sucks, but as of now I don't have ideas how to implement it better
        {
            for (int i = 4; i < 8; i++)
            {
                for (int j = 1; j <= dirs[i]; j++)
                {
                    Move tMove = directSlidingMove(j, i, x, y);
                    if (board[x + tMove.dx, y + tMove.dy] == "empty") res.Add(tMove);
                    else if (isWhite(board[x + tMove.dx, y + tMove.dy]) == thisWhite) break;
                    else if (isWhite(board[x + tMove.dx, y + tMove.dy]) != thisWhite)
                    {
                        res.Add(directSlidingMove(j, i, x, y));
                        break;
                    }
                }
            }
        }
        if (type == "rook" || type == "queen")
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= dirs[i]; j++)
                {
                    Move tMove = directSlidingMove(j, i, x, y);
                    if (board[x + tMove.dx, y + tMove.dy] == "empty") res.Add(tMove);
                    else if (isWhite(board[x + tMove.dx, y + tMove.dy]) == thisWhite) break;
                    else if (isWhite(board[x + tMove.dx, y + tMove.dy]) != thisWhite)
                    {
                        res.Add(directSlidingMove(j, i, x, y));
                        break;
                    }
                }
            }
        }

        return res;
    }

    private List<Move> getPawnMoves(int x, int y)
    {
        List<Move> res = new List<Move>();
        bool thisWhite = isWhite(board[x, y]);
        int dir = thisWhite ? 1 : -1;
        int yLim = thisWhite ? 7 : 0;

        if (y != yLim && board[x, y + dir] == "empty")
        {
            res.Add(new Move(0, dir, x, y));
        }

        if (y != yLim && x < 7 && board[x + 1, y + dir] != "empty" && thisWhite != isWhite(board[x + 1, y + dir])) // I would rather have y < 7 or y > 0, but it would be ugly, so y != yLim
        {
            res.Add(new Move(1, dir, x, y));
        }
        if (y != yLim && x > 0 && board[x - 1, y + dir] != "empty" && thisWhite != isWhite(board[x - 1, y + dir]))
        {
            res.Add(new Move(-1, dir, x, y));
        }

        if (false) // TODO: insert en passant here
        {

        }
        if (false) // TODO: insert upgrade behaviour here
        {

        }


        return res;
    }

    private List<Move> getKingMoves(int x, int y)
    {
        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        List<Move> res = new List<Move>();
        bool thisWhite = isWhite(board[x, y]);

        for (int i = 0; i < 8; i++)
        {
            if (x + dx[i] >= 0 && x + dx[i] < 8 &&
                y + dy[i] >= 0 && y + dy[i] < 8 &&
                (board[x + dx[i], y + dy[i]] == "empty" ||
                isWhite(board[x + dx[i], y + dy[i]]) != thisWhite))
            {
                res.Add(new Move(dx[i], dy[i], x, y));
            }
        }

        return res;
    }

    private List<Move> getKnightMoves(int x, int y)
    {
        List<Move> res = new List<Move>();
        bool thisWhite = isWhite(board[x, y]);

        int[] dx = { -2, -1, 1, 2, 2, 1, -1, -2 };
        int[] dy = { -1, -2, -2, -1, 1, 2, 2, 1 };

        for (int i = 0; i < 8; i++)
        {
            if (x + dx[i] >= 0 && x + dx[i] < 8 &&
                y + dy[i] >= 0 && y + dy[i] < 8 &&
                (board[x + dx[i], y + dy[i]] == "empty" ||
                isWhite(board[x + dx[i], y + dy[i]]) != thisWhite))
            {
                res.Add(new Move(dx[i], dy[i], x, y));
            }
        }

        return res;
    }
    private int[] dirLen(int x, int y)
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
