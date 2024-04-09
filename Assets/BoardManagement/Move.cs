using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a single move in a chess game.
/// </summary>
public struct Move
{
    /// <summary>
    /// The x-coordinate of the starting position.
    /// </summary>
    public int startx;

    /// <summary>
    /// The y-coordinate of the starting position.
    /// </summary>
    public int starty;

    /// <summary>
    /// The change in the x-coordinate for the move.
    /// </summary>
    public int dx;

    /// <summary>
    /// The change in the y-coordinate for the move.
    /// </summary>
    public int dy;

    /// <summary>
    /// The piece making the move.
    /// </summary>
    public string piece;

    /// <summary>
    /// The target position of the move.
    /// </summary>
    public string target;

    /// <summary>
    /// Additional targets for special moves like en passant or castling.
    /// </summary>
    public List<Move> additionalTargets;

    /// <summary>
    /// Initializes a new instance of the <see cref="Move"/> struct with specified parameters.
    /// </summary>
    /// <param name="tdx">The change in the x-coordinate for the move.</param>
    /// <param name="tdy">The change in the y-coordinate for the move.</param>
    /// <param name="sx">The x-coordinate of the starting position.</param>
    /// <param name="sy">The y-coordinate of the starting position.</param>
    /// <param name="who">The piece making the move.</param>
    /// <param name="what">The target position of the move.</param>
    /// <param name="addTarg">Additional targets for special moves (optional).</param>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Move"/> struct from a <see cref="MoveInt"/> object.
    /// </summary>
    /// <param name="mv">The MoveInt object to convert.</param>
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

    /// <summary>
    /// Determines whether the specified <see cref="Move"/> object is equal to the current <see cref="Move"/>.
    /// </summary>
    /// <param name="other">The <see cref="Move"/> to compare with the current <see cref="Move"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="Move"/> is equal to the current <see cref="Move"/>; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method compares the startx, starty, dx, dy, piece, and target fields of the <see cref="Move"/> objects.
    /// </remarks>
    public bool Equals(Move other)
    {
        return startx == other.startx && starty == other.starty && dx == other.dx && dy == other.dy && piece == other.piece && target == other.target;
    }
}



