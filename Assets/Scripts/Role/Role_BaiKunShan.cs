using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_BaiKunShan : RoleBase
{
    public override string name
    {
        get { return "°×À¥É½"; }
    }
    public override bool isWoman => false;

    public override string spritName
    {
        get { return "BaiKunShan"; }
    }
    //public override bool isBack
    //{
    //    get { return false; }
    //}

    public Role_BaiKunShan(int id)
    {
        playerId = id;
        isBack = true;
        skills = new List<SkillBase>() { new UserSkill_DuJi(id) };
    }
}
