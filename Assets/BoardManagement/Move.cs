using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Move(MoveInt mv)
    {
        dx = mv.dx;
        dy = mv.dy;
        startx = mv.startx;
        starty = mv.starty;
        piece = MoveCalculatorInt.reverseConversionIDdict[mv.piece];
        target = MoveCalculatorInt.reverseConversionIDdict[mv.target];
        if (mv.additionalTargets != null)
        {
            additionalTargets = new List<Move>();
            foreach (MoveInt submove in mv.additionalTargets)
            {
                additionalTargets.Add(new Move(submove));
            }
        }
        else additionalTargets = null;
    }
    public bool Equals(Move other) // this is bad, but I haven't found a way to iterate over all struct's fields.
    { // also this doesn't compare additional targets, mostly because situations where it is used shouldn't be repetetive (e.g. en passant)
        // And this function, ideally, shouldn't be used for anything else.
        return startx == other.startx && starty == other.starty && dx == other.dx && dy == other.dy && piece == other.piece && target == other.target;
    }
}

