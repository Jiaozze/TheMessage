using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerColorEnum
{
    Green = 0, // ������ݣ�������ɫ�������ˣ������ڿ��ƣ����Ǻ�ɫ
    Red = 1,   // ��ɫ
    Blue = 2,  // ��ɫ
}

public class Player
{
    public int playerId;
    public List<PlayerColorEnum>  playerColor = new List<PlayerColorEnum>() { PlayerColorEnum.Blue, PlayerColorEnum.Green, PlayerColorEnum.Red };
    public int cardCount;
    public List<CardFS> messages = new List<CardFS>();

    public Player(int id)
    {
        playerId = id;
        cardCount = 0;
    }
    public int DrawCard(int count)
    {
        cardCount = cardCount + count;
        return cardCount;
    }

    public void AddMessage(CardFS card)
    {
        messages.Add(card);
        //Debug.LogError("" + playerId + "����һ���鱨");

        //if(card.color.Contains(CardColorEnum.Black))
        //{
        //    messageBlack.Add(card);
        //}
        //switch (card.color)
        //{
        //    case CardColorEnum.Black:
        //        messageBlack.Add(card);
        //        break;
        //}
    }

    public void RemoveMessage(int targetCardId)
    {
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            if (messages[i].id == targetCardId)
            {
                messages.RemoveAt(i);
            }
        }
    }

    public int GetMessageCount(CardColorEnum color)
    {
        int count = 0;
        foreach(var msg in messages)
        {
            if(msg.color.Contains(color))
            {
                count++;
            }
        }
        return count;
    }

    public int GetMessageCountAll()
    {
        return messages.Count;
    }
}
