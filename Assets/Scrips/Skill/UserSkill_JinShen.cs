using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//你接收双色情报后，可以用一张手牌与该情报面朝上互换
public class UserSkill_JinShen : SkillBase
{
    public override string name { get { return "谨慎"; } }
    public override bool canUse { get { return false; } }

    private int selectCardId;
    public UserSkill_JinShen(int id)
    {
        playerId = id;
    }
    public override bool CheckTriger()
    {
        //GameManager.Singleton.gameUI.ShowPhase("是否发动技能-绵里藏针");
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
        if (GameManager.Singleton.cardsHand.Count == 0)
        {
            Cancel();
            return;
        }

        if (GameManager.Singleton.selectSkill == this)
        {
            return;
        }
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("可以用一张手牌与该情报面朝上互换");
    }
    public override void Use()
    {
        if (selectCardId > 0)
        {
            ProtoHelper.SendSkill_JinShen(selectCardId, GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一张手牌");
        }
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

    }

    public override void OnMessageSelect(int playerId, int cardId)
    {

    }

    public override void Cancel()
    {
        if (selectCardId > 0)
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
            selectCardId = 0;
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        ProtoHelper.SendEndReceive(GameManager.Singleton.seqId);
    }

    public override void OnUse()
    {
        if (selectCardId > 0)
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
            selectCardId = 0;
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUse(int playerId, CardFS card)
    {
        GameManager.Singleton.players[playerId].AddMessage(card);

        var message = GameManager.Singleton.messageReceived;
        string messageStr = message.GetCardInfo();
        string cardStr = card.GetCardInfo();
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
            GameManager.Singleton.cardsHand.Add(message.id, message);
            GameManager.Singleton.cardsHand.Remove(card.id);
            GameManager.Singleton.gameUI.Cards.Remove(card.id);
            GameManager.Singleton.gameUI.DrawCards(new List<CardFS>() { message });
        }
        GameManager.Singleton.players[playerId].RemoveMessage(message.id);
        GameManager.Singleton.gameUI.Players[playerId].RefreshMessage();

        string s = string.Format("{0}号玩家使用了技能谨慎，将情报{1}与手牌{2}互换", playerId, messageStr, cardStr);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
