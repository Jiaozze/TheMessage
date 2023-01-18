using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 白菲菲【怜悯】：你传出的非黑色情报被接收后，可以从你或接收者的情报区选择一张黑色情报加入你的手牌。
public class UserSkill_LianMin : SkillBase
{
    public override string name { get { return "怜悯"; } }
    public override bool canUse
    {
        get
        {
            return false;
        }
    }

    public override string Des => "怜悯:你传出的非黑色情报被接收后，可以从你或接收者的情报区选择一张黑色情报加入你的手牌。\n";

    private int selectMessageId = 0;
    private int selectPlayerId = -1;
    public UserSkill_LianMin(int id)
    {
        playerId = id;
    }

    public override bool CheckTriger()
    {
        if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
        {
            return false;
        }

        if (GameManager.Singleton.curPhase != PhaseEnum.Receive_Phase)
        {
            return false;
        }

        PrepareUse();
        return true;
    }
    public override void PrepareUse()
    {
        base.PrepareUse();
        bool haveBlack = false;

        foreach (var message in GameManager.Singleton.players[GameManager.SelfPlayerId].messages)
        {
            if (message.color.Contains(CardColorEnum.Black))
            {
                haveBlack = true;
                break;
            }
        }

        foreach (var message in GameManager.Singleton.players[GameManager.Singleton.CurMessagePlayerId].messages)
        {
            if (message.color.Contains(CardColorEnum.Black))
            {
                haveBlack = true;
                break;
            }
        }

        if (!haveBlack)
        {
            Cancel();
            return;
        }

        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("可以发动技能怜悯，从你或接收者的情报区选择一张黑色情报加入你的手牌");
        if (GameManager.Singleton.CurMessagePlayerId == GameManager.SelfPlayerId)
        {
            GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
        }
    }
    public override void Use()
    {
        if(selectMessageId != 0 && selectPlayerId != -1)
        {
            foreach(var message in GameManager.Singleton.players[selectPlayerId].messages)
            {
                if(message.id == selectMessageId)
                {
                    if(message.color.Contains(CardColorEnum.Black))
                    {
                        ProtoHelper.SendSkill_LianMin(selectPlayerId, selectMessageId, GameManager.Singleton.seqId);
                    }
                    else
                    {
                        GameManager.Singleton.gameUI.ShowInfo("必须选择黑色情报");
                    }
                    return;
                }
            }
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择要加入手牌的黑色情报");
        }
    }

    public override void OnCardSelect(int cardId)
    {

    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if (PlayerId == GameManager.Singleton.CurMessagePlayerId || playerId == GameManager.SelfPlayerId)
        {
            bool haveBlack = false;

            foreach (var message in GameManager.Singleton.players[PlayerId].messages)
            {
                if (message.color.Contains(CardColorEnum.Black))
                {
                    haveBlack = true;
                    break;
                }
            }

            if(!haveBlack)
            {
                GameManager.Singleton.gameUI.ShowInfo("该玩家没有黑情报");
                return;
            }

            selectMessageId = 0;
            selectPlayerId = -1;
            GameManager.Singleton.gameUI.ShowPlayerMessageInfo(PlayerId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("只能选择自己或接收者");
        }
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {
        selectPlayerId = playerId;
        selectMessageId = cardId;
    }

    public override void OnMessageInfoClose()
    {
        selectMessageId = 0;
        selectPlayerId = -1;
    }

    public override void Cancel()
    {
        selectMessageId = 0;
        selectPlayerId = -1;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
        ProtoHelper.SendEndReceive(GameManager.Singleton.seqId);
    }

    public override void OnUse()
    {
        selectMessageId = 0;
        selectPlayerId = -1;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
    }

    public static void OnReceiveUse(int playerId, int cardId, int targetPlayerId)
    {
        string cardStr = "";
        CardFS messageCard = new CardFS(null);

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }

        foreach (var message in GameManager.Singleton.players[targetPlayerId].messages)
        {
            if (message.id == cardId)
            {
                cardStr = message.GetCardInfo();
                messageCard = message;
                break;
            }
        }

        GameManager.Singleton.players[targetPlayerId].RemoveMessage(cardId);
        GameManager.Singleton.gameUI.Players[targetPlayerId].RefreshMessage();


        GameManager.Singleton.players[playerId].cardCount += 1;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        if(GameManager.SelfPlayerId != playerId)
        {
            GameManager.Singleton.gameUI.ShowAddMessage(playerId, messageCard, false, targetPlayerId);
        }
        if (playerId == GameManager.SelfPlayerId)
        {
            GameManager.Singleton.cardsHand.Add(cardId, messageCard);
            GameManager.Singleton.gameUI.DrawCards(new List<CardFS>() { messageCard });
        }

        string s = string.Format("{0}使用了技能怜悯,将{1}情报区的{2}置入手牌", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetPlayerId].name, cardStr);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

}
