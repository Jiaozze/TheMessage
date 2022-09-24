using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 裴玲【交际】A：出牌阶段限一次，你可以抽取一名角色的最多两张手牌。
// 裴玲【交际】B：然后将等量手牌交给该角色。你每收集一张黑色情报，便可以少交一张牌。
public class UserSkill_JiaoJi : SkillBase
{
    public override string name { get { return "交际"; } }
    public override bool canUse
    {
        get
        {
            return usedCount < 1 && playerId == GameManager.SelfPlayerId && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.curPhase == PhaseEnum.Main_Phase && !isWaitGive;
        }
    }

    public override string Des => "交际：出牌阶段限一次，你可以抽取一名角色的最多两张手牌,然后将等量手牌交给该角色。你每收集一张黑色情报，便可以少交一张牌。\n";

    private int usedCount = 0;

    private int selectPlayerId = -1;
    private List<int> selectCardIds = new List<int>();

    private bool isWaitGive = false;

    private int needGiveCount = 0;

    public UserSkill_JiaoJi(int id)
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
        if(GameManager.Singleton.selectSkill == this)
        {
            Cancel();
        }
        else
        {
            GameManager.Singleton.IsUsingSkill = true;
            GameManager.Singleton.selectSkill = this;
            GameManager.Singleton.gameUI.ShowPhase("选择一名角色做为技能目标，抽取最多两张手牌");
        }
    }

    public override void Use()
    {
        if(isWaitGive)
        {
            if(selectCardIds.Count == needGiveCount)
            {
                ProtoHelper.SendSkill_JiaoJiB(selectCardIds, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("需要给出" + needGiveCount + "张手牌");
            }
        }
        else
        {
            if(selectPlayerId > 0 && GameManager.Singleton.players[selectPlayerId].cardCount > 0)
            {
                ProtoHelper.SendSkill_JiaoJiA(selectPlayerId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("选择一名有手牌的角色做为技能目标，抽取最多两张手牌");
            }
        }
    }

    public override void OnCardSelect(int cardId)
    {
        if(isWaitGive)
        {
            if(selectCardIds.Contains(cardId))
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
                selectCardIds.Remove(cardId);
            }
            else
            {
                if(selectCardIds.Count< needGiveCount)
                {
                    GameManager.Singleton.gameUI.Cards[cardId].OnSelect(true);
                    selectCardIds.Add(cardId);
                }
                else
                {
                    GameManager.Singleton.gameUI.ShowInfo("只需交还" + needGiveCount + "张手牌");
                }                
            }
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("选择一名角色做为技能目标，抽取最多两张手牌");
        }
    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if(isWaitGive)
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
        else if (PlayerId > 0)
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {

    }

    public override void OnTurnEnd()
    {
        usedCount = 0;
        isWaitGive = false;
        needGiveCount = 0;
        if (selectCardIds.Count > 0)
        {
            foreach (var cardId in selectCardIds)
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
            }
            selectCardIds.Clear();
        }
    }

    public override void Cancel()
    {
        if (isWaitGive && needGiveCount > 0)
        {
            GameManager.Singleton.gameUI.ShowInfo("需要给出" + needGiveCount + "张手牌");
            return;
        }
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

    public override void OnUse()
    {
        usedCount = usedCount + 1;
        isWaitGive = false;
        needGiveCount = 0;
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

    public static void OnReceiveUseA(int playerId, int targetId, List<CardFS> cards, int unknowCount, int waitSeconds, uint seq)
    {
        GameManager.Singleton.seqId = seq;

        if (waitSeconds > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitSeconds);
        }
        int cardCount = cards.Count > 0 ? cards.Count : unknowCount;
        GameManager.Singleton.players[playerId].cardCount += cardCount;
        GameManager.Singleton.players[targetId].cardCount -= cardCount;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        GameManager.Singleton.gameUI.Players[targetId].RefreshCardCount();
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

        if (playerId == GameManager.SelfPlayerId)
        {
            if(GameManager.Singleton.selectSkill != null)
            {
                UserSkill_JiaoJi skill_JiaoJi = GameManager.Singleton.selectSkill as UserSkill_JiaoJi;
                int count = cardCount - GameManager.Singleton.players[playerId].GetMessageCount(CardColorEnum.Black);
                count = count > 0 ? count : 0;
                skill_JiaoJi.needGiveCount = count;
                if(waitSeconds == 0)
                {
                    skill_JiaoJi.OnUse();
                }
                else
                {
                    skill_JiaoJi.isWaitGive = true;
                    GameManager.Singleton.gameUI.ShowPhase("选择" + count + "张手牌交还");
                }
            }
            foreach (var card in cards)
            {
                GameManager.Singleton.cardsHand[card.id] = card;
            }
            GameManager.Singleton.gameUI.DrawCards(cards);
        }
        else if(targetId == GameManager.SelfPlayerId)
        {
            foreach (var card in cards)
            {
                GameManager.Singleton.cardsHand.Remove(card.id);
                var cardUI = GameManager.Singleton.gameUI.Cards[card.id];
                GameManager.Singleton.gameUI.Cards.Remove(card.id);
                GameObject.Destroy(cardUI.gameObject);
            }
        }

        string s = string.Format("{0}对{1}使用了技能交际，抽取了{2}张牌{3}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId].name, cardCount, cardStr);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, int targetId, List<CardFS> cards, int unknowCount)
    {
        int cardCount = cards.Count > 0 ? cards.Count : unknowCount;
        GameManager.Singleton.players[playerId].cardCount -= cardCount;
        GameManager.Singleton.players[targetId].cardCount += cardCount;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        GameManager.Singleton.gameUI.Players[targetId].RefreshCardCount();

        string cardStr = "";
        if (cards.Count > 0)
        {
            foreach (var card in cards)
            {
                cardStr += LanguageUtils.GetCardName(card.cardName);
            }
            GameManager.Singleton.gameUI.ShowCardsMove(playerId, targetId, cards);

        }
        else
        {
            List<CardFS> unknownCards = new List<CardFS>();
            for (int i = 0; i < unknowCount; i++)
            {
                unknownCards.Add(new CardFS(null));
            }
            GameManager.Singleton.gameUI.ShowCardsMove(playerId, targetId, unknownCards);
        }

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
            foreach (var card in cards)
            {
                GameManager.Singleton.cardsHand.Remove(card.id);
                var cardUI = GameManager.Singleton.gameUI.Cards[card.id];
                GameManager.Singleton.gameUI.Cards.Remove(card.id);
                GameObject.Destroy(cardUI.gameObject);
            }
        }
        else if (targetId == GameManager.SelfPlayerId)
        {
            foreach (var card in cards)
            {
                GameManager.Singleton.cardsHand[card.id] = card;
            }
            GameManager.Singleton.gameUI.DrawCards(cards);
        }

        string s = string.Format("{0}向{1}交还了{2}张牌{3}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId].name, cardCount, cardStr);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

}
