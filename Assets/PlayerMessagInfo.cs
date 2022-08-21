using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMessagInfo : MonoBehaviour
{
    public Text textTittle;
    public Button butChengQing;
    public GridLayoutGroup grid;
    public UICard itemCardUI;

    private Dictionary<int, UICard> items = new Dictionary<int, UICard>();

    private int cardId = 0;
    private void Awake()
    {
    }

    private void OnDisable()
    {
        cardId = 0;
    }

    public void Show(int playerId, bool showChengQing = false)
    {
        gameObject.SetActive(true);
        textTittle.text = "" + playerId + "����ҵ��鱨";

        foreach (var kv in items)
        {
            GameObject.Destroy(kv.Value.gameObject);
        }
        items.Clear();

        int i = GameManager.Singleton.players[playerId].messages.Count;
        foreach(var msg in GameManager.Singleton.players[playerId].messages)
        {
            UICard card = GameObject.Instantiate(itemCardUI, grid.transform);
            card.Init(i, msg);
            card.SetMessage(() => { 
                if(items.ContainsKey(cardId))
                {
                    items[cardId].OnSelect(false);
                }
                if(cardId != msg.id)
                {
                    cardId = msg.id;
                    if (items.ContainsKey(cardId))
                    {
                        items[cardId].OnSelect(true);
                    }
                    butChengQing.interactable = true;
                }
                else
                {
                    butChengQing.interactable = false;
                    cardId = 0;
                }
            });
            items[msg.id] = (card);
        }

        butChengQing.gameObject.SetActive(showChengQing);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClickChengQing()
    {
        var curCard = GameManager.Singleton.cardsHand[GameManager.Singleton.SelectCardId];

        if(GameManager.Singleton.IsWaitSaving != -1 && cardId != 0 && curCard.cardName == CardNameEnum.Cheng_Qing)
        {
            GameManager.Singleton.SendWhetherSave(true, cardId);
        }
        else if (cardId != 0 && curCard.cardName == CardNameEnum.Cheng_Qing && GameManager.Singleton.SelectPlayerId != -1)
        {
            ProtoHelper.SendUseCardMessage_ChengQing(GameManager.Singleton.SelectCardId, GameManager.Singleton.SelectPlayerId, cardId, GameManager.Singleton.seqId);
        }
        else
        {
            Debug.LogError("" + GameManager.Singleton.SelectPlayerId + cardId + GameManager.Singleton.IsWaitSaving + curCard.cardName);
        }
    }
}
