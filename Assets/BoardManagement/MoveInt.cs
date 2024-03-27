using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct MoveInt
{
    public int startx;
    public int starty;
    public int dx;
    public int dy;
    public int piece;
    public int target;
    public List<MoveInt> additionalTargets; // this is for en passant and maybe castling
    public MoveInt(int tdx, int tdy, int sx, int sy, int who, int what, List<MoveInt> addTarg = null)
    {
        dx = tdx;
        dy = tdy;
        startx = sx;
        starty = sy;
        piece = who;
        target = what;
        additionalTargets = addTarg;
    }
    public bool Equals(MoveInt other) // this is bad, but I haven't found a way to iterate over all struct's fields.
    { // also this doesn't compare additional targets, mostly because situations where it is used shouldn't be repetetive (e.g. en passant)
        // And this function, ideally, shouldn't be used for anything else.
        return startx == other.startx && starty == other.starty && dx == other.dx && dy == other.dy && piece == other.piece && target == other.target;
    }
}

