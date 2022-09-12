using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 端木静【新思潮】：出牌阶段限一次，你可以弃置一张手牌，然后摸两张牌。
public class UserSkill_XinSiChao : SkillBase
{
    public override string name { get { return "新思潮"; } }
    public override bool canUse
    {
        get
        {
            return usedCount < 1 && playerId == GameManager.SelfPlayerId && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.curPhase == PhaseEnum.Main_Phase;
        }
    }

    public override string Des => "新思潮：出牌阶段限一次，你可以弃置一张手牌，然后摸两张牌。\n";

    private int usedCount = 0;

    private int selectCardId = 0;

    public UserSkill_XinSiChao(int id)
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
            GameManager.Singleton.IsUsingSkill = true;
            GameManager.Singleton.selectSkill = this;
            GameManager.Singleton.gameUI.ShowPhase("请选择一张牌弃掉，然后摸2张牌");
        }
    }

    public override void Use()
    {
        if(selectCardId > 0)
        {
            ProtoHelper.SendSkill_XinSiChao(selectCardId, GameManager.Singleton.seqId);
        }
    }

    public override void OnCardSelect(int cardId)
    {
        if(selectCardId > 0)
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

    public override void OnTurnEnd()
    {
        usedCount = 0;
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
    }

    public override void OnUse()
    {
        usedCount = usedCount + 1;
        Cancel();
    }

    public static void OnReceiveUse(int playerId)
    {
        if(playerId == GameManager.SelfPlayerId)
        {
            if(GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
        string s = "" + playerId + "号玩家使用了技能新思潮";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
