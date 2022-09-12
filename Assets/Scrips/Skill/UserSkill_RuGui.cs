using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 老汉【如归】：你死亡前，可以将你情报区中的一张情报置入当前回合角色的情报区中。
public class UserSkill_RuGui : SkillBase
{
    public UserSkill_RuGui(int id)
    {
        playerId = id;
    }

    private int selectMessageId = 0;
    public override string name => "如归";

    public override bool canUse => false;

    public override string Des => "如归：你死亡前，可以将你情报区中的一张情报置入当前回合角色的情报区中\n";

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("触发技能如归，可以将你情报区中的一张情报置入当前回合角色的情报区中");
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {
        base.OnMessageSelect(playerId, cardId);
        selectMessageId = cardId;
    }

    public override void OnMessageInfoClose()
    {
        base.OnMessageInfoClose();
        Cancel();
    }

    public override void Use()
    {
        if(selectMessageId != 0)
        {
            ProtoHelper.SendSkill_RuGui(true, selectMessageId, GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择要置入当前回合角色情报区的情报");
        }
    }
    public override void Cancel()
    {
        base.Cancel();
        if (selectMessageId > 0)
        {
            selectMessageId = 0;
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
        GameManager.Singleton.gameUI.ShowPhase();
        ProtoHelper.SendSkill_RuGui(false, 0, GameManager.Singleton.seqId);
    }

    public override void OnUse()
    {
        if (selectMessageId > 0)
        {
            selectMessageId = 0;
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUse(int playerId, int cardId)
    {
        string cardInfo = "";
        foreach(var message in GameManager.Singleton.players[playerId].messages)
        {
            if(message.id == cardId)
            {
                int toPlayer = GameManager.Singleton.CurTurnPlayerId;
                GameManager.Singleton.players[playerId].RemoveMessage(cardId);
                GameManager.Singleton.players[toPlayer].AddMessage(message);
                GameManager.Singleton.gameUI.Players[playerId].RefreshMessage();
                GameManager.Singleton.gameUI.Players[toPlayer].RefreshMessage();
                GameManager.Singleton.gameUI.ShowAddMessage(toPlayer, message, false, playerId);
                cardInfo = message.GetCardInfo();
                break;
            }
        }

        if(playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }

        string s = string.Format("{0}号玩家发动了如归, 将情报{1}放入当前回合角色情报区", playerId, cardInfo);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
