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
    public GameObject goSelect;
    public GameObject goTurnOn;
    public GameObject goMessageOn;
    private int playerId;
    // Start is called before the first frame update
    void Start()
    {
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
    }

    public void OnDrawCard(int totalCount, int count)
    {
        textCardCount.text = "" + totalCount;
    }

    public void OnTurn(bool isTurn)
    {
        goTurnOn.SetActive(isTurn);
    }

    public void OnMessageTurnTo(bool isTurn)
    {
        goMessageOn.SetActive(isTurn);
    }

    public void DoWaiting(int seconds)
    {

    }

    public void OnSelect(bool select)
    {
        goSelect.SetActive(select);
    }
}
