using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 肥原龙川【诡诈】：出牌阶段限一次，你可以指定一名角色，然后视为你对其使用了一张【威逼】或【利诱】。
public class UserSkill_GuiZha_WeiBi : SkillBase
{
    public override string name { get { return "诡诈-威逼"; } }
    public override bool canUse
    {
        get
        {
            return usedCount < 1 && playerId == GameManager.SelfPlayerId && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.curPhase == PhaseEnum.Main_Phase;
        }
    }

    private int usedCount = 0;

    private int selectPlayerId = 0;

    public UserSkill_GuiZha_WeiBi(int id)
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
            if (GameManager.Singleton.selectSkill != null)
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

        if (selectPlayerId > 0)
        {
            if(GameManager.Singleton.gameUI.goWeiBiSelect.activeSelf)
            {
                WeiBiSelect weiBiSelect = GameManager.Singleton.gameUI.goWeiBiSelect.GetComponent<WeiBiSelect>();
                var cardWant = weiBiSelect.cardWant;
                ProtoHelper.SendSkill_GuiZha(selectPlayerId, CardNameEnum.WeiBi, cardWant, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowWeiBiSelect(true);
            }
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一名玩家做为威逼目标");
        }
    }

    public override void OnCardSelect(int cardId)
    {

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
        else if(PlayerId > 0)
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
        if (selectPlayerId > 0)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
            selectPlayerId = 0; 
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.ShowWeiBiSelect(false);
    }

    public override void OnUse()
    {
        usedCount = usedCount + 1;
        Cancel();
    }
}
