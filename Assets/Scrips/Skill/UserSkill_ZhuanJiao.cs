using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 白小年【转交】：你使用一张手牌后，可以从你的情报区选择一张非黑色情报，将其置入另一名角色的情报区，然后你摸两张牌。你不能通过此技能让任何角色收集三张或更多同色情报。
public class UserSkill_ZhuanJiao : SkillBase
{
    public UserSkill_ZhuanJiao(int id)
    {
        playerId = id;
    }

    private int selectMessageId = 0;
    private int selectPlayerId;

    public override string name => "转交";

    public override bool canUse => false;

    public override string Des => "转交：你使用一张手牌后，可以从你的情报区选择一张非黑色情报，将其置入另一名角色的情报区，然后你摸两张牌。你不能通过此技能让任何角色收集三张或更多同色情报。\n";

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("触发技能转交，可以从你的情报区选择一张非黑色情报，将其置入另一名角色的情报区，然后你摸两张牌");
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
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

    public override void OnMessageSelect(int playerId, int cardId)
    {
        if(playerId!= GameManager.SelfPlayerId)
        {
            GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
            return;
        }
        base.OnMessageSelect(playerId, cardId);
        selectMessageId = cardId;
    }

    public override void OnMessageInfoClose()
    {
        base.OnMessageInfoClose();
        //Cancel();
    }

    public override void Use()
    {
        if(selectMessageId != 0 && !GameManager.Singleton.players[GameManager.SelfPlayerId].messages[selectMessageId].color.Contains(CardColorEnum.Black))
        {
            var color = GameManager.Singleton.players[GameManager.SelfPlayerId].messages[selectMessageId].color[0];
            if(selectPlayerId > 0 && GameManager.Singleton.players[selectPlayerId].GetMessageCount(color) < 2)
            {
                ProtoHelper.SendSkill_ZhuanJiao(true, selectPlayerId, selectMessageId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择一名合法目标, 你不能通过此技能让任何角色收集三张或更多同色情报");
            }
        }
        else
        {
            GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
            GameManager.Singleton.gameUI.ShowInfo("请选择要置入情报区的非黑情报");
        }
    }
    public override void Cancel()
    {
        base.Cancel();
        if (selectMessageId > 0)
        {
            selectMessageId = 0;
        }
        if(selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
            selectPlayerId = -1;
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
        GameManager.Singleton.gameUI.ShowPhase();
        ProtoHelper.SendSkill_ZhuanJiao(false, 0, 0, GameManager.Singleton.seqId);
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
            selectPlayerId = -1;
        }
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

    public static void OnReceiveWaitUse(int playerId, int waitSeconds, uint seq)
    {
        GameManager.Singleton.seqId = seq;
        GameManager.Singleton.OnWait(playerId, waitSeconds);

        if(playerId == GameManager.SelfPlayerId)
        {
            foreach (var skill in GameManager.Singleton.players[playerId].role.skills)
            {
                if (skill is UserSkill_ZhuanJiao)
                {
                    var ruGui = skill as UserSkill_ZhuanJiao;
                    ruGui.PrepareUse();
                }
            }
        }
    }

    public static void OnReceiveUse(int playerId, int cardId, int targetId)
    {
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }

        {
            string cardInfo = "";
            foreach (var message in GameManager.Singleton.players[playerId].messages)
            {
                if (message.id == cardId)
                {
                    int toPlayer = targetId;
                    GameManager.Singleton.players[playerId].RemoveMessage(cardId);
                    GameManager.Singleton.players[toPlayer].AddMessage(message);
                    GameManager.Singleton.gameUI.Players[playerId].RefreshMessage();
                    GameManager.Singleton.gameUI.Players[toPlayer].RefreshMessage();
                    GameManager.Singleton.gameUI.ShowAddMessage(toPlayer, message, false, playerId);
                    cardInfo = message.GetCardInfo();
                    break;
                }
            }

            string s = string.Format("{0}发动了转交, 将情报{1}放入{2}情报区", GameManager.Singleton.players[playerId].name, cardInfo, GameManager.Singleton.players[targetId].name);
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);
        }
    }
}
