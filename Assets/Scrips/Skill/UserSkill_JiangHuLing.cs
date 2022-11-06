using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 王富贵【江湖令】A：你传出情报后，可以宣言一个颜色。
// 王富贵【江湖令】B：本回合中，当情报被接收后，你可以从接收者的情报区弃置一张被宣言颜色的情报，若弃置的是黑色情报，则你摸一张牌。
public class UserSkill_JiangHuLing : SkillBase
{
    public UserSkill_JiangHuLing(int id)
    {
        playerId = id;
    }

    public override string name => "江湖令";

    public override bool canUse => false;

    public override string Des => "江湖令：你传出情报后，可以宣言一个颜色。本回合中，当情报被接收后，你可以从接收者的情报区弃置一张被宣言颜色的情报，若弃置的是黑色情报，则你摸一张牌。\n";

    public static CardColorEnum color;

    public bool isUseB = false;
    public int selectMessageId;

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        if(isUseB)
        {
            GameManager.Singleton.gameUI.ShowPhase("触发技能江湖令，你可以从接收者的情报区弃置一张被宣言颜色的情报，若弃置的是黑色情报，则你摸一张牌。");
            GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.Singleton.CurMessagePlayerId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowPhase("触发技能江湖令，可以宣言一个颜色,本回合中，当情报被接收后，你可以从接收者的情报区弃置一张被宣言颜色的情报，若弃置的是黑色情报，则你摸一张牌");
            GameManager.Singleton.gameUI.ShowJiangHuLingSelect();
        }
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {
        if(isUseB)
        {
            if(playerId == GameManager.Singleton.CurMessagePlayerId)
            {
                selectMessageId = cardId;
            }
            else
            {
                GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.Singleton.CurMessagePlayerId);
            }
        }
    }

    public override void Use()
    {
        if(isUseB)
        {
            if(selectMessageId != 0)
            {
                ProtoHelper.SendSkill_JiangHuLingB(selectMessageId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.Singleton.CurMessagePlayerId);
                GameManager.Singleton.gameUI.ShowInfo("请选择一张"+ LanguageUtils.GetColorName(color) + "情报弃掉，或取消");
            }
        }
        else
        {
            ProtoHelper.SendSkill_JiangHuLingA(true, color, GameManager.Singleton.seqId);
        }
    }

    public override void Cancel()
    {
        base.Cancel();
        selectMessageId = 0;
        if(GameManager.Singleton.IsUsingSkill)
        {
            if (isUseB)
            {
                ProtoHelper.SendEndReceive(GameManager.Singleton.seqId);
            }
            else
            {
                ProtoHelper.SendSkill_JiangHuLingA(false, 0, GameManager.Singleton.seqId);
            }
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HideJiangHuLingSelect();
    }

    public override void OnUse()
    {
        selectMessageId = 0;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        GameManager.Singleton.gameUI.HideJiangHuLingSelect();
    }

    public static void OnReceiveWaitUseA(int playerId, int waitSeconds, uint seq)
    {
        GameManager.Singleton.seqId = seq;
        GameManager.Singleton.OnWait(playerId, waitSeconds);

        if (playerId == GameManager.SelfPlayerId)
        {
            foreach (var skill in GameManager.Singleton.players[playerId].role.skills)
            {
                if (skill is UserSkill_JiangHuLing)
                {
                    var jiangHuLing = skill as UserSkill_JiangHuLing;
                    jiangHuLing.isUseB = false;
                    jiangHuLing.PrepareUse();
                }
            }
        }
    }
    public static void OnReceiveWaitUseB(int playerId, CardColorEnum color, int waitSeconds, uint seq)
    {
        GameManager.Singleton.seqId = seq;
        GameManager.Singleton.OnWait(playerId, waitSeconds);

        if (playerId == GameManager.SelfPlayerId)
        {
            foreach (var skill in GameManager.Singleton.players[playerId].role.skills)
            {
                if (skill is UserSkill_JiangHuLing)
                {
                    var jiangHuLing = skill as UserSkill_JiangHuLing;
                    jiangHuLing.isUseB = true;
                    UserSkill_JiangHuLing.color = color;
                    jiangHuLing.PrepareUse();
                }
            }
        }
    }

    public static void OnReceiveUseA(int playerId, CardColorEnum color)
    {
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
            
        string des = LanguageUtils.GetColorName(color);

        string s = string.Format("{0}发动了江湖令, 宣言了颜色{1}", GameManager.Singleton.players[playerId].name, des);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
    public static void OnReceiveUseB(int playerId, int cardId)
    {
        string des = "";
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }

        int receivePlayer = GameManager.Singleton.CurMessagePlayerId;
        CardFS messageCard = GameManager.Singleton.players[receivePlayer].GetMessage(cardId);
        GameManager.Singleton.players[receivePlayer].RemoveMessage(cardId);
        GameManager.Singleton.gameUI.Players[receivePlayer].RefreshMessage();
        GameManager.Singleton.gameUI.OnPlayerMessageRemove(receivePlayer, new List<CardFS>() { messageCard });

        string s = string.Format("{0}发动了江湖令, 从{1}的情报区弃置了情报{2}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[receivePlayer].name, des);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
