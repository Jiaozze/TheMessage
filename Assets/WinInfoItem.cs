using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinInfoItem : MonoBehaviour
{
    public Text textName;
    public Text textIdentity;
    public Text textTask;
    public GameObject goDeclare;
    public GameObject goWin;
    public GameObject goLose;

    public void Show(int winnerId, bool isWin, PlayerColorEnum playerColorEnum, SecretTaskEnum secretTaskEnum, bool isDeclare)
    {
        gameObject.SetActive(true);
        goWin.SetActive(isWin);
        goLose.SetActive(!isWin);
        goDeclare.SetActive(isDeclare);
        textName.text = "" + GameManager.Singleton.players[winnerId].name;
        textIdentity.text = LanguageUtils.GetIdentityName(playerColorEnum);
        if(playerColorEnum == PlayerColorEnum.Green)
        {
            textTask.gameObject.SetActive(true);
            textTask.text = LanguageUtils.GetTaskName(secretTaskEnum);
        }
        else
        {
            textTask.gameObject.SetActive(false);
        }
    }
}
