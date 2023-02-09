using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_WangTianXiang : RoleBase
{
    public override string name
    {
        get { return "王田香"; }
    }
    public override string spritName
    {
        get { return "WangTianXiang"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_WangTianXiang(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JinBi(id)  };
    }
}
