using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//商玉【借刀杀人】A：争夺阶段，你可以翻开此角色牌，然后抽取另一名角色的一张手牌并展示之。若展示的牌是非黑色，则你摸一张牌。
// 商玉【借刀杀人】B：若展示的牌是黑色，则你可以将其置入一名角色的情报区，并将你的角色牌翻至面朝下。
public class UserSkill_JieDaoShaRen : SkillBase
{
    private int selectPlayerId = -1;
    public override string name { get { return "借刀杀人"; } }
    public override bool canUse { get {
            if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
            {
                return false;
            }
            if (GameManager.Singleton.curPhase != PhaseEnum.Fight_Phase)
            {
                return false;
            }
            if (!GameManager.Singleton.players[GameManager.SelfPlayerId].role.isBack)
            {
                return false;
            }
            return true;
        } }

    public override string Des => "借刀杀人:争夺阶段，你可以翻开此角色牌，然后抽取另一名角色的一张手牌并展示之。若展示的牌是黑色，则你可以将其置入一名角色的情报区，并将你的角色牌翻至面朝下。若展示的牌是非黑色，则你摸一张牌。\n";

    public UserSkill_JieDaoShaRen(int id)
    {
        playerId = id;
    }
    public override bool CheckTriger()
    {
        return false;
    }

    public override void PrepareUse()
    {
        base.PrepareUse();
        if (canUse && GameManager.Singleton.selectSkill == this)
        {
            GameManager.Singleton.selectSkill.Cancel();
            return;
        }

        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("是否翻面发动技能借刀杀人，抽取另一名角色的一张手牌并展示之");
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
        else if (PlayerId > -1)
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }

    public override void Use()
    {
        //第一阶段抽牌
        if(GameManager.Singleton.players[GameManager.SelfPlayerId].role.isBack)
        {
            if(selectPlayerId > 0 && GameManager.Singleton.players[selectPlayerId].cardCount > 0)
            {
                ProtoHelper.SendSkill_JieDaoShaRenA(selectPlayerId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择一名有手牌的玩家做为技能目标");
            }
        }
        else
        {
            if(selectPlayerId > -1)
            {
                ProtoHelper.SendSkill_JieDaoShaRenB(true, selectPlayerId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择要置入情报的玩家");
            }
        }
    }

    public override void Cancel()
    {
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;
        if (!GameManager.Singleton.players[GameManager.SelfPlayerId].role.isBack)
        {
            ProtoHelper.SendSkill_JieDaoShaRenB(false, 0, GameManager.Singleton.seqId);
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;

        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUseA(int playerId, int targetId, CardFS card, int waitSeconds, uint seq)
    {
        if (waitSeconds > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitSeconds);
        }

        GameManager.Singleton.players[playerId].cardCount += 1;
        GameManager.Singleton.players[targetId].cardCount -= 1;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        GameManager.Singleton.gameUI.Players[targetId].RefreshCardCount();
        GameManager.Singleton.gameUI.ShowCardsMove(targetId, playerId, new List<CardFS>() { card });


        if (playerId == GameManager.SelfPlayerId)
        {
            UserSkill_JieDaoShaRen userSkill_JieDaoShaRen = null;
            if (GameManager.Singleton.selectSkill != null)
            {
                userSkill_JieDaoShaRen = GameManager.Singleton.selectSkill as UserSkill_JieDaoShaRen;
                GameManager.Singleton.selectSkill.OnUse();
            }

            GameManager.Singleton.cardsHand.Add(card.id, card);
            GameManager.Singleton.gameUI.DrawCards(new List<CardFS>() { card });

            if (waitSeconds > 0)
            {
                GameManager.Singleton.seqId = seq;
                if(null != userSkill_JieDaoShaRen)
                {
                    GameManager.Singleton.IsUsingSkill = true;
                    GameManager.Singleton.selectSkill = userSkill_JieDaoShaRen;
                    GameManager.Singleton.gameUI.ShowPhase("是否将抽取的" + LanguageUtils.GetColorsName(card.color) + LanguageUtils.GetCardName(card.cardName) + "置入一名角色的情报区，并将你的角色牌翻至面朝下");
                }
                else
                {
                    Debug.LogError("借刀杀人技能出错");
                }
            }
        }
        else if(targetId == GameManager.SelfPlayerId)
        {
            GameManager.Singleton.cardsHand.Remove(card.id);
            //var cardUI = GameManager.Singleton.gameUI.Cards[card.id];
            //GameManager.Singleton.gameUI.Cards.Remove(card.id);
            //GameObject.Destroy(cardUI.gameObject);
        }

        if (playerId != GameManager.SelfPlayerId)
        {
            GameManager.Singleton.gameUI.ShowHandCard(playerId, new List<CardFS>() { card }, "借刀杀人抽取的牌");
        }

        string s = string.Format("{0}使用了技能借刀杀人，抽取了{1}的一张牌{2}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId].name, LanguageUtils.GetCardName(card.cardName));
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, int targetId, CardFS card)
    {
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();

        GameManager.Singleton.players[playerId].cardCount -= 1;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        GameManager.Singleton.players[targetId].AddMessage(card);
        GameManager.Singleton.gameUI.Players[targetId].RefreshMessage();
        GameManager.Singleton.gameUI.ShowAddMessage(targetId, card, false, playerId);

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }

            GameManager.Singleton.cardsHand.Remove(card.id);
            var cardUI = GameManager.Singleton.gameUI.Cards[card.id];
            GameManager.Singleton.gameUI.Cards.Remove(card.id);
            GameObject.Destroy(cardUI.gameObject);
        }

        string s = string.Format("{0}使用了技能借刀杀人，将{1}置入{2}情报区", GameManager.Singleton.players[playerId].name, LanguageUtils.GetCardName(card.cardName), GameManager.Singleton.players[targetId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }
}
