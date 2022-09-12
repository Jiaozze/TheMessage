using System;
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
    public List<CardFS> messages = new List<CardFS>();
    public bool alive = true;
    public RoleBase role;
    public Player(int id, RoleBase roleBase)
    {
        playerId = id;
        cardCount = 0;
        role = roleBase;
    }
    public int DrawCard(int count)
    {
        cardCount = cardCount + count;
        return cardCount;
    }

    public void DisCard(int count)
    {
        cardCount = cardCount - count;
    }

    public void AddMessage(CardFS card)
    {
        messages.Add(card);
        //Debug.LogError("" + playerId + "号玩家获得情报");

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
                break;
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

    internal void UpdateRole(role role)
    {
        if(this.role is Role_Unknown)
        {
            if(role != role.Unknown)
            {
                this.role = AllRoles.GetRole(role, playerId);
                this.role.isBack = false;
            }
        }
        else
        {
            if(role == role.Unknown)
            {
                this.role.isBack = true;
            }
            else
            {
                this.role.isBack = false;
            }
        }
    }
}
