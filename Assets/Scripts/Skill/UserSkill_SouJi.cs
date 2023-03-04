using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 李醒【搜辑】A：争夺阶段，你可以翻开此角色牌，然后查看一名角色的手牌和待收情报。
// 李醒【搜辑】B：并且你可以选择其中任意张黑色牌，展示并加入你的手牌。

public class UserSkill_SouJi : SkillBase
{
    public override string name { get { return "搜辑"; } }
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

    public override string Des => "搜辑:争夺阶段，你可以翻开此角色牌，然后查看一名角色的手牌和待收情报。并且你可以选择其中任意张黑色牌，展示并加入你的手牌。\n";
    private static bool isUseB = false;
    private List<int> selectCardIds = new List<int>();
    private int selectPlayerId = -1;
    private bool isSelectMessage = false;

    public UserSkill_SouJi(int id)
    {
        playerId = id;
    }

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        isUseB = false;
        GameManager.Singleton.gameUI.ShowPhase("发动技能搜辑，查看一名角色的手牌和待收情报, 并选择其中任意张黑色牌，展示并加入你的手牌");
    }
    public override void Use()
    {
        if (!isUseB)
        {
            if (selectPlayerId > 0)
            {
                ProtoHelper.SendSkill_SouJiA(selectPlayerId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择一名其他角色，查看其手牌和待接收情报");
            }
            return;
        }

        if (isSelectMessage || selectCardIds.Count > 0)
        {
            ProtoHelper.SendSkill_SouJiB(isSelectMessage, selectCardIds , GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("选择要加入手牌的牌，或点取消不加入");
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

    public void OnClickOthersCard(int playerId, int cardId, bool isSelect)
    {
        if (isUseB)
        {
            if(!selectCardIds.Contains(cardId))
            {
                selectCardIds.Add(cardId);
            }
        }
        else
        {
            if(selectCardIds.Contains(cardId))
            {
                selectCardIds.Remove(cardId);
            }
        }
    }

    public void OnClickMessage(bool select)
    {
        if (isUseB)
        {
            isSelectMessage = select;
        }
    }

    public override void Cancel()
    {
        selectCardIds.Clear();
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;
        isSelectMessage = false;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        if(isUseB)
        {
            isUseB = false;
            ProtoHelper.SendSkill_SouJiB(isSelectMessage, selectCardIds, GameManager.Singleton.seqId);
        }
    }

    public override void OnUse()
    {
        isUseB = false;
        selectCardIds.Clear();
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;
        isSelectMessage = false;
        GameManager.Singleton.gameUI.HideSouJiSelect();
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUseA(int playerId, int targetId, List<CardFS> cards, CardFS messageCard, int waitingSecond, uint seq)
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
                UserSkill_SouJi skill_SouJi = GameManager.Singleton.selectSkill as UserSkill_SouJi;
                if (skill_SouJi.selectPlayerId > -1)
                {
                    GameManager.Singleton.gameUI.Players[skill_SouJi.selectPlayerId].OnSelect(false);
                }
                skill_SouJi.selectPlayerId = -1;
            }

            UserSkill_SouJi.isUseB = true;
            GameManager.Singleton.gameUI.ShowSouJiSelect(targetId, cards, messageCard);
            GameManager.Singleton.gameUI.ShowPhase("可以选择其中任意张黑色牌，展示并加入你的手牌");
        }
        //GameManager.Singleton.gameUI.ShowMessagingDiaoBao(new CardFS(null));
        //GameManager.Singleton.gameUI.HideMessagingCard();

        string s = string.Format("{0}使用了技能搜辑,并查看了{1}的手牌和待接收情报", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, int targetPlayerId, List<CardFS> cards, CardFS messageCard)
    {
        string handCardsStr = "";
        string messageStr = "";
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
        if(messageCard == null && cards.Count == 0)
        {
            return;
        }

        List<CardFS> cardShow = new List<CardFS>();
        if(cards.Count > 0)
        {
            handCardsStr += GameManager.Singleton.players[targetPlayerId].name + "的手牌";
            foreach (var card in cards)
            {
                handCardsStr += card.GetCardInfo();
                cardShow.Add(card);
            }
            GameManager.Singleton.players[targetPlayerId].cardCount -= cards.Count;
            GameManager.Singleton.gameUI.Players[targetPlayerId].RefreshCardCount();
            if(targetPlayerId == GameManager.SelfPlayerId)
            {
                foreach(var card in cards)
                {
                    GameManager.Singleton.cardsHand.Remove(card.id);
                    var cardUI = GameManager.Singleton.gameUI.Cards[card.id];
                    GameManager.Singleton.gameUI.Cards.Remove(card.id);
                    GameObject.Destroy(cardUI.gameObject);
                }
            }
        }

        if(messageCard != null)
        {
            messageStr += "待接收情报" + messageCard.GetCardInfo();
            cardShow.Add(messageCard);
            GameManager.Singleton.gameUI.HideMessagingCard();
        }

        GameManager.Singleton.players[playerId].cardCount += cardShow.Count;
        if(playerId == GameManager.SelfPlayerId)
        {
            foreach (var card in cardShow)
            {
                GameManager.Singleton.cardsHand.Add(card.id, card);
            }
            GameManager.Singleton.gameUI.DrawCards(cardShow);
        }

        GameManager.Singleton.gameUI.ShowCardAndGet(cardShow, playerId);

        string s = string.Format("{0}使用了技能搜辑,展示了{1}{2},并加入手牌", GameManager.Singleton.players[playerId].name, handCardsStr, messageStr);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
