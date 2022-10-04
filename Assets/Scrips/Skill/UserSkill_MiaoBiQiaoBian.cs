using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 连鸢【妙笔巧辩】A：争夺阶段，你可以翻开此角色牌，然后从所有角色的情报区选择合计至多两张不含有相同颜色的情报，将其加入你的手牌。
public class UserSkill_MiaoBiQiaoBian : SkillBase
{
    public override string name { get { return "妙笔巧辩"; } }
    public override bool canUse { get {
            if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
            {
                return false;
            }
            if (GameManager.Singleton.curPhase != PhaseEnum.Fight_Phase)
            {
                return false;
            }
            if (!GameManager.Singleton.players[GameManager.SelfPlayerId].role.isBack)
            {
                return false;
            }
            return true;
        } }
    private bool isUsingB = false;
    private int selectMessageId = 0;
    private int selectPlayerId = -1;
    private List<CardColorEnum> colors;

    public override string Des => "妙笔巧辩:争夺阶段，你可以翻开此角色牌，然后从所有角色的情报区选择合计至多两张不含有相同颜色的情报，将其加入你的手牌。\n";

    public UserSkill_MiaoBiQiaoBian(int id)
    {
        playerId = id;
    }
    public override bool CheckTriger()
    {
        return false;
    }

    public override void PrepareUse()
    {
        base.PrepareUse();
        if (canUse && GameManager.Singleton.selectSkill == this)
        {
            GameManager.Singleton.selectSkill.Cancel();
            return;
        }

        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("发动技能妙笔巧辩，从所有角色的情报区选择合计至多两张不含有相同颜色的情报，将其加入你的手牌(选择第一张)");
    }
    public override void Use()
    {
        if(isUsingB)
        {
            if (selectPlayerId > -1 && selectMessageId > 0)
            {
                CardFS message = GameManager.Singleton.players[selectPlayerId].GetMessage(selectMessageId);
                foreach(var color in colors)
                {
                    if(message.color.Contains(color))
                    {
                        GameManager.Singleton.gameUI.ShowInfo("需要选择不含有相同颜色的情报");
                        return;
                    }
                }
                ProtoHelper.SendSkill_MiaoBiQiaoBianB(true, selectPlayerId, selectMessageId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择要加入手牌的情报");
            }

        }
        else
        {
            if(selectPlayerId > -1 && selectMessageId > 0)
            {
                ProtoHelper.SendSkill_MiaoBiQiaoBianA(selectPlayerId, selectMessageId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择要加入手牌的情报");
            }
        }
    }
    public override void OnPlayerSelect(int PlayerId)
    {
        selectMessageId = 0;
        selectPlayerId = -1;
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(PlayerId);
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
        if(isUsingB)
        {
            isUsingB = false;
            colors = null;
            selectMessageId = 0;
            selectPlayerId = -1;
            GameManager.Singleton.IsUsingSkill = false;
            GameManager.Singleton.selectSkill = null;
            GameManager.Singleton.gameUI.ShowPhase();
            ProtoHelper.SendSkill_MiaoBiQiaoBianB(false, 0, 0, GameManager.Singleton.seqId);
        }
        else
        {
            selectMessageId = 0;
            selectPlayerId = -1;
            GameManager.Singleton.IsUsingSkill = false;
            GameManager.Singleton.selectSkill = null;
            GameManager.Singleton.gameUI.ShowPhase();
        }
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
    }

    public override void OnUse()
    {
        isUsingB = false;
        colors = null;
        selectMessageId = 0;
        selectPlayerId = -1;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
    }

    public static void OnReceiveUseA(int playerId, int target, int cardId, int waitSeconds, uint seq)
    {
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();

        GameManager.Singleton.seqId = seq;

        if (waitSeconds > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitSeconds);
        }

        CardFS message = GameManager.Singleton.players[target].GetMessage(cardId);
        GameManager.Singleton.players[target].RemoveMessage(cardId);
        GameManager.Singleton.gameUI.Players[target].RefreshMessage();
        GameManager.Singleton.players[playerId].cardCount += 1;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        if(target != GameManager.SelfPlayerId)
        {
            GameManager.Singleton.gameUI.ShowCardsMove(target, playerId, new List<CardFS>() { message });
        }

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                UserSkill_MiaoBiQiaoBian userSkill_MiaoBiQiaoBian = GameManager.Singleton.selectSkill as UserSkill_MiaoBiQiaoBian;
                //GameManager.Singleton.selectSkill.OnUse();
                userSkill_MiaoBiQiaoBian.isUsingB = true;
                userSkill_MiaoBiQiaoBian.colors = message.color;
                GameManager.Singleton.gameUI.ShowPhase("发动技能妙笔巧辩，从所有角色的情报区选择合计至多两张不含有相同颜色的情报，将其加入你的手牌(选择第二张)");
            }
            GameManager.Singleton.cardsHand[cardId] = message;
            GameManager.Singleton.gameUI.DrawCards(new List<CardFS>() { message });
        }

        string s = string.Format("{0}使用了技能妙笔巧辩，将{1}的情报{2}加入手牌", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[target].name, message.GetCardInfo());
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, int cardId, int target)
    {
        CardFS message = GameManager.Singleton.players[target].GetMessage(cardId);
        GameManager.Singleton.players[target].RemoveMessage(cardId);
        GameManager.Singleton.gameUI.Players[target].RefreshMessage();
        GameManager.Singleton.players[playerId].cardCount += 1;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        if (target != GameManager.SelfPlayerId)
        {
            GameManager.Singleton.gameUI.ShowCardsMove(target, playerId, new List<CardFS>() { message });
        }

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
            GameManager.Singleton.cardsHand[cardId] = message;
            GameManager.Singleton.gameUI.DrawCards(new List<CardFS>() { message });
        }

        string s = string.Format("{0}使用了技能妙笔巧辩，将{1}的情报{2}加入手牌", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[target].name, message.GetCardInfo());
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }
}
