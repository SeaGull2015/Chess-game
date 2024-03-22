using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move
{
    public int startx;
    public int starty;
    public int dx;
    public int dy;
    public string piece;
    public string target;
    public List<Move> additionalTargets; // this is for en passant and maybe castling
    public bool nullMove;
    public Move(int tdx, int tdy, int sx, int sy, string who, string what, List<Move> addTarg = null, bool isNullMove = false)
    {
        dx = tdx;
        dy = tdy;
        startx = sx;
        starty = sy;
        piece = who;
        target = what;
        additionalTargets = addTarg;
        nullMove = isNullMove;
    }
    public Move(bool isNullMove, int tdx = 0, int tdy = 0, int sx = 0, int sy = 0, string who = "", string what = "", List<Move> addTarg = null)
    {
        dx = tdx;
        dy = tdy;
        startx = sx;
        starty = sy;
        piece = who;
        target = what;
        additionalTargets = addTarg;
        nullMove = true;
    }
}

