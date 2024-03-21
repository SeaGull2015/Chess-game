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
    public Move(int tdx, int tdy, int sx, int sy, string who, string what, List<Move> addTarg = null)
    {
        dx = tdx;
        dy = tdy;
        startx = sx;
        starty = sy;
        piece = who;
        target = what;
        additionalTargets = addTarg;
    }
}

