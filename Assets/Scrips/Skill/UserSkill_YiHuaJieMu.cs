using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//你接收双色情报后，可以从你的情报区选择一张情报加入手牌
public class UserSkill_YiHuaJieMu : SkillBase
{
    public override string name { get { return "移花接木"; } }
    public override bool canUse { get {
            if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
            {
                return false;
            }

            if (GameManager.Singleton.curPhase != PhaseEnum.Fight_Phase)
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

    private int selectMessageId = 0;
    private int messagePlayerId = -1;
    private int selectPlayerId = -1;
    public UserSkill_YiHuaJieMu(int id)
    {
        playerId = id;
    }

    public override void PrepareUse()
    {
        //if(GameManager.Singleton.players[GameManager.SelfPlayerId].messages.Count == 0)
        //{
        //    Cancel();
        //    return;
        //}
        //if (GameManager.Singleton.selectSkill == this)
        //{
        //    return;
        //}
        base.PrepareUse();
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("发动技能移花接木，选择一名角色将他的一张情报移至另一名角色情报区");
    }
    public override void Use()
    {
        if(selectPlayerId != -1 && messagePlayerId != -1 && selectMessageId != 0)
        {
            ProtoHelper.SendSkill_YiHuaJieMu(messagePlayerId, selectPlayerId, selectMessageId, GameManager.Singleton.seqId);
        }
        else if(messagePlayerId == -1 || selectMessageId == 0)
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择要移动情报的角色和情报");
        }
        //else if (selectMessageId == 0)
        //{
        //    GameManager.Singleton.gameUI.ShowInfo("请选择要移动的情报");
        //}
        else if (selectPlayerId == -1)
        {
            GameManager.Singleton.gameUI.ShowInfo("选择要移至的角色");
        }

    }

    public override void OnCardSelect(int cardId)
    {

    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if(selectMessageId != 0)
        {
            if (PlayerId == messagePlayerId)
            {
                GameManager.Singleton.gameUI.ShowInfo("需要选择移至另一名角色情报区");
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
        else
        {
            selectMessageId = 0;
            messagePlayerId = -1;
            GameManager.Singleton.gameUI.ShowPlayerMessageInfo(PlayerId);
            GameManager.Singleton.gameUI.ShowPhase("选择要移的情报");
        }
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {
        messagePlayerId = playerId;
        selectMessageId = cardId;
        GameManager.Singleton.gameUI.ShowPhase("选择要移至的角色");
    }

    public override void OnMessageInfoClose()
    {
        Cancel();
    }

    public override void Cancel()
    {
        selectMessageId = 0;
        selectPlayerId = -1;
        messagePlayerId = -1;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
    }

    public override void OnUse()
    {
        selectMessageId = 0;
        selectPlayerId = -1;
        messagePlayerId = -1;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
    }

    public static void OnReceiveUse(int playerId, int cardId, int from, int to, bool joinHand)
    {
        string cardStr = "";
        CardFS messageCard = new CardFS(null);

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }

        foreach(var message in GameManager.Singleton.players[from].messages)
        {
            if(message.id == cardId)
            {
                cardStr = message.GetCardInfo();
                messageCard = message;
                break;
            }
        }

        GameManager.Singleton.players[from].RemoveMessage(cardId);
        GameManager.Singleton.gameUI.Players[from].RefreshMessage();

        if(joinHand)
        {
            GameManager.Singleton.players[playerId].cardCount += 1;
            GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
            GameManager.Singleton.gameUI.ShowAddMessage(playerId, messageCard, false, from);

            string s = string.Format("{0}号玩家使用了技能移花接木,将{1}号玩家情报区的{2}置入手牌", playerId, from, cardStr);
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);
        }
        else
        {
            GameManager.Singleton.players[to].AddMessage(messageCard);
            GameManager.Singleton.gameUI.Players[to].RefreshMessage();
            GameManager.Singleton.gameUI.ShowAddMessage(to, messageCard, false, from);

            string s = string.Format("{0}号玩家使用了技能移花接木,将{1}号玩家情报区的{2}移至{3}号情报区", playerId, from, cardStr, to);
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);

        }

    }

}
