using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//争夺阶段你可以翻开此角色牌，然后视为你使用了一张【截获】。
public class UserSkill_TouTian : SkillBase
{
    public override string name { get { return "偷天"; } }
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

    public UserSkill_TouTian(int id)
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
        GameManager.Singleton.gameUI.ShowPhase("是否翻面发动技能偷天，视为使用一张截获");
    }
    public override void Use()
    {
        ProtoHelper.SendSkill_TouTian(GameManager.Singleton.seqId);
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


        string s = string.Format("{0}号玩家使用了技能偷天，将角色翻面，视为使用了一张截获", playerId);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
