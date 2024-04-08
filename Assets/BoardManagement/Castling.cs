using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Castling
{
    public bool leftBlack;
    public bool rightBlack;
    public bool leftWhite;
    public bool rightWhite;

    public Castling(bool lBlack, bool rBlack, bool lWhite, bool rWhite)
    {
        leftBlack = lBlack;
        rightBlack = rBlack;
        rightWhite = rWhite;
        leftWhite = lWhite;
    }
    public Castling(bool[] inps)
    {
        leftBlack = inps[0];
        rightBlack = inps[1];
        leftWhite = inps[2];
        rightWhite = inps[3];
    }

    public void voidUniversal(string inp)
    {
        if (ManageBoard.isWhite(inp)) voidWhite();
        else voidBlack();
    }

    public void voidBlack()
    {
        leftBlack = false;
        rightBlack = false;
    }

    public void voidWhite()
    {
        leftWhite = false;
        rightWhite = false;
    }
}
