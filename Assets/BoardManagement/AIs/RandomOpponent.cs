using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public partial class ManageBoard
{

    private List<Move> tryRND(bool thisWhite, string[,] brd, List<Move>[,] mvs, int sidePieces)
    {
        sidePieces = rnd.Next(sidePieces);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (brd[i, j] == "empty") continue;
                if (thisWhite == isWhite(brd[i, j]) && sidePieces == 0) return mvs[i, j];
                if (thisWhite == isWhite(brd[i, j])) sidePieces--;
            }
        }
        return new List<Move>();
    }
    private Move randomOpponent(bool thisWhite, string[,] brd, List<Move>[,] mvs)
    {
        int sidePieces = 0;
        foreach (string pc in brd)
        {
            if (pc == "empty") continue;
            if (thisWhite == isWhite(pc)) sidePieces++;
        }
        for (int i = 0; i < 10000; i++) // 10 tries to get a random move
        {
            List<Move> tmp = tryRND(thisWhite, brd, mvs, sidePieces);
            if (tmp.Count > 0)
            {
                return tmp[rnd.Next(tmp.Count)];
            }
        }

        return mvs[1, 1][1];
    }
}
