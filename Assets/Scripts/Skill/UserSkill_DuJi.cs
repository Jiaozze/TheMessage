using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 白昆山【毒计】A：争夺阶段，你可以翻开此角色牌，然后指定两名其他角色，令他们相互抽取对方的一张手牌并展示之，你将展示的牌加入你的手牌。
// 白昆山【毒计】B：若展示的是黑色牌，你可以改为令抽取者选择一项。
// 白昆山【毒计】C：
// ♦ 将其置入自己的情报区
// ♦ 将其置入对方的情报区
public class UserSkill_DuJi : SkillBase
{
    public override string name { get { return "毒计"; } }
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

    public override string Des => "毒计:争夺阶段，你可以翻开此角色牌，然后指定两名其他角色，令他们相互抽取对方的一张手牌并展示之，你将展示的牌加入你的手牌。若展示的是黑色牌，你可以改为令抽取者选择一项:\n♦ 将其置入自己的情报区\n♦ 将其置入对方的情报区\n";
    private static SkillState_DuJi state;
    private List<int> selectPlayerIds = new List<int>();

    public UserSkill_DuJi(int id)
    {
        playerId = id;
    }

    public override void PrepareUse()
    {
        if(!canUse)
        {
            return;
        }
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        state = SkillState_DuJi.isUsingA;
        selectPlayerIds.Clear();
        GameManager.Singleton.gameUI.ShowPhase("是否发动技能毒计，翻开此角色牌，然后指定两名角色，令他们相互抽取对方的一张手牌并展示之，你将展示的牌加入你的手牌");
    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if(state == SkillState_DuJi.isUsingA)
        {
            if (selectPlayerIds.Contains(PlayerId))
            {
                GameManager.Singleton.gameUI.Players[PlayerId].OnSelect(false);
                selectPlayerIds.Remove(PlayerId);
            }
            else
            {
                if (selectPlayerIds.Count > 1)
                {
                    GameManager.Singleton.gameUI.ShowInfo("只能选择2名角色");
                }
                else
                {
                    if(PlayerId != 0)
                    {
                        if(GameManager.Singleton.players[PlayerId].cardCount < 1)
                        {
                            GameManager.Singleton.gameUI.ShowInfo("目标没有手牌");
                            return;
                        }
                        GameManager.Singleton.gameUI.Players[PlayerId].OnSelect(true);
                        selectPlayerIds.Add(PlayerId);
                    }
                    else
                    {
                        GameManager.Singleton.gameUI.ShowInfo("只能选择其他角色");
                    }
                }
            }

        }
    }

