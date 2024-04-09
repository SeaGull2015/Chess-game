using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Random opponent - returs a random move.
/// </summary>
public class RandomerOpponent : AItemplate
{
    private System.Random rnd = new System.Random();
    public override Move getMove(bool thisWhite, string[,] brd, List<Move> mvs)
    {
        int r = rnd.Next(mvs.Count);
        return mvs[r];
    }
}

