using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages castling options for both black and white players.
/// </summary>
public struct Castling
{
    public bool leftBlack;
    public bool rightBlack;
    public bool leftWhite;
    public bool rightWhite;

    /// <summary>
    /// Initializes a new instance of the <see cref="Castling"/> struct with specified parameters.
    /// </summary>
    /// <param name="lBlack">Whether left castling is available for black.</param>
    /// <param name="rBlack">Whether right castling is available for black.</param>
    /// <param name="lWhite">Whether left castling is available for white.</param>
    /// <param name="rWhite">Whether right castling is available for white.</param>
    public Castling(bool lBlack, bool rBlack, bool lWhite, bool rWhite)
    {
        leftBlack = lBlack;
        rightBlack = rBlack;
        rightWhite = rWhite;
        leftWhite = lWhite;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Castling"/> struct with an array of booleans.
    /// </summary>
    /// <param name="inps">An array representing the availability of castling options.</param>
    public Castling(bool[] inps)
    {
        leftBlack = inps[0];
        rightBlack = inps[1];
        leftWhite = inps[2];
        rightWhite = inps[3];
    }

    /// <summary>
    /// Disables castling options based on the player's color.
    /// </summary>
    /// <param name="inp">The color of the player.</param>
    public void voidUniversal(string inp)
    {
        if (ManageBoard.isWhite(inp)) voidWhite();
        else voidBlack();
    }

    /// <summary>
    /// Disables castling options for the black player.
    /// </summary>
    public void voidBlack()
    {
        leftBlack = false;
        rightBlack = false;
    }

    /// <summary>
    /// Disables castling options for the white player.
    /// </summary>
    public void voidWhite()
    {
        leftWhite = false;
        rightWhite = false;
    }
}
