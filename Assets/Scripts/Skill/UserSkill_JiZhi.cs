using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 顾小梦【集智】：一名角色濒死时，或争夺阶段，你可以翻开此角色牌，然后摸四张牌。
public class UserSkill_JiZhi : SkillBase
{
    public override string name { get { return "集智"; } }
    public override bool canUse { get {
            if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
            {
                return false;
            }
            if (GameManager.Singleton.curPhase != PhaseEnum.Fight_Phase && GameManager.Singleton.IsWaitSaving == -1)
            {
                return false;
            }
            if (!GameManager.Singleton.players[GameManager.SelfPlayerId].role.isBack)
            {
                return false;
            }
            return true;
        } }

    public override string Des => "集智：一名角色濒死时，或争夺阶段，你可以翻开此角色牌，然后摸四张牌。\n";

    public UserSkill_JiZhi(int id)
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
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("是否翻面发动技能集智，翻开此角色牌，然后摸四张牌");
    }
    public override void Use()
    {
        ProtoHelper.SendSkill_JiZhi(GameManager.Singleton.seqId);
    }

    public override void Cancel()
    {
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUse(int playerId)
    {
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
        //if(GameManager.Singleton.players[playerId].role is Role_Unknown)
        //{
        //    GameManager.Singleton.players[playerId].role = new Role_ZhengWenXian(playerId);
        //    GameManager.Singleton.gameUI.Players[playerId].InitRole();
        //}
        //GameManager.Singleton.players[playerId].role.isBack = false;
        //GameManager.Singleton.gameUI.Players[playerId].OnTurnBack(false);


        string s = string.Format("{0}使用了技能集智，将角色翻面", GameManager.Singleton.players[playerId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
