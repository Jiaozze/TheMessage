using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 李宁玉【遗信】：你死亡前，可以将一张手牌置入另一名角色的情报区。
public class UserSkill_YiXin : SkillBase
{
    public UserSkill_YiXin(int id)
    {
        playerId = id;
    }

    private int selectCardId = 0;
    private int selectPlayerId = 0;
    public override string name => "遗信";

    public override bool canUse => false;

    public override string Des => "遗信：你死亡前，可以将一张手牌置入另一名角色的情报区\n";

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("触发技能遗信，可以将一张手牌置入另一名角色的情报区中");
    }
    public override void OnCardSelect(int cardId)
    {
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
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        if (selectPlayerId == PlayerId)
        {
            selectPlayerId = 0;
        }
        else if(PlayerId > 0)
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }
    public override void Use()
    {
        if(selectCardId != 0 && selectPlayerId > 0)
        {
            ProtoHelper.SendSkill_YiXin(true, selectPlayerId, selectCardId, GameManager.Singleton.seqId);
        }
        else if(selectCardId == 0)
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择要置入其他角色情报区的手牌");
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择要置入情报的角色");
        }
    }
    public override void Cancel()
    {
        base.Cancel();
        if (selectCardId > 0)
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
        }
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectCardId = 0;
        selectPlayerId = -1;

        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        ProtoHelper.SendSkill_YiXin(false, 0, 0, GameManager.Singleton.seqId);
    }

    public override void OnUse()
    {
        if (selectCardId > 0)
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
        }
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectCardId = 0;
        selectPlayerId = -1;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUse(int playerId, int targetId, CardFS card, bool enable)
    {
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
        if(enable)
        {
            if(playerId == GameManager.SelfPlayerId)
            {
                GameManager.Singleton.cardsHand.Remove(card.id);
                GameManager.Singleton.gameUI.RemoveCards(new List<CardFS>() { card });
            }
            string cardInfo = "";
            GameManager.Singleton.players[playerId].cardCount -= 1;
            GameManager.Singleton.players[targetId].AddMessage(card);
            GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
            GameManager.Singleton.gameUI.Players[targetId].RefreshMessage();
            GameManager.Singleton.gameUI.ShowAddMessage(targetId, card, false, playerId);
            cardInfo = card.GetCardInfo();

            string s = string.Format("{0}发动了遗信, 将手牌{1}放入{2}情报区", GameManager.Singleton.players[playerId].name, cardInfo, GameManager.Singleton.players[targetId].name);
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);
        }
    }
}
