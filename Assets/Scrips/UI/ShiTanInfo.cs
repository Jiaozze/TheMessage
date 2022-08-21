using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShiTanInfo : MonoBehaviour
{
    public UICard card;
    public Slider slider;
    public Text textInfo;
    public Button butDrawCard;
    public Button butDisCard;
    // Start is called before the first frame update
    private bool isDrawCard = false;
    public void Show(CardFS cardInfo, int time)
    {
        gameObject.SetActive(true);
        card.Init(1, cardInfo);
        StopAllCoroutines();
        StartCoroutine(TimeDown(time));

        isDrawCard = false;
        foreach(var color in cardInfo.shiTanColor)
        {
            if((int)color == (int)GameManager.Singleton.GetPlayerColor())
            {
                isDrawCard = true;
                break;
            }
        }
        butDrawCard.interactable = isDrawCard;
        string info = isDrawCard? "你被试探了，请摸一张牌" : "你被试探了，请弃一张牌";
        textInfo.text = info;
    }

    private IEnumerator TimeDown(int seconds)
    {
        float total = seconds;
        float secondsF = seconds;
        while (secondsF > 0)
        {
            butDisCard.interactable = (!isDrawCard && GameManager.Singleton.cardsHand.Count < 1) || (!isDrawCard && GameManager.Singleton.SelectCardId != -1) ;
            slider.value = secondsF / total;
            secondsF = secondsF - 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        gameObject.SetActive(false);
    }

    public void OnClickDrawCard()
    {
        if(isDrawCard)
        {
            GameManager.Singleton.SendDoShiTan(0);
        }
    }

    public void OnClickDisCard()
    {
        if(!isDrawCard)
        {
            int cardId = GameManager.Singleton.SelectCardId;
            cardId = cardId < 0 ? 0 : cardId;
            if (cardId != -1 && GameManager.Singleton.cardsHand.ContainsKey(cardId))
            {
                GameManager.Singleton.SendDoShiTan(cardId);
            }
            else
            {
                Debug.LogError("选一张牌丢弃");
            }
        }
    }
}
