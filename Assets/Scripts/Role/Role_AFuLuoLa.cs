using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_AFuLuoLa : RoleBase
{
    public override string name
    {
        get { return "°¢Ü½ÂÞÀ­"; }
    }
    public override bool isWoman => true;

    public override string spritName
    {
        get { return "AFuLuoLa"; }
    }
    //public override bool isBack
    //{
    //    get { return false; }
    //}

    public Role_AFuLuoLa(int id)
    {
        playerId = id;
        isBack = true;
        skills = new List<SkillBase>() { new UserSkill_MiaoShou(id) };
    }
}
