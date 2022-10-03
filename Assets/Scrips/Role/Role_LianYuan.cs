using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_LianYuan : RoleBase
{
    public override string name
    {
        get { return "连鸢"; }
    }
    public override string spritName
    {
        get { return "LianYuan"; }
    }

    public override bool isWoman => true;
    public Role_LianYuan(int id)
    {
        isBack = true;
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_MiaoBiQiaoBian(id)};
    }
}