    public override void OnCardSelect(int cardId)
    {
        base.OnCardSelect(cardId);
        GameManager.Singleton.gameUI.duJiSelect.OnCardSelect(cardId);
    }
    public override void Use()
    {
        if (state == SkillState_DuJi.isUsingA)
        {
            if (selectPlayerIds.Count == 2)
            {
                ProtoHelper.SendSkill_DuJiA(selectPlayerIds, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("必须选择2名其他角色");
            }
        }
        else if (state == SkillState_DuJi.isUsingB)
        {
            if (DuJiSelect.selectCardId != 0)
            {
                ProtoHelper.SendSkill_DuJiB(true, DuJiSelect.selectCardId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("选择一张牌， 令抽取者选择置入自己的情报区或对方情报区");
            }
        }
        else if (state == SkillState_DuJi.isUsingC)
        {
            ProtoHelper.SendSkill_DuJiC(true, GameManager.Singleton.seqId);
        }
    }

    public override void Cancel()
    {
        if(state == SkillState_DuJi.isUsingA)
        {
            if (selectPlayerIds.Count > 0)
            {
                foreach (var playerId in selectPlayerIds)
                {
                    GameManager.Singleton.gameUI.Players[playerId].OnSelect(false);
                }
                selectPlayerIds.Clear();
            }
        }
        else if(state == SkillState_DuJi.isUsingB)
        {
            GameManager.Singleton.gameUI.duJiSelect.gameObject.SetActive(false);
            if (GameManager.Singleton.IsUsingSkill)
            {
                ProtoHelper.SendSkill_DuJiB(false, DuJiSelect.selectCardId, GameManager.Singleton.seqId);
            }
        }
        else if(state == SkillState_DuJi.isUsingC)
        {
            if(GameManager.Singleton.IsUsingSkill)
            {
                GameManager.Singleton.gameUI.duJiSelect.gameObject.SetActive(false);
                ProtoHelper.SendSkill_DuJiC(false, GameManager.Singleton.seqId);
            }
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        if (selectPlayerIds.Count > 0)
        {
            foreach (var playerId in selectPlayerIds)
            {
                GameManager.Singleton.gameUI.Players[playerId].OnSelect(false);
            }
            selectPlayerIds.Clear();
        }
        GameManager.Singleton.gameUI.duJiSelect.gameObject.SetActive(false);
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUseA(int playerId, int targetId1, CardFS cardFS1, int targetId2, CardFS cardFS2)
    {
        GameManager.Singleton.players[targetId1].cardCount -= 1;
        GameManager.Singleton.gameUI.Players[targetId1].RefreshCardCount();
        GameManager.Singleton.players[targetId2].cardCount -= 1;
        GameManager.Singleton.gameUI.Players[targetId2].RefreshCardCount();
        GameManager.Singleton.players[playerId].cardCount += 2;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
            GameManager.Singleton.cardsHand.Add(cardFS1.id, cardFS1);
            GameManager.Singleton.cardsHand.Add(cardFS2.id, cardFS2);
            GameManager.Singleton.gameUI.DrawCards(new List<CardFS>() { cardFS1, cardFS2 });
        }
        if (targetId1 == GameManager.SelfPlayerId || targetId2 == GameManager.SelfPlayerId)
        {
            int cardId = targetId1 == GameManager.SelfPlayerId ? cardFS1.id : cardFS2.id;
            if(GameManager.Singleton.cardsHand.ContainsKey(cardId))
            {
                GameManager.Singleton.cardsHand.Remove(cardId);
            }
        }
        GameManager.Singleton.gameUI.ShowCardsMove(targetId1, playerId, new List<CardFS>() { cardFS1 });
        GameManager.Singleton.gameUI.ShowCardsMove(targetId2, playerId, new List<CardFS>() { cardFS2 }, false, 0.8f);
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        GameManager.Singleton.gameUI.Players[targetId1].RefreshCardCount();
        GameManager.Singleton.gameUI.Players[targetId2].RefreshCardCount();

        string s = string.Format("{0}使用了技能毒计，指定了{1}和{2}，令他们相互抽取对方的一张手牌，从{3}抽取了{4}，从{5}抽取了{6}，并加入了手牌", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId1].name, GameManager.Singleton.players[targetId2].name, GameManager.Singleton.players[targetId1].name, cardFS1.GetCardInfo(), GameManager.Singleton.players[targetId2].name, cardFS2.GetCardInfo());
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(bool enable, int playerId, CardFS card, int targetId, int waitingPlayer, int waitSeconds, uint seq)
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
            GameManager.Singleton.OnWait(waitingPlayer, waitSeconds);
        }

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }

        if(waitingPlayer == GameManager.SelfPlayerId)
        {
            foreach(var skill in GameManager.Singleton.players[playerId].role.skills)
            {
                if(skill is UserSkill_DuJi)
                {
                    GameManager.Singleton.IsUsingSkill = true;
                    UserSkill_DuJi duJi = skill as UserSkill_DuJi;
                    UserSkill_DuJi.state = SkillState_DuJi.isUsingC;
                    GameManager.Singleton.selectSkill = duJi;
                    GameManager.Singleton.gameUI.duJiSelect.ShowUseC(targetId, card);
                    break;
                }
            }
        }

        string s = string.Format("{0}使用了技能毒计, 令{1}选择将卡牌{2}置入自己或者{3}的情报区", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[waitingPlayer].name, card.GetCardInfo(), GameManager.Singleton.players[targetId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseC(int playerId, CardFS card, int targetId, int waitingPlayerId)
    {
        GameManager.Singleton.players[playerId].cardCount -= 1;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        GameManager.Singleton.players[targetId].AddMessage(card);
        GameManager.Singleton.gameUI.Players[targetId].RefreshMessage();
        GameManager.Singleton.gameUI.ShowAddMessage(targetId, card, false, playerId);


        if (playerId == GameManager.SelfPlayerId)
        {
            if(GameManager.Singleton.cardsHand.ContainsKey(card.id))
            {
                GameManager.Singleton.cardsHand.Remove(card.id);
                var cardUI = GameManager.Singleton.gameUI.Cards[card.id];
                GameManager.Singleton.gameUI.Cards.Remove(card.id);
                GameObject.Destroy(cardUI.gameObject);
            }
        }
        else if(waitingPlayerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }

        string s = string.Format("{0}选择将抽取的牌{1}置入{2}的情报区", GameManager.Singleton.players[waitingPlayerId].name, card.GetCardInfo(), GameManager.Singleton.players[targetId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }

    public static void OnReceiveWaitingUseB(int playerId, List<int> targets, List<int> cards, int waitingSecond, uint seq)
    {
        GameManager.Singleton.seqId = seq;

        if (waitingSecond > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitingSecond);
        }

        if (playerId == GameManager.SelfPlayerId)
        {
            foreach(var skill in GameManager.Singleton.players[playerId].role.skills)
            {
                if(skill is UserSkill_DuJi)
                {
                    UserSkill_DuJi.state = SkillState_DuJi.isUsingB;
                    GameManager.Singleton.selectSkill = skill;
                    GameManager.Singleton.IsUsingSkill = true;
                    GameManager.Singleton.gameUI.duJiSelect.Show(cards, targets);
                    break;
                }
            }
        }
    }
}

public enum SkillState_DuJi
{
    isUsingA = 0,
    isUsingB = 1,
    isUsingC = 2,
}
