using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 肥原龙川【诡诈】：出牌阶段限一次，你可以指定一名角色，然后视为你对其使用了一张【威逼】或【利诱】。
public class UserSkill_GuiZha_LiYou : SkillBase
{
    public override string name { get { return "诡诈-利诱"; } }
    public override bool canUse
    {
        get
        {
            return usedCount < 1 && playerId == GameManager.SelfPlayerId && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.curPhase == PhaseEnum.Main_Phase;
        }
    }

    private int usedCount = 0;

    private int selectPlayerId = -1;

    public UserSkill_GuiZha_LiYou(int id)
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
            if(GameManager.Singleton.selectSkill!= null)
            {
                GameManager.Singleton.selectSkill.Cancel();
            }
            GameManager.Singleton.IsUsingSkill = true;
            GameManager.Singleton.selectSkill = this;
            GameManager.Singleton.gameUI.ShowPhase("正在使用技能【诡诈】，请选择一名玩家做为利诱目标");
        }
    }

    public override void Use()
    {
        if(selectPlayerId > -1)
        {
            ProtoHelper.SendSkill_GuiZha(selectPlayerId, CardNameEnum.LiYou, CardNameEnum.ChengQing, GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一名玩家做为利诱目标");
        }
    }

    public override void OnCardSelect(int cardId)
    {

    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if(selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        if(selectPlayerId == PlayerId)
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
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
            selectPlayerId = -1; 
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        usedCount = usedCount + 1;
        Cancel();
    }

    public static void OnReceiveUse(int playerId, int tatgetId, CardNameEnum cardName)
    {
        if(playerId == GameManager.SelfPlayerId)
        {
            if(GameManager.Singleton.selectSkill != null)
            {
                foreach(var skill in GameManager.Singleton.players[playerId].role.skills)
                {
                    skill.OnUse();
                }
            }
        }
        string s = string.Format("{0}号玩家对{1}号玩家使用了技能诡诈，视为使用了一张{2}", playerId, tatgetId, LanguageUtils.GetCardName(cardName));
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
