using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AItemplate
{
    public abstract Move getMove(bool thisWhite, string[,] brd, List<Move> mvs);
    public abstract Move getMove(bool thisWhite, Move lastMove, List<Move> mvs);

    public AItemplate() { }
}
