using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 王田香【禁闭】A：出牌阶段限一次，你可以指定一名角色。
// 王田香【禁闭】B：除非其交给你两张手牌，否则其本回合不能使用手牌，且所有角色技能无效。
public class UserSkill_JinBi : SkillBase
{
    public UserSkill_JinBi(int id)
    {
        playerId = id;
    }


    private int selectPlayerId;

    private bool beUsed = false;

    private List<int> selectCardIds = new List<int>();
    private int needGiveCount = 2;

    private int usedCount = 0;
    public override string name => "禁闭";

    public override bool canUse
    {
        get
        {
            return usedCount < 1 && playerId == GameManager.SelfPlayerId && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.curPhase == PhaseEnum.Main_Phase;
        }
    }

    public override string Des => "禁闭：出牌阶段限一次，你可以指定一名角色，除非其交给你两张手牌，否则其本回合不能使用手牌，且所有角色技能无效。\n";

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("指定一名角色作为禁闭的目标");
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
        else if (PlayerId > 0)
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }

    public override void OnCardSelect(int cardId)
    {
        if (beUsed)
        {
            if (selectCardIds.Contains(cardId))
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
                selectCardIds.Remove(cardId);
            }
            else
            {
                if (selectCardIds.Count < needGiveCount)
                {
                    GameManager.Singleton.gameUI.Cards[cardId].OnSelect(true);
                    selectCardIds.Add(cardId);
                }
                else
                {
                    GameManager.Singleton.gameUI.ShowInfo("只需给出" + needGiveCount + "张手牌");
                }
            }
        }
    }

    public override void Use()
    {
        if (beUsed)
        {
            if (selectCardIds.Count == needGiveCount)
            {
                ProtoHelper.SendSkill_JinBiB(selectCardIds, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("需要给出" + needGiveCount + "张手牌");
            }
        }
        else
        {
            if (selectPlayerId > 0)
            {
                ProtoHelper.SendSkill_JinBiA(selectPlayerId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowPhase("请指定正确角色作为禁闭的目标");
            }
        }
    }
    public override void Cancel()
    {
        base.Cancel();
        if (beUsed)
        {
            ProtoHelper.SendSkill_JinBiB(new List<int>(), GameManager.Singleton.seqId);
            beUsed = false;
        }
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
            selectPlayerId = -1;
        }
        if(selectCardIds.Count > 0)
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

    public override void OnUse()
    {
        if(!beUsed)
        {
            usedCount += 1;
        }
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
            selectPlayerId = -1;
        }
        beUsed = false;
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

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
        usedCount = 0;
    }

    public static void OnReceiveUseA(int playerId, int targetId, int waitSeconds, uint seq)
    {
        GameManager.Singleton.seqId = seq;
        GameManager.Singleton.OnWait(targetId, waitSeconds);

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
        if (targetId == GameManager.SelfPlayerId)
        {
            foreach (var skill in GameManager.Singleton.players[playerId].role.skills)
            {
                if (skill is UserSkill_JinBi)
                {
                    UserSkill_JinBi skill_JinBi = skill as UserSkill_JinBi;
                    skill_JinBi.beUsed = true;
                    GameManager.Singleton.selectSkill = skill;
                    GameManager.Singleton.IsUsingSkill = true;
                    GameManager.Singleton.gameUI.ShowPhase("交给" + GameManager.Singleton.players[playerId].name + "两张手牌，否则本回合不能使用手牌，且所有角色技能无效。");
                    break;
                }
            }
        }
        string s = string.Format("{0}对{1}发动了技能禁闭", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }

    public static void OnReceiveUseB(int playerId, int targetId, List<CardFS> cards, int unknowCount)
    {

        int cardCount = cards.Count > 0 ? cards.Count : unknowCount;
        if (cardCount > 0)
        {
            GameManager.Singleton.players[targetId].cardCount -= cardCount;
            GameManager.Singleton.players[playerId].cardCount += cardCount;
            GameManager.Singleton.gameUI.Players[targetId].RefreshCardCount();
            GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();

            if (targetId == GameManager.SelfPlayerId)
            {
                if (GameManager.Singleton.selectSkill != null)
                {
                    GameManager.Singleton.selectSkill.OnUse();
                }
                foreach (var card in cards)
                {
                    GameManager.Singleton.cardsHand.Remove(card.id);
                }
            }
            else if (playerId == GameManager.SelfPlayerId)
            {
                foreach (var card in cards)
                {
                    GameManager.Singleton.cardsHand[card.id] = card;
                }
                GameManager.Singleton.gameUI.DrawCards(cards);
            }

            string cardStr = "";
            if (cards.Count > 0)
            {
                foreach (var card in cards)
                {
                    cardStr += LanguageUtils.GetCardName(card.cardName);
                }
                GameManager.Singleton.gameUI.ShowCardsMove(targetId, playerId, cards);

            }
            else
            {
                List<CardFS> unknownCards = new List<CardFS>();
                for (int i = 0; i < unknowCount; i++)
                {
                    unknownCards.Add(new CardFS(null));
                }
                GameManager.Singleton.gameUI.ShowCardsMove(targetId, playerId, unknownCards);
            }

            string s = string.Format("{0}交给{1}{2}张牌{3}", GameManager.Singleton.players[targetId].name, GameManager.Singleton.players[playerId].name, cardCount, cardStr);
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);
        }
        else
        {
            if (targetId == GameManager.SelfPlayerId)
            {
                if (GameManager.Singleton.selectSkill != null)
                {
                    GameManager.Singleton.selectSkill.OnUse();
                }
            }

            GameManager.Singleton.gameUI.Players[targetId].SetJinBi(true);

            string s = string.Format("{0}被禁闭了", GameManager.Singleton.players[targetId].name);
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);
        }
    }

}
