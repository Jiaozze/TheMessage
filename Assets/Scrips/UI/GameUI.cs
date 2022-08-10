using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public UIPlayer itemPlayerUI;
    public UICard itemCardUI;
    public RectTransform transCards;
    public Transform transPlayerSelf;
    public GridLayoutGroup gridCards;
    public GridLayoutGroup topPlayerGrid;
    public GridLayoutGroup leftPlayerGrid;
    public GridLayoutGroup rightPlayerGrid;

    public Dictionary<int, UICard> UICards = new Dictionary<int, UICard>();
    public Dictionary<int, UIPlayer> uiPlayers = new Dictionary<int, UIPlayer>();
    public void InitPlayers(int num)
    {
        if(uiPlayers.Count > 0)
        {
            foreach(var playerUI in uiPlayers)
            {
                GameObject.Destroy(playerUI.Value.gameObject);
            }
            uiPlayers.Clear();
        }
        int leftNum = (num - 1) / 3;
        int topNum = num - 1 - 2 * leftNum;

        var self = GameObject.Instantiate(itemPlayerUI, transPlayerSelf);
        self.Init(0);
        uiPlayers[0] = self;
        for(int i = 1; i < leftNum + 1; i++)
        {
            var player = GameObject.Instantiate(itemPlayerUI, rightPlayerGrid.transform);
            player.Init(i);
            uiPlayers[i] = player;
        }
        for (int i = 1 + leftNum; i < 1 + leftNum + topNum; i ++)
        {
            var player = GameObject.Instantiate(itemPlayerUI, topPlayerGrid.transform);
            player.Init(i);
            uiPlayers[i] = player;
        }
        for (int i = 1 + leftNum + topNum; i <1 + leftNum + topNum + leftNum; i++)
        {
            var player = GameObject.Instantiate(itemPlayerUI, leftPlayerGrid.transform);
            player.Init(i);
            uiPlayers[i] = player;
        }
        leftPlayerGrid.spacing = new Vector2(0, (leftPlayerGrid.GetComponent<RectTransform>().sizeDelta.y - leftPlayerGrid.cellSize.y * leftNum) / leftNum);
        rightPlayerGrid.spacing = new Vector2(0, (rightPlayerGrid.GetComponent<RectTransform>().sizeDelta.y - rightPlayerGrid.cellSize.y * leftNum) / leftNum);
        topPlayerGrid.spacing = new Vector2((topPlayerGrid.GetComponent<RectTransform>().sizeDelta.x - rightPlayerGrid.cellSize.x * topNum) / topNum, 0);


    }

    public void InitCards(int count)
    {
        if(count <= 4)
        {
            this.ClearCards();
            for(int i = count; i > 0; i--)
            {
                UICard card = GameObject.Instantiate(itemCardUI, transCards);
                card.Init(i);
                UICards[i] = (card);
            }
        }
        else 
        { 
        }
        CardsSizeFitter();
    }

    public void DrawCards(List<CardFS> cards)
    {
        int count = cards .Count;
        for (int i = count; i > 0; i--)
        {
            UICard card = GameObject.Instantiate(itemCardUI, transCards);
            card.Init(i, cards[count - i]);
            UICards[cards[i - 1].id] = card;
        }
        CardsSizeFitter();
    }

    public void ClearCards()
    {
        if (UICards.Count > 0)
        {
            foreach(var kv in UICards)
            {
                GameObject.Destroy(kv.Value.gameObject);
            }
            UICards.Clear();
        }
    }

    private void CardsSizeFitter()
    {
        if(UICards.Count * gridCards.cellSize.x <= transCards.sizeDelta.x)
        {
            gridCards.spacing = Vector2.zero;
        }
        else
        {
            float spacingX = -(UICards.Count * gridCards.cellSize.x - transCards.sizeDelta.x) / (UICards.Count - 1);
            gridCards.spacing = new Vector2(spacingX, 0);
        }
    }
    public void SetDeckNum(int num)
    {
        throw new NotImplementedException();
    }

    public void AddMsg(string v)
    {
        Debug.Log(v);
        //throw new NotImplementedException();
    }

    public void SetTurn()
    {
        throw new NotImplementedException();
    }

}
