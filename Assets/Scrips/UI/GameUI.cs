using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public ShiTanInfo shiTanInfo;
    public GameObject goWeiBiSelect;
    public WeiBiGiveCard weiBiGiveCard;
    public PlayerMessagInfo playerMessagInfo;
    public UIPlayer itemPlayerUI;
    public UICard itemCardUI;
    public Text textInfo;
    public RectTransform transCards;
    public Transform transPlayerSelf;
    public GridLayoutGroup gridCards;
    public GridLayoutGroup topPlayerGrid;
    public GridLayoutGroup leftPlayerGrid;
    public GridLayoutGroup rightPlayerGrid;

    public Dictionary<int, UICard> Cards = new Dictionary<int, UICard>();
    public Dictionary<int, UIPlayer> Players = new Dictionary<int, UIPlayer>();
    public void InitPlayers(int num)
    {
        if (Players.Count > 0)
        {
            foreach (var playerUI in Players)
            {
                GameObject.Destroy(playerUI.Value.gameObject);
            }
            Players.Clear();
        }
        int leftNum = (num - 1) / 3;
        int topNum = num - 1 - 2 * leftNum;

        var self = GameObject.Instantiate(itemPlayerUI, transPlayerSelf);
        self.Init(0);
        Players[0] = self;
        for (int i = 1; i < leftNum + 1; i++)
        {
            var player = GameObject.Instantiate(itemPlayerUI, rightPlayerGrid.transform);
            player.Init(i);
            Players[i] = player;
        }
        for (int i = 1 + leftNum; i < 1 + leftNum + topNum; i++)
        {
            var player = GameObject.Instantiate(itemPlayerUI, topPlayerGrid.transform);
            player.Init(i);
            Players[i] = player;
        }
        for (int i = 1 + leftNum + topNum; i < 1 + leftNum + topNum + leftNum; i++)
        {
            var player = GameObject.Instantiate(itemPlayerUI, leftPlayerGrid.transform);
            player.Init(i);
            Players[i] = player;
        }
        leftPlayerGrid.spacing = new Vector2(0, (leftPlayerGrid.GetComponent<RectTransform>().sizeDelta.y - leftPlayerGrid.cellSize.y * leftNum) / leftNum);
        rightPlayerGrid.spacing = new Vector2(0, (rightPlayerGrid.GetComponent<RectTransform>().sizeDelta.y - rightPlayerGrid.cellSize.y * leftNum) / leftNum);
        topPlayerGrid.spacing = new Vector2((topPlayerGrid.GetComponent<RectTransform>().sizeDelta.x - rightPlayerGrid.cellSize.x * topNum) / topNum, 0);


    }

    public void InitCards(int count)
    {
        if (count <= 4)
        {
            this.ClearCards();
            for (int i = count; i > 0; i--)
            {
                UICard card = GameObject.Instantiate(itemCardUI, transCards);
                card.Init(i);
                Cards[i] = (card);
            }
        }
        else
        {
        }
        CardsSizeFitter();
    }

    public void DrawCards(List<CardFS> cards)
    {
        int count = cards.Count;
        for (int i = count; i > 0; i--)
        {
            UICard card = GameObject.Instantiate(itemCardUI, transCards);
            card.Init(i, cards[count - i]);
            Cards[cards[count - i].id] = card;
        }
        CardsSizeFitter();
    }

    public void DisCards(List<CardFS> cards)
    {
        foreach (var card in cards)
        {
            int cardId = card.id;

            if (Cards.ContainsKey(cardId))
            {
                Cards[cardId].OnDiscard();
                Cards.Remove(cardId);
            }
        }
    }

    public void ClearCards()
    {
        if (Cards.Count > 0)
        {
            foreach (var kv in Cards)
            {
                GameObject.Destroy(kv.Value.gameObject);
            }
            Cards.Clear();
        }
    }

    private void CardsSizeFitter()
    {
        if (Cards.Count * gridCards.cellSize.x <= transCards.sizeDelta.x)
        {
            gridCards.spacing = Vector2.zero;
        }
        else
        {
            float spacingX = -(Cards.Count * gridCards.cellSize.x - transCards.sizeDelta.x) / (Cards.Count - 1);
            gridCards.spacing = new Vector2(spacingX, 0);
        }
    }
    public void SetDeckNum(int num)
    {
        throw new NotImplementedException();
    }

    public void AddMsg(string v)
    {
        textInfo.text = textInfo.text + "\n" + v;
        //Debug.Log(v);
        //throw new NotImplementedException();
    }

    public void OnclickUserCard()
    {
        GameManager.Singleton.SendUseCard();
    }
    public void OnclickEnd()
    {
        GameManager.Singleton.SendEndWaiting();
    }
    public void ShowShiTanInfo(CardFS card, int waitingTime)
    {
        shiTanInfo.Show(card, waitingTime);
    }

    public void HideShiTanInfo()
    {
        shiTanInfo.gameObject.SetActive(false);
    }
    public void OnUseCard(int user, int targetUser, CardFS card = null)
    {
        if (Players.ContainsKey(user))
        {
            Players[user].UseCard(card);
        }
        
        if (user == GameManager.Singleton.SelfPlayerId && card != null)
        {
            int cardId = card.id;
            if (Cards.ContainsKey(cardId))
            {
                Cards[cardId].OnUse();
                Cards.Remove(cardId);
            }
        }
    }

    public void ShowTopCard(CardFS card)
    {
        Debug.LogError("展示了牌堆顶的牌，" + card.cardName);
    }

    public void ShowWeiBiSelect(bool show)
    {
        goWeiBiSelect.SetActive(show);
    }

    public void ShowWeiBiGiveCard(CardNameEnum cardWant, int user, int waitTime)
    {
        weiBiGiveCard.Show(cardWant, user, waitTime);
    }

    public void ShowPlayerMessageInfo(int playerId, bool showChengQing = false)
    {
        playerMessagInfo.Show(playerId, showChengQing);
    }

    public void HidePlayerMessageInfo()
    {
        playerMessagInfo.gameObject.SetActive(false);
    }
}
