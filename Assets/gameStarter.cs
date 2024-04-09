using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// This is the class that manages the main menu, game start
public class gameStarter : MonoBehaviour 
{
    // Start is called before the first frame update
    public TMP_Dropdown blackdropdown;
    public TMP_Dropdown whitedropdown;

    /// <summary>
    ///  Start up function sets up saved dropdown settings, when the main menu game scene is loaded
    /// </summary>
    void Start()
    {
        int black = PlayerPrefs.GetInt("blackAIval");
        int white = PlayerPrefs.GetInt("whiteAIval");
        if (black != 0) blackdropdown.value = black;
        if (white != 0) whitedropdown.value = white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Game start happens when the start button is pressed. Starting the game saves the chosen options of the game into permanent storage and loads the game scene.
    /// </summary>
    public void gameStart()
    {
        PlayerPrefs.SetString("blackAIstr", blackdropdown.options[blackdropdown.value].text);
        PlayerPrefs.SetString("whiteAIstr", whitedropdown.options[whitedropdown.value].text);
        PlayerPrefs.SetInt("blackAIval", blackdropdown.value);
        PlayerPrefs.SetInt("whiteAIval", whitedropdown.value);
        SceneManager.LoadScene("gameScene");
    }
}
