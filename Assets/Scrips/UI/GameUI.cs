using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public PlayerUI playerUI;
    public CardUI cardUIItem;
    public RectTransform transCards;
    public Transform transPlayerSelf;
    public GridLayoutGroup gridCards;
    public GridLayoutGroup topPlayerGrid;
    public GridLayoutGroup leftPlayerGrid;
    public GridLayoutGroup rightPlayerGrid;

    private List<CardUI> cards = new List<CardUI>();
    public void InitPlayers(int num)
    {
        int leftNum = (num - 1) / 3;
        int topNum = num - 1 - 2 * leftNum;

        var self = GameObject.Instantiate(playerUI, transPlayerSelf);
    }

    public void InitCards(int count)
    {
        if(count <= 4)
        {
            this.ClearCards();
            for(int i = count; i > 0; i--)
            {
                CardUI card = GameObject.Instantiate(cardUIItem, transCards);
                card.Init(i);
                cards.Add(card);
            }
        }
        else 
        { 
        }
        CardsSizeFitter();
    }

    public void DrawCards(int count)
    {
        for (int i = count; i > 0; i--)
        {
            CardUI card = GameObject.Instantiate(cardUIItem, transCards);
            card.Init(i);
            cards.Add(card);
        }
        CardsSizeFitter();
    }

    public void ClearCards()
    {
        if (cards.Count > 0)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                GameObject.Destroy(cards[i].gameObject);
            }
            cards.Clear();
        }
    }

    private void CardsSizeFitter()
    {
        if(cards.Count * gridCards.cellSize.x <= transCards.sizeDelta.x)
        {
            gridCards.spacing = Vector2.zero;
        }
        else
        {
            float spacingX = -(cards.Count * gridCards.cellSize.x - transCards.sizeDelta.x) / (cards.Count - 1);
            gridCards.spacing = new Vector2(spacingX, 0);
        }
    }
    public void SetDeckNum(int num)
    {
        throw new NotImplementedException();
    }

    public void AddMsg(string v)
    {
        throw new NotImplementedException();
    }

    public void SetTurn()
    {
        throw new NotImplementedException();
    }

}
