using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SP李宁玉【应变】：你的【截获】可以当作【误导】使用
public class UserSkill_YingBian : SkillBase
{
    public UserSkill_YingBian(int id)
    {
        playerId = id;
    }

    private const int needDropCount = 2;
    private int selectPlayerId = -1;
    private int selectCardId;

    public override string name => "应变";

    public override bool canUse { get {
            if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
            {
                return false;
            }
            if (!GameUtils.IsFightPhase())
            {
                return false;
            }
            return true;
        } }

    public override string Des => "应变：你的【截获】可以当作【误导】使用。\n";

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("你可以将【截获】可以当作【误导】使用");
    }
    public override void OnPlayerSelect(int PlayerId)
    {
        if(selectCardId > 0)
        {
            if (selectPlayerId > -1)
            {
                GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
            }
            if (selectPlayerId == PlayerId)
            {
                selectPlayerId = -1;
            }
            else if (PlayerId > -1)
            {
                selectPlayerId = PlayerId;
                GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
            }
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一张截获当作误导使用");
        }
    }


    public override void OnCardSelect(int cardId)
    {
        if (selectCardId > 0)
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
            GameManager.Singleton.gameUI.CheckTargetAvailable();
        }
        if (selectCardId == cardId)
        {
            selectCardId = 0;
        }
        else if(cardId > 0)
        {
            if(GameManager.Singleton.cardsHand[cardId].cardName == CardNameEnum.JieHuo)
            {
                selectCardId = cardId;
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(true);
                SetWuDaoTargetAvailable();
            }
            else
            {
                Debug.LogError(GameManager.Singleton.cardsHand[cardId].cardName);
                GameManager.Singleton.gameUI.ShowInfo("请选择一张截获当作误导使用");
            }
        }
        else
        {
            selectCardId = 0;
        }
        OnPlayerSelect(-1);
    }

    private void SetWuDaoTargetAvailable()
    {
        bool banClick = false;
        foreach (var kv in GameManager.Singleton.gameUI.Players)
        {
            banClick =
                kv.Key != GameManager.Singleton.GetPlayerAliveLeft(GameManager.Singleton.CurMessagePlayerId)
                && kv.Key != GameManager.Singleton.GetPlayerAliveRight(GameManager.Singleton.CurMessagePlayerId);
            kv.Value.SetBanClick(banClick);
        }
    }
    public override void Use()
    {
        if(UserSkill_QiangLing.cardTypes.Contains(CardNameEnum.WuDao))
        {
            GameManager.Singleton.gameUI.ShowInfo("被强令禁用的卡牌无法使用");
            return;
        }
        if (selectCardId > 0 && selectPlayerId > -1)
        {
            ProtoHelper.SendUseCardMessage_WuDao(selectCardId, selectPlayerId, GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择正确的卡牌和目标");
        }
    }
    public override void Cancel()
    {
        base.Cancel();
        if (selectCardId > 0 && GameManager.Singleton.gameUI.Cards.ContainsKey(selectCardId))
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
        }
        selectCardId = 0;

        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }

        selectPlayerId = -1;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.CheckTargetAvailable();
    }

}
