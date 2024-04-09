using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing the behavior of a square on a game board.
/// </summary>
public class SquareBehaviour : MonoBehaviour
{
    /// <summary>
    /// Indicates whether the square is considered a light-colored square.
    /// </summary>
    public bool isLight = false;

    /// <summary>
    /// Color of the square when it is considered light.
    /// </summary>
    public Color ColorLight = Color.white;

    /// <summary>
    /// Color of the square when it is considered dark.
    /// </summary>
    public Color ColorDark = Color.black;

    /// <summary>
    /// Reference to the SpriteRenderer component attached to this GameObject.
    /// </summary>
    public SpriteRenderer render;

    /// <summary>
    /// Method called before the first frame update.
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// Method called once per frame.
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Changes the color of the square to the specified color.
    /// </summary>
    /// <param name="color">The color to which the square should be changed.</param>
    public void lightUp(Color color)
    {
        render.color = color;
    }

    /// <summary>
    /// Updates the color of the square based on whether it is considered light or dark.
    /// </summary>
    public void updateColor()
    {
        if (isLight)
        {
            render.color = ColorLight;
        }
        else
        {
            render.color = ColorDark;
        }
    }
}