using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ManageBoard : MonoBehaviour
{
    /// <summary>
    /// Handles the game over state by resetting the game timer and setting the game restart flag to true.
    /// </summary>
    private void gameOver()
    {
        time = 0;
        gameRestart = true;
    }

    /// <summary>
    /// Displays a draw message and triggers the game over state.
    /// </summary>
    private void draw()
    {
        victoryTexter.SetText("Draw!\nThe game is over");
        gameOver();
    }

    /// <summary>
    /// Handles victory condition by displaying the appropriate victory message based on whether white has lost or not.
    /// </summary>
    /// <param name="whiteLost">Indicates whether the white player has lost.</param>
    private void victory(bool whiteLost)
    {
        if (!whiteLost)
            victoryTexter.SetText("White has won!");
        else
            victoryTexter.SetText("Black has won!");

        gameOver();
    }

    /// <summary>
    /// Checks if the move results in the death of a king piece and triggers victory accordingly.
    /// </summary>
    /// <param name="move">The move being made.</param>
    private void checkKingDeath(Move move)
    {
        if (move.target.ToLower() == "king")
        {
            victory(isWhite(move.target));
        }
    }

    /// <summary>
    /// Checks if the specified target piece is a king and triggers victory accordingly.
    /// </summary>
    /// <param name="target">The target piece.</param>
    private void checkKingDeath(string target)
    {
        if (target.ToLower() == "king")
        {
            victory(isWhite(target));
        }
    }

    /// <summary>
    /// Applies game over effects by adjusting gravity and adding random velocity to pieces.
    /// </summary>
    private void gameoverEffect()
    {
        effectDone = true;
        Physics2D.gravity = new Vector2(0, -9);
        foreach (var piece in pieces)
        {
            if (piece != null)
                piece.thisRigidbody.velocity = new Vector2(rnd.Next(-20, 20), rnd.Next(0, 20));
        }
    }
}
