using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for managing the promotion selection UI.
/// </summary>
public class promotionSelectScript : MonoBehaviour
{
    /// <summary>
    /// Reference to the TMP_Dropdown component used for selecting promotion options.
    /// </summary>
    public TMP_Dropdown myDown;

    /// <summary>
    /// Reference to the PieceBehaviour object that called the promotion selection.
    /// </summary>
    private PieceBehaviour caller;

    /// <summary>
    /// Method called before the first frame update.
    /// </summary>
    void Start()
    {
        // Deactivates the game object upon start.
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Method called once per frame.
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Sets up the promotion selection UI at the specified position with the caller PieceBehaviour object.
    /// </summary>
    /// <param name="x">The x-coordinate of the position.</param>
    /// <param name="y">The y-coordinate of the position.</param>
    /// <param name="whoCalled">The PieceBehaviour object that called the promotion selection.</param>
    public void setNewSelect(float x, float y, PieceBehaviour whoCalled)
    {
        // Sets the position of the UI element.
        transform.position = new Vector3(x, y, 0);

        // Sets the caller PieceBehaviour object.
        caller = whoCalled;

        // Activates the UI element.
        gameObject.SetActive(true);

        // Resets the dropdown value.
        myDown.value = 0;

        // Adds a listener to the dropdown to handle selection changes.
        myDown.onValueChanged.AddListener(delegate
        {
            onSelection(myDown);
        });
    }

    /// <summary>
    /// Handles the selection of an option in the dropdown.
    /// </summary>
    /// <param name="change">The TMP_Dropdown component representing the dropdown.</param>
    private void onSelection(TMP_Dropdown change)
    {
        // Calls the recallPromotion method of the caller PieceBehaviour object with the selected option.
        caller.recallPromotion(change.options[change.value].text.ToLower());

        // Resets the caller reference and removes the listener from the dropdown.
        caller = null;
        myDown.onValueChanged.RemoveAllListeners();

        // Deactivates the UI element.
        gameObject.SetActive(false);
    }
}