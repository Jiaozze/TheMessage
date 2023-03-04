using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 阿芙罗拉【妙手】A：争夺阶段，你可以翻开此角色牌，然后弃置待接收情报，并查看一名角色的手牌和情报区。
// 阿芙罗拉【妙手】B：从中选择一张牌作为待收情报，面朝上移至一名角色的面前。
public class UserSkill_MiaoShou : SkillBase
{
    public override string name { get { return "妙手"; } }
    public override bool canUse
    {
        get
        {
            if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
            {
                return false;
            }

            if (!GameUtils.IsFightPhase())
            {
                return false;
            }
            if (!GameManager.Singleton.players[playerId].role.isBack)
            {
                return false;
            }
            return true;
        }
    }

    public override string Des => "妙手:争夺阶段，你可以翻开此角色牌，然后弃置待接收情报，并查看一名角色的手牌和情报区。从中选择一张牌作为待收情报，面朝上移至一名角色的面前。\n";
    private int index = 0;
    private static bool isUseB = false;
    private int selectCardId = 0;
    private int selectPlayerId = -1;
    private int selectMessageId = 0;

    public UserSkill_MiaoShou(int id)
    {
        playerId = id;
    }

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        isUseB = false;
        GameManager.Singleton.gameUI.ShowPhase("发动技能妙手，弃置待接收情报，并查看一名角色的手牌和情报区。从中选择一张牌作为待收情报，面朝上移至一名角色的面前 ");
    }
    public override void Use()
    {
        if (!isUseB)
        {
            if (selectPlayerId != -1)
            {
                ProtoHelper.SendSkill_MiaoShouA(selectPlayerId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择查看一名角色的手牌和情报区");
            }
            return;
        }

        if (selectPlayerId != -1 && (selectCardId != 0 || selectMessageId != 0))
        {
            ProtoHelper.SendSkill_MiaoShouB(selectPlayerId, selectCardId, selectMessageId, GameManager.Singleton.seqId);
        }
        else if (selectCardId == 0 && selectMessageId == 0)
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一张牌作为待收情报");
        }
        else if (selectPlayerId == -1)
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择要将情报移至的角色");
        }

    }
    public override void OnMessageSelect(int playerId, int cardId)
    {
        if (isUseB)
        {
            selectMessageId = cardId;
            selectCardId = 0;
        }
    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        if (selectPlayerId == PlayerId)
        {
            selectPlayerId = -1;
        }
        else if (PlayerId > -1)
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }

    public void OnClickOthersCard(int playerId, int cardId)
    {
        if (isUseB)
        {
            selectCardId = cardId;
            selectMessageId = 0;
        }
    }

    public override void Cancel()
    {
        if (isUseB)
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一张牌作为待收情报，面朝上移至一名角色的面前");
            return;
        }

        if (selectCardId > 0)
        {
            selectCardId = 0;
        }
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        if (selectCardId > 0)
        {
            selectCardId = 0;
        }
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;
        GameManager.Singleton.gameUI.HideMiaoShouSelect();
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUseA(int playerId, int targetId, List<CardFS> cards, CardFS message, int waitingSecond, uint seq)
    {
        GameManager.Singleton.seqId = seq;

        if (waitingSecond > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitingSecond);
        }
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                UserSkill_MiaoShou skill_MiaoShou = GameManager.Singleton.selectSkill as UserSkill_MiaoShou;
                if (skill_MiaoShou.selectPlayerId > -1)
                {
                    GameManager.Singleton.gameUI.Players[skill_MiaoShou.selectPlayerId].OnSelect(false);
                }
                skill_MiaoShou.selectPlayerId = -1;
            }

            UserSkill_MiaoShou.isUseB = true;
            GameManager.Singleton.gameUI.ShowMiaoShouSelect(targetId, cards);
            GameManager.Singleton.gameUI.ShowPhase("请选择一张牌作为待收情报，面朝上移至一名角色的面前");
        }
        GameManager.Singleton.gameUI.ShowMessagingDiaoBao(message);

        string s = string.Format("{0}使用了技能妙手,弃置待接收情报,并查看了{1}的手牌和情报区", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, int fromPlayerId, int targetPlayerId, CardFS card, int messageId)
    {
        string isHand = "";
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
        CardFS messageNew = card;
        if(messageNew != null)
        {
            isHand = "手牌";
            GameManager.Singleton.players[fromPlayerId].cardCount -= 1;
            GameManager.Singleton.gameUI.Players[fromPlayerId].RefreshCardCount();
            if(fromPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.cardsHand.ContainsKey(card.id))
            {
                GameManager.Singleton.cardsHand.Remove(card.id);
            }
        }
        else
        {
            isHand = "情报";
            messageNew = GameManager.Singleton.players[fromPlayerId].GetMessage(messageId);
            GameManager.Singleton.players[fromPlayerId].RemoveMessage(messageId);
            GameManager.Singleton.gameUI.Players[fromPlayerId].RefreshMessage();
        }

        GameManager.Singleton.gameUI.ShowMessagingCard(messageNew, targetPlayerId, false);
        GameManager.Singleton.gameUI.ShowCardsMove(fromPlayerId, targetPlayerId, new List<CardFS>() { messageNew }, true);

        string s = string.Format("{0}使用了技能妙手,将{1}的{2}{3}作为待接收情报，并移至{4}的面前", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[fromPlayerId].name,isHand, messageNew.GetCardInfo(), GameManager.Singleton.players[targetPlayerId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
