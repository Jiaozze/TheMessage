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
    public GameObject goTogRed;
    public GameObject goTogBlue;
    public GameObject goTogGreen;

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

        goTogRed.SetActive(haveRed);
        goTogBlue.SetActive(haveBlue);
        goTogGreen.SetActive(haveGreen);
    }

    public void OnClick()
    {
        Debug.LogError(playerColors.Count);
        if(goToggles.activeSelf)
        {
            goToggles.SetActive(false);            
        }
        else if(playerColors.Count > 1)
        {
            goToggles.SetActive(true);
        }
    }
}
