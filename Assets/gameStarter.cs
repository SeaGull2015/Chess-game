using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameStarter : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Dropdown blackdropdown;
    public TMP_Dropdown whitedropdown;
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

    public void gameStart()
    {
        PlayerPrefs.SetString("blackAIstr", blackdropdown.options[blackdropdown.value].text);
        PlayerPrefs.SetString("whiteAIstr", whitedropdown.options[whitedropdown.value].text);
        PlayerPrefs.SetInt("blackAIval", blackdropdown.value);
        PlayerPrefs.SetInt("whiteAIval", whitedropdown.value);
        SceneManager.LoadScene("gameScene");
    }
}
