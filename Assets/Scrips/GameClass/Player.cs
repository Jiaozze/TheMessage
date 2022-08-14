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
    public List<CardFS> messageRed = new List<CardFS>();
    public List<CardFS> messageBlue = new List<CardFS>();
    public List<CardFS> messageBlack = new List<CardFS>();

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
        Debug.LogError("" + playerId + "����һ���鱨");

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
}
