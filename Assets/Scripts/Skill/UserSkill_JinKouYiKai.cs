using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 玄青子【金口一开】A：你的回合的争夺阶段限一次，你可以查看牌堆顶的一张牌。
// 玄青子【金口一开】B：然后选择一项：
// ♦ 你摸一张牌。
// ♦ 将牌堆顶的一张牌和待接收情报面朝下互换
public class UserSkill_JinKouYiKai : SkillBase
{
    public override string name { get { return "金口一开"; } }
    public override bool canUse
    {
        get
        {
            return usedCount < 1 && playerId == GameManager.Singleton.CurTurnPlayerId && playerId == GameManager.SelfPlayerId && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.curPhase == PhaseEnum.Fight_Phase && !isWaitChoose;
        }
    }

    public override string Des => "金口一开：你的回合的争夺阶段限一次，你可以查看牌堆顶的一张牌。然后选择一项：\n♦ 你摸一张牌。\n♦ 将牌堆顶的一张牌和待接收情报面朝下互换";

    private int usedCount = 0;

    private bool isWaitChoose = false;

    public UserSkill_JinKouYiKai(int id)
    {
        playerId = id;
    }
    public override void PrepareUse()
    {
        if (!canUse)
        {
            return;
        }
        base.PrepareUse();
        if (GameManager.Singleton.selectSkill == this)
        {
            Cancel();
        }
        else
        {
            GameManager.Singleton.IsUsingSkill = true;
            GameManager.Singleton.selectSkill = this;
            GameManager.Singleton.gameUI.ShowPhase("是否发动技能金口一开");
        }
    }

    public override void Use()
    {
        if (isWaitChoose)
        {
            ProtoHelper.SendSkill_JinKouYiKaiB(true, GameManager.Singleton.seqId);
        }
        else
        {
            ProtoHelper.SendSkill_JinKouYiKaiA(GameManager.Singleton.seqId);
        }
    }


    public override void OnTurnEnd()
    {
        usedCount = 0;
        isWaitChoose = false;
    }

    public override void Cancel()
    {
        if (isWaitChoose)
        {
            ProtoHelper.SendSkill_JinKouYiKaiB(false, GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.IsUsingSkill = false;
            GameManager.Singleton.selectSkill = null;
            GameManager.Singleton.gameUI.ShowPhase();
        }
    }

    public override void OnUse()
    {
        usedCount = usedCount + 1;
        isWaitChoose = false;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();

    }

    public static void OnReceiveUseA(int playerId, CardFS card, int waitSeconds, uint seq)
    {
        GameManager.Singleton.seqId = seq;

        if (waitSeconds > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitSeconds);
        }

        if (playerId == GameManager.SelfPlayerId)
        {
            GameManager.Singleton.gameUI.ShowHandCard(playerId, new List<CardFS>() { card }, "牌堆顶的牌");

            GameManager.Singleton.gameUI.ShowPhase("选择一项:确定-将牌堆顶这张牌牌和待接收情报面朝下互换，取消-摸一张牌");

            UserSkill_JinKouYiKai skill_JinKouYiKai = GameManager.Singleton.selectSkill as UserSkill_JinKouYiKai;
            skill_JinKouYiKai.isWaitChoose = true;
        }

        string s = string.Format("{0}使用了技能金口一开，查看了牌堆顶的牌", GameManager.Singleton.players[playerId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, bool exchange)
    {
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }

        if (exchange)
        {
            GameManager.Singleton.gameUI.ShowExchangeMessageAndTop();
            string s = string.Format("{0}将牌堆顶的牌与待接收情报互换", GameManager.Singleton.players[playerId].name);
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);
        }
    }

}
