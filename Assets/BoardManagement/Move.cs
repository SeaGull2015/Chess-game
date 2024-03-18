using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Move
{
    public int startx;
    public int starty;
    public int dx;
    public int dy;
    public List<Move> additionalTargets; // this is for en passant and maybe castling
    public Move(int tdx, int tdy, int sx, int sy, List<Move> addTarg = null)
    {
        dx = tdx;
        dy = tdy;
        startx = sx;
        starty = sy;
        additionalTargets = addTarg;
    }
}

