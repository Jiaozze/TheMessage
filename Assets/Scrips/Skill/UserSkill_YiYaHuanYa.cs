using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//// 王魁【以牙还牙】：你接收黑色情报后，可以将一张黑色手牌置入情报传出者或其相邻角色的情报区，然后摸一张牌。
public class UserSkill_YiYaHuanYa : SkillBase
{
    public override string name { get { return "以牙还牙"; } }
    public override bool canUse { get { return false; } }

    public override string Des => "以牙还牙：你接收黑色情报后，可以将一张黑色手牌置入情报传出者或其相邻角色的情报区，然后摸一张牌。";

    private int selectCardId;
    private int selectPlayerId = -1;
    public UserSkill_YiYaHuanYa(int id)
    {
        playerId = id;
    }
    public override bool CheckTriger()
    {
        if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
        {
            return false;
        }

        if (GameManager.Singleton.curPhase != PhaseEnum.Receive_Phase)
        {
            return false;
        }
        if (GameManager.Singleton.CurMessagePlayerId != GameManager.SelfPlayerId)
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
        GameManager.Singleton.gameUI.ShowPhase("可以发动技能将一张黑色手牌置入情报传出者或其相邻角色的情报区，然后摸一张牌");
    }
    public override void Use()
    {
        int senderId = GameManager.Singleton.CurTurnPlayerId;
        if (!(selectCardId > 0 && GameManager.Singleton.cardsHand[selectCardId].color.Contains(CardColorEnum.Black)))
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一张黑色手牌");
        }
        else if (!(selectPlayerId == senderId || selectPlayerId == GameManager.Singleton.GetPlayerAliveLeft(senderId) || selectPlayerId == GameManager.Singleton.GetPlayerAliveRight(senderId)))
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择情报传出者或其相邻角色");
        }
        else
        {
            ProtoHelper.SendSkill_YiYaHuanYa(selectCardId, selectPlayerId, GameManager.Singleton.seqId);
        }

    }

    public override void OnCardSelect(int cardId)
    {
        if (!GameManager.Singleton.cardsHand[cardId].color.Contains(CardColorEnum.Black))
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一张黑色手牌");
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
        int senderId = GameManager.Singleton.CurTurnPlayerId;

        if (!(PlayerId == senderId || PlayerId == GameManager.Singleton.GetPlayerAliveLeft(senderId) || PlayerId == GameManager.Singleton.GetPlayerAliveRight(senderId)))
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择情报传出者或其相邻角色");
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
        else
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {

    }

    public override void Cancel()
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

        ProtoHelper.SendEndReceive(GameManager.Singleton.seqId);
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

    public static void OnReceiveUse(int playerId, int target, CardFS card)
    {
        string cardStr = card.GetCardInfo();
        if(playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
            GameManager.Singleton.cardsHand.Remove(card.id);
            GameManager.Singleton.gameUI.RemoveCards(new List<CardFS>() { card });
        }
        GameManager.Singleton.players[playerId].cardCount = GameManager.Singleton.players[playerId].cardCount - 1;
        GameManager.Singleton.players[target].AddMessage(card);

        GameManager.Singleton.gameUI.ShowAddMessage(target, card, false, playerId);
        GameManager.Singleton.gameUI.Players[target].RefreshMessage();

        string s = string.Format("{0}号玩家使用了技能以牙还牙,将一张{1}放入{2}号玩家情报区");
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
