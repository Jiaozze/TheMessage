using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSkill_QiHuoKeJu : SkillBase
{
    public override string name { get { return "奇货可居"; } }
    public override bool canUse { get { return false; } }

    private int selectCardId;
    public UserSkill_QiHuoKeJu(int id)
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
        if(GameManager.Singleton.players[GameManager.SelfPlayerId].messages.Count == 0)
        {
            Cancel();
            return;
        }
        if (GameManager.Singleton.selectSkill == this)
        {
            return;
        }
        base.PrepareUse();
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("可以发动技能从你的情报区选择一张情报加入手牌");
    }
    public override void Use()
    {
        if(selectCardId > 0)
        {
            ProtoHelper.SendSkill_QiHuoKeJu(selectCardId, GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择要加入手牌的情报");
        }
    }

    public override void OnCardSelect(int cardId)
    {

    }

    public override void OnPlayerSelect(int PlayerId)
    {

    }

    public override void OnMessageSelect(int playerId, int cardId)
    {
        selectCardId = 0;
    }

    public override void OnMessageInfoClose()
    {
        Cancel();
    }

    public override void Cancel()
    {
        selectCardId = 0;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();

        ProtoHelper.SendEndReceive(GameManager.Singleton.seqId);
    }

    public override void OnUse()
    {
        selectCardId = 0;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
    }

    public static void OnReceiveUse(int playerId, int cardId)
    {
        GameManager.Singleton.players[playerId].DrawCard(1);

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
            foreach (var card in GameManager.Singleton.players[playerId].messages)
            {
                if (card.id == cardId)
                {
                    GameManager.Singleton.cardsHand[cardId] = card;
                    GameManager.Singleton.gameUI.DrawCards(new List<CardFS>() { card });
                }
            }
        }
        GameManager.Singleton.players[playerId].RemoveMessage(cardId);
        GameManager.Singleton.gameUI.Players[playerId].RefreshMessage();

        string s = "" + playerId + "号玩家使用了技能奇货可居";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
