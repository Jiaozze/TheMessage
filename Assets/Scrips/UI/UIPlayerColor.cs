using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerColor : MonoBehaviour
{
    public Slider colorRed;
    public Slider colorBlue;
    public Slider colorGreen;
    public GameObject goToggles;
    public Toggle goTogRed;
    public Toggle goTogBlue;
    public Toggle goTogGreen;

    private List<PlayerColorEnum> playerColors = new List<PlayerColorEnum>();
    // Start is called before the first frame update
    void Start()
    {
        goToggles.SetActive(false);
    }

    public void SetColor(List<PlayerColorEnum> colors)
    {
        playerColors.Clear();
        foreach(var color in colors)
        {
            playerColors.Add(color);
        }

        bool haveRed = colors.Contains(PlayerColorEnum.Red);
        bool haveBlue = colors.Contains(PlayerColorEnum.Blue);
        bool haveGreen = colors.Contains(PlayerColorEnum.Green);
        colorRed.value = haveRed ? 1 : 0;
        colorBlue.value = haveBlue ? (haveRed ? (haveGreen ? 0.66f : 0.5f):1) : 0;
        colorGreen.value = haveGreen ? ((haveRed || haveBlue) ? ((haveRed && haveBlue) ? 0.33f : 0.5f) : 1) : 0;

        goTogRed.gameObject.SetActive(haveRed);
        goTogBlue.gameObject.SetActive(haveBlue);
        goTogGreen.gameObject.SetActive(haveGreen);
    }

    public void OnClick()
    {
        //Debug.LogError(playerColors.Count);
        if(goToggles.activeSelf)
        {
            goToggles.SetActive(false);            
        }
        else
        {
            goToggles.SetActive(true);
        }
    }
}
