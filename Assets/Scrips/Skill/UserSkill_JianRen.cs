using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 吴志国【坚韧】A：你接收黑色情报后，可以展示牌堆顶的一张牌，若是黑色牌，则将展示的牌加入你的手牌。
// 吴志国【坚韧】B：并从一名角色的情报区弃置一张黑色情报。
public class UserSkill_JianRen : SkillBase
{
    public override string name { get { return "坚韧"; } }
    public override bool canUse
    {
        get
        {
            return false;
        }
    }

    public override string Des => "坚韧:你接收黑色情报后，可以展示牌堆顶的一张牌，若是黑色牌，则将展示的牌加入你的手牌。并从一名角色的情报区弃置一张黑色情报。\n";

    private int selectMessageId = 0;
    private int selectPlayerId = -1;

    private static bool isUsingB;
    public UserSkill_JianRen(int id)
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

        isUsingB = false;
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("是否发动技能坚韧，展示牌堆顶的一张牌，若是黑色牌，则将展示的牌加入你的手牌。并从一名角色的情报区弃置一张黑色情报。");
    }
    public override void Use()
    {
        if(!isUsingB)
        {
            ProtoHelper.SendSkill_JianRenA(GameManager.Singleton.seqId);
        }

        if(selectMessageId != 0 && selectPlayerId != -1)
        {
            foreach(var message in GameManager.Singleton.players[selectPlayerId].messages)
            {
                if(message.id == selectMessageId)
                {
                    if(message.color.Contains(CardColorEnum.Black))
                    {
                        ProtoHelper.SendSkill_JianRenB(selectPlayerId, selectMessageId, GameManager.Singleton.seqId);
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
            GameManager.Singleton.gameUI.ShowInfo("请选择要弃置的黑色情报");
        }
    }

    public override void OnCardSelect(int cardId)
    {

    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if(!isUsingB)
        {
            return;
        }
        bool haveBlack = false;

        foreach (var message in GameManager.Singleton.players[PlayerId].messages)
        {
            if (message.color.Contains(CardColorEnum.Black))
            {
                haveBlack = true;
                break;
            }
        }

        if (!haveBlack)
        {
            GameManager.Singleton.gameUI.ShowInfo("该玩家没有黑情报");
            return;
        }

        selectMessageId = 0;
        selectPlayerId = -1;
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(PlayerId);
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {
        if (!isUsingB)
        {
            return;
        }

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
        if(isUsingB)
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一名角色弃置其一张黑色情报");
            return;
        }
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

    public static void OnReceiveUseA(int playerId, CardFS card, int waitingSecond, uint seq)
    {
        GameManager.Singleton.seqId = seq;
        GameManager.Singleton.OnWait(playerId, waitingSecond);
        string draw = "";
        if (playerId != GameManager.SelfPlayerId)
        {
            GameManager.Singleton.gameUI.ShowHandCard(playerId, new List<CardFS>() { card }, "坚韧展示的牌");
        }
        GameManager.Singleton.gameUI.ShowTopCard(card);

        if (card.color.Contains(CardColorEnum.Black))
        {
            GameManager.Singleton.players[playerId].cardCount += 1;
            GameManager.Singleton.gameUI.Players[playerId].OnDrawCard(1);

            if (playerId == GameManager.SelfPlayerId)
            {
                GameManager.Singleton.cardsHand.Add(card.id, card);
                GameManager.Singleton.gameUI.DrawCards(new List<CardFS>() { card });
                UserSkill_JianRen.isUsingB = true;
                GameManager.Singleton.gameUI.ShowPhase("请选择一名角色弃置其一张黑色情报");
            }
            draw = ", 并加入手牌";
        }
        else
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
        string s = string.Format("{0}使用了技能坚韧,展示了牌堆顶的{1}{2}", GameManager.Singleton.players[playerId].name, card.GetCardInfo(), draw);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, int cardId, int targetPlayerId)
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
        GameManager.Singleton.gameUI.OnPlayerMessageRemove(targetPlayerId, new List<CardFS>() { messageCard });

        string s = string.Format("{0}使用了技能坚韧,将{1}情报区的{2}弃置", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetPlayerId].name, cardStr);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

}
