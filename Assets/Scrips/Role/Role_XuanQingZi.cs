using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_XuanQingZi : RoleBase
{
    public override string name
    {
        get { return "玄青子"; }
    }
    public override string spritName
    {
        get { return "XuanQingZi"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_XuanQingZi(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JinKouYiKai(id)  };
    }
}
