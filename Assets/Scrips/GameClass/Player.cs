using System.Collections.Generic;
using UnityEngine;

public enum PlayerColorEnum
{
    Green = 0, // 对于身份，则是绿色（神秘人）；对于卡牌，则是黑色
    Red = 1,   // 红色
    Blue = 2,  // 蓝色
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
        Debug.LogError("" + playerId + "号玩家获得情报");

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
