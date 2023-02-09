using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 程小蝶【惊梦】A：你接收黑色情报后，可以查看一名角色的手牌。
// 程小蝶【惊梦】B：然后从中选择一张弃置。

public class UserSkill_JingMeng : SkillBase
{
    public override string name { get { return "惊梦"; } }
    public override bool canUse { get { return false; } }

    public override string Des => "惊梦:你接收黑色情报后，可以查看一名角色的手牌，然后从中选择一张弃置\n";

    private int selectPlayerId;
    public List<CardFS> cards;
    private int selectCardId;
    public UserSkill_JingMeng(int id)
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

        PrepareUse();
        return true;
    }

    public override void PrepareUse()
    {
        if (GameManager.Singleton.selectSkill == this)
        {
            return;
        }
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("可以发动技能惊梦，查看一名角色的手牌，然后从中选择一张弃置");
    }
    public override void Use()
    {
        if(cards == null)
        {
            if (selectPlayerId > 0)
            {
                ProtoHelper.SendSkill_JingMengA(selectPlayerId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择惊梦的技能目标");
            }

        }
        else
        {
            if(selectCardId != 0)
            {
                ProtoHelper.SendSkill_JingMengB(selectCardId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择一张牌弃置");
            }
        }
    }

    public void OnClickOthersCard(int playerId, int cardId)
    {
        if(cards != null)
        {
            selectCardId = cardId;
        }
    }

    public override void OnPlayerSelect(int PlayerId)
    {
        //第二阶段选择弃置牌，直接返回
        if(cards != null)
        {
            return;
        }

        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        if (selectPlayerId == PlayerId)
        {
            selectPlayerId = 0;
        }
        else if (PlayerId > 0)
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }
    public override void OnMessageInfoClose()
    {
        if(cards != null)
        {
            Cancel();
        }
    }
    public override void Cancel()
    {

        if(cards == null)
        {
            if (selectCardId > 0)
            {
                selectCardId = 0;
            }
            if (selectPlayerId > -1)
            {
                GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
                selectPlayerId = -1;
            }
            GameManager.Singleton.IsUsingSkill = false;
            GameManager.Singleton.selectSkill = null;
            GameManager.Singleton.gameUI.ShowPhase();
            ProtoHelper.SendEndReceive(GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一张牌弃置");
            GameManager.Singleton.gameUI.ShowHandCard(selectPlayerId, cards);
        }
    }

    public static void OnReceiveUseA(int playerId, int targetPlayerId, List<CardFS> cards)
    {
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill is UserSkill_JingMeng)
            {
                UserSkill_JingMeng skill_JingMeng = GameManager.Singleton.selectSkill as UserSkill_JingMeng;
                skill_JingMeng.cards = cards;
                GameManager.Singleton.gameUI.ShowHandCard(targetPlayerId, cards);
            }
        }

        string s = string.Format("{0}使用了技能惊梦，查看{1}的手牌", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetPlayerId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, int targetPlayerId, CardFS card)
    {
        if(playerId == GameManager.SelfPlayerId)
        {
            GameManager.Singleton.gameUI.HidePlayerMessageInfo();

            if (GameManager.Singleton.selectSkill is UserSkill_JingMeng)
            {
                UserSkill_JingMeng skill_JingMeng = GameManager.Singleton.selectSkill as UserSkill_JingMeng;
                if (skill_JingMeng.selectCardId > 0)
                {
                    skill_JingMeng.selectCardId = 0;
                }
                if (skill_JingMeng.selectPlayerId > -1)
                {
                    GameManager.Singleton.gameUI.Players[skill_JingMeng.selectPlayerId].OnSelect(false);
                    skill_JingMeng.selectPlayerId = -1;
                }
                skill_JingMeng.cards = null;

                GameManager.Singleton.IsUsingSkill = false;
                GameManager.Singleton.selectSkill = null;
                GameManager.Singleton.gameUI.ShowPhase();
            }
        }
        string cardStr = LanguageUtils.GetCardName(card.cardName);
        string s = string.Format("{0}选择了{1}的{2}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetPlayerId].name, cardStr);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
