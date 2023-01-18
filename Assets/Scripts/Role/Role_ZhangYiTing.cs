using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_ZhangYiTing : RoleBase
{
    public override string name
    {
        get { return "张一挺"; }
    }
    public override string spritName
    {
        get { return "ZhangYiTing"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_ZhangYiTing(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_QiangLing(id)  };
    }
}
