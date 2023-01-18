using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_HanMei : RoleBase
{
    public override string name
    {
        get { return "º«Ã·"; }
    }
    public override bool isWoman => true;

    public override string spritName
    {
        get { return "HanMei"; }
    }
    //public override bool isBack
    //{
    //    get { return false; }
    //}

    public Role_HanMei(int id)
    {
        playerId = id;
        isBack = true;
        skills = new List<SkillBase>() { new UserSkill_YiHuaJieMu(id) };
    }
}
