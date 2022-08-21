using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public Button button;
    public Text textPlayerId;
    public Text textCardCount;
    public Text textMessageRedCount;
    public Text textMessageBlueCount;
    public Text textMessageBlackCount;
    public Slider slider;
    public GameObject goSelect;
    public GameObject goTurnOn;
    public GameObject goMessageOn;
    public UIPlayerColor playerColor;
    public GameObject goDie;
    public GameObject goLose;
    public GameObject goLock;


    private int playerId;

    // Start is called before the first frame update
    void Start()
    {
        goDie.SetActive(false);
        goLock.SetActive(false);
        slider.gameObject.SetActive(false);
        button.onClick.AddListener(() => { GameManager.Singleton.SelectPlayerId = playerId; });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(int id)
    {
        this.gameObject.SetActive(true);
        goSelect.SetActive(false);
        goTurnOn.SetActive(false);
        goMessageOn.SetActive(false);
        playerId = id;
        textPlayerId.text = "" + id;
        textCardCount.text = "0";
        playerColor.SetColor(GameManager.Singleton.players[id].playerColor);
        RefreshMessage();
    }

    public void OnClickMessage()
    {
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(playerId);
    }

    public void OnDrawCard(int totalCount, int count)
    {
        textCardCount.text = "" + totalCount;
    }

    public void UseCard(CardFS cardInfo = null)
    {
        textCardCount.text = "" + GameManager.Singleton.players[playerId].cardCount;
    }
    public void SendCard()
    {
        textCardCount.text = "" + GameManager.Singleton.players[playerId].cardCount;
    }
    public void Discard(List<CardFS> cards)
    {
        textCardCount.text = "" + GameManager.Singleton.players[playerId].cardCount;
    }

    public void RefreshMessage()
    {
        textMessageRedCount.text = "" + GameManager.Singleton.players[playerId].GetMessageCount(CardColorEnum.Red);
        textMessageBlueCount.text = "" + GameManager.Singleton.players[playerId].GetMessageCount(CardColorEnum.Blue);
        textMessageBlackCount.text = "" + GameManager.Singleton.players[playerId].GetMessageCount(CardColorEnum.Black);
    }

    public void OnTurn(bool isTurn)
    {
        goTurnOn.SetActive(isTurn);
    }

    public void OnMessageTurnTo(bool isTurn)
    {
        goMessageOn.SetActive(isTurn);
    }

    public void OnWaiting(int seconds)
    {
        //if (playerId == 0) Debug.LogError(seconds);

        if (seconds <= 0)
        {
            StopAllCoroutines();
            slider.gameObject.SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(true);
            StartCoroutine(ShowSlider(seconds));
        }
    }

    public void OnDie(bool loseGame)
    {
        goDie.SetActive(true);
        goLose.SetActive(loseGame);
    }

    private IEnumerator ShowSlider(int seconds)
    {
        float total = seconds;
        float secondsF = seconds;
        while (secondsF > 0)
        {
            slider.value = secondsF / total;
            secondsF = secondsF - 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        //if (playerId == 0) Debug.LogError("0000000000000000");
        slider.gameObject.SetActive(false);
    }

    public void OnSelect(bool select)
    {
        goSelect.SetActive(select);
    }

    public void SetLock(bool isLock)
    {
        goLock.SetActive(isLock);
    }
}
