using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 小九【广发报】A：争夺阶段，你可以翻开此角色牌，然后摸三张牌。
// 小九【广发报】B：并且你可以将你的任意张手牌置入任意名角色的情报区。你不能通过此技能让任何角色收集三张或更多的同色情报。
public class UserSkill_GuangFaBao : SkillBase
{
    public override string name { get { return "广发报"; } }
    public override bool canUse { get {
            if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
            {
                return false;
            }

            if (GameManager.Singleton.curPhase != PhaseEnum.Fight_Phase)
            {
                return false;
            }
            if(!GameManager.Singleton.players[playerId].role.isBack)
            {
                return false;
            }
            return true;
        }
    }

    public override string Des => "广发报:争夺阶段，你可以翻开此角色牌，然后摸三张牌。并且你可以将你的任意张手牌置入任意名角色的情报区。你不能通过此技能让任何角色收集三张或更多的同色情报。\n";
    private int index = 0;
    private bool isUseB = false;
    private int selectCardId = -1;
    private int selectPlayerId = -1;
    public UserSkill_GuangFaBao(int id)
    {
        playerId = id;
    }

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("发动技能广发报，翻开此角色牌，然后摸三张牌");
    }
    public override void Use()
    {
        if(!isUseB)
        {
            ProtoHelper.SendSkill_GuangFaBaoA(GameManager.Singleton.seqId);
            return;
        }

        if(selectPlayerId != -1 &&  selectCardId != 0)
        {
            var colors = GameManager.Singleton.cardsHand[selectCardId].color;
            foreach(var color in colors)
            {
                if(GameManager.Singleton.players[selectPlayerId].GetMessageCount(color) >= 2)
                {
                    GameManager.Singleton.gameUI.ShowInfo("你不能通过此技能让任何角色收集三张或更多的同色情报");
                    return;
                }
            }
            ProtoHelper.SendSkill_GuangFaBaoB(true, selectCardId, selectPlayerId, GameManager.Singleton.seqId);
        }
        else if( selectCardId == 0)
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择要置入情报区的手牌");
        }
        else if (selectPlayerId == -1)
        {
            GameManager.Singleton.gameUI.ShowInfo("选择要置入情报区的角色");
        }

    }

    public override void OnCardSelect(int cardId)
    {
        if (!isUseB)
        {
            return;
        }

        if (selectCardId > 0)
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
        }

        if (selectCardId == cardId)
        {
            selectCardId = 0;
        }
        else
        {
            selectCardId = cardId;
            GameManager.Singleton.gameUI.Cards[cardId].OnSelect(true);
        }

    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if(!isUseB)
        {
            return;
        }

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

    public override void Cancel()
    {
        if (selectCardId > 0)
        {
            if (GameManager.Singleton.gameUI.Cards.ContainsKey(selectCardId))
            {
                GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
            }

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
        if (isUseB)
        {
            ProtoHelper.SendSkill_GuangFaBaoB(false, 0, 0, GameManager.Singleton.seqId);
        }
    }

    public override void OnUse()
    {
        if (selectCardId > 0)
        {
            if (GameManager.Singleton.gameUI.Cards.ContainsKey(selectCardId))
            {
                GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
            }

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

    public static void OnReceiveUseA(int playerId)
    {
        string s = string.Format("{0}使用了技能广发报", GameManager.Singleton.players[playerId].name);
    }

    public static void OnReceiveWaitUse(int playerId, int waitSeconds, uint seq)
    {
        GameManager.Singleton.seqId = seq;

        if (waitSeconds > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitSeconds);
        }

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                UserSkill_GuangFaBao skill_GuangFaBao = GameManager.Singleton.selectSkill as UserSkill_GuangFaBao;
                skill_GuangFaBao.index += 1;
                skill_GuangFaBao.isUseB = true;
                GameManager.Singleton.gameUI.ShowPhase("可以将你的任意张手牌置入任意名角色的情报区(第" + skill_GuangFaBao.index + "张)");
            }
        }
    }

    public static void OnReceiveUseB(int playerId, bool enable, int targetPlayerId, CardFS card)
    {
        if(!enable)
        {
            if (playerId == GameManager.SelfPlayerId)
            {
                if (GameManager.Singleton.selectSkill != null)
                {
                    GameManager.Singleton.selectSkill.OnUse();
                }
            }
            return;
        }

        GameManager.Singleton.players[playerId].cardCount -= 1;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();

        GameManager.Singleton.players[targetPlayerId].AddMessage(card);
        GameManager.Singleton.gameUI.Players[targetPlayerId].RefreshMessage();
        GameManager.Singleton.gameUI.ShowCardsMove(playerId, targetPlayerId, new List<CardFS>() { card });

        if (playerId == GameManager.SelfPlayerId)
        {
            GameManager.Singleton.cardsHand.Remove(card.id);
            UserSkill_GuangFaBao skill_GuangFaBao = GameManager.Singleton.selectSkill as UserSkill_GuangFaBao;
            skill_GuangFaBao.selectCardId = 0;
        }
        string s = string.Format("{0}使用了技能广发报,将{1}置入{2}情报区", GameManager.Singleton.players[playerId].name, card.GetCardInfo(), GameManager.Singleton.players[targetPlayerId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }
}
