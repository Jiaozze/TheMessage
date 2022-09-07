using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//你使用【调包】或【破译】后，可以将你的角色牌翻至面朝下。
public class UserSkill_HuanRi : SkillBase
{
    public override string name { get { return "换日"; } }
    public override bool canUse { get { return false; } }

    public UserSkill_HuanRi(int id)
    {
        playerId = id;
    }
    public override bool CheckTriger()
    {
        return true;
    }

    public override void PrepareUse()
    {

    }
    public override void Use()
    {
        base.Use();
    }
    public override void OnUse()
    {
        //GameManager.Singleton.IsUsingSkill = false;
        //GameManager.Singleton.selectSkill = null;
        //GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUse(int playerId)
    {
        GameManager.Singleton.players[playerId].role.isBack = true;
        GameManager.Singleton.gameUI.Players[playerId].OnTurnBack(true);


        string s = string.Format("{0}号玩家使用了技能换日,将角色翻至面朝下", playerId);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
