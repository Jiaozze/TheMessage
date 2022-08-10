using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public PlayerUI playerUI;
    public UICard cardUIItem;
    public RectTransform transCards;
    public Transform transPlayerSelf;
    public GridLayoutGroup gridCards;
    public GridLayoutGroup topPlayerGrid;
    public GridLayoutGroup leftPlayerGrid;
    public GridLayoutGroup rightPlayerGrid;

    private List<UICard> UICards = new List<UICard>();
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
                UICard card = GameObject.Instantiate(cardUIItem, transCards);
                card.Init(i);
                UICards.Add(card);
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
            UICard card = GameObject.Instantiate(cardUIItem, transCards);
            card.Init(i, cards[count - i]);
            UICards.Add(card);
        }
        CardsSizeFitter();
    }

    public void ClearCards()
    {
        if (UICards.Count > 0)
        {
            for (int i = 0; i < UICards.Count; i++)
            {
                GameObject.Destroy(UICards[i].gameObject);
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
