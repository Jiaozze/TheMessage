using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 黄济仁【对症下药】A：争夺阶段，你可以翻开此角色牌，然后摸三张牌。
// 黄济仁【对症下药】B：并且你可以展示两张含有相同颜色的手牌。
// 黄济仁【对症下药】C：然后从一名角色的情报区，弃置一张对应颜色情报。
public class UserSkill_DuiZhengXiaYao : SkillBase
{
    public override string name { get { return "对症下药"; } }
    public override bool canUse { get {
            if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
            {
                return false;
            }

            if (!GameUtils.IsFightPhase())
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

    public override string Des => "对症下药:争夺阶段，你可以翻开此角色牌，然后摸三张牌。并且你可以展示两张含有相同颜色的手牌。然后从一名角色的情报区，弃置一张对应颜色情报。\n";
    private int index = 0;
    private static SkillState_DuiZhengXiaYao state;
    private List<int> selectCardIds = new List<int>();
    private List<CardColorEnum> colors = new List<CardColorEnum>();
    private int selectPlayerId = -1;
    private int selectMessageId = 0;
    public UserSkill_DuiZhengXiaYao(int id)
    {
        playerId = id;
    }

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        state = SkillState_DuiZhengXiaYao.isUsingA;
        GameManager.Singleton.gameUI.ShowPhase("发动技能广发报，翻开此角色牌，然后摸三张牌");
    }
    public override void Use()
    {
        if(state == SkillState_DuiZhengXiaYao.isUsingA)
        {
            ProtoHelper.SendSkill_DuiZhengXiaYaoA(GameManager.Singleton.seqId);
        }
        else if(state == SkillState_DuiZhengXiaYao.isUsingB)
        {
            if(selectCardIds.Count == 2)
            {
                ProtoHelper.SendSkill_DuiZhengXiaYaoB(true, selectCardIds, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择要展示的两张含有相同颜色的手牌");
            }
        }
        else if(state == SkillState_DuiZhengXiaYao.isUsingC)
        {
            if(selectMessageId > 0 && selectPlayerId > -1)
            {
                CardFS message = GameManager.Singleton.players[selectPlayerId].GetMessage(selectMessageId);
                foreach(var color in message.color)
                {
                    if(colors.Contains(color))
                    {
                        ProtoHelper.SendSkill_DuiZhengXiaYaoC(selectPlayerId, selectMessageId, GameManager.Singleton.seqId);
                        return;
                    }
                }
                string colorStr = LanguageUtils.GetColorsName(colors);
                GameManager.Singleton.gameUI.ShowPhase("需要弃置一张对应颜色情报:" + colorStr);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择要弃置的情报");
            }
        }
    }

    public override void OnCardSelect(int cardId)
    {
        if (state == SkillState_DuiZhengXiaYao.isUsingB)
        {
            if (selectCardIds.Contains(cardId))
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
                selectCardIds.Remove(cardId);
            }
            else
            {
                if (selectCardIds.Count > 1)
                {
                    GameManager.Singleton.gameUI.ShowInfo("最多选择2张");
                }
                else if (selectCardIds.Count > 0)
                {
                    foreach (var color in GameManager.Singleton.cardsHand[cardId].color)
                    {
                        if (GameManager.Singleton.cardsHand[selectCardIds[0]].color.Contains(color))
                        {
                            GameManager.Singleton.gameUI.Cards[cardId].OnSelect(true);
                            selectCardIds.Add(cardId);
                            return;
                        }
                    }

                    GameManager.Singleton.gameUI.ShowInfo("需要选择含有相同颜色的2张手牌");
                }
                else
                {
                    GameManager.Singleton.gameUI.Cards[cardId].OnSelect(true);
                    selectCardIds.Add(cardId);
                }
            }

        }
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {
        if(state == SkillState_DuiZhengXiaYao.isUsingC)
        {
            base.OnMessageSelect(playerId, cardId);
            selectPlayerId = playerId;
            selectMessageId = cardId;
        }
    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if(state == SkillState_DuiZhengXiaYao.isUsingC)
        {
            GameManager.Singleton.gameUI.ShowPlayerMessageInfo(PlayerId);
        }
    }

    public override void Cancel()
    {
        if(state == SkillState_DuiZhengXiaYao.isUsingA)
        {
        }
        else if(state == SkillState_DuiZhengXiaYao.isUsingB)
        {
            if(GameManager.Singleton.IsUsingSkill)
            {
                ProtoHelper.SendSkill_DuiZhengXiaYaoB(false, selectCardIds, GameManager.Singleton.seqId);
            }
            if (selectCardIds.Count > 0)
            {
                foreach (var cardId in selectCardIds)
                {
                    GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
                }
                selectCardIds.Clear();
            }
        }
        else if(state == SkillState_DuiZhengXiaYao.isUsingC)
        {
            if(GameManager.Singleton.IsUsingSkill)
            {
                string colorStr = LanguageUtils.GetColorsName(colors);
                GameManager.Singleton.gameUI.ShowPhase("需要从一名角色的情报区，弃置一张对应颜色情报:" + colorStr);
                return;
            }
            else
            {
                selectPlayerId = -1;
                selectMessageId = 0;
            }
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        selectPlayerId = -1;
        selectMessageId = 0;
        if (selectCardIds.Count > 0)
        {
            foreach (var cardId in selectCardIds)
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
            }
            selectCardIds.Clear();
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUseA(int playerId, int waitingSecond, uint seq)
    {
        GameManager.Singleton.seqId = seq;

        if (waitingSecond > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitingSecond);
        }

        if(playerId == GameManager.SelfPlayerId)
        {
            UserSkill_DuiZhengXiaYao.state = SkillState_DuiZhengXiaYao.isUsingB;
            UserSkill_DuiZhengXiaYao duiZhengXiaYao = GameManager.Singleton.selectSkill as UserSkill_DuiZhengXiaYao;
            duiZhengXiaYao.selectCardIds.Clear();
            GameManager.Singleton.gameUI.ShowPhase("可以展示两张含有相同颜色的手牌");
        }

        string s = string.Format("{0}使用了技能对症下药", GameManager.Singleton.players[playerId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(bool enable, int playerId, List<CardFS> cards, int waitSeconds, uint seq)
    {
        if (!enable)
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

        GameManager.Singleton.seqId = seq;

        if (waitSeconds > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitSeconds);
        }

        string cardStr = "";
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                UserSkill_DuiZhengXiaYao skill_GuangFaBao = GameManager.Singleton.selectSkill as UserSkill_DuiZhengXiaYao;
                if (skill_GuangFaBao.selectCardIds.Count > 0)
                {
                    foreach (var cardId in skill_GuangFaBao.selectCardIds)
                    {
                        GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
                    }
                    skill_GuangFaBao.selectCardIds.Clear();
                }
                skill_GuangFaBao.colors.Clear();
                foreach(var color in cards[0].color)
                {
                    if(cards[1].color.Contains(color))
                    {
                        skill_GuangFaBao.colors.Add(color);
                    }
                }
                UserSkill_DuiZhengXiaYao.state = SkillState_DuiZhengXiaYao.isUsingC;
                string colorStr = LanguageUtils.GetColorsName(skill_GuangFaBao.colors);
                GameManager.Singleton.gameUI.ShowPhase("从一名角色的情报区，弃置一张对应颜色情报:" + colorStr);
            }
        }
        else
        {
            GameManager.Singleton.gameUI.ShowHandCard(playerId, cards, "对症下药展示手牌");
        }
        foreach(var card in cards)
        {
            cardStr += card.GetCardInfo();
        }
        string s = string.Format("{0}使用了技能对症下药, 展示了手牌{1}", GameManager.Singleton.players[playerId].name, cardStr);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }

    public static void OnReceiveUseC(int playerId, int targetPlayerId, int messageId)
    {
        CardFS message = GameManager.Singleton.players[targetPlayerId].GetMessage(messageId);
        GameManager.Singleton.players[targetPlayerId].RemoveMessage(messageId);
        GameManager.Singleton.gameUI.Players[targetPlayerId].RefreshMessage();
        GameManager.Singleton.gameUI.OnPlayerMessageRemove(targetPlayerId, new List<CardFS>() { message });

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
            GameManager.Singleton.gameUI.HidePlayerMessageInfo();
        }
        else
        {
            GameManager.Singleton.gameUI.HidePlayerMessageInfo();
        }
        string s = string.Format("{0}使用了技能对症下药,弃置了{1}情报区的{2}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetPlayerId].name, message.GetCardInfo());
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }
}

public enum SkillState_DuiZhengXiaYao
{
    isUsingA = 0,
    isUsingB = 1,
    isUsingC = 2,
}
