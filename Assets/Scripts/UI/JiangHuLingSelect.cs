using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JiangHuLingSelect : MonoBehaviour
{
    public Toggle tgRed;
    public Toggle tgBlue;
    public Toggle tgBlack;

    public CardColorEnum selectColor;

    private void OnEnable()
    {
        switch (UserSkill_JiangHuLing.color)
        {
            case CardColorEnum.Red:
                tgRed.isOn = true;
                break;
            case CardColorEnum.Blue:
                tgBlue.isOn = true;
                break;
            case CardColorEnum.Black:
                tgBlack.isOn = true;
                break;
        }
    }
    public void OnToggle()
    {
        if (tgRed.isOn) UserSkill_JiangHuLing.color = CardColorEnum.Red;
        else if (tgBlue.isOn) UserSkill_JiangHuLing.color = CardColorEnum.Blue;
        else if (tgBlack.isOn) UserSkill_JiangHuLing.color = CardColorEnum.Black;
    }
}
