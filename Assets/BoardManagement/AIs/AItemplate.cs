using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for other AIs
/// </summary>
public abstract class AItemplate
{
    public abstract Move getMove(bool thisWhite, string[,] brd, List<Move> mvs);

    public AItemplate() { }
}
