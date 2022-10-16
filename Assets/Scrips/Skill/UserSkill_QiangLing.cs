using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 张一挺【强令】：你传出情报后，或你决定接收情报后，可以宣言至多两个卡牌名称。本回合中，所有角色均不能使用被宣言的卡牌。
public class UserSkill_QiangLing : SkillBase
{
    public UserSkill_QiangLing(int id)
    {
        playerId = id;
    }

    public override string name => "强令";

    public override bool canUse => false;

    public override string Des => "强令：你传出情报后，或你决定接收情报后，可以宣言至多两个卡牌名称。本回合中，所有角色均不能使用被宣言的卡牌。\n";

    public static List<CardNameEnum> cardTypes = new List<CardNameEnum>();

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("触发技能强令，可以宣言至多两个卡牌名称。本回合中，所有角色均不能使用被宣言的卡牌");
        GameManager.Singleton.gameUI.ShowQiangLingSelect(true);
    }



    public override void Use()
    {
        GameManager.Singleton.gameUI.QiangLingSelect.OnClickSure();
    }
    public override void Cancel()
    {
        base.Cancel();
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.ShowQiangLingSelect(false);
        ProtoHelper.SendSkill_QiangLing(false, new List<CardNameEnum>(), GameManager.Singleton.seqId);
    }

    public override void OnUse()
    {
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveWaitUse(int playerId, int waitSeconds, uint seq)
    {
        GameManager.Singleton.seqId = seq;
        GameManager.Singleton.OnWait(playerId, waitSeconds);

        if (playerId == GameManager.SelfPlayerId)
        {
            foreach (var skill in GameManager.Singleton.players[playerId].role.skills)
            {
                if (skill is UserSkill_QiangLing)
                {
                    var ruGui = skill as UserSkill_QiangLing;
                    ruGui.PrepareUse();
                }
            }
        }
    }

    public static void OnReceiveUse(int playerId, List<CardNameEnum> types)
    {
        string des = "";
        foreach(var type in types)
        {
            if(!cardTypes.Contains(type))
            {
                UserSkill_QiangLing.cardTypes.Add(type);
            }
            des += LanguageUtils.GetCardName(type);
        }
        GameManager.Singleton.gameUI.RefreshQiangLingInfo();

        string s = string.Format("{0}发动了强令, 禁用了卡牌{1}", GameManager.Singleton.players[playerId].name, des);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }
}
