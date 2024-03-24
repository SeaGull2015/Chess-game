using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ManageBoard : MonoBehaviour
{
    private void gameOver()
    {
        time = 0;
        gameRestart = true;
    }

    private void draw()
    {
        victoryTexter.SetText("Draw!\nThe game is over");
        gameOver();
    }

    private void victory(bool whiteLost)
    {
        if (!whiteLost) victoryTexter.SetText("White has won!");
        else victoryTexter.SetText("Black has won!");
        gameOver();
    }

    private void checkKingDeath(Move move)
    {
        if (move.target.ToLower() == "king")
        {
            victory(isWhite(move.target));
        }
    }
    private void checkKingDeath(string target)
    {
        if (target.ToLower() == "king")
        {
            victory(isWhite(target));
        }
    }

    private void gameoverEffect()
    {
        effectDone = true;
        Physics2D.gravity = new Vector2(0, -9);
        foreach (var piece in pieces)
        {
            if (piece != null) piece.thisRigidbody.velocity = new Vector2(rnd.Next(-20, 20), rnd.Next(0, 20));
        }
    }
}
